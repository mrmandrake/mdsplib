using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;

namespace mdsplib
{
    public class DFT
    {
        /// <summary>
        /// DFT Class
        /// </summary>
        public DFT() { }

        private double mDFTScale;       // DFT ONLY Scale Factor
        private UInt32 mLengthTotal;    // mN + mZp
        private UInt32 mLengthHalf;     // (mN + mZp) / 2

        private double[,] mCosTerm;      // Caching of multiplication terms to save time
        private double[,] mSinTerm;      // on smaller DFT's
        private bool mOutOfMemory;       // True = Caching ran out of memory.


        /// <summary>
        /// Read only Boolean property. True meas the currently defined DFT is using cached memory to speed up calculations.
        /// </summary>
        public bool IsUsingCached
        {
            private set { }
            get { return !mOutOfMemory; }
        }

        /// <summary>
        /// Pre-Initializes the DFT.
        /// Must call first and this anytime the FFT setup changes.
        /// </summary>
        /// <param name="inputDataLength"></param>
        /// <param name="zeroPaddingLength"></param>
        /// <param name="forceNoCache">True will force the DFT to not use pre-calculated caching.</param>
        public void Initialize(UInt32 inputDataLength, UInt32 zeroPaddingLength = 0, bool forceNoCache = false)
        {
            // Save the sizes for later
            mLengthTotal = inputDataLength + zeroPaddingLength;
            mLengthHalf = (mLengthTotal / 2) + 1;

            // Set the overall scale factor for all the terms
            mDFTScale = Math.Sqrt(2) / (double)(inputDataLength + zeroPaddingLength);                 // Natural DFT Scale Factor                                           // Window Scale Factor
            mDFTScale *= ((double)(inputDataLength + zeroPaddingLength)) / (double)inputDataLength;   // Account For Zero Padding                           // Zero Padding Scale Factor


            if (forceNoCache == true)
            {
                // If optional No Cache - just flag that the cache failed 
                // then the routines will use the brute force DFT methods.
                mOutOfMemory = true;
                return;
            }

            // Try to make pre-calculated sin/cos arrays. If not enough memory, then 
            // use a brute force DFT.
            // Note: pre-calculation speeds the DFT up by about 5X (on a core i7)
            mOutOfMemory = false;
            try
            {
                mCosTerm = new double[mLengthTotal, mLengthTotal];
                mSinTerm = new double[mLengthTotal, mLengthTotal];

                double scaleFactor = 2.0 * Math.PI / mLengthTotal;

                //Parallel.For(0, mLengthHalf, (j) =>
                for (int j = 0; j < mLengthHalf; j++)
                {
                    double a = j * scaleFactor;
                    for (int k = 0; k < mLengthTotal; k++)
                    {
                        mCosTerm[j, k] = Math.Cos(a * k) * mDFTScale;
                        mSinTerm[j, k] = Math.Sin(a * k) * mDFTScale;
                    }
                } //);
            }
            catch (OutOfMemoryException)
            {
                // Could not allocate enough room for the cache terms
                // So, will use brute force DFT
                mOutOfMemory = true;
            }
        }


        /// <summary>
        /// Execute the DFT.
        /// </summary>
        /// <param name="timeSeries"></param>
        /// <returns>Complex[] FFT Result</returns>
        public Complex[] Execute(double[] timeSeries)
        {
            Debug.Assert(timeSeries.Length <= mLengthTotal, "The input timeSeries length was greater than the total number of points that was initialized. DFT.Exectue()");

            // Account for zero padding in size of DFT input array
            double[] totalInputData = new double[mLengthTotal];
            Array.Copy(timeSeries, totalInputData, timeSeries.Length);

            Complex[] output;
            if (mOutOfMemory)
                output = Dft(totalInputData);
            else
                output = DftCached(totalInputData);

            return output;
        }

        /// <summary>
        /// A brute force DFT - Uses Task / Parallel pattern
        /// </summary>
        /// <param name="timeSeries"></param>
        /// <returns>Complex[] result</returns>
        private Complex[] Dft(double[] timeSeries)
        {
            UInt32 n = mLengthTotal;
            UInt32 m = mLengthHalf;
            double[] re = new double[m];
            double[] im = new double[m];
            Complex[] result = new Complex[m];
            double sf = 2.0 * Math.PI / n;

            Parallel.For(0, m, (j) =>
            //for (UInt32 j = 0; j < m; j++)
            {
                double a = j * sf;
                for (UInt32 k = 0; k < n; k++)
                {
                    re[j] += timeSeries[k] * Math.Cos(a * k) * mDFTScale;
                    im[j] -= timeSeries[k] * Math.Sin(a * k) * mDFTScale;
                }

                result[j] = new Complex(re[j], im[j]);
            });

            // DC and Fs/2 Points are scaled differently, since they have only a real part
            result[0] = new Complex(result[0].Real / Math.Sqrt(2), 0.0);
            result[mLengthHalf - 1] = new Complex(result[mLengthHalf - 1].Real / Math.Sqrt(2), 0.0);

            return result;
        }

        /// <summary>
        /// DFT with Pre-calculated Sin/Cos arrays + Task / Parallel pattern.
        /// DFT can only be so big before the computer runs out of memory and has to use
        /// the brute force DFT.
        /// </summary>
        /// <param name="timeSeries"></param>
        /// <returns>Complex[] result</returns>
        private Complex[] DftCached(double[] timeSeries)
        {
            UInt32 n = mLengthTotal;
            UInt32 m = mLengthHalf;
            double[] re = new double[m];
            double[] im = new double[m];
            Complex[] result = new Complex[m];

            Parallel.For(0, m, (j) =>
            //for (UInt32 j = 0; j < m; j++)
            {
                for (UInt32 k = 0; k < n; k++)
                {
                    re[j] += timeSeries[k] * mCosTerm[j, k];
                    im[j] -= timeSeries[k] * mSinTerm[j, k];
                }
                result[j] = new Complex(re[j], im[j]);
            });

            // DC and Fs/2 Points are scaled differently, since they have only a real part
            result[0] = new Complex(result[0].Real / Math.Sqrt(2), 0.0);
            result[mLengthHalf - 1] = new Complex(result[mLengthHalf - 1].Real / Math.Sqrt(2), 0.0);

            return result;
        }
    }
}

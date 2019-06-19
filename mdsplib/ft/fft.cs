using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using mdsplib.DSP;

namespace mdsplib.FT
{ 
    public class FFT
    {
        /// <summary>
        /// FFT Class
        /// </summary>
        public FFT() { }

        private double mFFTScale = 1.0;
        private UInt32 mLogN = 0;       // log2 of FFT size
        private UInt32 mN = 0;          // Time series length
        private UInt32 mLengthTotal;    // mN + mZp
        private UInt32 mLengthHalf;     // (mN + mZp) / 2
        private FFTElement[] mX;        // Vector of linked list elements

        // Element for linked list to store input/output data.
        private class FFTElement
        {
            public double re = 0.0;     // Real component
            public double im = 0.0;     // Imaginary component
            public FFTElement next;     // Next element in linked list
            public UInt32 revTgt;       // Target position post bit-reversal
        }

        /// <summary>
        /// Initialize the FFT. Must call first and this anytime the FFT setup changes.
        /// </summary>
        /// <param name="inputDataLength"></param>
        /// <param name="zeroPaddingLength"></param>
        public FFT Initialize(UInt32 inputDataLength, UInt32 zeroPaddingLength = 0)
        {
            mN = inputDataLength;

            // Find the power of two for the total FFT size up to 2^32
            bool foundIt = false;
            for (mLogN = 1; mLogN <= 32; mLogN++)
            {
                double n = Math.Pow(2.0, mLogN);
                if ((inputDataLength + zeroPaddingLength) == n)
                {
                    foundIt = true;
                    break;
                }
            }

            if (foundIt == false)
                throw new ArgumentOutOfRangeException("inputDataLength + zeroPaddingLength was not an even power of 2! FFT cannot continue.");

            // Set global parameters.
            mLengthTotal = inputDataLength + zeroPaddingLength;
            mLengthHalf = (mLengthTotal / 2) + 1;

            // Set the overall scale factor for all the terms
            mFFTScale = Math.Sqrt(2) / (double)(mLengthTotal);                // Natural FFT Scale Factor  // Window Scale Factor
            mFFTScale *= ((double)mLengthTotal) / (double)inputDataLength;    // Zero Padding Scale Factor

            // Allocate elements for linked list of complex numbers.
            mX = new FFTElement[mLengthTotal];
            for (UInt32 k = 0; k < (mLengthTotal); k++)
                mX[k] = new FFTElement();

            // Set up "next" pointers.
            for (UInt32 k = 0; k < (mLengthTotal) - 1; k++)
                mX[k].next = mX[k + 1];

            // Specify target for bit reversal re-ordering.
            for (UInt32 k = 0; k < (mLengthTotal); k++)
                mX[k].revTgt = DSP.Util.BitReverse(k, mLogN);

            return this;
        }

        /// <summary>
        /// Executes a FFT of the input time series.
        /// </summary>
        /// <param name="timeSeries"></param>
        /// <returns>Complex[] Spectrum</returns>
        public Complex[] Direct(double[] timeSeries)
        {
            Complex[] c = new Complex[timeSeries.Length];
            for (int i = 0; i < timeSeries.Length; i++)
                c[i] = new Complex(timeSeries[i], 0);

            return Execute(c);
        }

        public Complex[] Inverse(Complex[] series)
        {
            Complex[] c = new Complex[series.Length];
            for (int i = 0; i < series.Length; i++)
                c[i] = new Complex(series[i].Imaginary, series[i].Real);

            return Execute(c, false);
        }

        /// <summary>
        /// Executes a FFT 
        /// </summary>
        /// <param name="series"></param>
        /// <returns>Complex[] Spectrum</returns>
        private Complex[] Execute(Complex[] series, bool direct = true)
        {
            UInt32 numFlies = mLengthTotal >> 1;  // Number of butterflies per sub-FFT
            UInt32 span = mLengthTotal >> 1;      // Width of the butterfly
            UInt32 spacing = mLengthTotal;        // Distance between start of sub-FFTs
            UInt32 wIndexStep = 1;          // Increment for twiddle table index

            Debug.Assert(series.Length <= mLengthTotal,
                "The input timeSeries length was greater than the total number of points that was initialized. FFT.Execute()");

            // Copy data into linked complex number objects
            FFTElement x = mX[0];
            UInt32 k = 0;
            for (UInt32 i = 0; i < mN; i++)
            {
                x.re = series[k].Real;
                x.im = series[k].Imaginary;
                x = x.next;
                k++;
            }

            // If zero padded, clean the 2nd half of the linked list from previous results
            if (mN != mLengthTotal)
            {
                for (UInt32 i = mN; i < mLengthTotal; i++)
                {
                    x.re = 0.0;
                    x.im = 0.0;
                    x = x.next;
                }
            }

            // For each stage of the FFT
            for (UInt32 stage = 0; stage < mLogN; stage++)
            {
                // Compute a multiplier factor for the "twiddle factors".
                // The twiddle factors are complex unit vectors spaced at
                // regular angular intervals. The angle by which the twiddle
                // factor advances depends on the FFT stage. In many FFT
                // implementations the twiddle factors are cached, but because
                // array lookup is relatively slow in C#, it's just
                // as fast to compute them on the fly.
                double wAngleInc = wIndexStep * -2.0 * Math.PI / (mLengthTotal);
                double wMulRe = Math.Cos(wAngleInc);
                double wMulIm = Math.Sin(wAngleInc);

                for (UInt32 start = 0; start < (mLengthTotal); start += spacing)
                {
                    FFTElement xTop = mX[start];
                    FFTElement xBot = mX[start + span];

                    double wRe = 1.0;
                    double wIm = 0.0;

                    // For each butterfly in this stage
                    for (UInt32 flyCount = 0; flyCount < numFlies; ++flyCount)
                    {
                        // Get the top & bottom values
                        double xTopRe = xTop.re;
                        double xTopIm = xTop.im;
                        double xBotRe = xBot.re;
                        double xBotIm = xBot.im;

                        // Top branch of butterfly has addition
                        xTop.re = xTopRe + xBotRe;
                        xTop.im = xTopIm + xBotIm;

                        // Bottom branch of butterfly has subtraction,
                        // followed by multiplication by twiddle factor
                        xBotRe = xTopRe - xBotRe;
                        xBotIm = xTopIm - xBotIm;
                        xBot.re = xBotRe * wRe - xBotIm * wIm;
                        xBot.im = xBotRe * wIm + xBotIm * wRe;

                        // Advance butterfly to next top & bottom positions
                        xTop = xTop.next;
                        xBot = xBot.next;

                        // Update the twiddle factor, via complex multiply
                        // by unit vector with the appropriate angle
                        // (wRe + j wIm) = (wRe + j wIm) x (wMulRe + j wMulIm)
                        double tRe = wRe;
                        wRe = wRe * wMulRe - wIm * wMulIm;
                        wIm = tRe * wMulIm + wIm * wMulRe;
                    }
                }

                numFlies >>= 1;   // Divide by 2 by right shift
                span >>= 1;
                spacing >>= 1;
                wIndexStep <<= 1;     // Multiply by 2 by left shift
            }

            // The algorithm leaves the result in a scrambled order.
            // Unscramble while copying values from the complex
            // linked list elements to a complex output vector & properly apply scale factors.

            x = mX[0];
            Complex[] unswizzle = new Complex[mLengthTotal];
            while (x != null)
            {
                UInt32 target = x.revTgt;
                unswizzle[target] = direct ? 
                    new Complex(x.re * mFFTScale, x.im * mFFTScale) : 
                    new Complex(x.im, x.re);
                x = x.next;
            }

            Complex[] result = new Complex[mLengthTotal];
            Array.Copy(unswizzle, result, mLengthTotal);

            if (direct)
            {
                // DC and Fs/2 Points are scaled differently, since they have only a real part
                result[0] = new Complex(result[0].Real / Math.Sqrt(2), 0.0);
                result[mLengthHalf - 1] = new Complex(result[mLengthHalf - 1].Real / Math.Sqrt(2), 0.0);
            }
            else
                result = result.Multiply(1 / System.Math.Sqrt(2));

            return result;
        }
    }

    public static class FFTExtension
    {
        public static double[] Window(this double[] a, mdsplib.DSP.Window.Type wnd = mdsplib.DSP.Window.Type.Hann)
        {
            double[] window = mdsplib.DSP.Window.Coefficients(wnd, (uint)a.Length);
            return a.Multiply(window);
        }

        public static Complex[] FFT(this Double[] a, UInt32 zeroPaddingLength = 0)
        {
            return new FFT().Initialize((uint)a.Length, zeroPaddingLength).Direct(a);
        }

        public static Complex[] iFFT(this Complex[] a, UInt32 zeroPaddingLength = 0)
        {            
            var result = new FFT().Initialize((uint)a.Length).Inverse(a);
            return result.Take(result.Length - (int)zeroPaddingLength).ToArray();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace mdsplib.DSP
{
    public static class Util
    {
        /// <Summary>
        /// Do bit reversal of specified number of places of an int
        ///For example, 1101 bit-reversed is 1011
        /// </summary>
        ///<param name="x">Number to be bit-reverse.</param>
        ///<param name="numBits">Number of bits in the number.</param>
        /// <returns></returns>
        public static UInt32 BitReverse(UInt32 x, UInt32 numBits)
        {
            UInt32 y = 0;
            for (UInt32 i = 0; i < numBits; i++)
            {
                y <<= 1;
                y |= x & 0x0001;
                x >>= 1;
            }
            return y;
        }

        public static class FFT
        {
            /// <summary>
            /// Nyquist Frequency
            /// </summary>
            /// <param name="fs"></param>
            /// <returns></returns>
            public static double Nyquist(double fs) { return fs / 2; }

            /// <summary>
            /// Number of bins
            /// </summary>
            /// <param name="wlen"></param>
            /// <returns></returns>
            public static UInt32 Bins(UInt32 wlen) { return wlen / 2; }

            /// <summary>
            /// Frequency band of a bin
            /// </summary>
            /// <param name="wlen"></param>
            /// <param name="fs"></param>
            /// <returns></returns>
            public static double Fb(UInt32 wlen, double fs) { return Nyquist(fs) / Bins(wlen); }

            /// <summary>
            /// Return the Frequency Array for the currently defined FFT.
            /// Takes into account the total number of points and zero padding points that were defined.
            /// </summary>
            /// <param name="samplingFrequencyHz"></param>
            /// <returns></returns>
            public static double[] FrequencySpan(double samplingFrequencyHz, UInt32 points)
            {
                double[] result = new double[points];
                double stopValue = Nyquist(samplingFrequencyHz);
                double increment = stopValue / ((double)points - 1.0);

                for (Int32 i = 0; i < points; i++)
                    result[i] += increment * i;

                return result;
            }
        }

        public static double[] ZeroPad(this double[] val, UInt16 padLen)
        {
            double[] result = new double[val.Length + 2 * padLen];
            for (Int32 i = 0; i < val.Length; i++)
                result[padLen + i] = val[i];

            return result;
        }

        public static void Dump(this double[] val, string path)
        {
            File.WriteAllLines(path, val.Select(d => d.ToString()));
        }

        public static void Dump(this Complex[] val, string path)
        {
            File.WriteAllLines(path, val.Select(d => d.Real.ToString() + "," + d.Imaginary.ToString()));
        }

        public static void Dump(this List<Complex[]> val, string path)
        {
            throw new NotImplementedException();
        }

        public static double NextPowerOf2(double val)
        {
            double nextPowerOf2 = 1;
            while (nextPowerOf2 < val)
                nextPowerOf2 *= 2;

            return nextPowerOf2;
        }
    }
}
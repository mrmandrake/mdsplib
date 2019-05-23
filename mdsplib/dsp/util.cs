using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace mdsplib.DSP
{
    public static class Util
    {
        /// <summary>
        /// Return the Frequency Array for the currently defined FFT.
        /// Takes into account the total number of points and zero padding points that were defined.
        /// </summary>
        /// <param name="samplingFrequencyHz"></param>
        /// <returns></returns>
        public static double[] FrequencySpan(double samplingFrequencyHz, UInt32 points)
        {
            double[] result = new double[points];
            double stopValue = samplingFrequencyHz / 2.0;
            double increment = stopValue / ((double)points - 1.0);

            for (Int32 i = 0; i < points; i++)
                result[i] += increment * i;

            return result;
        }

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

    }
}
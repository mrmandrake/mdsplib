using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace mdsplib.DSP
{
    /// <summary>
    /// DFT / FFT Format Conversion Functions
    /// </summary>
    public static class Magnitude
    {
        /// <summary>
        /// Convert Magnitude FT Result to: Magnitude Squared Format
        /// </summary>
        /// <param name="magnitude"></param>
        /// <returns></returns>
        public static double[] ToMagnitudeSquared(this double[] magnitude)
        {
            UInt32 np = (UInt32)magnitude.Length;
            double[] mag2 = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                mag2[i] = magnitude[i] * magnitude[i];
            }

            return mag2;
        }

        /// <summary>
        /// Convert Magnitude FT Result to: Magnitude dBVolts
        /// </summary>
        /// <param name="magnitude"></param>
        /// <returns>double[] array</returns>
        public static double[] ToMagnitudeDBV(this double[] magnitude)
        {
            UInt32 np = (UInt32)magnitude.Length;
            double[] magDBV = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                double magVal = magnitude[i];
                if (magVal <= 0.0)
                    magVal = double.Epsilon;

                magDBV[i] = 20 * System.Math.Log10(magVal);
            }

            return magDBV;
        }
    }

    public static class MagnitudeSquared
    {

        /// <summary>
        /// Convert Magnitude Squared FFT Result to: Magnitude Vrms
        /// </summary>
        /// <param name="magSquared"></param>
        /// <returns>double[] array</returns>
        public static double[] ToMagnitude(this double[] magSquared)
        {
            UInt32 np = (UInt32)magSquared.Length;
            double[] mag = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                mag[i] = System.Math.Sqrt(magSquared[i]);
            }

            return mag;
        }

        /// <summary>
        /// Convert Magnitude Squared FFT Result to: Magnitude dBVolts
        /// </summary>
        /// <param name="magSquared"></param>
        /// <returns>double[] array</returns>
        public static double[] ToMagnitudeDBV(this double[] magSquared)
        {
            UInt32 np = (UInt32)magSquared.Length;
            double[] magDBV = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                double magSqVal = magSquared[i];
                if (magSqVal <= 0.0)
                    magSqVal = double.Epsilon;

                magDBV[i] = 10 * System.Math.Log10(magSqVal);
            }

            return magDBV;
        }
    }

    public static class ComplexMath
    {
        public static double[] RealPart(this Complex[] vecIn)
        {
            return (from c in vecIn select c.Real).ToArray();
        }

        public static double[] ImagPart(this Complex[] vecIn)
        {
            return (from c in vecIn select c.Imaginary).ToArray();
        }

        /// <summary>
        /// Convert Complex DFT/FFT Result to: Magnitude Squared V^2 rms
        /// </summary>
        /// <param name="rawFFT"></param>
        /// <returns>double[] MagSquared Format</returns>
        public static double[] ToMagnitudeSquared(this Complex[] rawFFT)
        {
            UInt32 np = (UInt32)rawFFT.Length;
            double[] magSquared = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                double mag = rawFFT[i].Magnitude;
                magSquared[i] = mag * mag;
            }

            return magSquared;
        }

        /// <summary>
        /// Convert Complex DFT/FFT Result to: Magnitude Vrms
        /// </summary>
        /// <param name="rawFFT"></param>
        /// <returns>double[] Magnitude Format (Vrms)</returns>
        public static double[] ToMagnitude(this Complex[] rawFFT)
        {
            UInt32 np = (UInt32)rawFFT.Length;
            double[] mag = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                mag[i] = rawFFT[i].Magnitude;
            }

            return mag;
        }

        /// <summary>
        /// Convert Complex DFT/FFT Result to: Log Magnitude dBV
        /// </summary>
        /// <param name="rawFFT"> Complex[] input array"></param>
        /// <returns>double[] Magnitude Format (dBV)</returns>
        public static double[] ToMagnitudeDBV(this Complex[] rawFFT)
        {
            UInt32 np = (UInt32)rawFFT.Length;
            double[] mag = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                double magVal = rawFFT[i].Magnitude;

                if (magVal <= 0.0)
                    magVal = double.Epsilon;

                mag[i] = 20 * System.Math.Log10(magVal);
            }

            return mag;
        }

        /// <summary>
        /// Convert Complex DFT/FFT Result to: Phase in Degrees
        /// </summary>
        /// <param name="rawFFT"> Complex[] input array"></param>
        /// <returns>double[] Phase (Degrees)</returns>
        public static double[] ToPhaseDegrees(this Complex[] rawFFT)
        {
            double sf = 180.0 / System.Math.PI; // Degrees per Radian scale factor

            UInt32 np = (UInt32)rawFFT.Length;
            double[] phase = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                phase[i] = rawFFT[i].Phase * sf;
            }

            return phase;
        }

        /// <summary>
        /// Convert Complex DFT/FFT Result to: Phase in Radians
        /// </summary>
        /// <param name="rawFFT"> Complex[] input array"></param>
        /// <returns>double[] Phase (Degrees)</returns>
        public static double[] ToPhaseRadians(this Complex[] rawFFT)
        {
            UInt32 np = (UInt32)rawFFT.Length;
            double[] phase = new double[np];
            for (UInt32 i = 0; i < np; i++)
            {
                phase[i] = rawFFT[i].Phase;
            }

            return phase;
        }
    }
}
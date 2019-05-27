using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace mdsplib.DSP
{

    /// <summary>
    /// Simple signal generator helper
    /// </summary>

    public static class Generate
    {
        /// <summary>
        /// Generate linearly spaced array. Like the Octave function of the same name.
        /// EX: DSP.Generate.LinSpace(1, 10, 10) -> Returns array: 1, 2, 3, 4....10.
        /// </summary>
        /// <param name="startVal">Any value</param>
        /// <param name="stopVal">Any value > startVal</param>
        /// <param name="points">Number of points to generate</param>
        /// <returns>double[] array</returns>
        public static double[] LinSpace(double startVal, double stopVal, UInt32 points)
        {
            double[] result = new double[points];
            double increment = (stopVal - startVal) / ((double)points - 1.0);

            for (UInt32 i = 0; i < points; i++)
                result[i] = startVal + increment * i;

            return result;
        }

        private static double Deg2Phase(double deg)
        {
            return deg * System.Math.PI / 180.0;
        }


        /// <summary>
        /// Generates a Sine Wave Tone using Sampling Terms.
        /// </summary>
        /// <param name="amplitudeVrms"></param>
        /// <param name="frequencyHz"></param>
        /// <param name="samplingFrequencyHz"></param>
        /// <param name="points"></param>
        /// <param name="dcV">[Optional] DC Voltage offset</param>
        /// <param name="phaseDeg">[Optional] Phase of signal in degrees</param>
        /// <returns>double[] array</returns>
        public static double[] ToneSampling(double amplitudeVrms, double frequencyHz, double samplingFrequencyHz, UInt32 points, double dcV = 0.0, double phaseDeg = 0)
        {
            double[] rval = new double[points];
            for (UInt32 i = 0; i < points; i++)
            {
                double time = (double)i / samplingFrequencyHz;
                rval[i] = System.Math.Sqrt(2) * amplitudeVrms * System.Math.Sin(Const._2PI * time * frequencyHz + Deg2Phase(phaseDeg)) + dcV;
            }
            return rval;
        }

        ///
        public static double[] Sine(double frequencyHz, double samplingFrequencyHz, UInt32 points, double phase = 0)
        {
            double[] rval = new double[points];
            for (UInt32 i = 0; i < points; i++)
            {
                double time = (double)i / samplingFrequencyHz;
                rval[i] = System.Math.Sin(Const._2PI * time * frequencyHz + phase);
            }
            return rval;
        }

        ///
        public static double[] Square(double frequencyHz, double samplingFrequencyHz, UInt32 points, UInt32 harmonics = 50)
        {
            double[] rval = new double[points];
            for (UInt32 i = 0; i < points; i++)
            {
                double time = (double)i / samplingFrequencyHz;

                double acc = 0;
                for (int k = 1; k < harmonics; k++)
                    acc += System.Math.Sin(Const._2PI * (2 * k - 1.0) * time * frequencyHz) / (2 * k - 1.0);

                rval[i] = 4 / System.Math.PI * acc;
            }

            return rval;
        }

        ///
        public static double[] Saw(double frequencyHz, double samplingFrequencyHz, UInt32 points, UInt32 harmonics = 50)
        {
            double[] rval = new double[points];
            for (UInt32 i = 0; i < points; i++)
            {
                double time = (double)i / samplingFrequencyHz;

                double acc = 0;
                for (int k = 1; k < harmonics; k++)
                    acc += System.Math.Pow(-1, k) * System.Math.Sin(Const._2PI * k * time * frequencyHz) / k;

                rval[i] = 4.0 * System.Math.Pow(Const._2PI, -1) * acc;
            }

            return rval;
        }

        ///
        public static double[] Triangle(double frequencyHz, double samplingFrequencyHz, UInt32 points, UInt32 harmonics = 50)
        {
            double[] rval = new double[points];
            for (UInt32 i = 0; i < points; i++)
            {
                double time = (double)i / samplingFrequencyHz;

                double acc = 0;
                for (int k = 1; k < harmonics; k++)
                    acc += System.Math.Pow(-1, k) * System.Math.Sin((2 * k + 1) * time * frequencyHz) / System.Math.Pow(2 * k + 1, 2);

                rval[i] = (8 / System.Math.Pow(System.Math.PI, 2)) * acc;
            }

            return rval;
        }


        /// <summary>
        /// Generates a Sine Wave Tone using Number of Cycles Terms.
        /// </summary>
        /// <param name="amplitudeVrms"></param>
        /// <param name="cycles"></param>
        /// <param name="points"></param>
        /// <param name="dcV">[Optional] DC Voltage offset</param>
        /// <param name="phaseDeg">[Optional] Phase of signal in degrees</param>
        /// <returns>double[] array</returns>
        public static double[] ToneCycles(double amplitudeVrms, double cycles, UInt32 points, double dcV = 0.0, double phaseDeg = 0)
        {
            double ph_r = phaseDeg * System.Math.PI / 180.0;
            double ampPeak = System.Math.Sqrt(2) * amplitudeVrms;

            double[] rval = new double[points];
            for (UInt32 i = 0; i < points; i++)
            {
                rval[i] = ampPeak * System.Math.Sin((Const._2PI * (double)i / (double)points * cycles) + ph_r) + dcV;
            }
            return rval;
        }


        /// <summary>
        /// Generates a normal distribution noise signal of the specified power spectral density (Vrms / rt-Hz).
        /// </summary>
        /// <param name="amplitudePsd (Vrms / rt-Hz)"></param>
        /// <param name="samplingFrequencyHz"></param>
        /// <param name="points"></param>
        /// <returns>double[] array</returns>
        public static double[] NoisePsd(double amplitudePsd, double samplingFrequencyHz, UInt32 points)
        {
            // Calculate what the noise amplitude needs to be in Vrms/rt_Hz
            double arms = amplitudePsd * System.Math.Sqrt(samplingFrequencyHz / 2.0);

            // Make an n length noise vector
            double[] rval = NoiseRms(arms, points);

            return rval;
        }



        /// <summary>
        /// Generates a normal distribution noise signal of the specified Volts RMS.
        /// </summary>
        /// <param name="amplitudeVrms"></param>
        /// <param name="points"></param>
        /// <param name="dcV"></param>
        /// <returns>double[] array</returns>
        public static double[] NoiseRms(double amplitudeVrms, UInt32 points, double dcV = 0.0)
        {
            double[] rval = new double[points];

            // Make an n length noise vector
            rval = Noise(points, amplitudeVrms);
            rval = rval.Add(dcV);
            return rval;
        }

        #region Private - Random Number Generator Core

        //=====[ Gaussian Noise ]=====

        private static Random mRandom = new Random(); // Class level variable

        private static double[] Noise(UInt32 size, double scaling_vrms)
        {

            // Based on - Polar method (Marsaglia 1962)

            // Scaling used,
            // * For DFT Size => "Math.Sqrt(size)"
            // * The Sqrt(2) is a scaling factor to get the
            // output spectral power to be what the desired "scaling_vrms"
            // was as requested. The scaling will produce a "scaling_vrms"
            // value that is correct for Vrms/Rt(Hz) in the frequency domain
            // of a "1/N" scaled DFT or FFT.
            // Most DFT / FFT's are 1/N scaled - check your documentation to be sure...

            double output_scale = scaling_vrms;

            double[] data = new double[size];
            double sum = 0;

            for (UInt32 n = 0; n < size; n++)
            {
                double s;
                double v1;
                do
                {
                    v1 = 2.0 * mRandom.NextDouble() - 1.0;
                    double v2 = 2.0 * mRandom.NextDouble() - 1.0;
                    s = v1 * v1 + v2 * v2;
                } while (s >= 1.0);

                if (s == 0.0)
                    data[n] = 0.0;
                else
                    data[n] = (v1 * System.Math.Sqrt(-2.0 * System.Math.Log(s) / s)) * output_scale;

                sum += data[n];
            }

            // Remove the average value
            double average = sum / size;
            for (UInt32 n = 0; n < size; n++)
                data[n] -= average;

            // Return the Gaussian noise
            return data;
        }

        #endregion
    }
}
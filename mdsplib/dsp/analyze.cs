using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace mdsplib.DSP
{
    /// <summary>
    /// DFT / FFT Output Analysis Functions
    /// </summary>
    public static class Analyze
    {
        /// <summary>
        /// Find the RMS value of a[].
        /// </summary>
        /// <param name="inData"> = of N data points, 0 based.</param>
        /// <param name="startBin"> = Bin to start the counting at (0 based)."></param>
        /// <param name="stopBin"> = Bin FROM END to stop counting at (Max = N - 1)."></param>
        /// <returns>RMS value of input array between start and stop bins.</returns>
        public static double FindRms(double[] a, UInt32 startBin = 10, UInt32 stopBin = 10)
        {
            double sum2 = 0.0;
            UInt32 actualSumCount = 0;
            UInt32 n = (UInt32)a.Length;
            for (UInt32 i = 0; i < n; i++)
            {
                if (i <= startBin - 1)
                    continue;
                if (i > (n - 1) - (stopBin))
                    continue;

                sum2 += a[i] * a[i];
                actualSumCount++;
            }

            double avg2 = sum2 / actualSumCount;
            double rms = System.Math.Sqrt(avg2);

            return rms;
        }

        /// <summary>
        /// Finds the mean of the input array.
        /// </summary>
        /// <param name="inData"> = of N data points, 0 based.</param>
        /// <param name="startBin"> = Bin to start the counting at (0 based)."></param>
        /// <param name="stopBin"> = Bin FROM END to stop counting at (Max = N - 1)."></param>
        /// <returns>Mean value of input array between start and stop bins.</returns>
        public static double FindMean(double[] inData, UInt32 startBin = 10, UInt32 stopBin = 10)
        {
            double sum = 0;
            double n = inData.Length;
            UInt32 actualSumCount = 0;

            for (UInt32 i = 0; i < n; i++)
            {
                if (i <= startBin - 1)
                    continue;
                if (i > (n - 1) - (stopBin))
                    continue;

                sum = sum + inData[i];
                actualSumCount++;
            }
            return sum / actualSumCount;
        }


        /// <summary>
        /// Finds the maximum value in an array.
        /// </summary>
        /// <param name="inData"></param>
        /// <returns>Maximum value of input array</returns>
        public static double FindMaxAmplitude(double[] inData)
        {
            double n = inData.Length;
            double maxVal = -1e300;

            for (UInt32 i = 0; i < n; i++)
            {
                if (inData[i] > maxVal)
                {
                    maxVal = inData[i];
                }
            }

            return maxVal;
        }


        /// <summary>
        /// Finds the position in the inData array where the maximum value happens.
        /// </summary>
        /// <param name="inData"></param>
        /// <returns>Position of maximum value in input array</returns>
        public static UInt32 FindMaxPosition(double[] inData)
        {
            double n = inData.Length;
            double maxVal = -1e300;
            UInt32 maxIndex = 0;

            for (UInt32 i = 0; i < n; i++)
            {
                if (inData[i] > maxVal)
                {
                    maxIndex = i;
                    maxVal = inData[i];
                }
            }

            return maxIndex;
        }

        /// <summary>
        /// Finds the maximum frequency from the given inData and fSpan arrays.
        /// </summary>
        /// <param name="inData"></param>
        /// <param name="fSpan"></param>
        /// <returns>Maximum frequency from input arrays</returns>
        public static double FindMaxFrequency(double[] inData, double[] fSpan)
        {
            double n = inData.Length;
            double maxVal = -1e300;
            UInt32 maxIndex = 0;

            for (UInt32 i = 0; i < n; i++)
            {
                if (inData[i] > maxVal)
                {
                    maxIndex = i;
                    maxVal = inData[i];
                }
            }

            return fSpan[maxIndex];
        }

        /// <summary>
        /// Unwraps the phase so that it is continuous, without jumps.
        /// </summary>
        /// <param name="inPhaseDeg">Array of Phase Data from FT in Degrees</param>
        /// <returns>Continuous Phase data</returns>
        public static double[] UnwrapPhaseDegrees(double[] inPhaseDeg)
        {
            UInt32 N = (UInt32)inPhaseDeg.Length;
            double[] unwrappedPhase = new double[N];

            double[] tempInData = new double[N];
            inPhaseDeg.CopyTo(tempInData, 0);

            // First point is unchanged
            unwrappedPhase[0] = tempInData[0];

            for (UInt32 i = 1; i < N; i++)
            {
                double delta = System.Math.Abs(tempInData[i - 1] - tempInData[i]);
                if (delta >= 180)
                {
                    // Phase jump!
                    if (tempInData[i - 1] < 0.0)
                    {
                        for (UInt32 j = i; j < N; j++)
                            tempInData[j] += -360;
                    }
                    else
                    {
                        for (UInt32 j = i; j < N; j++)
                            tempInData[j] += 360;
                    }
                }
                unwrappedPhase[i] = tempInData[i];
            }
            return unwrappedPhase;
        }

        /// <summary>
        /// Unwraps the phase so that it is continuous, without jumps.
        /// </summary>
        /// <param name="inPhaseRad">Array of Phase Data from FT in Radians</param>
        /// <returns>Continuous Phase data</returns>
        public static double[] UnwrapPhaseRadians(double[] inPhaseRad)
        {
            double pi = System.Math.PI;
            double twoPi = System.Math.PI * 2.0;

            UInt32 N = (UInt32)inPhaseRad.Length;

            double[] tempInData = new double[N];
            inPhaseRad.CopyTo(tempInData, 0);

            double[] unwrappedPhase = new double[N];

            // First point is unchanged
            unwrappedPhase[0] = tempInData[0];

            for (UInt32 i = 1; i < N; i++)
            {
                double delta = System.Math.Abs(tempInData[i - 1] - tempInData[i]);
                if (delta >= pi)
                {
                    // Phase jump!
                    if (tempInData[i - 1] < 0.0)
                    {
                        for (UInt32 j = i; j < N; j++)
                            tempInData[j] += -twoPi;
                    }
                    else
                    {
                        for (UInt32 j = i; j < N; j++)
                            tempInData[j] += twoPi;
                    }
                }
                unwrappedPhase[i] = tempInData[i];
            }
            return unwrappedPhase;
        }
    }
}
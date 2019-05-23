using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace mdsplib.DSP
{
    /// <summary>
    /// Double[] Array Math Operations (All Static)
    /// </summary>
    public static class Math
    {

        /// <summary>
        /// result[] = a[] * b[]
        /// </summary>
        public static Double[] Multiply(Double[] a, Double[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] * b[i];

            return result;
        }

        public static double[] Slice(double[] arrayin, uint start, uint length)
        {
            return arrayin.Skip((int)start).Take((int)length).ToArray();
        }

        /// <summary>
        /// result[] = a[] * b
        /// </summary>
        public static Double[] Multiply(Double[] a, Double b)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] * b;

            return result;
        }

        /// <summary>
        /// result[] = a[] + b[]
        /// </summary>
        public static Double[] Add(Double[] a, Double[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] + b[i];

            return result;
        }

        public static Double[] Add(Double[] a, Double[] b, uint offset)
        {
            Debug.Assert(a.Length > (b.Length + offset), "a.Length < (b.Length + offset)");
            double[] result = (double[])a.Clone();
            for (UInt32 i = 0; i < b.Length; i++)
                result[i + offset] += b[i];

            return result;
        }

        /// <summary>
        /// result[] = a[] + b
        /// </summary>
        public static Double[] Add(Double[] a, Double b)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] + b;

            return result;
        }

        /// <summary>
        /// result[] = a[] - b[]
        /// </summary>
        public static Double[] Subtract(Double[] a, Double[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] - b[i];

            return result;
        }

        /// <summary>
        /// result[] = a[] - b
        /// </summary>
        public static Double[] Subtract(Double[] a, Double b)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] - b;

            return result;
        }

        /// <summary>
        /// result[] = a[] / b[]
        /// </summary>
        public static Double[] Divide(Double[] a, Double[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] / b[i];

            return result;
        }

        /// <summary>
        /// result[] = a[] / b
        /// </summary>
        public static Double[] Divide(Double[] a, Double b)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] / b;

            return result;
        }

        /// <summary>
        /// Square root of a[].
        /// </summary>
        public static double[] Sqrt(double[] a)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = System.Math.Sqrt(a[i]);

            return result;
        }

        /// <summary>
        /// Squares a[].
        /// </summary>
        public static double[] Square(double[] a)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] * a[i];

            return result;
        }

        /// <summary>
        /// Log10 a[].
        /// </summary>
        public static double[] Log10(double[] a)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
            {
                double val = a[i];
                if (val <= 0.0)
                    val = double.Epsilon;

                result[i] = System.Math.Log10(val);
            }

            return result;
        }

        /// <summary>
        /// Removes mean value from a[].
        /// </summary>
        public static double[] RemoveMean(double[] a)
        {
            double sum = 0.0;
            for (UInt32 i = 0; i < a.Length; i++)
                sum += a[i];

            double mean = sum / a.Length;

            return (DSP.Math.Subtract(a, mean));
        }

    }
}
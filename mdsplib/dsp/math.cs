using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace mdsplib.DSP
{    
    public static class MathExtension
    {
        public static Double[] Multiply(this Double[] a, Double[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] * b[i];

            return result;
        }

        public static double[] Slice(this double[] arrayin, uint start, uint length)
        {
            return arrayin.Skip((int)start).Take((int)length).ToArray();
        }
        
        public static Complex[] Multiply(this Complex[] a, Double b)
        {
            Complex[] result = new Complex[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = new Complex(a[i].Real * b, a[i].Imaginary * b);

            return result;
        }
        
        public static Double[] Multiply(this Double[] a, Double b)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] * b;

            return result;
        }
        
        public static Double[] Add(this Double[] a, Double[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] + b[i];

            return result;
        }

        public static Complex[] Add(this Complex[] a, Complex[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            Complex[] result = new Complex[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = new Complex(a[i].Real + b[i].Real, a[i].Imaginary + b[i].Imaginary);

            return result;
        }

        public static Double[] Add(this Double[] a, Double[] b, uint offset)
        {
            Debug.Assert(a.Length > (b.Length + offset), "a.Length < (b.Length + offset)");
            double[] result = (double[])a.Clone();
            for (UInt32 i = 0; i < b.Length; i++)
                result[i + offset] += b[i];

            return result;
        }
        
        public static Double[] Add(this Double[] a, Double b)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] + b;

            return result;
        }
        
        public static Double[] Subtract(this Double[] a, Double[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] - b[i];

            return result;
        }
        
        public static Double[] Subtract(this Double[] a, Double b)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] - b;

            return result;
        }
        
        public static Double[] Divide(this Double[] a, Double[] b)
        {
            Debug.Assert(a.Length == b.Length, "Length of arrays a[] and b[] must match.");

            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] / b[i];

            return result;
        }
        
        public static Double[] Divide(this Double[] a, Double b)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] / b;

            return result;
        }
        
        public static double[] Sqrt(this double[] a)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = System.Math.Sqrt(a[i]);

            return result;
        }
        
        public static double[] Square(this double[] a)
        {
            double[] result = new double[a.Length];
            for (UInt32 i = 0; i < a.Length; i++)
                result[i] = a[i] * a[i];

            return result;
        }
        
        public static double[] Log10(this double[] a)
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
        
        public static double[] RemoveMean(this double[] a)
        {
            double sum = 0.0;
            for (UInt32 i = 0; i < a.Length; i++)
                sum += a[i];

            double mean = sum / a.Length;

            return (a.Subtract(mean));
        }
        
        public static double[] Cast(this float[] buf)
        {
            return buf.Select(a => (double)a).ToArray();
        }

        public static double[] Normalize(this double[] val)
        {
            double max = val.Max();
            return val.Select(a => a / max).ToArray();
        }

        public static Complex[] Shift(this Complex[] val, int shift)
        {
            Complex[] result = new Complex[val.Length];
            Array.Copy(val, shift, result, 0, (shift > 0) ? val.Length - shift : val.Length);
            if (shift > 0)
                for (int i = 0; i < shift; i++)
                    val[i] = new Complex(0, 0);
            else
                for (int i = 0; i < val.Length; i++)
                    val[i] = new Complex(0, 0);

            return result;
        }

        public static List<Complex[]> Shift(List<Complex[]> val, int shift)
        {
            var result = new System.Collections.Generic.List<System.Numerics.Complex[]>();
            foreach (var slice in val)
                result.Add(Shift(slice, shift));

            return result;
        }
    }
}
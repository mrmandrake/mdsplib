using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using mdsplib.FT;

namespace mdsplib.DSP
{
    internal class Convolve
    {
        public static double[] Simple(double[] input, double[] ir)
        {
            if (ir.Length > input.Length)
                return Simple(ir, input);

            var result = new double[input.Length + ir.Length - 1];

            for (int n = 0; n < ir.Length; ++n)
                for (int m = 0; m <= n; ++m)
                    result[n] += ir[m] * input[n - m];

            for (int n = ir.Length; n < input.Length; ++n)
                for (int m = 0; m < ir.Length; ++m)
                    result[n] += ir[m] * input[n - m];

            for (int n = input.Length; n < input.Length + ir.Length - 1; ++n)
                for (int m = n - input.Length + 1; m < ir.Length; ++m)
                    result[n] += ir[m] * input[n - m];

            return result;
        }

        public static double[] Ola(double[] input, double[] ir, int len)
        {
            var fftConvolver = new FFTConvolver();
            fftConvolver.Init(len, ir);
            return null;
        }
    }

    public static class ConvolveExtension
    {
        public static double[] Convolution(this double[] val, double[] ir)
        {
            return Convolve.Simple(val, ir);
        }

        private static double[] OlaConvolution(this double[] val, double[] ir, int len)
        {
            return Convolve.Ola(val, ir, len);
        }
    }
}

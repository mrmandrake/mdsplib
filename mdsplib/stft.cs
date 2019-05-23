using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace mdsplib
{
    public class STFT
    {
        public static List<Complex[]> Direct(double[] wavein, DSP.Window.Type wnd = DSP.Window.Type.Hann, UInt32 wlength = 1024)
        {
            var wlength2 = wlength / 2;
            double numWindow = (wavein.Length / (wlength / 2)) - 1;
            double[] wCoeff = DSP.Window.Coefficients(wnd, wlength);
            List<Complex[]> result = new List<Complex[]>();

            // ciclo su array di campioni lunghi 1024 e sovrapposti al 50% tra loro
            for (uint i = 0; i < numWindow; i++)
            {
                double[] windowSlice = DSP.Math.Multiply(DSP.Math.Slice(wavein, i * wlength2, wlength), wCoeff);               
                result.Add(new FFT().Initialize(wlength).Direct(windowSlice));
            }
            return result;
        }

        public static double[] Inverse(List<Complex[]> stft) {
            int wlength = stft.First().Length;
            int totalLength = (stft.Count() + 1) * wlength / 2;

            var waveout = new double[totalLength];
            uint offset = 0;
            foreach (var spectrum in stft)
            {
                double[] slice = DSP.Convert.Complex.RealPart(new FFT().Initialize((uint)wlength).Inverse(spectrum));
                DSP.Math.Add(waveout, slice, offset);
                offset += (uint)slice.Length;
            }
            return null;
        }
    }
}
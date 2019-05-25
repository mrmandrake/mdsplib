using System;
using System.Numerics;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using mdsplib.DSP;

namespace mdsplib.FT
{
    public class STFT
    {
        public static List<Complex[]> Direct(double[] wavein, DSP.Window.Type wnd = DSP.Window.Type.Hann, UInt32 wlength = 1024)
        {
            var wlength2 = wlength / 2;
            double numWindow = (wavein.Length / (wlength / 2)) - 1;
            List<Complex[]> result = new List<Complex[]>();

            // ciclo su array di campioni lunghi 1024 e sovrapposti al 50% tra loro
            for (uint i = 0; i < numWindow; i++)
                result.Add(wavein.Slice(i * wlength2, wlength).Window(Window.Type.Hann).FFT());

            return result;
        }

        public static double[] Inverse(List<Complex[]> stft) {
            int wlength = stft.First().Length;
            int totalLength = (stft.Count() + 1) * wlength / 2;

            var waveout = new double[totalLength];
            uint offset = 0;
            foreach (var spectrum in stft)
            {
                double[] slice = spectrum.iFFT().RealPart();
                waveout = waveout.Add(slice, offset);
                offset += (uint)(slice.Length / 2);
            }
            return waveout;
        }
    }

    public static class STFTExtension
    {
        public static List<Complex[]> STFT(this double[] wavein, DSP.Window.Type wnd = DSP.Window.Type.Hann, UInt32 wlength = 1024)
        {
            return mdsplib.FT.STFT.Direct(wavein, wnd, wlength);
        }

        public static double[] iSTFT(this List<Complex[]> stft)
        {
            return mdsplib.FT.STFT.Inverse(stft);
        }
    }
}
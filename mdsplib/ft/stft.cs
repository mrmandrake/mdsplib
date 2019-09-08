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
        public static List<Complex[]> Direct(double[] wavein, DSP.Window.Type wnd = DSP.Window.Type.Hann, UInt32 wlength = 1024, UInt32 fftPad = 0)
        {
            var wlength2 = wlength / 2;
            double numWindow = (wavein.Length / (wlength / 2)) - 1;
            List<Complex[]> result = new List<Complex[]>();

            // ciclo su array di campioni lunghi wlength e sovrapposti al 50% tra loro
            for (uint i = 0; i < numWindow; i++)
                result.Add(wavein.Slice(i * wlength2, wlength).Window(Window.Type.Hann).FFT(fftPad));

            return result;
        }

        public static double[] Inverse(List<Complex[]> stft, UInt32 wlen, UInt32 fftPad) {
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

        public static double[] InverseNoOverlap(List<Complex[]> stft, UInt32 wlen, UInt32 fftPad)
        {
            int wlength = stft.First().Length;
            int totalLength = stft.Count() * (int)wlen;

            var waveout = new double[totalLength];
            uint offset = 0;
            foreach (var spectrum in stft)
            {
                double[] slice = spectrum.iFFT().RealPart();
                waveout = waveout.Add(slice.Slice(0, wlen), offset);
                offset += wlen;
            }
            return waveout;
        }
    }

    public static class STFTExtension
    {
        public static List<Complex[]> STFT(this double[] wavein, DSP.Window.Type wnd = DSP.Window.Type.Hann, UInt32 wlength = 1024, UInt32 fftpad = 0)
        {
            return mdsplib.FT.STFT.Direct(wavein, wnd, wlength, fftpad);
        }

        public static Complex[] Bin(this List<Complex[]> stft, UInt32 bin)
        {
            if (stft.Count <= 0 || bin > stft.First().Length)
                return null;

            var result = new Complex[stft.First().Length];
            for (int i = 0; i < stft.Count(); i++)
                result[i] = stft[i][bin];

            return result;
        }

        public static double[] iSTFT(this List<Complex[]> stft, UInt32 wlength = 1024, UInt32 fftpad = 0)
        {
            return mdsplib.FT.STFT.Inverse(stft, wlength, fftpad);
        }

        public static double[] iSTFTNoOverlap(this List<Complex[]> stft, UInt32 wlength = 1024, UInt32 fftpad = 0)
        {
            return mdsplib.FT.STFT.InverseNoOverlap(stft, wlength, fftpad);
        }

        public static double[,] Magnitude(this List<Complex[]> stft)
        {
            double[,] mag = new double[stft.First().Length / 2, stft.Count()];
            for (int j = 0; j < stft.Count(); j++)
            {
                Complex[] s = stft[j];
                for (int i = 0; i < s.Length / 2; i++)
                    mag[j, i] = s[i].Magnitude;
            }
            return mag;
        }

        public static double[,] Phase(this List<Complex[]> stft)
        {
            double[,] phase = new double[stft.First().Length / 2, stft.Count()];
            for (int j = 0; j < stft.Count(); j++)
            {
                Complex[] s = stft[j];
                for (int i = 0; i < s.Length / 2; i++)
                    phase[j, i] = s[i].Phase;
            }
            return phase;
        }
    }
}
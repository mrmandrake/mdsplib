using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace mdsplib.Audio {
    public class Note {

        const UInt16 _OCTAVES = 9;
        const UInt16 _NOTES_OCTAVE = 12;
        const double _A4_FREQ = 440.0;
        const UInt16 _A4_KEY = 49;

        public enum NoteEnum
        {
            c = 1, 
            csharp_dflat = 2, 
            d = 3, 
            dsharp_eflat = 4,
            e = 5,
            f = 6,
            fsharp_gflat = 7,
            g = 8,
            gsharp_aflat = 9,
            a = 10,
            asharp_bflat = 11,
            b = 12
        }

        public static UInt32 ToKey(NoteEnum note, UInt32 octave)
        {
            return  _NOTES_OCTAVE * octave + ((UInt32)note);
        }

        public static Tuple<NoteEnum, UInt32> FromKey(UInt32 key)
        {
            return new Tuple<NoteEnum, UInt32>((NoteEnum)(key % _NOTES_OCTAVE), key / _NOTES_OCTAVE);
        }

        public static double ToFreq(UInt32 n)
        {
            return System.Math.Pow(2, (double)(n - _A4_KEY) / 12.0) * _A4_FREQ;
        }

        public static UInt32 FromFreq(double f)
        {
            return (UInt32)(12.0 * System.Math.Log(f / _A4_FREQ)) + _A4_KEY;
        }
    };
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;
using System.Linq;
using System.Diagnostics;

namespace mdsplib.Audio
{
    /// <summary>
    /// Note Frequency helper
    /// </summary>

    public class Note
    {

        const UInt16 _OCTAVES = 9;
        const UInt16 _NOTES_OCTAVE = 12;
        const double _A4_FREQ = 440.0;
        const UInt16 _A4_KEY = 49;

        public enum NoteEnum
        {
            C, CSharp, DFlat, D, DSharp, EFlat, E, F, FSharp, GFlat, G, GSharp, AFlat, A, ASharp, BFlat, B
        }

        public static Dictionary<NoteEnum, uint> NoteNumber = new Dictionary<NoteEnum, uint>() {
            { NoteEnum.C, 1 },
            { NoteEnum.CSharp, 2 },
            { NoteEnum.DFlat,3 },
            { NoteEnum.D,3 },
            { NoteEnum.DSharp,4 },
            { NoteEnum.EFlat,5 },
            { NoteEnum.E,6 },
            { NoteEnum.F,7 },
            { NoteEnum.FSharp,8 },
            { NoteEnum.GFlat,9 },
            { NoteEnum.G,10 },
            { NoteEnum.GSharp,11 },
            { NoteEnum.AFlat,12 },
            { NoteEnum.A,13 },
            { NoteEnum.ASharp,14 },
            { NoteEnum.BFlat,15 },
            { NoteEnum.B,16 }
        };

        public static UInt32 ToKey(NoteEnum note, UInt32 octave)
        {
            return _NOTES_OCTAVE * octave + NoteNumber[note];
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

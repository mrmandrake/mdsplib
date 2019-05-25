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

        public static Dictionary<NoteEnum, UInt16> NoteNumber = new Dictionary<NoteEnum, UInt16>() {
            { NoteEnum.C,       1 },
            { NoteEnum.CSharp,  2 },
            { NoteEnum.DFlat,   2 },
            { NoteEnum.D,       3 },
            { NoteEnum.DSharp,  4 },
            { NoteEnum.EFlat,   4 },
            { NoteEnum.E,       5 },
            { NoteEnum.F,       6 },
            { NoteEnum.FSharp,  7 },
            { NoteEnum.GFlat,   7 },
            { NoteEnum.G,       8 },
            { NoteEnum.GSharp,  9 },
            { NoteEnum.AFlat,   9 },
            { NoteEnum.A,       10 },
            { NoteEnum.ASharp,  11 },
            { NoteEnum.BFlat,   11 },
            { NoteEnum.B,       12 }
        };

        private NoteEnum _note { get; set; }
        private UInt16 _octave { get; set; }

        Note(NoteEnum note, UInt16 octave)
        {
            _note = note;
            _octave = octave;
        }

        public UInt16 ToKey()
        {
            return (UInt16)((_NOTES_OCTAVE * _octave) + NoteNumber[_note]);
        }

        public static Note FromKey(UInt16 key)
        {
            return new Note((NoteEnum)(key % _NOTES_OCTAVE), (UInt16)(key / _NOTES_OCTAVE));
        }

        public double ToFreq(UInt16 n)
        {
            return System.Math.Pow(2, (double)(n - _A4_KEY) / 12.0) * _A4_FREQ;
        }

        public static UInt32 KeyFromFreq(double f)
        {
            return (UInt32)(12.0 * System.Math.Log(f / _A4_FREQ)) + _A4_KEY;
        }
    };
}

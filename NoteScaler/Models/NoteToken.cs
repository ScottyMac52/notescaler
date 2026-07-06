namespace NoteScaler.Models
{
	using NoteScaler.Enums;
	using System;
	using System.Linq;

	public sealed class NoteToken
	{
		private const string SHARP_NOTE = "#";
		private const string FLAT_NOTE = "b";

		private NoteToken(string name, int octave, bool hasOctave, ToneTypes toneType)
		{
			Name = name;
			Octave = octave;
			HasOctave = hasOctave;
			ToneType = toneType;
		}

		public string Name { get; }
		public int Octave { get; }
		public bool HasOctave { get; }
		public ToneTypes ToneType { get; }

		public static NoteToken Parse(string token)
		{
			if (token == null)
			{
				throw new ArgumentNullException(nameof(token));
			}

			var name = new string(token.Where(ch => !char.IsDigit(ch)).ToArray());
			var octaveString = new string(token.Where(ch => char.IsDigit(ch)).ToArray());
			var hasOctave = !string.IsNullOrEmpty(octaveString);
			var octave = hasOctave ? int.Parse(octaveString) : 0;
			return new NoteToken(name, octave, hasOctave, GetToneType(name));
		}

		private static ToneTypes GetToneType(string name)
		{
			if (name.Contains(SHARP_NOTE))
			{
				return ToneTypes.Sharp;
			}

			if (name.Contains(FLAT_NOTE))
			{
				return ToneTypes.Flat;
			}

			return ToneTypes.Natural;
		}
	}
}

namespace NoteScaler.Classes
{
	using NoteScaler.Enums;
	using System.Collections.Generic;
	using System.Linq;

	public class Guitar
	{
		public Guitar(int numberofStrings = 6, TuningScheme scheme = TuningScheme.Standard, int numberofFrets = 24)
		{
			NumberOfStrings = numberofStrings;
			TuningScheme = scheme;
			Frets = numberofFrets; 
			InitializeStrings();
		}

		public TuningScheme TuningScheme { get; }
		public int Frets { get; }
		public int NumberOfStrings { get; }

		public IEnumerable<GuitarString> Strings { get; protected set; }

		public void CustomTune(int stringNumber, string noteToTune)
		{
			var stringToTune = Strings?.SingleOrDefault(str => str.Number.Equals(stringNumber));
			if(stringToTune != null)
			{
				stringToTune.SetTuning(noteToTune);
			}
		}

		public string GetNote(int stringNumber, int fret)
		{
			return Strings?.SingleOrDefault(str => str.Number.Equals(stringNumber))?.FretToNote[fret];
		}

		public int GetNote(int stringNumber, string note)
		{
			return Strings?.SingleOrDefault(str => str.Number.Equals(stringNumber))?.NoteToFret[note] ?? -1;
		}

		private void InitializeStrings()
		{
			switch(TuningScheme)
			{
				case TuningScheme.Standard:
					Strings = new List<GuitarString>()
					{
						new GuitarString(1, "E", Frets, 4),
						new GuitarString(2, "B", Frets, 4),
						new GuitarString(3, "G", Frets, 3),
						new GuitarString(4, "D", Frets, 3),
						new GuitarString(5, "A", Frets, 3),
						new GuitarString(6, "E", Frets, 2)
					};
					break;

				case TuningScheme.DropC:
					Strings = new List<GuitarString>()
					{
						new GuitarString(1, "D", Frets, 4),
						new GuitarString(2, "A", Frets, 4),
						new GuitarString(3, "F", Frets, 3),
						new GuitarString(4, "C", Frets, 3),
						new GuitarString(5, "G", Frets, 3),
						new GuitarString(6, "C", Frets, 2)
					};
					break;

				case TuningScheme.DropCSharp:
					Strings = new List<GuitarString>()
					{
						new GuitarString(1, "D#", Frets, 4),
						new GuitarString(2, "A#", Frets, 4),
						new GuitarString(3, "F#", Frets, 3),
						new GuitarString(4, "C#", Frets, 3),
						new GuitarString(5, "G#", Frets, 3),
						new GuitarString(6, "C#", Frets, 2)
					};
					break;

				case TuningScheme.DropD:
					Strings = new List<GuitarString>()
					{
						new GuitarString(1, "D", Frets, 4),
						new GuitarString(2, "B", Frets, 4),
						new GuitarString(3, "G", Frets, 3),
						new GuitarString(4, "D", Frets, 3),
						new GuitarString(5, "A", Frets, 3),
						new GuitarString(6, "D", Frets, 2)
					};
					break;
			}
		}

	}
}

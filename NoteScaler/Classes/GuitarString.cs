namespace NoteScaler.Classes
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class GuitarString
	{
		private static string[] NOTES_LIST = new string[] { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C1", "C#1", "D1", "D#1", "E1", "F1", "F#1", "G1", "G#1", "A1", "A#1", "B1", "C2", "C#2", "D2", "D#2", "E2", "F2", "F#2", "G2", "G#2", "A2", "A#2", "B2" };

		public GuitarString(int number, string tuning, int frets, int octave)
		{
			Number = number;
			Tuning = tuning;
			MaxFret = frets;
			Octave = octave;
			NoteToFret = new Dictionary<string, int>();
			FretToNote = new Dictionary<int, string>();
			InitializeFretBoard();
		}

		public int Number { get; }
		public string Tuning { get; protected set; }
		public int MaxFret { get; }
		public int Octave { get; protected set; }
		public Dictionary<string, int> NoteToFret { get; protected set; }

		public void SetTuning(string noteToTune)
		{
			Tuning = noteToTune;
			InitializeFretBoard();
		}

		public Dictionary<int, string> FretToNote { get; private set; }

		private void InitializeFretBoard()
		{
			var startingNote = Array.FindIndex(NOTES_LIST, note => note.Equals(Tuning));
			var startingIndex = 0;
			var firstSegmentTake = NOTES_LIST.Length - startingNote;
			var secondSegmentTake = NOTES_LIST.Length - firstSegmentTake;
			var notes = NOTES_LIST.Skip(startingNote).Take(firstSegmentTake).Concat(NOTES_LIST.Take(secondSegmentTake)).Take(MaxFret+1);
			string lastNote = null;
			var currentOctave = Octave;
			foreach(var note in notes)
			{
				var currentNote = note;
				if(lastNote?.Contains("B") ?? false)
				{
					currentOctave++;
				}
				currentNote = new String(note.Where(ch => !char.IsDigit(ch)).ToArray());
				var finalNote = $"{currentNote}{currentOctave}";
				NoteToFret.Add(finalNote, startingIndex);
				FretToNote.Add(startingIndex, finalNote);
				startingIndex++;
				lastNote = finalNote;
			}
		}
	}
}
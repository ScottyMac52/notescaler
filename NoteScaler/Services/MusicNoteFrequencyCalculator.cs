namespace NoteScaler.Services
{
	using System;
	using System.Linq;

	public sealed class MusicNoteFrequencyCalculator
	{
		private const double NOTE_REF = 1.059463;
		private static readonly string[] NotesSharp = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C" };

		public float[] CalculateFrequencies(int noteIndex, int reference)
		{
			if (noteIndex < 0 || noteIndex >= NotesSharp.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(noteIndex), noteIndex, "Note index is outside the supported note range.");
			}

			var frequencyList = GetFrequenciesFromReference(reference);
			return new float[]
			{
				frequencyList[noteIndex] * (float)Math.Pow(2, -4),
				frequencyList[noteIndex] * (float)Math.Pow(2, -3),
				frequencyList[noteIndex] * (float)Math.Pow(2, -2),
				frequencyList[noteIndex] * (float)Math.Pow(2, -1),
				frequencyList[noteIndex] * (float)Math.Pow(2, 0),
				frequencyList[noteIndex] * (float)Math.Pow(2, 1),
				frequencyList[noteIndex] * (float)Math.Pow(2, 2),
				frequencyList[noteIndex] * (float)Math.Pow(2, 3),
				frequencyList[noteIndex] * (float)Math.Pow(2, 4),
				frequencyList[noteIndex] * (float)Math.Pow(2, 5),
				frequencyList[noteIndex] * (float)Math.Pow(2, 6),
				frequencyList[noteIndex] * (float)Math.Pow(2, 7)
			};
		}

		private static float[] GetFrequenciesFromReference(int reference)
		{
			var frequencyList = new float[NotesSharp.Length];
			var a4Position = Array.IndexOf(NotesSharp, "A");
			NotesSharp.ToList().ForEach(note =>
			{
				var currentIndex = Array.IndexOf(NotesSharp, note);
				double index = currentIndex - a4Position;
				if (index == 0)
				{
					frequencyList[currentIndex] = reference;
				}
				else
				{
					frequencyList[currentIndex] = (float)(reference * Math.Pow(NOTE_REF, index));
				}
			});
			frequencyList[^1] = (float)(reference * Math.Pow(NOTE_REF, 3));
			return frequencyList;
		}
	}
}

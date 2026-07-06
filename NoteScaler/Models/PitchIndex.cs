namespace NoteScaler.Models
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;

	public sealed class PitchIndex
	{
		private static readonly string[] ChromaticNotes = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };

		private readonly IReadOnlyDictionary<string, int> noteIndexes;

		private PitchIndex(IReadOnlyList<string> notes)
		{
			Notes = new ReadOnlyCollection<string>(notes.ToArray());
			noteIndexes = Notes
				.Select((note, index) => new { note, index })
				.ToDictionary(item => item.note, item => item.index);
		}

		public static PitchIndex Default { get; } = CreateDefault();

		public IReadOnlyList<string> Notes { get; }

		public bool Contains(string note)
		{
			return !string.IsNullOrWhiteSpace(note) && noteIndexes.ContainsKey(note);
		}

		public int GetIndex(string note)
		{
			if (!Contains(note))
			{
				throw new ArgumentException($"Unsupported note: {note}", nameof(note));
			}

			return noteIndexes[note];
		}

		public int GetInterval(string startNote, string endNote)
		{
			return GetIndex(endNote) - GetIndex(startNote);
		}

		public string GetNoteAbove(string note, int semitones)
		{
			var targetIndex = GetIndex(note) + semitones;
			if (targetIndex < 0 || targetIndex >= Notes.Count)
			{
				throw new ArgumentOutOfRangeException(nameof(semitones), semitones, $"The requested note is outside the supported pitch range for {note}.");
			}

			return Notes[targetIndex];
		}

		public IEnumerable<string> GetNotesStartingAt(string note, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), count, "Count cannot be negative.");
			}

			return Notes.Skip(GetIndex(note)).Take(count);
		}

		private static PitchIndex CreateDefault()
		{
			var notes = Enumerable.Range(0, 9)
				.SelectMany(octave => ChromaticNotes.Select(note => $"{note}{octave}"))
				.ToArray();

			return new PitchIndex(notes);
		}
	}
}

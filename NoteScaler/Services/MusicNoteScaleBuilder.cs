namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class MusicNoteScaleBuilder
	{
		private const int RELATIVE_MINOR_POSITION = 5;
		private const string B_NOTE = "B";
		private const string C_NOTE = "C";
		private const string E_NOTE = "E";
		private const string WHOLE_STEP = "W";
		private const string HALF_STEP = "H";
		private const char Separator = ',';

		private static readonly string[] NotesFlat = { "C", "Db", "D", "Eb", "E", "F", "Gb", "G", "Ab", "A", "Bb", "B", "C" };
		private static readonly string[] NotesSharp = { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B", "C" };
		private static readonly string MAJOR_SCALE = $"{WHOLE_STEP},{WHOLE_STEP},{HALF_STEP},{WHOLE_STEP},{WHOLE_STEP},{WHOLE_STEP},{HALF_STEP}";
		private static readonly string MINOR_SCALE = $"{WHOLE_STEP},{HALF_STEP},{WHOLE_STEP},{WHOLE_STEP},{HALF_STEP},{WHOLE_STEP},{WHOLE_STEP}";

		public MusicNoteScaleContext GetNoteContext(string note)
		{
			var sharpNotes = NotesSharp.ToList();
			var flatNotes = NotesFlat.ToList();
			var currentSharpNoteIndex = sharpNotes.IndexOf(note);
			var currentFlatNoteIndex = flatNotes.IndexOf(note);
			var currentNoteIndex = currentSharpNoteIndex == -1 ? currentFlatNoteIndex : currentSharpNoteIndex;
			if (currentNoteIndex == -1)
			{
				throw new ArgumentException("Unable to find referenced note!", note);
			}

			if (currentNoteIndex > 0)
			{
				RotateNotes(currentNoteIndex, ref sharpNotes, ref flatNotes);
			}

			return new MusicNoteScaleContext(currentNoteIndex, sharpNotes, flatNotes);
		}

		public IEnumerable<string> BuildMajorScale(string note, ToneTypes toneType)
		{
			var context = GetNoteContext(note);
			return BuildMajorScale(note, toneType, context);
		}

		public IEnumerable<string> BuildMajorScale(string note, ToneTypes toneType, MusicNoteScaleContext context)
		{
			return BuildScale(note, true, toneType == ToneTypes.Flat, context.FlatNotes, context.SharpNotes);
		}

		public IEnumerable<string> BuildMinorScale(string note, ToneTypes toneType)
		{
			var context = GetNoteContext(note);
			return BuildMinorScale(note, toneType, context);
		}

		public IEnumerable<string> BuildMinorScale(string note, ToneTypes toneType, MusicNoteScaleContext context)
		{
			return BuildScale(note, false, toneType == ToneTypes.Flat, context.FlatNotes, context.SharpNotes);
		}

		public IEnumerable<string> BuildRelativeMinorScale(IEnumerable<string> majorScale, ToneTypes toneType)
		{
			var relativeMinorNote = majorScale.ElementAt(RELATIVE_MINOR_POSITION);
			if (relativeMinorNote.Any(ch => char.IsDigit(ch)))
			{
				relativeMinorNote = new string(relativeMinorNote.Where(ch => !char.IsDigit(ch)).ToArray());
			}

			return BuildMinorScale(relativeMinorNote, toneType);
		}

		private static void RotateNotes(int currentNoteIndex, ref List<string> sharpNotes, ref List<string> flatNotes)
		{
			var takeFlat = flatNotes.Count - currentNoteIndex - 1;
			var notes = flatNotes.Skip(currentNoteIndex).Take(takeFlat).ToList();
			notes.AddRange(flatNotes.Take(currentNoteIndex + 1).Select(note => $"{note}"));
			flatNotes = notes;

			var takeSharp = sharpNotes.Count - currentNoteIndex - 1;
			notes = sharpNotes.Skip(currentNoteIndex).Take(takeSharp).ToList();
			notes.AddRange(sharpNotes.Take(currentNoteIndex + 1).Select(note => $"{note}"));
			sharpNotes = notes;
		}

		private static IEnumerable<string> BuildScale(string note, bool isMajor, bool useFlats, IEnumerable<string> flatNotes, IEnumerable<string> sharpNotes)
		{
			var scaleNotes = useFlats ? flatNotes.ToList() : sharpNotes.ToList();
			int currentOctave = 0;
			var noteList = new List<string> { $"{note}{currentOctave}" };
			int currentNotesInScale = 0;
			int notesCounted = 0;
			var currentScale = isMajor ? MAJOR_SCALE.Split(Separator) : MINOR_SCALE.Split(Separator);
			while (noteList.Count() < 8)
			{
				int index = 1;
				if (currentScale[currentNotesInScale] == WHOLE_STEP && (scaleNotes[notesCounted] != B_NOTE || scaleNotes[notesCounted] != E_NOTE))
				{
					index = 2;
				}
				notesCounted += index;
				var newNote = scaleNotes.ElementAt(notesCounted);
				if (newNote.Contains(C_NOTE) && currentOctave == 0)
				{
					currentOctave++;
				}
				noteList.Add($"{newNote}{currentOctave}");
				currentNotesInScale++;
			}
			return noteList;
		}
	}
}

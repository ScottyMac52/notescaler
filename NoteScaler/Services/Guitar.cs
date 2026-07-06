namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public class Guitar : IStringInstrument
	{
		private static readonly IReadOnlyDictionary<TuningScheme, string[]> TuningDefinitions = new Dictionary<TuningScheme, string[]>
		{
			[TuningScheme.Standard] = new[] { "E4", "B3", "G3", "D3", "A2", "E2" },
			[TuningScheme.DropC] = new[] { "D4", "A3", "F3", "C3", "G2", "C2" },
			[TuningScheme.DropCSharp] = new[] { "D#4", "A#3", "F#3", "C#3", "G#2", "C#2" },
			[TuningScheme.DropD] = new[] { "D4", "B3", "G3", "D3", "A2", "D2" },
			[TuningScheme.OpenC] = new[] { "E4", "C3", "G3", "C3", "G2", "C2" },
			[TuningScheme.OpenD] = new[] { "D4", "A3", "F#3", "D3", "A2", "D2" }
		};

		/// <summary>
		/// Copnstructs a Guitar!
		/// </summary>
		/// <param name="scheme"></param>
		/// <param name="numberofFrets"></param>
		public Guitar(TuningScheme scheme = TuningScheme.Standard, int numberofFrets = 24)
		{
			TuningScheme = scheme;
			Frets = numberofFrets;
			InitializeStrings();
		}

		/// <inheritdoc/>
		public TuningScheme TuningScheme { get; }
		/// <inheritdoc/>
		public int Frets { get; }
		/// <inheritdoc/>
		public IEnumerable<IInstrumentString> Strings { get; protected set; }
		/// <inheritdoc/>
		public void CustomTune(int stringNumber, string noteToTune)
		{
			var stringToTune = Strings?.SingleOrDefault(str => str.Number.Equals(stringNumber));
			if (stringToTune != null)
			{
				stringToTune.SetTuning(noteToTune);
			}
		}
		/// <inheritdoc/>
		public string GetNote(int stringNumber, int fret)
		{
			return Strings?.SingleOrDefault(str => str.Number.Equals(stringNumber))?.GetNote(fret);
		}

		/// <inheritdoc/>
		public int GetNoteInterval(int startString, int startFret, int endString, int endFret, out List<Exception> errorList)
		{
			IInstrumentString startStringRef = null;
			IInstrumentString endStringRef = null;
			errorList = new List<Exception>();
			if (!Strings.Any(str => str.Number == startString || str.Number == endString))
			{
				if (!Strings.Any(str => str.Number == startString))
				{
					errorList.Add(new ArgumentException($"There is no string number: {startString}", nameof(GuitarString.Number)));
				}
				if (!Strings.Any(str => str.Number == endString))
				{
					errorList.Add(new ArgumentException($"There is no string number: {endString}", nameof(GuitarString.Number)));
				}
			}
			else
			{
				startStringRef = Strings.Single(str => str.Number.Equals(startString));
				endStringRef = Strings.Single(str => str.Number.Equals(endString));
				if (startFret > startStringRef.MaxFret)
				{
					errorList.Add(new ArgumentException($"Fret: {startFret} doesn't exist on String: {startString}.", nameof(GuitarString.MaxFret)));
				}
				if (endFret > endStringRef.MaxFret)
				{
					errorList.Add(new ArgumentException($"Fret: {endFret} doesn't exist on String: {endString}.", nameof(GuitarString.MaxFret)));
				}
			}
			if (errorList.Count() == 0)
			{
				var startNote = startStringRef.GetNote(startFret);
				var endNote = endStringRef.GetNote(endFret);
				return GetNoteInterval(startNote, endNote);
			}
			else
			{
				return 0;
			}
		}

		/// <inheritdoc/>
		public int GetNoteInterval(string startNote, string endNote)
		{
			return PitchIndex.Default.GetInterval(startNote, endNote);
		}

		/// <inheritdoc/>
		public int GetNote(int stringNumber, string note)
		{
			var stringRef = Strings?.SingleOrDefault(str => str.Number.Equals(stringNumber));
			if (stringRef == null)
			{
				throw new ArgumentException($"There is no string {stringNumber}.", nameof(stringNumber));
			}
			var noteRef = stringRef.GetNote(note);
			if (noteRef == GuitarString.NOTE_NOT_FOUND)
			{
				var stringNotes = string.Join(',', stringRef.GetNotesInString());
				throw new ArgumentException($"String {stringNumber} does not contain the note: {note}. The string does contain: {stringNotes}", nameof(note));
			}
			return noteRef;
		}

		/// <summary>
		/// Human readable Guitar
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return $"Guitar tuned to {TuningScheme}. Strings {string.Join('|', Strings.Reverse().Select(s => s.Tuning))}";
		}

		private void InitializeStrings()
		{
			Strings = TuningDefinitions[TuningScheme]
				.Select((note, index) => new GuitarString(index + 1, note, Frets))
				.ToList();
		}
	}
}
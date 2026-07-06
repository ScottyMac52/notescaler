namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class MusicNoteFactory : IMusicNoteFactory
	{
		private const int RELATIVE_MINOR_POSITION = 5;
		private const int RELATIVE_MAJOR_POSITION = 2;
		private const int FIRST = 0;
		private const int THIRD = 2;
		private const int FIFTH = 4;
		private const int SEVENTH = 6;
		private const int NINTH = 1;
		private const int ELEVENTH = 3;
		private const int THIRTEENTH = 5;
		private const int FIFTEENTH = 7;

		private readonly IMusicNoteCache cache;
		private readonly MusicNoteScaleBuilder scaleBuilder;
		private readonly MusicNoteFrequencyCalculator frequencyCalculator;

		public MusicNoteFactory(IMusicNoteCache cache, MusicNoteScaleBuilder scaleBuilder, MusicNoteFrequencyCalculator frequencyCalculator)
		{
			this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
			this.scaleBuilder = scaleBuilder ?? throw new ArgumentNullException(nameof(scaleBuilder));
			this.frequencyCalculator = frequencyCalculator ?? throw new ArgumentNullException(nameof(frequencyCalculator));
		}

		public MusicNote Create(string note, int a4Reference = 440, IPlayer player = null, int duration = 500, InstrumentType currentInstrument = default, ChordType chordType = ChordType.Note)
		{
			if (cache.TryGet(note, a4Reference, out var cachedNote))
			{
				ApplyRuntimeOptions(cachedNote, player, duration, currentInstrument, chordType);
				return cachedNote;
			}

			try
			{
				var musicNote = CreateInitializedNote(note, a4Reference);
				ApplyRuntimeOptions(musicNote, player, duration, currentInstrument, chordType);
				cache.Add(note, a4Reference, musicNote);
				MusicNote.RaiseFactoryCreateNote(musicNote);
				return musicNote;
			}
			catch (Exception ex)
			{
				MusicNote.RaiseFactoryError(this, ex);
				return null;
			}
		}

		private MusicNote CreateInitializedNote(string note, int a4Reference)
		{
			var token = NoteToken.Parse(note);
			var context = scaleBuilder.GetNoteContext(token.Name);
			var flatNotes = context.FlatNotes.ToArray();
			var sharpNotes = context.SharpNotes.ToArray();
			var frequencies = frequencyCalculator.CalculateFrequencies(context.NoteIndex, a4Reference);
			var minorScale = scaleBuilder.BuildMinorScale(token.Name, token.ToneType, context).ToArray();
			var majorScale = scaleBuilder.BuildMajorScale(token.Name, token.ToneType, context).ToArray();
			var relativeMinor = majorScale.ElementAt(RELATIVE_MINOR_POSITION);
			var relativeMajor = majorScale.ElementAt(RELATIVE_MAJOR_POSITION);
			var allNotes = GetAllNotes(token.ToneType, flatNotes, sharpNotes);
			var noteBefore = GetNoteBefore(token.Name, allNotes);
			var noteAfter = GetNoteAfter(token.Name, allNotes);
			var minorNoteBefore = GetNoteBefore(token.Name, minorScale);
			var minorNoteAfter = GetNoteAfter(token.Name, minorScale);
			var majorNoteBefore = GetNoteBefore(token.Name, majorScale);
			var majorNoteAfter = GetNoteAfter(token.Name, majorScale);
			var minorChord = GetChord(minorScale);
			var majorChord = GetChord(majorScale);
			var relativeMinorScale = scaleBuilder.BuildRelativeMinorScale(majorScale, token.ToneType).ToArray();

			return new MusicNote(
				token.Name,
				a4Reference,
				token.ToneType,
				token.Octave,
				flatNotes,
				sharpNotes,
				frequencies,
				minorScale,
				majorScale,
				relativeMinorScale,
				relativeMinor,
				relativeMajor,
				noteBefore,
				noteAfter,
				minorNoteBefore,
				minorNoteAfter,
				majorNoteBefore,
				majorNoteAfter,
				majorChord,
				minorChord);
		}

		private static void ApplyRuntimeOptions(MusicNote musicNote, IPlayer player, int duration, InstrumentType currentInstrument, ChordType chordType)
		{
			musicNote.Duration = duration;
			musicNote.Instrument = currentInstrument;
			musicNote.NotePlayer = player;
			musicNote.ChordType = chordType;
		}

		private static IEnumerable<string> GetAllNotes(ToneTypes toneType, IEnumerable<string> flatNotes, IEnumerable<string> sharpNotes)
		{
			return toneType == ToneTypes.Sharp ? sharpNotes : flatNotes;
		}

		private static string GetNoteBefore(string key, IEnumerable<string> noteList)
		{
			var notes = noteList.ToArray();
			var currentPosition = Array.FindIndex(notes, note => note.Contains(key));
			var targetPosition = (currentPosition + notes.Count() - 2) % (notes.Count() - 1);
			return notes.ElementAt(targetPosition);
		}

		private static string GetNoteAfter(string key, IEnumerable<string> noteList)
		{
			var notes = noteList.ToArray();
			var currentPosition = Array.FindIndex(notes, note => note.Contains(key));
			var targetPosition = (currentPosition + 1) % notes.Count();
			return notes.ElementAt(targetPosition);
		}

		private static string[] GetChord(IReadOnlyList<string> noteList)
		{
			return new[]
			{
				noteList[FIRST],
				noteList[THIRD],
				noteList[FIFTH],
				noteList[SEVENTH],
				noteList[NINTH],
				noteList[ELEVENTH],
				noteList[THIRTEENTH],
				noteList[FIFTEENTH]
			};
		}
	}
}

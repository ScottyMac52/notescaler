namespace NoteScaler.Services
{
	using CommandLine;
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	public sealed class NoteScalerRunner : INoteScalerRunner
	{
		private const int MINIMUM_SCALE_STARTING_OCTAVE = 0;
		private const int MAXIMUM_SCALE_STARTING_OCTAVE = 10;

		private readonly ICommandLineOptionsService commandLineOptionsService;
		private readonly IPlayableSequenceFactory playableSequenceFactory;
		private readonly IStringInstrumentFactory stringInstrumentFactory;
		private readonly IConsoleOutputService consoleOutputService;

		public NoteScalerRunner(
			ICommandLineOptionsService commandLineOptionsService,
			IPlayableSequenceFactory playableSequenceFactory,
			IStringInstrumentFactory stringInstrumentFactory,
			IConsoleOutputService consoleOutputService)
		{
			this.commandLineOptionsService = commandLineOptionsService;
			this.playableSequenceFactory = playableSequenceFactory;
			this.stringInstrumentFactory = stringInstrumentFactory;
			this.consoleOutputService = consoleOutputService;
		}

		public int Run(string[] args)
		{
			commandLineOptionsService.ParseArguments(args)
				.WithParsed(Run);

			return 0;
		}

		private void Run(NoteScalerOptions options)
		{
			MusicNote.FactoryError += MusicNote_FactoryError;
			MusicNote.FactoryCreateNote += MusicNote_FactoryCreateNote;

			try
			{
				InitializeNoteScalerOptions(options, out var a4Reference, out var key, out var fileName, out var tabName);
				var playableSequence = playableSequenceFactory.Create(options, a4Reference);
				playableSequence.PlayableSequenceEvent += PlayableSequence_PlayableSequenceEvent;

				PlayNoteAsRequired(options.Note, options.Octave.GetValueOrDefault(), a4Reference, playableSequence);
				PlayTabAsRequired(tabName, playableSequence);
				PlaySongAsRequired(key, fileName, playableSequence);
			}
			finally
			{
				MusicNote.FactoryError -= MusicNote_FactoryError;
				MusicNote.FactoryCreateNote -= MusicNote_FactoryCreateNote;
			}
		}

		private void MusicNote_FactoryCreateNote(object sender, EventArgs e)
		{
			if (sender is MusicNote musicNote)
			{
				consoleOutputService.WriteMessage($"Created: {musicNote.Key}{musicNote.DesiredOctave}", ConsoleColor.Green);
			}
		}

		private void MusicNote_FactoryError(object sender, Exception e)
		{
			consoleOutputService.WriteMessage($"{e}", ConsoleColor.Red);
		}

		private void PlayableSequence_PlayableSequenceEvent(object sender, PlayableSequenceEvent e)
		{
			var textColor = ConsoleColor.White;
			switch (e.EventType)
			{
				case PlayableEventType.StartSequence:
				case PlayableEventType.CreateNote:
					textColor = ConsoleColor.Blue;
					break;
				case PlayableEventType.SettingSequence:
				case PlayableEventType.StopSequence:
					textColor = ConsoleColor.Yellow;
					break;
				case PlayableEventType.PlayingNote:
					textColor = ConsoleColor.Green;
					break;
				case PlayableEventType.Error:
					textColor = ConsoleColor.Red;
					break;
				case PlayableEventType.SequenceLoaded:
					textColor = ConsoleColor.White;
					break;
			}
			consoleOutputService.WriteMessage($"Event: {e.EventType} -> {e.EventDetails}", textColor);
		}

		private void InitializeNoteScalerOptions(NoteScalerOptions options, out int a4Reference, out string key, out string fileName, out string tabName)
		{
			key = options.Key;
			fileName = options.File;
			tabName = options.Tab;
			var pause = options.Speed.GetValueOrDefault() * options.PreWait.GetValueOrDefault();
			a4Reference = options.Range.GetValueOrDefault();
			if (pause > 0)
			{
				consoleOutputService.WriteMessage($"Pausing {pause}ms prior to playing...");
				Thread.Sleep(pause);
			}
		}

		private void PlayNoteAsRequired(string note, int octave, int a4Reference, PlayableSequence playableSequence)
		{
			if (note != null)
			{
				var effectiveOctave = GetEffectiveOctave(note, octave);
				if (!IsValidScaleStartingOctave(effectiveOctave))
				{
					consoleOutputService.WriteMessage($"Octave {effectiveOctave} is out of range. Valid scale starting octaves are {MINIMUM_SCALE_STARTING_OCTAVE} through {MAXIMUM_SCALE_STARTING_OCTAVE}.", ConsoleColor.Red);
					return;
				}

				var noteToCreate = GetNoteWithOctave(note, octave);
				var musicNote = MusicNote.Create(noteToCreate, a4Reference);
				if (musicNote?.IsValid ?? false)
				{
					ShowNote(musicNote);
					var musicNotes = ApplyStartingOctave(musicNote.MajorScale, musicNote.DesiredOctave).ToArray();
					consoleOutputService.WriteMessage($"Playing Major Scale: {string.Join(',', musicNotes)}");
					playableSequence.LoadSequenceFromString(musicNotes);
					playableSequence.Prepare();
					playableSequence.Play();
					Thread.Sleep(1000);
					musicNotes = ApplyStartingOctave(musicNote.MinorScale, musicNote.DesiredOctave).ToArray();
					consoleOutputService.WriteMessage($"Playing Minor Scale: {string.Join(',', musicNotes)}");
					playableSequence.LoadSequenceFromString(musicNotes);
					playableSequence.Prepare();
					playableSequence.Play();
					Thread.Sleep(1000);
					musicNotes = ApplyStartingOctave(musicNote.RelativeMinorScale, musicNote.DesiredOctave).ToArray();
					consoleOutputService.WriteMessage($"Playing Relative Minor Scale {musicNote.RelativeMinor}m: {string.Join(',', musicNotes)}");
					playableSequence.LoadSequenceFromString(musicNotes);
					playableSequence.Prepare();
					playableSequence.Play();
				}
				else
				{
					consoleOutputService.WriteMessage($"{noteToCreate} is NOT a valid note!", ConsoleColor.Red);
				}
			}
		}

		private void PlaySongAsRequired(string key, string fileName, PlayableSequence playableSequence)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				var result = playableSequence.LoadFromFile(fileName, "Songs", out var errorString, out Song song);
				if (!result)
				{
					consoleOutputService.WriteMessage(errorString, ConsoleColor.Red);
				}
				else
				{
					var currentSong = GetTheSongByKeyAsRequired(key, song);
					playableSequence.ConvertSongNotesToNoteSequence(currentSong);
					playableSequence.Prepare();
					playableSequence.Play();

					if (song.Reverse)
					{
						playableSequence.ReverseSequence();
						playableSequence.Play();
					}
				}
			}
		}

		private void PlayTabAsRequired(string tabName, PlayableSequence playableSequence)
		{
			if (!string.IsNullOrEmpty(tabName))
			{
				var result = playableSequence.LoadFromFile(tabName, "Tabs", out var errorString, out Tablature tabs);
				if (!result)
				{
					consoleOutputService.WriteMessage(errorString, ConsoleColor.Red);
				}
				else
				{
					tabs.FixUp();
					var guitar = stringInstrumentFactory.Create(tabs.Tuning, 21);
					playableSequence.ConvertTabsToNoteSequence(guitar, tabs);
					playableSequence.Repeat = tabs.Repeat;
					playableSequence.Prepare();
					playableSequence.Play();
				}
			}
		}

		private SongKey GetTheSongByKeyAsRequired(string key, Song song)
		{
			var currentSong = (SongKey)song;
			if (!string.IsNullOrEmpty(key) && song.Keys.Any(songKey => songKey?.Name?.Equals(key) ?? false))
			{
				currentSong = song.Keys.SingleOrDefault(songKey => songKey?.Name?.Equals(key, StringComparison.InvariantCultureIgnoreCase) ?? false);
			}
			else
			{
				if (!string.IsNullOrEmpty(song.Name) && song.Keys != null)
				{
					currentSong = song.Keys.SingleOrDefault(songKey => songKey?.Name?.Equals(song.Name, StringComparison.InvariantCultureIgnoreCase) ?? false);
				}
			}

			return currentSong;
		}

		private void ShowNote(MusicNote musicNote)
		{
			var relativeMinor = musicNote.RelativeMinor;
			var relativeMajor = musicNote.RelativeMajor;
			consoleOutputService.WriteMessage($"{musicNote} is the current note");
			consoleOutputService.WriteMessage($"Frequencies for this note are: {string.Join(',', musicNote.Frequencies.Select(f => f.ToString()))}");
			consoleOutputService.WriteMessage($"Of the 12 natural, flat and sharp notes {musicNote} has {musicNote.NoteBefore} before it and {musicNote.NoteAfter} after it.");
			consoleOutputService.WriteMessage($"Of the 7 notes in the {musicNote} scale {musicNote} has {musicNote.MajorNoteBefore} before it and {musicNote.MajorNoteAfter} after it.");
			consoleOutputService.WriteMessage($"Of the 7 notes in the {musicNote}m scale {musicNote} has {musicNote.MinorNoteBefore} before it and {musicNote.MinorNoteAfter} after it.");
			consoleOutputService.WriteMessage($"Notes in {musicNote} are: {string.Join(',', ApplyStartingOctave(musicNote.MajorScale, musicNote.DesiredOctave))}");
			consoleOutputService.WriteMessage($"Notes in {musicNote}m are: {string.Join(',', ApplyStartingOctave(musicNote.MinorScale, musicNote.DesiredOctave))}");
			consoleOutputService.WriteMessage($"Major Chord: {string.Join(',', ApplyStartingOctave(musicNote.MajorChord15, musicNote.DesiredOctave))} minor Chord: {string.Join(',', ApplyStartingOctave(musicNote.MinorChord15, musicNote.DesiredOctave))}");
			consoleOutputService.WriteMessage($"The relative minor is {relativeMinor}m");
			consoleOutputService.WriteMessage($"The relative minor scale is {string.Join(',', ApplyStartingOctave(musicNote.RelativeMinorScale, musicNote.DesiredOctave))}");
			consoleOutputService.WriteMessage($"The relative Major to the minor is {relativeMajor}");
		}

		private static int GetEffectiveOctave(string note, int octave)
		{
			if (!NoteContainsOctave(note))
			{
				return octave;
			}

			var octaveString = new string(note.Where(ch => char.IsDigit(ch)).ToArray());
			return int.Parse(octaveString);
		}

		private static string GetNoteWithOctave(string note, int octave)
		{
			return NoteContainsOctave(note) ? note : $"{note}{octave}";
		}

		private static bool IsValidScaleStartingOctave(int octave)
		{
			return octave >= MINIMUM_SCALE_STARTING_OCTAVE && octave <= MAXIMUM_SCALE_STARTING_OCTAVE;
		}

		private static bool NoteContainsOctave(string note)
		{
			return note.Any(ch => char.IsDigit(ch));
		}

		private static IEnumerable<string> ApplyStartingOctave(IEnumerable<string> notes, int startingOctave)
		{
			var noteArray = notes.ToArray();
			if (!noteArray.Any())
			{
				return noteArray;
			}

			var firstOctave = GetNoteOctave(noteArray.First());
			return noteArray.Select(note => $"{GetNoteName(note)}{startingOctave + GetNoteOctave(note) - firstOctave}");
		}

		private static string GetNoteName(string note)
		{
			return new string(note.Where(ch => !char.IsDigit(ch)).ToArray());
		}

		private static int GetNoteOctave(string note)
		{
			var octaveString = new string(note.Where(ch => char.IsDigit(ch)).ToArray());
			return string.IsNullOrEmpty(octaveString) ? 0 : int.Parse(octaveString);
		}
	}
}

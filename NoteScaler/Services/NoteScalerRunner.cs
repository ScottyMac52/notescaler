namespace NoteScaler.Services
{
	using CommandLine;
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Linq;
	using System.Threading;

	public sealed class NoteScalerRunner : INoteScalerRunner
	{
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

				PlayNoteAsRequired(options.Note, a4Reference, playableSequence);
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

		private void PlayNoteAsRequired(string note, int a4Reference, PlayableSequence playableSequence)
		{
			if (note != null)
			{
				var musicNote = MusicNote.Create(note, a4Reference);
				if (musicNote?.IsValid ?? false)
				{
					ShowNote(musicNote);
					var musicNotes = musicNote.MajorScale.ToArray();
					consoleOutputService.WriteMessage($"Playing Major Scale: {string.Join(',', musicNotes)}");
					playableSequence.LoadSequenceFromString(musicNote.MajorScale);
					playableSequence.Prepare();
					playableSequence.Play();
					Thread.Sleep(1000);
					musicNotes = musicNote.MinorScale.ToArray();
					consoleOutputService.WriteMessage($"Playing Minor Scale: {string.Join(',', musicNotes)}");
					playableSequence.LoadSequenceFromString(musicNote.MinorScale);
					playableSequence.Prepare();
					playableSequence.Play();
					Thread.Sleep(1000);
					consoleOutputService.WriteMessage($"Playing Relative Minor Scale {musicNote.RelativeMinor}m: {string.Join(',', musicNote.RelativeMinorScale)}");
					playableSequence.LoadSequenceFromString(musicNote.RelativeMinorScale);
					playableSequence.Prepare();
					playableSequence.Play();
				}
				else
				{
					consoleOutputService.WriteMessage($"{note} is NOT a valid note!", ConsoleColor.Red);
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
			consoleOutputService.WriteMessage($"Notes in {musicNote} are: {string.Join(',', musicNote.MajorScale)}");
			consoleOutputService.WriteMessage($"Notes in {musicNote}m are: {string.Join(',', musicNote.MinorScale)}");
			consoleOutputService.WriteMessage($"Major Chord: {string.Join(',', musicNote.MajorChord15)} minor Chord: {string.Join(',', musicNote.MinorChord15)}");
			consoleOutputService.WriteMessage($"The relative minor is {relativeMinor}m");
			consoleOutputService.WriteMessage($"The relative minor scale is {string.Join(',', musicNote.RelativeMinorScale)}");
			consoleOutputService.WriteMessage($"The relative Major to the minor is {relativeMajor}");
		}
	}
}

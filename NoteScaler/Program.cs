namespace NoteScaler
{
	using CommandLine;
	using NoteScaler.Classes;
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Threading;

	[ExcludeFromCodeCoverage]
	class Program
	{
		static int Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<NoteScalerOptions>(args)
				.WithParsed(o =>
				{
					MusicNote.FactoryError += MusicNote_FactoryError;
					MusicNote.FactoryCreateNote += MusicNote_FactoryCreateNote;
					InitializeNoteScalerOptions(o, out int a4Reference, out string key, out string fileName, out string tabName);
					var playableSequence = CreatePlayableSequence(o, a4Reference);
					PlayNoteAsRequired(o.Note, a4Reference, playableSequence);
					PlayTabAsRequired(tabName, playableSequence);
					PlaySongAsRequired(key, fileName, playableSequence);
				});
			return 0;
		}

		private static void MusicNote_FactoryCreateNote(object sender, EventArgs e)
		{
			var musicNote = sender as MusicNote;
			WriteMessage($"Created: {musicNote.Key}{musicNote.DesiredOctave}", ConsoleColor.Green);
		}

		private static void MusicNote_FactoryError(object sender, Exception e)
		{
			WriteMessage($"{e}", ConsoleColor.Red);
		}

		private static PlayableSequence CreatePlayableSequence(NoteScalerOptions o, int a4Reference)
		{
			var playableSequence =  new PlayableSequence()
			{
				MeasureTime = o.Speed.GetValueOrDefault(),
				Octave = o.Octave.GetValueOrDefault(),
				A4Reference = a4Reference,
				InstrumentType = o.Instrument
			};

			playableSequence.PlayableSequenceEvent += PlayableSequence_PlayableSequenceEvent;
			return playableSequence;
		}

		private static void PlayableSequence_PlayableSequenceEvent(object sender, PlayableSequenceEvent e)
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
			WriteMessage($"Event: {e.EventType} -> {e.EventDetails}", textColor);
		}

		private static void InitializeNoteScalerOptions(NoteScalerOptions o, out int a4Reference, out string key, out string fileName, out string tabName)
		{
			key = o.Key;
			fileName = o.File;
			tabName = o.Tab;
			var pause = o.Speed.GetValueOrDefault() * o.PreWait.GetValueOrDefault();
			a4Reference = o.Range.GetValueOrDefault();
			if (pause > 0)
			{
				WriteMessage($"Pausing {pause}ms prior to playing...");
				Thread.Sleep(pause);
			}
		}

		private static void PlayNoteAsRequired(string note, int a4Reference, PlayableSequence playableSequence)
		{
			if (note != null)
			{
				var musicNote = MusicNote.Create(note, a4Reference);
				if (musicNote?.IsValid ?? false)
				{
					ShowNote(musicNote);
					var musicNotes = musicNote.MajorScale.ToArray();
					WriteMessage($"Playing Major Scale: {string.Join(',', musicNotes)}");
					playableSequence.LoadSequenceFromString(musicNote.MajorScale);
					playableSequence.Prepare();
					playableSequence.Play();
					Thread.Sleep(1000);
					musicNotes = musicNote.MinorScale.ToArray();
					WriteMessage($"Playing Minor Scale: {string.Join(',', musicNotes)}");
					playableSequence.LoadSequenceFromString(musicNote.MinorScale);
					playableSequence.Prepare();
					playableSequence.Play();
					Thread.Sleep(1000);
					WriteMessage($"Playing Relative Minor Scale {musicNote.RelativeMinor}m: {string.Join(',', musicNote.RelativeMinorScale)}");
					playableSequence.LoadSequenceFromString(musicNote.RelativeMinorScale);
					playableSequence.Prepare();
					playableSequence.Play();
				}
				else
				{
					WriteMessage($"{note} is NOT a valid note!", ConsoleColor.Red);
				}
			}
		}

		private static void PlaySongAsRequired(string key, string fileName, PlayableSequence playableSequence)
		{
			if (!string.IsNullOrEmpty(fileName))
			{
				var result = playableSequence.LoadFromFile(fileName, "Songs", out string errorString, out Song song);
				if (!result)
				{
					WriteMessage(errorString, ConsoleColor.Red);
				}
				else
				{
					var currentSong = GetTheSongByKeyAsRequired(key, song);
					playableSequence.ConvertSongNotesToNoteSequence(currentSong);
					playableSequence.Prepare();
					playableSequence.Play();

					if(song.Reverse)
					{
						playableSequence.ReverseSequence();
						playableSequence.Play();
					}
				}
			}
		}

		private static void PlayTabAsRequired(string tabName, PlayableSequence playableSequence)
		{
			if (!string.IsNullOrEmpty(tabName))
			{
				var result = playableSequence.LoadFromFile(tabName, "Tabs", out string errorString, out Tablature tabs);
				if (!result)
				{
					WriteMessage(errorString, ConsoleColor.Red);
				}
				else
				{
					tabs.FixUp();
					IStringInstrument guitar = new Guitar(tabs.Tuning, 21);
					playableSequence.ConvertTabsToNoteSequence(guitar, tabs);
					playableSequence.Repeat = tabs.Repeat;
					playableSequence.Prepare();
					playableSequence.Play();
			}
			}
		}

		private static SongKey GetTheSongByKeyAsRequired(string key, Song song)
		{
			var currentSong = (SongKey) song;
			if (!string.IsNullOrEmpty(key) && song.Keys.Any(songKey => songKey?.Name?.Equals(key) ?? false))
			{
				currentSong = song.Keys.SingleOrDefault(songKey => songKey?.Name?.Equals(key, StringComparison.InvariantCultureIgnoreCase) ?? false);
			}
			else
			{
				if(!string.IsNullOrEmpty(song.Name) && song.Keys != null)
				{
					currentSong = song.Keys.SingleOrDefault(songKey => songKey?.Name?.Equals(song.Name, StringComparison.InvariantCultureIgnoreCase) ?? false);
				}
			}

			return currentSong;
		}

		private static void WriteMessage(string message, ConsoleColor textColor = ConsoleColor.White)
		{
			var currentForeground = Console.ForegroundColor;
			Console.ForegroundColor = textColor;
			Console.WriteLine(message);
			Console.ForegroundColor = currentForeground;
		}

		private static void ShowNote(MusicNote musicNote)
		{
			var relativeMinor = musicNote.RelativeMinor;
			var relativeMajor = musicNote.RelativeMajor;
			WriteMessage($"{musicNote} is the current note");
			WriteMessage($"Frequencies for this note are: {string.Join(',', musicNote.Frequencies.Select(f => f.ToString()))}");
			WriteMessage($"Of the 12 natural, flat and sharp notes {musicNote} has {musicNote.NoteBefore} before it and {musicNote.NoteAfter} after it.");
			WriteMessage($"Of the 7 notes in the {musicNote} scale {musicNote} has {musicNote.MajorNoteBefore} before it and {musicNote.MajorNoteAfter} after it.");
			WriteMessage($"Of the 7 notes in the {musicNote}m scale {musicNote} has {musicNote.MinorNoteBefore} before it and {musicNote.MinorNoteAfter} after it.");
			WriteMessage($"Notes in {musicNote} are: {string.Join(',', musicNote.MajorScale)}");
			WriteMessage($"Notes in {musicNote}m are: {string.Join(',', musicNote.MinorScale)}");
			WriteMessage($"Major Chord: {string.Join(',', musicNote.MajorChord15)} minor Chord: {string.Join(',', musicNote.MinorChord15)}");
			WriteMessage($"The relative minor is {relativeMinor}m");
			WriteMessage($"The relative minor scale is {string.Join(',', musicNote.RelativeMinorScale)}");
			WriteMessage($"The relative Major to the minor is {relativeMajor}");
		}
	}
}

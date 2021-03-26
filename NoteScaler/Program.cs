using CommandLine;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NoteScaler
{
	class Program
	{
		static int Main(string[] args)
		{
			var result = Parser.Default.ParseArguments<NoteScalerOptions>(args)
				.WithParsed(o =>
			   {
					var currentForeground = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Yellow;
				   var a4Reference = o.Range.GetValueOrDefault();
				   var octave = o.Octave.GetValueOrDefault();
				   var speed = o.Speed.GetValueOrDefault();
				   var instrumentType = o.Instrument;
				   string fileName = o.File;
				   MusicNote musicNote = null;
				   var pause = o.Speed.GetValueOrDefault() * o.PreWait.GetValueOrDefault();
					if (pause > 0)
					{
						Console.WriteLine($"Pausing {pause}ms prior to playing...");
						Console.ForegroundColor = currentForeground;
						Thread.Sleep(pause);
					}

					if (o.Note != null)
					{
						musicNote = new MusicNote(o.Note, a4Reference);
						ShowNote(musicNote);
						var musicNotes = musicNote.MajorScale.ToArray();
						Console.WriteLine($"Playing Major Scale: {string.Join(',', musicNotes)}");
						PlaySequence(musicNotes, speed, octave, instrumentType, a4Reference);
						Thread.Sleep(1000);
						musicNotes = musicNote.MinorScale.ToArray();
						Console.WriteLine($"Playing Minor Scale: {string.Join(',', musicNotes)}");
						PlaySequence(musicNotes, speed, octave, instrumentType, a4Reference);
					}

				   if (!string.IsNullOrEmpty(fileName))
				   {
					   var completeFileName = Path.Combine(Environment.CurrentDirectory, $"Songs\\{fileName}.json");
					   if (!File.Exists(completeFileName))
					   {
						   Console.Error.WriteLine($"Unable to find a file named {completeFileName}");
					   }
					   var song = JsonConvert.DeserializeObject<Song>(File.ReadAllText(completeFileName));
					   PlaySequence(song.Default, speed, octave, instrumentType, a4Reference);
				   }
			   });
			return 0;
		}

		private static void ShowNote(MusicNote musicNote)
		{
			var relativeMinor = musicNote.RelativeMinor;
			var relativeMajor = musicNote.RelativeMajor;
			Console.WriteLine($"{musicNote} is the current note");
			Console.WriteLine($"Frequencies for this note are: {string.Join(',', musicNote.Frequencies.Select(f => f.ToString()))}");
			Console.WriteLine($"Of the 12 natural, flat and sharp notes {musicNote} has {musicNote.NoteBefore} before it and {musicNote.NoteAfter} after it.");
			Console.WriteLine($"Of the 7 notes in the {musicNote} scale {musicNote} has {musicNote.MajorNoteBefore} before it and {musicNote.MajorNoteAfter} after it.");
			Console.WriteLine($"Of the 7 notes in the {musicNote}m scale {musicNote} has {musicNote.MinorNoteBefore} before it and {musicNote.MinorNoteAfter} after it.");
			Console.WriteLine($"Notes in {musicNote} are: {string.Join(',', musicNote.MajorScale)}");
			Console.WriteLine($"Notes in {musicNote}m are: {string.Join(',', musicNote.MinorScale)}");
			Console.WriteLine($"Major Chord: {string.Join(',', musicNote.MajorChord)} minor Chord: {string.Join(',', musicNote.MinorChord)}");
			Console.WriteLine($"The relative minor is {relativeMinor}m");
			Console.WriteLine($"The relative Major to the minor is {relativeMajor}");
		}

		private static void PlaySequence(string[] song, int measureTime = 1000, int octave = 2, InstrumentType defaultInstrument = InstrumentType.Horn, int a4Reference = 400)
		{
			var notePlayer = new NotePlayer();
			var notesInTheSong = LoadSong(notePlayer, song, measureTime, octave, defaultInstrument, a4Reference);
			foreach (var songNoteDuration in notesInTheSong)
			{
				try
				{
					songNoteDuration.PlayNote(songNoteDuration.Duration);
				}
				catch(Exception ex)
				{
					var currentForeground = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(ex.Message);
					Console.ForegroundColor = currentForeground;
				}
			}
		}

		private static IEnumerable<MusicNote> LoadSong(INotePlayer player, string[] song, int measureTime, int octave, InstrumentType defaultInstrument, int a4Reference = 400)
		{
			var currentInstrument = defaultInstrument;
			return song.Select(s => s.Split(',')[0]).Select(songDef =>
			{
				var arry = songDef.Split('-');
				var currentNote = arry[0];
				var rawNote = arry[0];
				currentInstrument = GetInstrument(arry, defaultInstrument);
				var currentOctave = octave;
				if (currentNote.Any(ch => char.IsDigit(ch)))
				{
					var octaveString = new String(currentNote.Where(ch => char.IsDigit(ch)).ToArray());
					var noteOctave = int.Parse(octaveString);
					currentOctave += noteOctave;
					rawNote = currentNote.Substring(0, currentNote.IndexOf(octaveString));
				}
				currentNote = $"{rawNote}{currentOctave}";
				return new MusicNote(currentNote, a4Reference, player, GetNoteDuration(arry, measureTime), currentInstrument);
			});
		}

		private static int GetNoteDuration(string[] arry, int measureTime)
		{
			if (arry.Length > 1)
			{
				return (int)(measureTime * (float.Parse(arry[1])));
			}
			else
			{
				return measureTime;
			}
		}

		private static InstrumentType GetInstrument(string[] arry, InstrumentType instrumentType)
		{
			if (arry.Length == 3)
			{
				return (InstrumentType)int.Parse(arry[2]);
			}
			return instrumentType;
		}
	}
}

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
		private static int a4Reference = 440;

		static int Main(string[] args)
		{
			var octave = 0;
			var speed = 750;
			var instrumentType = InstrumentType.Horn;
			string fileName = null;
			MusicNote musicNote = null;

			if (args.Any(arg => arg.Contains("-r")))
			{
				var index = Array.IndexOf(args, "-r");
				a4Reference = int.Parse(args[index + 1]);
			}

			if (args.Any(arg => arg.Contains("-o")))
			{
				var index = Array.IndexOf(args, "-o");
				octave = int.Parse(args[index+1]);
			}

			if (args.Any(arg => arg.Contains("-s")))
			{
				var index = Array.IndexOf(args, "-s");
				speed = int.Parse(args[index+1]);
			}

			if (args.Any(arg => arg.Contains("-i")))
			{
				var index = Array.IndexOf(args, "-i");
				instrumentType = (InstrumentType) Enum.Parse(typeof(InstrumentType), args[index+1]);
			}

			if (args.Any(arg => arg.Contains("-n")))
			{
				var index = Array.IndexOf(args, "-n");
				musicNote = new MusicNote(args[index + 1], a4Reference);
			}

			if (args.Any(arg => arg.Contains("-f")))
			{
				var index = Array.IndexOf(args, "-f");
				fileName = args[index + 1];
			}

			if(musicNote != null)
			{
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
				if(!File.Exists(completeFileName))
				{
					Console.Error.WriteLine($"Unable to find a file named {completeFileName}");
					return -2;
				}
				var song = JsonConvert.DeserializeObject<Song>(File.ReadAllText(completeFileName));
				PlaySequence(song.Default, speed, octave, instrumentType, a4Reference);
			}
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
				songNoteDuration.PlayNote(songNoteDuration.Duration);
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
					var noteOctave = int.Parse(currentNote[^1].ToString());
					currentOctave += noteOctave;
					rawNote = currentNote.Substring(0, currentNote.IndexOf(noteOctave.ToString()));
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

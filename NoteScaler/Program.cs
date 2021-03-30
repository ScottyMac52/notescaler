namespace NoteScaler
{
	using CommandLine;
	using Newtonsoft.Json;
	using System;
	using System.IO;
	using System.Linq;
	using System.Threading;

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
				   string key = o.Key;
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

				   var playableSequence = new PlayableSequence()
				   {
					   MeasureTime = speed,
					   Octave = octave,
					   A4Reference = a4Reference,
					   InstrumentType = instrumentType
				   };

				   if (o.Note != null)
					{
						musicNote = new MusicNote(o.Note, a4Reference);
						ShowNote(musicNote);
						var musicNotes = musicNote.MajorScale.ToArray();
						Console.WriteLine($"Playing Major Scale: {string.Join(',', musicNotes)}");
						playableSequence.LoadSequenceFromString(musicNote.MajorScale);
						playableSequence.Prepare();
						playableSequence.Play();
						Thread.Sleep(1000);
						musicNotes = musicNote.MinorScale.ToArray();
						Console.WriteLine($"Playing Minor Scale: {string.Join(',', musicNotes)}");
						playableSequence.LoadSequenceFromString(musicNote.MinorScale);
						playableSequence.Prepare();
						playableSequence.Play();
				   }

				   if (!string.IsNullOrEmpty(fileName))
				   {
					   var completeFileName = Path.Combine(Environment.CurrentDirectory, $"Songs\\{fileName}.json");
					   if (!File.Exists(completeFileName))
					   {
						   Console.Error.WriteLine($"Unable to find a file named {completeFileName}");
					   }
					   playableSequence.LoadSequenceFromFile(fileName, key);
					   playableSequence.Prepare();
					   playableSequence.Play();
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
	}
}

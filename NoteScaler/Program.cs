using System;
using System.Linq;

namespace NoteScaler
{
	class Program
	{
		static void Main(string[] args)
		{
			if((args?.Count() ?? 0) < 1)
			{
				Console.Error.WriteLine("You must specify a note");
				return;
			}
			var targetNote = args[0];
			var musicNote = new MusicNote(targetNote);
			ShowNote(musicNote);

			Console.WriteLine("Making note more sharp...");
			musicNote.MakeSharper();
			ShowNote(musicNote);
			musicNote.MakeSharper();
			ShowNote(musicNote);
			musicNote.MakeSharper();
			ShowNote(musicNote);
			Console.WriteLine("Making note more flat...");
			musicNote.MakeFlatter();
			ShowNote(musicNote);
			musicNote.MakeFlatter();
			ShowNote(musicNote);
			musicNote.MakeFlatter();
			ShowNote(musicNote);
		}

		private static void ShowNote(MusicNote musicNote)
		{
			var relativeMinor = musicNote.RelativeMinor;
			var relativeMajor = musicNote.RelativeMajor;
			Console.WriteLine($"{musicNote.Key} is the current note");
			Console.WriteLine($"Of the 12 natural, flat and sharp notes {musicNote.Key} has {musicNote.NoteBefore} before it and {musicNote.NoteAfter} after it.");
			Console.WriteLine($"Of the 7 notes in the {musicNote.Key} scale {musicNote.Key} has {musicNote.MajorNoteBefore} before it and {musicNote.MajorNoteAfter} after it.");
			Console.WriteLine($"Of the 7 notes in the {musicNote.Key}m scale {musicNote.Key} has {musicNote.MinorNoteBefore} before it and {musicNote.MinorNoteAfter} after it.");
			Console.WriteLine($"Notes in {musicNote.Key} are: {string.Join(',', musicNote.MajorScale)}");
			Console.WriteLine($"Notes in {musicNote.Key}m are: {string.Join(',', musicNote.MinorScale)}");
			Console.WriteLine($"Major Chord: {string.Join(',', musicNote.MajorChord)} minor Chord: {string.Join(',', musicNote.MinorChord)}");
			Console.WriteLine($"The relative minor is {relativeMinor.Key}m and contains the following notes {string.Join(',', relativeMinor.MinorScale)}");
			Console.WriteLine($"The relative Major to the minor is {relativeMajor.Key} and contains the following notes {string.Join(',', relativeMajor.MajorScale)}");
		}
	}
}

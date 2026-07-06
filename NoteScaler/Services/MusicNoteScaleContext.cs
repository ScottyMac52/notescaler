namespace NoteScaler.Services
{
	using System.Collections.Generic;

	public sealed class MusicNoteScaleContext
	{
		public MusicNoteScaleContext(int noteIndex, IEnumerable<string> sharpNotes, IEnumerable<string> flatNotes)
		{
			NoteIndex = noteIndex;
			SharpNotes = sharpNotes;
			FlatNotes = flatNotes;
		}

		public int NoteIndex { get; }
		public IEnumerable<string> SharpNotes { get; }
		public IEnumerable<string> FlatNotes { get; }
	}
}

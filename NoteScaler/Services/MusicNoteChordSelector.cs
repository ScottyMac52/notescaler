namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Services.Interfaces;

	public sealed class MusicNoteChordSelector : IMusicNoteChordSelector
	{
		public string[] SelectChord(MusicNote musicNote)
		{
			switch (musicNote.ChordType)
			{
				case ChordType.Power:
					return musicNote.PowerChord;
				case ChordType.MinorThird:
					return musicNote.MinorChord3;
				case ChordType.MajorThird:
					return musicNote.MajorChord3;
				case ChordType.MinorSeventh:
					return musicNote.MinorChord7;
				case ChordType.MajorSeventh:
					return musicNote.MajorChord7;
				case ChordType.Note:
				default:
					return new[] { musicNote.Key };
			}
		}
	}
}

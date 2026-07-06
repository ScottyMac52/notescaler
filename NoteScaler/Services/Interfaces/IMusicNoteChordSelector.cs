namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Services;

	public interface IMusicNoteChordSelector
	{
		string[] SelectChord(MusicNote musicNote);
	}
}

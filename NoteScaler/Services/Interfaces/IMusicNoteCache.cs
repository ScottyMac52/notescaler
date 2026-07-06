namespace NoteScaler.Services.Interfaces
{
	public interface IMusicNoteCache
	{
		bool TryGet(string note, int a4Reference, out MusicNote musicNote);
		void Add(string note, int a4Reference, MusicNote musicNote);
	}
}

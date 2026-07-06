namespace NoteScaler.Services
{
	using NoteScaler.Services.Interfaces;
	using System.Collections.Generic;

	public sealed class MusicNoteCache : IMusicNoteCache
	{
		private readonly Dictionary<string, MusicNote> notes = new Dictionary<string, MusicNote>();

		public bool TryGet(string note, int a4Reference, out MusicNote musicNote)
		{
			return notes.TryGetValue(GetCacheKey(note, a4Reference), out musicNote);
		}

		public void Add(string note, int a4Reference, MusicNote musicNote)
		{
			notes[GetCacheKey(note, a4Reference)] = musicNote;
		}

		private static string GetCacheKey(string note, int a4Reference)
		{
			return $"{a4Reference}:{note}";
		}
	}
}

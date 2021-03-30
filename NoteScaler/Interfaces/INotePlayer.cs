using NoteScaler.Enums;

namespace NoteScaler.Interfaces
{
	public interface INotePlayer
	{
		/// <summary>
		/// Plays the note
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="instrument"></param>
		/// <param name="duration"></param>
		void PlayNote(float frequency, InstrumentType instrument, int duration = 500);
	}
}
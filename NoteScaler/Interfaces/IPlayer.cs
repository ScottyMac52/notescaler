using NoteScaler.Enums;
using System;

namespace NoteScaler.Interfaces
{
	public interface IPlayer
	{
		/// <summary>
		/// Plays the note
		/// </summary>
		/// <param name="frequency"></param>
		/// <param name="instrument"></param>
		/// <param name="duration"></param>
		void Play(float frequency, InstrumentType instrument, int duration = 500);

		/// <summary>
		/// Does this implementation support a pause action?
		/// </summary>
		/// <returns></returns>
		bool CanPause();

		/// <summary>
		/// Does this implementation support stopping the playing?
		/// </summary>
		/// <returns></returns>
		bool CanStop();

		/// <summary>
		/// Stop playing
		/// </summary>
		void Stop();

		/// <summary>
		/// Pause the playing
		/// </summary>
		void Pause();
	}
}
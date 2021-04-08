namespace NoteScaler.Interfaces
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using System.Collections.Generic;
	using static NoteScaler.Classes.PlayEngineBase;

	public interface IPlayer
	{
		/// <summary>
		/// Plays the array of notes
		/// </summary>
		/// <param name="noteList"></param>
		/// <param name="instrument"></param>
		void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument);

		/// <summary>
		/// Stop playing
		/// </summary>
		void Stop();

		/// <summary>
		/// Pause the playing
		/// </summary>
		void Pause();

		/// <summary>
		/// If true then Player can be paused
		/// </summary>
		bool CanPause { get; }

		/// <summary>
		/// If true then Player can be stopped
		/// </summary>
		bool CanStop { get; }

		/// <summary>
		/// Events for Player
		/// </summary>
		event PlayerEventHandler PlayerEvent;

	}
}
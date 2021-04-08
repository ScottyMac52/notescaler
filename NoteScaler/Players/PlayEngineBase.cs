namespace NoteScaler.Classes
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using System.Collections.Generic;
	using System.Linq;

	public abstract class PlayEngineBase
	{
		/// <summary>
		/// Player event
		/// </summary>
		public event PlayerEventHandler PlayerEvent;
		
		/// <summary>
		/// Delegate for the Player event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void PlayerEventHandler(object sender, PlayerEngineEvent e);

		/// <summary>
		/// If true then Player can be paused
		/// </summary>
		public abstract bool CanPause { get; }

		/// <summary>
		/// If true then Player can be stopped
		/// </summary>
		public abstract bool CanStop { get; }

		/// <summary>
		/// Pause method
		/// </summary>
		public void Pause()
		{
			if(CanPause)
			{
				PlayerEvent?.Invoke(this, new PlayerEngineEvent() { EventType = PlayerEventType.Pause, Message = "Paused" });
				OnPause();
			}
		}

		/// <summary>
		/// Stop method
		/// </summary>
		public void Stop()
		{
			if(CanStop)
			{
				PlayerEvent?.Invoke(this, new PlayerEngineEvent() { EventType = PlayerEventType.Stop, Message = "Stopped" });
				OnStop();
			}
		}

		/// <summary>
		/// Override for Pause
		/// </summary>
		public virtual void OnPause()
		{
		}

		/// <summary>
		/// Override for Stop
		/// </summary>
		public virtual void OnStop()
		{
		}

		/// <summary>
		/// Plays a single or composite notes
		/// </summary>
		/// <param name="noteList"></param>
		/// <param name="instrument"></param>
		public virtual void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
		{
			PlayerEvent?.Invoke(this, new PlayerEngineEvent() { EventType = PlayerEventType.PlayNotes, Message = $"Playing {noteList.Count()} Notes: [{string.Join(',', noteList.Select(note => $"{note}"))}]" });
		}
	}
}
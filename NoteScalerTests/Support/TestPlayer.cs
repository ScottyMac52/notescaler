namespace NoteScalerTests.Support
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System.Collections.Generic;
	using System.Linq;
	using static NoteScaler.Services.PlayEngineBase;

	public sealed class TestPlayer : IPlayer
	{
		public bool CanPause => true;
		public bool CanStop => true;
		public int PlayCount { get; private set; }
		public IEnumerable<FrequencyDuration> LastNotes { get; private set; }
		public InstrumentType LastInstrument { get; private set; }
		public event PlayerEventHandler PlayerEvent;

		public void Pause()
		{
		}

		public void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
		{
			PlayCount++;
			LastNotes = noteList.ToArray();
			LastInstrument = instrument;
		}

		public void Stop()
		{
		}
	}
}

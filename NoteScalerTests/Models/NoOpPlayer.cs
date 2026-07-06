namespace NoteScalerTests.Models
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using System.Collections.Generic;

	public sealed class NoOpPlayer : PlayEngineBase, IPlayer
	{
		public override bool CanPause => false;

		public override bool CanStop => false;

		public override void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
		{
			base.Play(noteList, instrument);
		}
	}
}

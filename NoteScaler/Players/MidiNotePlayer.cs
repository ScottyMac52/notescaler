namespace NoteScaler.Players
{
	using NoteScaler.Classes;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System.Collections.Generic;

	public class MidiNotePlayer : PlayEngineBase, IPlayer
	{
		/// <inheritdoc/>
		public override bool CanPause => true;

		/// <inheritdoc/>
		public override bool CanStop => true;

		public MidiNotePlayer()
		{
		}

		public override void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
		{
		}
	}
}

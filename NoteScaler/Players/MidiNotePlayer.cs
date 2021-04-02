namespace NoteScaler.Players
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class MidiNotePlayer : IPlayer
	{
		public void Play(float frequency, InstrumentType instrument, int duration = 500)
		{
			throw new NotImplementedException();
		}
	}
}

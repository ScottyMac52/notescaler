using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using NoteScaler.Enums;
using System;

namespace NoteScaler.Classes
{
	public abstract class SignalNotePlayerBase
	{
		public event SignalNotePlayer.PlayerEventHandler NotePlayerEvent;

		public abstract bool CanPause();
		public abstract bool CanStop();

		public virtual void Pause()
		{
			var waveOut = new WaveOut();
			MixingSampleProvider mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))
			{
				ReadFully = true
			};
			waveOut.Init(mixer);
			waveOut.Play();
		}

		public abstract void Play(float frequency, InstrumentType instrument, int duration = 500);

		public virtual void Stop()
		{
			throw new NotImplementedException();
		}
	}
}
namespace NoteScaler.Classes
{
	using NAudio.Wave;
	using NAudio.Wave.SampleProviders;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using System;
	
	public class SignalNotePlayer : SignalNotePlayerBase, IPlayer
	{
		public delegate void PlayerEventHandler(object sender, PlayerEvent e);

		public override bool CanPause()
		{
			return false;
		}

		public override bool CanStop()
		{
			return false;
		}

		/// <inheritdoc/>
		public override void Play(float frequency, InstrumentType instrument, int duration = 500)
		{
			var signalType = SignalGeneratorType.Sin;
			switch (instrument)
			{
				case InstrumentType.Horn:
					signalType = SignalGeneratorType.SawTooth;
					break;
				case InstrumentType.Flute:
					signalType = SignalGeneratorType.Triangle;
					break;
				case InstrumentType.Clarinet:
					signalType = SignalGeneratorType.Square;
					break;
				case InstrumentType.Recorder:
					signalType = SignalGeneratorType.Sin;
					break;

			}

			var sineWave = new SignalGenerator(44100, 3)
			{
				Gain = 0.2,
				Frequency = frequency,
				Type = signalType
			}
			.Take(TimeSpan.FromMilliseconds(duration));
			using var wo = new WaveOutEvent();
			wo.Init(sineWave);

			var waveOut = new WaveOut();
			MixingSampleProvider mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))
			{
				ReadFully = true
			};
			mixer.AddMixerInput(sineWave);
			wo.Init(mixer);
			waveOut.Play();


			wo.Play();
			while (wo.PlaybackState == PlaybackState.Playing)
			{
			}
		}
	}
}

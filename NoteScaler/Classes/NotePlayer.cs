namespace NoteScaler.Classes
{
	using NAudio.Wave;
	using NAudio.Wave.SampleProviders;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using System;
	using System.Threading;

	public class NotePlayer : INotePlayer
	{
		/// <inheritdoc/>
		public void PlayNote(float frequency, InstrumentType instrument, int duration = 500)
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

			var sineWave = new SignalGenerator()
			{
				Gain = 0.2,
				Frequency = frequency,
				Type = signalType
			}
			.Take(TimeSpan.FromMilliseconds(duration));
			using var wo = new WaveOutEvent();
			wo.Init(sineWave);
			wo.Play();
			while (wo.PlaybackState == PlaybackState.Playing)
			{
			}
		}
	}
}

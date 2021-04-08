namespace NoteScaler.Classes
{
	using NAudio.Wave;
	using NAudio.Wave.SampleProviders;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	public class SignalNotePlayer : PlayEngineBase, IPlayer
	{
		/// <inheritdoc/>
		public override bool CanPause => false;
		/// <inheritdoc/>
		public override bool CanStop => false;

		protected ISampleProvider sampleProvider = null;
		protected ISampleProvider[] sampleProviders = null;

		private SignalGeneratorType signalType;
		private readonly MixingSampleProvider mixer;

		public SignalNotePlayer()
		{
			mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 1))
			{
				ReadFully = true
			};
		}

		/// <inheritdoc/>
		public override void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
		{
			base.Play(noteList, instrument);
			SetInstrument(instrument);
			sampleProviders = noteList.Select(nl => CreateSignal(nl.Frequency, nl.Duration, true)).ToArray();
			using var wo = new WaveOut();
			wo.Init(mixer);
			wo.Play();
			Thread.Sleep(noteList.Max(nl => nl.Duration));
		}

		private void SetInstrument(InstrumentType instrument)
		{
			signalType = SignalGeneratorType.Sin;
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
		}

		private ISampleProvider CreateSignal(float frequency, int duration, bool addToMixer = false)
		{
			var sampleProvider = new SignalGenerator(44100, 1)
			{
				Gain = 0.2,
				Frequency = frequency,
				Type = signalType
			}
			.Take(TimeSpan.FromMilliseconds(duration));
			if (addToMixer)
			{
				mixer.AddMixerInput(sampleProvider);
			}
			return sampleProvider;
		}

	}
}

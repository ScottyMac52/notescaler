namespace NoteScalerTests
{
	using NoteScaler.Config;
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class NoteScalerRunnerTests
	{
		[Theory]
		[InlineData(new[] { "--note", "C", "--octave", "3" }, "C3-500ms is the current note", "Playing Major Scale: C3,D3,E3,F3,G3,A3,B3,C4")]
		[InlineData(new[] { "--note", "C" }, "C3-500ms is the current note", "Playing Major Scale: C3,D3,E3,F3,G3,A3,B3,C4")]
		public void Run_WhenNoteOptionIsUsed_StartsAtSpecifiedOrDefaultOctave(string[] args, string expectedCurrentNoteMessage, string expectedMajorScaleMessage)
		{
			var consoleOutputService = new CapturingConsoleOutputService();
			var runner = new NoteScalerRunner(
				new CommandLineOptionsService(),
				new TestPlayableSequenceFactory(),
				new UnusedStringInstrumentFactory(),
				consoleOutputService);

			runner.Run(args);

			Assert.Contains(expectedCurrentNoteMessage, consoleOutputService.Messages);
			Assert.Contains(expectedMajorScaleMessage, consoleOutputService.Messages);
		}

		private sealed class CapturingConsoleOutputService : IConsoleOutputService
		{
			public List<string> Messages { get; } = new List<string>();

			public void WriteMessage(string message, ConsoleColor textColor = ConsoleColor.White)
			{
				Messages.Add(message);
			}
		}

		private sealed class TestPlayableSequenceFactory : IPlayableSequenceFactory
		{
			public PlayableSequence Create(NoteScalerOptions options, int a4Reference)
			{
				return new PlayableSequence(() => new NoOpPlayer(), () => new Guitar())
				{
					MeasureTime = options.Speed.GetValueOrDefault(),
					Octave = options.Octave.GetValueOrDefault(),
					A4Reference = a4Reference,
					InstrumentType = options.Instrument
				};
			}
		}

		private sealed class UnusedStringInstrumentFactory : IStringInstrumentFactory
		{
			public IStringInstrument Create(TuningScheme tuningScheme, int numberOfFrets)
			{
				throw new InvalidOperationException($"The string instrument factory should not be used by {nameof(NoteScalerRunnerTests)}.");
			}
		}

		private sealed class NoOpPlayer : PlayEngineBase, IPlayer
		{
			public override bool CanPause => false;

			public override bool CanStop => false;

			public override void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
			{
				base.Play(noteList, instrument);
			}
		}
	}
}

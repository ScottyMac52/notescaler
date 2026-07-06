namespace NoteScalerTests
{
	using NoteScaler.Services;
	using NoteScalerTests.Models;
	using Xunit;

	public class NoteScalerRunnerTests
	{
		[Theory]
		[InlineData(new[] { "--note", "C", "--octave", "0" }, "C0-500ms is the current note", "Playing Major Scale: C0,D0,E0,F0,G0,A0,B0,C1")]
		[InlineData(new[] { "--note", "C", "--octave", "3" }, "C3-500ms is the current note", "Playing Major Scale: C3,D3,E3,F3,G3,A3,B3,C4")]
		[InlineData(new[] { "--note", "C", "--octave", "5" }, "C5-500ms is the current note", "Playing Major Scale: C5,D5,E5,F5,G5,A5,B5,C6")]
		[InlineData(new[] { "--note", "C", "--octave", "10" }, "C10-500ms is the current note", "Playing Major Scale: C10,D10,E10,F10,G10,A10,B10,C11")]
		[InlineData(new[] { "--note", "C" }, "C3-500ms is the current note", "Playing Major Scale: C3,D3,E3,F3,G3,A3,B3,C4")]
		[InlineData(new[] { "--note", "C5" }, "C5-500ms is the current note", "Playing Major Scale: C5,D5,E5,F5,G5,A5,B5,C6")]
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

		[Theory]
		[InlineData(new[] { "--note", "C", "--octave=-1" }, "-1")]
		[InlineData(new[] { "--note", "C", "--octave", "11" }, "11")]
		[InlineData(new[] { "--note", "C11" }, "11")]
		public void Run_WhenNoteOptionIsUsedWithOutOfRangeOctave_WritesValidationMessage(string[] args, string expectedOctave)
		{
			var consoleOutputService = new CapturingConsoleOutputService();
			var runner = new NoteScalerRunner(
				new CommandLineOptionsService(),
				new TestPlayableSequenceFactory(),
				new UnusedStringInstrumentFactory(),
				consoleOutputService);

			runner.Run(args);

			Assert.Contains($"Octave {expectedOctave} is out of range. Valid scale starting octaves are 0 through 10.", consoleOutputService.Messages);
		}
	}
}

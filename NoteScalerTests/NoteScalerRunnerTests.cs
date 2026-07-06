namespace NoteScalerTests
{
	using NoteScaler.Services;
	using NoteScalerTests.Models;
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
	}
}

namespace NoteScalerTests.Services
{
	using NoteScaler.Players;
	using NoteScaler.Services;
	using System;
	using System.IO;
	using Xunit;

	public class SimpleServiceCoverageTests
	{
		[Fact]
		public void ConsoleOutputService_WritesMessageAndRestoresColor()
		{
			var service = new ConsoleOutputService();
			var originalColor = Console.ForegroundColor;
			using var writer = new StringWriter();
			var originalOut = Console.Out;
			Console.SetOut(writer);
			try
			{
				service.WriteMessage("hello", ConsoleColor.Green);
			}
			finally
			{
				Console.SetOut(originalOut);
			}

			Assert.Contains("hello", writer.ToString());
			Assert.Equal(originalColor, Console.ForegroundColor);
		}

		[Fact]
		public void SignalNotePlayerFactory_CreatesSignalNotePlayer()
		{
			var factory = new SignalNotePlayerFactory();

			var player = factory.Create();

			Assert.IsType<SignalNotePlayer>(player);
		}

		[Fact]
		public void MidiNotePlayer_ReportsPauseAndStopSupport()
		{
			var player = new MidiNotePlayer();

			Assert.True(player.CanPause);
			Assert.True(player.CanStop);
		}

		[Fact]
		public void SignalNotePlayer_ReportsNoPauseOrStopSupport()
		{
			var player = new SignalNotePlayer();

			Assert.False(player.CanPause);
			Assert.False(player.CanStop);
		}
	}
}

namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using System.Collections.Generic;
	using Xunit;

	public class PlayEngineBaseTests
	{
		[Theory]
		[InlineData(true, PlayerEventType.Pause, "Paused")]
		[InlineData(false, null, null)]
		public void Pause_OnlyRaisesEventWhenSupported(bool canPause, PlayerEventType? expectedEventType, string expectedMessage)
		{
			var player = new TestPlayEngine(canPause, true);
			PlayerEngineEvent captured = null;
			player.PlayerEvent += (_, e) => captured = e;

			player.Pause();

			AssertEvent(expectedEventType, expectedMessage, captured);
		}

		[Theory]
		[InlineData(true, PlayerEventType.Stop, "Stopped")]
		[InlineData(false, null, null)]
		public void Stop_OnlyRaisesEventWhenSupported(bool canStop, PlayerEventType? expectedEventType, string expectedMessage)
		{
			var player = new TestPlayEngine(true, canStop);
			PlayerEngineEvent captured = null;
			player.PlayerEvent += (_, e) => captured = e;

			player.Stop();

			AssertEvent(expectedEventType, expectedMessage, captured);
		}

		[Fact]
		public void Play_RaisesPlayNotesEventWithNoteDetails()
		{
			var player = new TestPlayEngine(true, true);
			PlayerEngineEvent captured = null;
			player.PlayerEvent += (_, e) => captured = e;
			var notes = new[] { new FrequencyDuration("C", 4, 261.63F, 250) };

			player.Play(notes, InstrumentType.Horn);

			Assert.NotNull(captured);
			Assert.Equal(PlayerEventType.PlayNotes, captured.EventType);
			Assert.Contains("Playing 1 Notes", captured.Message);
			Assert.Contains("C4[250ms]", captured.Message);
		}

		private static void AssertEvent(PlayerEventType? expectedEventType, string expectedMessage, PlayerEngineEvent captured)
		{
			if (expectedEventType == null)
			{
				Assert.Null(captured);
				return;
			}

			Assert.NotNull(captured);
			Assert.Equal(expectedEventType.Value, captured.EventType);
			Assert.Equal(expectedMessage, captured.Message);
		}

		private sealed class TestPlayEngine : PlayEngineBase
		{
			public TestPlayEngine(bool canPause, bool canStop)
			{
				CanPause = canPause;
				CanStop = canStop;
			}

			public override bool CanPause { get; }
			public override bool CanStop { get; }
		}
	}
}

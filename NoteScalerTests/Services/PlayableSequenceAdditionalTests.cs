namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using NoteScalerTests.Support;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using Xunit;
	using static NoteScaler.Services.PlayEngineBase;

	public class PlayableSequenceAdditionalTests : IDisposable
	{
		private readonly string originalCurrentDirectory;
		private readonly string testDirectory;

		public PlayableSequenceAdditionalTests()
		{
			originalCurrentDirectory = Environment.CurrentDirectory;
			testDirectory = Path.Combine(Path.GetTempPath(), $"PlayableSequenceAdditionalTests_{Guid.NewGuid():N}");
			Directory.CreateDirectory(testDirectory);
			Environment.CurrentDirectory = testDirectory;
		}

		[Fact]
		public void Constructor_RequiresNotePlayerFactory()
		{
			var exception = Assert.Throws<ArgumentNullException>(() => new PlayableSequence(null, () => new Guitar()));

			Assert.Equal("notePlayerFactory", exception.ParamName);
		}

		[Fact]
		public void Constructor_RequiresStringInstrumentFactory()
		{
			var exception = Assert.Throws<ArgumentNullException>(() => new PlayableSequence(() => new TestPlayer(), null));

			Assert.Equal("stringInstrumentFactory", exception.ParamName);
		}

		[Fact]
		public void LoadSequenceFromString_RaisesEventForCurrentNoteSequence()
		{
			var playableSequence = CreateSequence(new TestPlayer());
			PlayableSequenceEvent captured = null;
			playableSequence.PlayableSequenceEvent += (_, e) => captured = e;
			playableSequence.ConvertSongNotesToNoteSequence(new SongKey("Test", "C"));

			playableSequence.LoadSequenceFromString(new[] { "D", "E-0.5" });

			Assert.NotNull(captured);
			Assert.Equal(PlayableEventType.SequenceLoaded, captured.EventType);
			Assert.Equal("1 Notes loaded.", captured.EventDetails);
			Assert.Equal(new[] { "C3-1000" }, playableSequence.NoteSequence.Single().Notes);
		}

		[Fact]
		public void Prepare_ForwardsPlayerEventsAsPlayableSequenceEvents()
		{
			var player = new TestPlayer();
			var playableSequence = CreateSequence(player);
			var events = new List<PlayableSequenceEvent>();
			playableSequence.PlayableSequenceEvent += (_, e) => events.Add(e);
			playableSequence.ConvertSongNotesToNoteSequence(new SongKey("Test", "C"));
			playableSequence.Prepare();

			playableSequence.Play();

			Assert.Contains(events, e => e.EventType == PlayableEventType.PlayingNote && e.EventDetails.Contains("Playing 1 notes"));
			Assert.Contains(events, e => e.EventType == PlayableEventType.StopSequence && e.EventDetails.Contains("Finished playing"));
		}

		[Fact]
		public void Play_RaisesErrorWhenPlayerThrows()
		{
			var playableSequence = CreateSequence(new ThrowingPlayer());
			var events = new List<PlayableSequenceEvent>();
			playableSequence.PlayableSequenceEvent += (_, e) => events.Add(e);
			playableSequence.ConvertSongNotesToNoteSequence(new SongKey("Test", "C"));
			playableSequence.Prepare();

			playableSequence.Play();

			Assert.Contains(events, e => e.EventType == PlayableEventType.Error && e.EventDetails.Contains("Player failed"));
		}

		[Theory]
		[InlineData(null)]
		[InlineData("")]
		public void LoadFromFile_ReturnsFalseWhenFileNameIsMissing(string fileName)
		{
			var playableSequence = CreateSequence(new TestPlayer());

			var result = playableSequence.LoadFromFile(fileName, "Songs", out var errorString, out Song song);

			Assert.False(result);
			Assert.Null(song);
			Assert.Contains("fileName cannot be null", errorString);
		}

		[Fact]
		public void LoadFromFile_ReturnsFalseWhenJsonCannotDeserialize()
		{
			Directory.CreateDirectory("Songs");
			File.WriteAllText(Path.Combine("Songs", "broken.json"), "not json");
			var playableSequence = CreateSequence(new TestPlayer());

			var result = playableSequence.LoadFromFile("broken", "Songs", out var errorString, out Song song);

			Assert.False(result);
			Assert.Null(song);
			Assert.False(string.IsNullOrWhiteSpace(errorString));
		}

		public void Dispose()
		{
			Environment.CurrentDirectory = originalCurrentDirectory;
			if (Directory.Exists(testDirectory))
			{
				Directory.Delete(testDirectory, true);
			}
		}

		private static PlayableSequence CreateSequence(IPlayer player)
		{
			return new PlayableSequence(() => player, () => new Guitar())
			{
				MeasureTime = 1000,
				Octave = 3,
				InstrumentType = InstrumentType.Horn,
				A4Reference = 440
			};
		}

		private sealed class ThrowingPlayer : IPlayer
		{
			public bool CanPause => false;
			public bool CanStop => false;
			public event PlayerEventHandler PlayerEvent;

			public void Pause()
			{
			}

			public void Play(IEnumerable<FrequencyDuration> noteList, InstrumentType instrument)
			{
				throw new InvalidOperationException("Player failed");
			}

			public void Stop()
			{
			}
		}
	}
}

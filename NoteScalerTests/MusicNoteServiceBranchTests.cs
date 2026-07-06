namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using NoteScaler.Services.Interfaces;
	using System;
	using Xunit;

	public class MusicNoteServiceBranchTests
	{
		[Fact]
		public void MusicNoteFactory_WhenCacheIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MusicNoteFactory(null, new MusicNoteScaleBuilder(), new MusicNoteFrequencyCalculator()));
		}

		[Fact]
		public void MusicNoteFactory_WhenScaleBuilderIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MusicNoteFactory(new MusicNoteCache(), null, new MusicNoteFrequencyCalculator()));
		}

		[Fact]
		public void MusicNoteFactory_WhenFrequencyCalculatorIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MusicNoteFactory(new MusicNoteCache(), new MusicNoteScaleBuilder(), null));
		}

		[Fact]
		public void MusicNotePlayer_WhenFactoryIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => new MusicNotePlayer(null, new MusicNoteChordSelector()));
		}

		[Fact]
		public void MusicNotePlayer_WhenChordSelectorIsNull_ThrowsArgumentNullException()
		{
			var factory = CreateFactory();

			Assert.Throws<ArgumentNullException>(() => new MusicNotePlayer(factory, null));
		}

		[Fact]
		public void MusicNotePlayer_WhenChordSelectionThrows_RaisesError()
		{
			var factory = CreateFactory();
			var note = factory.Create("C4");
			var player = new MusicNotePlayer(factory, new ThrowingChordSelector());
			Exception error = null;
			note.Error += (_, ex) => error = ex;

			player.Play(note);

			Assert.NotNull(error);
			Assert.Contains("selector failure", error.Message);
		}

		[Fact]
		public void MusicNoteChordSelector_WhenChordTypeIsUnknown_ReturnsSingleNote()
		{
			var factory = CreateFactory();
			var note = factory.Create("C4", chordType: (ChordType)999);
			var selector = new MusicNoteChordSelector();

			var actual = selector.SelectChord(note);

			Assert.Equal(new[] { "C" }, actual);
		}

		private static MusicNoteFactory CreateFactory()
		{
			return new MusicNoteFactory(new MusicNoteCache(), new MusicNoteScaleBuilder(), new MusicNoteFrequencyCalculator());
		}

		private sealed class ThrowingChordSelector : IMusicNoteChordSelector
		{
			public string[] SelectChord(MusicNote musicNote)
			{
				throw new InvalidOperationException("selector failure");
			}
		}
	}
}

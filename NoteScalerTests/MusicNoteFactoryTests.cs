namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using System;
	using Xunit;

	public class MusicNoteFactoryTests
	{
		[Fact]
		public void Create_WhenNoteIncludesOctave_CreatesInitializedMusicNote()
		{
			var factory = CreateFactory();

			var actual = factory.Create("C3", duration: 750, currentInstrument: InstrumentType.Flute, chordType: ChordType.MajorThird);

			Assert.NotNull(actual);
			Assert.True(actual.IsValid);
			Assert.Equal("C", actual.Key);
			Assert.Equal(3, actual.DesiredOctave);
			Assert.Equal(750, actual.Duration);
			Assert.Equal(InstrumentType.Flute, actual.Instrument);
			Assert.Equal(ChordType.MajorThird, actual.ChordType);
		}

		[Fact]
		public void Create_WhenNoteIsSharp_UsesSharpNoteContext()
		{
			var factory = CreateFactory();

			var actual = factory.Create("C#3");

			Assert.NotNull(actual);
			Assert.True(actual.IsSharp);
			Assert.Equal("C#", actual.Key);
			Assert.Equal("C", actual.NoteBefore);
			Assert.Equal("D", actual.NoteAfter);
		}

		[Fact]
		public void Create_WhenCalledTwiceWithSameNoteAndReference_ReturnsCachedInstanceAndUpdatesRuntimeOptions()
		{
			var factory = CreateFactory();
			var first = factory.Create("A4", duration: 250, currentInstrument: InstrumentType.Horn);

			var second = factory.Create("A4", duration: 1000, currentInstrument: InstrumentType.Recorder, chordType: ChordType.Power);

			Assert.Same(first, second);
			Assert.Equal(1000, second.Duration);
			Assert.Equal(InstrumentType.Recorder, second.Instrument);
			Assert.Equal(ChordType.Power, second.ChordType);
		}

		[Fact]
		public void Create_WhenReferenceChanges_CachesNotesSeparately()
		{
			var factory = CreateFactory();

			var a440 = factory.Create("A4", a4Reference: 440);
			var a442 = factory.Create("A4", a4Reference: 442);

			Assert.NotSame(a440, a442);
			Assert.True(Math.Abs(a440.CurrentFrequency - 440F) < 0.001F);
			Assert.True(Math.Abs(a442.CurrentFrequency - 442F) < 0.001F);
		}

		[Fact]
		public void Create_WhenNoteIsInvalid_ReturnsNull()
		{
			var factory = CreateFactory();

			var actual = factory.Create("Nope");

			Assert.Null(actual);
		}

		private static MusicNoteFactory CreateFactory()
		{
			return new MusicNoteFactory(new MusicNoteCache(), new MusicNoteScaleBuilder(), new MusicNoteFrequencyCalculator());
		}
	}
}

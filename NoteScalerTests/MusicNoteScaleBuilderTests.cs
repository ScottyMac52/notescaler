namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using System;
	using System.Linq;
	using Xunit;

	public class MusicNoteScaleBuilderTests
	{
		[Fact]
		public void GetNoteContext_ReturnsRotatedSharpAndFlatNotesForRequestedNote()
		{
			var builder = new MusicNoteScaleBuilder();

			var actual = builder.GetNoteContext("D");

			Assert.Equal(2, actual.NoteIndex);
			Assert.Equal("D", actual.SharpNotes.First());
			Assert.Equal("D", actual.FlatNotes.First());
			Assert.Equal("D", actual.SharpNotes.Last());
			Assert.Equal("D", actual.FlatNotes.Last());
		}

		[Fact]
		public void BuildMajorScale_ReturnsExpectedScale()
		{
			var builder = new MusicNoteScaleBuilder();

			var actual = builder.BuildMajorScale("C", ToneTypes.Natural).ToArray();

			Assert.Equal(new[] { "C0", "D0", "E0", "F0", "G0", "A0", "B0", "C1" }, actual);
		}

		[Fact]
		public void BuildMinorScale_ReturnsExpectedScale()
		{
			var builder = new MusicNoteScaleBuilder();

			var actual = builder.BuildMinorScale("A", ToneTypes.Natural).ToArray();

			Assert.Equal(new[] { "A0", "B0", "C1", "D1", "E1", "F1", "G1", "A1" }, actual);
		}

		[Fact]
		public void BuildMajorScale_WhenUsingFlatTone_UsesFlatNotes()
		{
			var builder = new MusicNoteScaleBuilder();

			var actual = builder.BuildMajorScale("Bb", ToneTypes.Flat).ToArray();

			Assert.Contains("Eb1", actual);
			Assert.DoesNotContain("D#1", actual);
		}

		[Fact]
		public void GetNoteContext_WhenNoteIsUnsupported_ThrowsArgumentException()
		{
			var builder = new MusicNoteScaleBuilder();

			var exception = Assert.Throws<ArgumentException>(() => builder.GetNoteContext("Nope"));

			Assert.Contains("Unable to find referenced note", exception.Message);
		}
	}
}

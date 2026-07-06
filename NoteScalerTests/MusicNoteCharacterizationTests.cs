namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using System.Linq;
	using Xunit;

	public class MusicNoteCharacterizationTests
	{
		[Fact]
		public void Create_WhenNoteIncludesOctave_PreservesExistingPublicBehavior()
		{
			var actual = MusicNote.Create("C3", duration: 500);

			Assert.NotNull(actual);
			Assert.True(actual.IsValid);
			Assert.Equal("C", actual.Key);
			Assert.Equal(3, actual.DesiredOctave);
			Assert.Equal(ToneTypes.Natural, actual.ToneType);
			Assert.Equal("C3-500ms", actual.ToString());
			Assert.Equal(new[] { "C0", "D0", "E0", "F0", "G0", "A0", "B0", "C1" }, actual.MajorScale.ToArray());
			Assert.Equal(new[] { "C0", "D0", "D#0", "F0", "G0", "G#0", "A#0", "C1" }, actual.MinorScale.ToArray());
			Assert.Equal(new[] { "C0", "E0", "G0", "B0", "D0", "F0", "A0", "C1" }, actual.MajorChord15);
		}

		[Fact]
		public void Create_WhenNoteIsFlat_PreservesFlatToneAndFlatScaleNames()
		{
			var actual = MusicNote.Create("Bb2");

			Assert.NotNull(actual);
			Assert.True(actual.IsValid);
			Assert.Equal("Bb", actual.Key);
			Assert.Equal(2, actual.DesiredOctave);
			Assert.Equal(ToneTypes.Flat, actual.ToneType);
			Assert.Contains("Eb1", actual.MajorScale);
			Assert.DoesNotContain("D#1", actual.MajorScale);
		}
	}
}

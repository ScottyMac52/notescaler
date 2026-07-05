namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using System.Linq;
	using Xunit;

	public class MusicNoteParsingTests
	{
		[Theory]
		[InlineData("C", "C", 0, ToneTypes.Natural, true, false, false)]
		[InlineData("C#4", "C#", 4, ToneTypes.Sharp, false, true, false)]
		[InlineData("Bb3", "Bb", 3, ToneTypes.Flat, false, false, true)]
		[InlineData("A4", "A", 4, ToneTypes.Natural, true, false, false)]
		public void Create_ParsesToneAndOctave(string note, string expectedKey, int expectedOctave, ToneTypes expectedToneType, bool expectedNatural, bool expectedSharp, bool expectedFlat)
		{
			var actual = MusicNote.Create(note);

			Assert.NotNull(actual);
			Assert.True(actual.IsValid);
			Assert.Equal(expectedKey, actual.Key);
			Assert.Equal(expectedOctave, actual.DesiredOctave);
			Assert.Equal(expectedToneType, actual.ToneType);
			Assert.Equal(expectedNatural, actual.IsNatural);
			Assert.Equal(expectedSharp, actual.IsSharp);
			Assert.Equal(expectedFlat, actual.IsFlat);
		}

		[Theory]
		[InlineData("A4", 440, 4, 440F)]
		[InlineData("A4", 432, 4, 432F)]
		public void Create_UsesRequestedA4ReferenceForCurrentFrequency(string note, int a4Reference, int expectedOctave, float expectedFrequency)
		{
			var actual = MusicNote.Create(note, a4Reference);

			Assert.NotNull(actual);
			Assert.Equal(expectedOctave, actual.DesiredOctave);
			Assert.Equal(expectedFrequency, actual.CurrentFrequency, 1);
		}

		[Theory]
		[InlineData("C", "C0,D0,E0,F0,G0,A0,B0,C1", "C0,D0,D#0,F0,G0,G#0,A#0,C1", "A0")]
		[InlineData("G", "G0,A0,B0,C1,D1,E1,F#1,G1", "G0,A0,A#0,C1,D1,D#1,F1,G1", "E1")]
		public void Create_BuildsMajorMinorAndRelativeMinorScales(string note, string expectedMajorScaleCsv, string expectedMinorScaleCsv, string expectedRelativeMinor)
		{
			var actual = MusicNote.Create(note);

			Assert.Equal(expectedMajorScaleCsv.Split(','), actual.MajorScale.ToArray());
			Assert.Equal(expectedMinorScaleCsv.Split(','), actual.MinorScale.ToArray());
			Assert.Equal(expectedRelativeMinor, actual.RelativeMinor);
		}
	}
}

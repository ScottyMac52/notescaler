namespace NoteScalerTests
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using System;
	using Xunit;

	public class NoteTokenTests
	{
		[Theory]
		[InlineData("C", "C", 0, false, ToneTypes.Natural)]
		[InlineData("C#5", "C#", 5, true, ToneTypes.Sharp)]
		[InlineData("Bb10", "Bb", 10, true, ToneTypes.Flat)]
		public void Parse_ReturnsNoteNameOctaveAndToneType(string token, string expectedName, int expectedOctave, bool expectedHasOctave, ToneTypes expectedToneType)
		{
			var actual = NoteToken.Parse(token);

			Assert.Equal(expectedName, actual.Name);
			Assert.Equal(expectedOctave, actual.Octave);
			Assert.Equal(expectedHasOctave, actual.HasOctave);
			Assert.Equal(expectedToneType, actual.ToneType);
		}

		[Fact]
		public void Parse_WhenTokenIsNull_ThrowsArgumentNullException()
		{
			Assert.Throws<ArgumentNullException>(() => NoteToken.Parse(null));
		}
	}
}

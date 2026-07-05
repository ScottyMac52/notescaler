namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class GuitarAdditionalTests
	{
		[Theory]
		[InlineData(TuningScheme.Standard, "Guitar tuned to Standard. Strings E2|A2|D3|G3|B3|E4")]
		[InlineData(TuningScheme.DropC, "Guitar tuned to DropC. Strings C2|G2|C3|F3|A3|D4")]
		[InlineData(TuningScheme.DropCSharp, "Guitar tuned to DropCSharp. Strings C#2|G#2|C#3|F#3|A#3|D#4")]
		[InlineData(TuningScheme.DropD, "Guitar tuned to DropD. Strings D2|A2|D3|G3|B3|D4")]
		[InlineData(TuningScheme.OpenC, "Guitar tuned to OpenC. Strings C2|G2|C3|G3|C3|E4")]
		[InlineData(TuningScheme.OpenD, "Guitar tuned to OpenD. Strings D2|A2|D3|F#3|A3|D4")]
		public void ToString_DescribesConfiguredTuning(TuningScheme tuningScheme, string expected)
		{
			var guitar = new Guitar(tuningScheme);

			Assert.Equal(expected, guitar.ToString());
		}

		[Theory]
		[InlineData(6, "C2", 0, "C2")]
		[InlineData(1, "D4", 0, "D4")]
		public void CustomTune_UpdatesRequestedString(int stringNumber, string tuning, int fret, string expectedNote)
		{
			var guitar = new Guitar();

			guitar.CustomTune(stringNumber, tuning);

			Assert.Equal(expectedNote, guitar.GetNote(stringNumber, fret));
		}

		[Theory]
		[InlineData(7, 0, 8, 0, "There is no string number: 7")]
		[InlineData(1, 25, 2, 0, "Fret: 25 doesn't exist on String: 1.")]
		public void GetNoteInterval_ReturnsErrorsForInvalidLocations(int startString, int startFret, int endString, int endFret, string expectedError)
		{
			var guitar = new Guitar();

			var interval = guitar.GetNoteInterval(startString, startFret, endString, endFret, out List<Exception> errors);

			Assert.Equal(0, interval);
			Assert.Contains(errors, error => error.Message.Contains(expectedError));
		}

		[Theory]
		[InlineData(7, "C4", "There is no string 7.")]
		[InlineData(6, "C9", "String 6 does not contain the note: C9")]
		public void GetNote_ThrowsWhenStringOrNoteIsInvalid(int stringNumber, string note, string expectedMessage)
		{
			var guitar = new Guitar();

			var exception = Assert.Throws<ArgumentException>(() => guitar.GetNote(stringNumber, note));

			Assert.Contains(expectedMessage, exception.Message);
		}
	}
}

namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Services;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public class GuitarTuningCatalogTests
	{
		public static IEnumerable<object[]> ExpectedTuningDefinitions => new[]
		{
			new object[] { TuningScheme.Standard, new[] { "E4", "B3", "G3", "D3", "A2", "E2" } },
			new object[] { TuningScheme.DropC, new[] { "D4", "A3", "F3", "C3", "G2", "C2" } },
			new object[] { TuningScheme.DropCSharp, new[] { "D#4", "A#3", "F#3", "C#3", "G#2", "C#2" } },
			new object[] { TuningScheme.DropD, new[] { "D4", "B3", "G3", "D3", "A2", "D2" } },
			new object[] { TuningScheme.OpenC, new[] { "E4", "C3", "G3", "C3", "G2", "C2" } },
			new object[] { TuningScheme.OpenD, new[] { "D4", "A3", "F#3", "D3", "A2", "D2" } },
			new object[] { TuningScheme.DropB, new[] { "C#4", "G#3", "E3", "B2", "F#2", "B1" } },
			new object[] { TuningScheme.DStandard, new[] { "D4", "A3", "F3", "C3", "G2", "D2" } },
			new object[] { TuningScheme.EbStandard, new[] { "D#4", "A#3", "F#3", "C#3", "G#2", "D#2" } },
			new object[] { TuningScheme.CSharpStandard, new[] { "C#4", "G#3", "E3", "B2", "F#2", "C#2" } },
			new object[] { TuningScheme.DADGAD, new[] { "D4", "A3", "G3", "D3", "A2", "D2" } },
			new object[] { TuningScheme.OpenG, new[] { "D4", "B3", "G3", "D3", "G2", "D2" } }
		};

		[Theory]
		[MemberData(nameof(ExpectedTuningDefinitions))]
		public void GetDefinition_ReturnsExpectedOpenStringNotes(TuningScheme tuningScheme, string[] expectedOpenStringNotes)
		{
			var definition = GuitarTuningCatalog.GetDefinition(tuningScheme);

			Assert.Equal(tuningScheme, definition.TuningScheme);
			Assert.Equal(expectedOpenStringNotes, definition.OpenStringNotes);
		}

		[Fact]
		public void SupportedTunings_ContainsEveryTuningScheme()
		{
			var expectedTunings = Enum.GetValues<TuningScheme>().OrderBy(tuningScheme => tuningScheme).ToArray();
			var actualTunings = GuitarTuningCatalog.SupportedTunings
				.Select(definition => definition.TuningScheme)
				.OrderBy(tuningScheme => tuningScheme)
				.ToArray();

			Assert.Equal(expectedTunings, actualTunings);
		}

		[Fact]
		public void SupportedTunings_ReturnsDefinitionsWithOpenStringData()
		{
			var supportedTunings = GuitarTuningCatalog.SupportedTunings.ToArray();

			Assert.All(supportedTunings, definition => Assert.NotEmpty(definition.OpenStringNotes));
			Assert.All(supportedTunings, definition => Assert.All(definition.OpenStringNotes, Assert.False));
		}

		[Fact]
		public void GetDefinition_WhenTuningSchemeIsUnsupported_ThrowsControlledException()
		{
			var exception = Assert.Throws<ArgumentException>(() => GuitarTuningCatalog.GetDefinition((TuningScheme)999));

			Assert.Equal("tuningScheme", exception.ParamName);
			Assert.Contains("Unsupported tuning scheme", exception.Message);
		}
	}
}

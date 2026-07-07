namespace NoteScalerTests.Models
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public class TuningDefinitionTests
	{
		[Fact]
		public void Constructor_RequiresOpenStringNotes()
		{
			var exception = Assert.Throws<ArgumentNullException>(() => new TuningDefinition(TuningScheme.Standard, null));

			Assert.Equal("openStringNotes", exception.ParamName);
		}

		[Fact]
		public void Constructor_RequiresAtLeastOneOpenStringNote()
		{
			var exception = Assert.Throws<ArgumentException>(() => new TuningDefinition(TuningScheme.Standard, Array.Empty<string>()));

			Assert.Equal("openStringNotes", exception.ParamName);
			Assert.Contains("at least one", exception.Message);
		}

		[Theory]
		[InlineData("")]
		[InlineData(" ")]
		public void Constructor_RejectsEmptyOpenStringNotes(string invalidNote)
		{
			var exception = Assert.Throws<ArgumentException>(() => new TuningDefinition(TuningScheme.Standard, new[] { "E4", invalidNote }));

			Assert.Equal("openStringNotes", exception.ParamName);
			Assert.Contains("empty", exception.Message);
		}

		[Fact]
		public void Constructor_CopiesOpenStringNotesIntoDefinition()
		{
			var notes = new[] { "E4", "B3", "G3", "D3", "A2", "E2" };

			var definition = new TuningDefinition(TuningScheme.Standard, notes);
			notes[0] = "D4";

			Assert.Equal(TuningScheme.Standard, definition.TuningScheme);
			Assert.Equal(new[] { "E4", "B3", "G3", "D3", "A2", "E2" }, definition.OpenStringNotes);
		}

		[Fact]
		public void OpenStringNotes_CannotBeMutatedThroughExposedCollection()
		{
			var definition = new TuningDefinition(TuningScheme.Standard, new[] { "E4", "B3" });
			var exposedNotes = Assert.IsAssignableFrom<IList<string>>(definition.OpenStringNotes);

			Assert.Throws<NotSupportedException>(() => exposedNotes[0] = "D4");
		}
	}
}

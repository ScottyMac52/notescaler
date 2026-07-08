namespace NoteScalerTests.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using NoteScaler.Services;
	using System.Linq;
	using Xunit;

	public class GuitarPerformanceEventFactoryTests
	{
		[Fact]
		public void Create_WhenTabContainsSingleFrettedNote_PreservesGuitarMetadata()
		{
			var factory = new GuitarPerformanceEventFactory();
			var guitar = new Guitar(TuningScheme.Standard, 24);
			var tablature = new Tablature
			{
				Name = "Single Note",
				Tuning = "Standard",
				TabString = "1-3"
			};

			var performanceEvent = Assert.Single(factory.Create(guitar, tablature, 1000));

			Assert.Equal("G4", performanceEvent.Note);
			Assert.Equal(1, performanceEvent.StringNumber);
			Assert.Equal(3, performanceEvent.Fret);
			Assert.Equal(0, performanceEvent.StartOffsetMilliseconds);
			Assert.Equal(1000, performanceEvent.DurationMilliseconds);
			Assert.Equal(100, performanceEvent.Velocity);
			Assert.Equal(GuitarArticulation.Pick, performanceEvent.Articulation);
		}

		[Fact]
		public void Create_WhenTabContainsChordGroup_UsesSameStartOffsetForAllChordNotes()
		{
			var factory = new GuitarPerformanceEventFactory();
			var guitar = new Guitar(TuningScheme.Standard, 24);
			var tablature = new Tablature
			{
				Name = "Chord",
				Tuning = "Standard",
				TabString = "1-0|2-1|3-0"
			};

			var performanceEvents = factory.Create(guitar, tablature, 1000).ToArray();

			Assert.Equal(3, performanceEvents.Length);
			Assert.All(performanceEvents, performanceEvent => Assert.Equal(0, performanceEvent.StartOffsetMilliseconds));
			Assert.Equal(new[] { "E4", "C4", "G3" }, performanceEvents.Select(performanceEvent => performanceEvent.Note));
		}

		[Fact]
		public void Create_WhenTabContainsSequentialGroups_AdvancesStartOffsetByGroupDuration()
		{
			var factory = new GuitarPerformanceEventFactory();
			var guitar = new Guitar(TuningScheme.Standard, 24);
			var tablature = new Tablature
			{
				Name = "Sequential",
				Tuning = "Standard",
				TabString = "1-0,2-1,3-0"
			};

			var performanceEvents = factory.Create(guitar, tablature, 1000).ToArray();

			Assert.Equal(new[] { 0, 1000, 2000 }, performanceEvents.Select(performanceEvent => performanceEvent.StartOffsetMilliseconds));
		}

		[Fact]
		public void Create_WhenTabContainsDurationModifier_UsesMeasureTimePercentage()
		{
			var factory = new GuitarPerformanceEventFactory();
			var guitar = new Guitar(TuningScheme.Standard, 24);
			var tablature = new Tablature
			{
				Name = "Duration",
				Tuning = "Standard",
				TabString = "1-0-0.5,2-1"
			};

			var performanceEvents = factory.Create(guitar, tablature, 1000).ToArray();

			Assert.Equal(500, performanceEvents[0].DurationMilliseconds);
			Assert.Equal(0, performanceEvents[0].StartOffsetMilliseconds);
			Assert.Equal(1000, performanceEvents[1].DurationMilliseconds);
			Assert.Equal(500, performanceEvents[1].StartOffsetMilliseconds);
		}

		[Fact]
		public void Create_WhenChordContainsMixedDurations_AdvancesStartOffsetByLongestChordDuration()
		{
			var factory = new GuitarPerformanceEventFactory();
			var guitar = new Guitar(TuningScheme.Standard, 24);
			var tablature = new Tablature
			{
				Name = "Mixed Duration Chord",
				Tuning = "Standard",
				TabString = "1-0-0.5|2-1,3-0"
			};

			var performanceEvents = factory.Create(guitar, tablature, 1000).ToArray();

			Assert.Equal(0, performanceEvents[0].StartOffsetMilliseconds);
			Assert.Equal(0, performanceEvents[1].StartOffsetMilliseconds);
			Assert.Equal(1000, performanceEvents[2].StartOffsetMilliseconds);
		}
	}
}

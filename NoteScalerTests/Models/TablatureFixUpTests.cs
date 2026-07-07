namespace NoteScalerTests.Models
{
	using NoteScaler.Models;
	using System.Linq;
	using Xunit;

	public class TablatureFixUpTests
	{
		[Theory]
		[InlineData("Lead", "Lead", "1-3,1-5", "DropD")]
		[InlineData("Rhythm", "Rhythm", "6-0,5-2", "DropC")]
		public void FixUp_UsesDefaultVersionWhenItExists(string defaultVersion, string expectedName, string expectedTab, string expectedTuning)
		{
			var tablature = new Tablature
			{
				Name = "Original",
				Speed = 800,
				TabString = "1-0",
				Tuning = "Standard",
				Default = defaultVersion,
				TabVersions = new[]
				{
					new TabVersion { Name = "Lead", Speed = 0, TabString = "1-3,1-5", Tuning = "DropD" },
					new TabVersion { Name = "Rhythm", Speed = 1200, TabString = "6-0,5-2", Tuning = "DropC" }
				}
			};

			tablature.FixUp();

			Assert.Equal(expectedName, tablature.Name);
			Assert.Equal(expectedTab, tablature.TabString);
			Assert.Equal(expectedTuning, tablature.Tuning);
		}

		[Fact]
		public void FixUp_AppliesParentSpeedToVersionsWithoutSpeed()
		{
			var tablature = new Tablature
			{
				Speed = 900,
				TabVersions = new[]
				{
					new TabVersion { Name = "Slow", Speed = 0, TabString = "1-1", Tuning = "Standard" },
					new TabVersion { Name = "Fast", Speed = 1200, TabString = "1-2", Tuning = "Standard" }
				}
			};

			tablature.FixUp();

			Assert.Equal(900, tablature.TabVersions.Single(version => version.Name == "Slow").Speed);
			Assert.Equal(1200, tablature.TabVersions.Single(version => version.Name == "Fast").Speed);
		}
	}
}

namespace NoteScalerTests.Models
{
	using NoteScaler.Models;
	using Xunit;

	public class TablatureAdditionalFixUpTests
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("Missing")]
		public void FixUp_LeavesCurrentTabWhenDefaultIsMissing(string defaultVersion)
		{
			var tablature = new Tablature
			{
				Name = "Current",
				Speed = 600,
				TabString = "1-0",
				Tuning = "Standard",
				Default = defaultVersion,
				TabVersions = new[]
				{
					new TabVersion { Name = "Lead", Speed = 600, TabString = "1-3", Tuning = "DropD" }
				}
			};

			tablature.FixUp();

			Assert.Equal("Current", tablature.Name);
			Assert.Equal("1-0", tablature.TabString);
			Assert.Equal("Standard", tablature.Tuning);
		}
	}
}

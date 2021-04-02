
namespace NoteScalerTests
{
	using Newtonsoft.Json;
	using NoteScaler.Enums;
	using NoteScaler.Models;
	using Xunit;

	public class TabVersionTests : TestBase
	{
		private readonly string expectedName;
		private readonly int expectedSpeed;
		private readonly string expectedTabString;
		private readonly TuningScheme expectedTuning;
		private TabVersion actual;
		private readonly string jsonFormatString;

		public TabVersionTests()
		{
			jsonFormatString = "\"name\": \"{0}\", \"speed\": {1}, \"tab\": \"{2}\", \"tuning\": \"{3}\"";
			expectedName = "TabSong";
			expectedSpeed = 1000;
			expectedTabString = "1-3, 1-5, 1-7";
			expectedTuning = TuningScheme.Standard;
		}

		[Fact]
		public void TabVersionTest_EnsureTabVersionIsDeserialized()
		{
			// ARRANGE
			var jsonString = string.Format(jsonFormatString, expectedName, expectedSpeed, expectedTabString, expectedTuning);

			// ACT 
			jsonString = FixupJsonString(jsonString);
			actual = JsonConvert.DeserializeObject<TabVersion>(jsonString);

			// ASSERT
			Assert.Equal(expectedName, actual.Name);
			Assert.Equal(expectedSpeed, actual.Speed);
			Assert.Equal(expectedTabString, actual.TabString);
			Assert.Equal(expectedTuning, actual.Tuning);
		}

		[Fact]
		public void TabVersionTest_EnsureExceptionOnInvalidTuning()
		{
			// ARRANGE
			var invalidTuning = "invalid";
			var jsonString = string.Format(jsonFormatString, expectedName, expectedSpeed, expectedTabString, invalidTuning);
			jsonString = FixupJsonString(jsonString);

			// ACT 
			// ASSERT
			Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<TabVersion>(jsonString));
		}
	}
}

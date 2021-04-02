namespace NoteScalerTests
{
	using Newtonsoft.Json;
	using NoteScaler.Models;
	using System.Collections.Generic;
	using System.Linq;
	using Xunit;

	public class SongKeyTests : TestBase
	{
		private readonly string expectedKeyName;
		private readonly string expectedSequence;
		private readonly IEnumerable<string> expectedSongSequence;
		private SongKey actual;
		private string jsonFormatString;
		private string invalidJson;

		public SongKeyTests()
		{
			jsonFormatString = "\"keyName\": \"{0}\", \"sequence\": \"{1}\"";
			invalidJson = "\"sequence\": \"{0}\"";
			expectedKeyName = "A";
			expectedSequence = "A, B, C";
			expectedSongSequence = new string[] { "A", "B", "C" };
		}

		[Fact]
		public void SongKeyTest_EnsureArrayValuesAreTrimmed()
		{
			// ARRANGE
			var songName = expectedKeyName;
			var songSequence = expectedSequence;

			// ACT
			actual = new SongKey(songName, songSequence);

			// ASSERT
			AssertValues();
		}

		[Fact]
		public void SongKeyTest_EnsureDeserializedValuesAreTrimmed()
		{
			// ARRANGE
			var jsonString = FixupJsonString(string.Format(jsonFormatString, expectedKeyName, expectedSequence));

			// ACT
			actual = JsonConvert.DeserializeObject<SongKey>(jsonString);

			// ASSERT
			AssertValues();
		}

		[Fact]
		public void SongKeyTest_EnsureExceptionOnNullValues()
		{
			// ARRANGE
			var jsonString = FixupJsonString(string.Format(invalidJson, expectedSequence));

			// ACT
			// ASSERT
			Assert.Throws<JsonSerializationException>(() => JsonConvert.DeserializeObject<SongKey>(jsonString));
		}

		private void AssertValues()
		{
			Assert.Equal(expectedKeyName, actual.Name);
			Assert.Equal(expectedSequence, actual.Sequence);
			Assert.True(Enumerable.SequenceEqual(expectedSongSequence, actual.SongNotes));

		}
	}
}

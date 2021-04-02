namespace NoteScalerTests
{
	public abstract class TestBase
	{
		private const string JSON_START = "{";
		private const string JSON_END = "}";

		protected string FixupJsonString(string jsonString)
		{
			if(!jsonString.StartsWith(JSON_START))
			{
				jsonString = jsonString.Insert(0, JSON_START);
			}
			if(!jsonString[^1].Equals(JSON_END))
			{
				jsonString = jsonString.Insert(jsonString.Length, JSON_END);
			}
			return jsonString;
		}
	}
}

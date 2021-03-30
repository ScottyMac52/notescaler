namespace NoteScaler.Enums
{
	using Newtonsoft.Json;

	public enum TuningScheme
	{
		[JsonProperty("standard")]
		Standard,
		[JsonProperty("dropC")]
		DropC,
		[JsonProperty("dropCSharp")]
		DropCSharp,
		[JsonProperty("dropD")]
		DropD,
		[JsonProperty("openC")]
		OpenC,
		[JsonProperty("openD")]
		OpenD
	}
}
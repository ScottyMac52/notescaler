namespace NoteScaler.Models
{
	using Newtonsoft.Json;
	using System.Collections.Generic;
	using System.Linq;

	public class Tablature : TabVersion
	{
		[JsonProperty("repeat")]
		public int? Repeat { get; set; } = 1;

		[JsonProperty("default")]
		public string Default { get; set; }

		[JsonProperty("versions")]
		public IEnumerable<TabVersion> TabVersions { get; set; }

		public void FixUp()
		{
			TabVersions.Where(tv => tv.Speed == 0).ToList().ForEach(version =>
			{
				version.Speed = Speed;
			});

			if(!string.IsNullOrEmpty(Default) && TabVersions.Any(tv => tv.Name == Default))
			{
				var currentVersion = TabVersions.Single(tv => tv.Name == Default);
				Name = currentVersion.Name;
				TabString = currentVersion.TabString;
				Tuning = currentVersion.Tuning;
			}
		}
	}
}

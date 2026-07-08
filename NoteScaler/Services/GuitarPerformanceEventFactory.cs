namespace NoteScaler.Services
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System.Collections.Generic;
	using System.Linq;

	public sealed class GuitarPerformanceEventFactory : IGuitarPerformanceEventFactory
	{
		public IEnumerable<GuitarPerformanceEvent> Create(IStringInstrument stringInstrument, Tablature tablature, int measureTime, int velocity = 100)
		{
			var startOffsetMilliseconds = 0;
			foreach (var group in GetGroups(tablature))
			{
				var events = group
					.Select(item => BuildEvent(stringInstrument, item, measureTime, startOffsetMilliseconds, velocity))
					.ToArray();

				foreach (var currentEvent in events)
				{
					yield return currentEvent;
				}

				startOffsetMilliseconds += events.Any() ? events.Max(item => item.DurationMilliseconds) : measureTime;
			}
		}

		private static IEnumerable<string[]> GetGroups(Tablature tablature)
		{
			return (tablature?.TabString ?? string.Empty)
				.Split(',')
				.Where(group => !string.IsNullOrWhiteSpace(group))
				.Select(group => group.Split('|'));
		}

		private static GuitarPerformanceEvent BuildEvent(IStringInstrument stringInstrument, string tabItem, int measureTime, int startOffsetMilliseconds, int velocity)
		{
			var parts = tabItem.Split('-');
			var stringNumber = int.Parse(parts[0]);
			var fret = int.Parse(parts[1]);
			var durationMilliseconds = GetDurationMilliseconds(parts, measureTime);
			var note = stringInstrument.GetNote(stringNumber, fret);

			return new GuitarPerformanceEvent(note, stringNumber, fret, startOffsetMilliseconds, durationMilliseconds, velocity, GuitarArticulation.Pick);
		}

		private static int GetDurationMilliseconds(string[] parts, int measureTime)
		{
			if (parts.Length > 2)
			{
				return (int)(measureTime * float.Parse(parts[2]));
			}

			return measureTime;
		}
	}
}

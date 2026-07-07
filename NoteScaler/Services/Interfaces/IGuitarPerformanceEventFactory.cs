namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Interfaces;
	using NoteScaler.Models;
	using System.Collections.Generic;

	public interface IGuitarPerformanceEventFactory
	{
		IEnumerable<GuitarPerformanceEvent> Create(IStringInstrument stringInstrument, Tablature tablature, int measureTime, int velocity = 100);
	}
}

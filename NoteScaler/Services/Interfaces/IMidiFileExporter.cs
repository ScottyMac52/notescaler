namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Models;
	using System.Collections.Generic;

	public interface IMidiFileExporter
	{
		void Export(IEnumerable<GuitarPerformanceEvent> performanceEvents, string outputPath);
		void ExportCompositeNotes(IEnumerable<CompositeNote> compositeNotes, string outputPath);
	}
}

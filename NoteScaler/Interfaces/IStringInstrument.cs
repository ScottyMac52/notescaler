namespace NoteScaler.Interfaces
{
	using NoteScaler.Classes;
	using NoteScaler.Enums;
	using System;
	using System.Collections.Generic;

	public interface IStringInstrument
	{
		IEnumerable<IInstrumentString> Strings { get; }
		TuningScheme TuningScheme { get; }
		void CustomTune(int stringNumber, string noteToTune);
		string GetNote(int stringNumber, int fret);
		int GetNote(int stringNumber, string note);
		int GetNoteInterval(int startString, int startFret, int endString, int endFret, out List<Exception> errorList);
		int GetNoteInterval(string startNote, string endNote);
	}
}
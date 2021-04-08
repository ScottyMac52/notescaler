namespace NoteScaler.Interfaces
{
	using NoteScaler.Enums;
	using System;
	using System.Collections.Generic;

	public interface IStringInstrument
	{
		/// <summary>
		/// The number of Frets 
		/// </summary>
		int Frets { get; }

		/// <summary>
		/// Instrument strings
		/// </summary>
		IEnumerable<IInstrumentString> Strings { get; }
		
		/// <summary>
		/// <see cref="TuningScheme"/> for the string
		/// </summary>
		TuningScheme TuningScheme { get; }

		/// <summary>
		/// The string can be tuned to any note
		/// </summary>
		/// <param name="stringNumber"></param>
		/// <param name="noteToTune"></param>
		void CustomTune(int stringNumber, string noteToTune);
		
		/// <summary>
		/// Gets the note from the specified string and fret
		/// </summary>
		/// <param name="stringNumber"></param>
		/// <param name="fret"></param>
		/// <returns></returns>
		string GetNote(int stringNumber, int fret);
		
		/// <summary>
		/// Gets the fret from the specified string and note
		/// </summary>
		/// <param name="stringNumber"></param>
		/// <param name="note"></param>
		/// <returns></returns>
		int GetNote(int stringNumber, string note);

		/// <summary>
		/// Gets the distance between the two notes
		/// </summary>
		/// <param name="startString"></param>
		/// <param name="startFret"></param>
		/// <param name="endString"></param>
		/// <param name="endFret"></param>
		/// <param name="errorList"></param>
		/// <returns></returns>
		int GetNoteInterval(int startString, int startFret, int endString, int endFret, out List<Exception> errorList);

		/// <summary>
		/// Gets the distance between the two notes
		/// </summary>
		/// <param name="startNote"></param>
		/// <param name="endNote"></param>
		/// <returns></returns>
		int GetNoteInterval(string startNote, string endNote);
	}
}
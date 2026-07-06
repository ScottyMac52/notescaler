namespace NoteScaler.Services.Interfaces
{
	using NoteScaler.Enums;
	using NoteScaler.Interfaces;

	public interface IMusicNoteFactory
	{
		MusicNote Create(string note, int a4Reference = 440, IPlayer player = null, int duration = 500, InstrumentType currentInstrument = default, ChordType chordType = ChordType.Note);
	}
}

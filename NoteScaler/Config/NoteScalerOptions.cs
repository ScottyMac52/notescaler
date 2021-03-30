namespace NoteScaler.Config
{
	using CommandLine;
	using NoteScaler.Enums;

	public class NoteScalerOptions
	{
		[Option('r', "range", Default = 440, HelpText = "The frequency of the A4 Note, defaults to 440.")] 
		public int? Range { get; set; }

		[Option('o', "octave", Default = 3, HelpText = "The starting Octave for all voicings.")] 
		public int? Octave { get; set; }

		[Option('w', "prewait", Default = 0, HelpText = "How many measures to pause before starting to play.")] 
		public int? PreWait { get; set; }

		[Option('k', "key", Required = false, Default = null, HelpText = "For sequences and songs, allows the key to be selected.")]
		public string Key { get; set; }

		[Option('s', "speed", Default = 300, HelpText = "Starting speed i.e. bpm in a full measure.")] 
		public int? Speed { get; set; }

		[Option('i', "instrument", Default = InstrumentType.Horn, HelpText = "Instrument to start playing.")] 
		public InstrumentType Instrument { get; set; }

		[Option('n', "note", Default = null, HelpText = "Note to display details for and to play Major and Minor Scales")] 
		public string Note { get; set; }

		[Option('f', "file", Default = null, HelpText = "Name of the file to play from the Songs directory.")] 
		public string File { get; set; }

		[Option('t', "tab", Default = null, HelpText = "Name of the tab file to play from the Tabs directory.")]
		public string Tab { get; set; }

	}
}

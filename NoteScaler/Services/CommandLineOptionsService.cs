namespace NoteScaler.Services
{
	using CommandLine;
	using NoteScaler.Config;
	using NoteScaler.Services.Interfaces;
	using System.Collections.Generic;

	public sealed class CommandLineOptionsService : ICommandLineOptionsService
	{
		public ParserResult<NoteScalerOptions> ParseArguments(IEnumerable<string> args)
		{
			return Parser.Default.ParseArguments<NoteScalerOptions>(args);
		}
	}
}

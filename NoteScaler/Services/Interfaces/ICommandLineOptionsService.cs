namespace NoteScaler.Services.Interfaces
{
	using CommandLine;
	using NoteScaler.Config;
	using System.Collections.Generic;

	public interface ICommandLineOptionsService
	{
		ParserResult<NoteScalerOptions> ParseArguments(IEnumerable<string> args);
	}
}

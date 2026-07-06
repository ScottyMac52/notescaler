namespace NoteScaler
{
	using Microsoft.Extensions.DependencyInjection;
	using NoteScaler.Services;
	using NoteScaler.Services.Interfaces;
	using System.Diagnostics.CodeAnalysis;

	[ExcludeFromCodeCoverage]
	class Program
	{
		public static int Main(string[] args)
		{
			var serviceProvider = ConfigureServices();
			var runner = serviceProvider.GetRequiredService<INoteScalerRunner>();
			return runner.Run(args);
		}

		private static ServiceProvider ConfigureServices()
		{
			var services = new ServiceCollection();

			services.AddSingleton<ICommandLineOptionsService, CommandLineOptionsService>();
			services.AddSingleton<IConsoleOutputService, ConsoleOutputService>();
			services.AddSingleton<IPlayerFactory, SignalNotePlayerFactory>();
			services.AddSingleton<IStringInstrumentFactory, StringInstrumentFactory>();
			services.AddSingleton<IMusicNoteCache, MusicNoteCache>();
			services.AddSingleton<MusicNoteScaleBuilder>();
			services.AddSingleton<MusicNoteFrequencyCalculator>();
			services.AddSingleton<IMusicNoteFactory, MusicNoteFactory>();
			services.AddSingleton<IMusicNoteChordSelector, MusicNoteChordSelector>();
			services.AddSingleton<IMusicNotePlayer, MusicNotePlayer>();
			services.AddSingleton<IPlayableSequenceFactory, PlayableSequenceFactory>();
			services.AddSingleton<INoteScalerRunner, NoteScalerRunner>();

			return services.BuildServiceProvider();
		}
	}
}
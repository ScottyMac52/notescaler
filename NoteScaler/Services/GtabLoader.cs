namespace NoteScaler.Services
{
	using Newtonsoft.Json;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;

	public sealed class GtabLoader : IGtabLoader
	{
		private const string GtabDirectory = "GTabs";

		private static readonly IDictionary<string, string> KnownTunings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			{ "E,A,D,G,B,E", "Standard" },
			{ "D,A,D,G,B,E", "Drop D" },
			{ "D,G,C,F,A,D", "D Standard" },
			{ "D#,G#,C#,F#,A#,D#", "Eb Standard" },
			{ "Eb,Ab,Db,Gb,Bb,Eb", "Eb Standard" },
			{ "C#,F#,B,E,G#,C#", "C# Standard" },
			{ "Db,Gb,B,E,Ab,Db", "C# Standard" }
		};

		public bool Load(string gtabName, out string errorString, out Tablature tablature)
		{
			tablature = null;
			if (string.IsNullOrWhiteSpace(gtabName))
			{
				errorString = "gtabName cannot be null";
				return false;
			}

			var path = ResolvePath(gtabName);
			if (!File.Exists(path))
			{
				errorString = new FileNotFoundException($"File not found in {Path.GetDirectoryName(path)}", Path.GetFileName(path)).ToString();
				return false;
			}

			try
			{
				var document = JsonConvert.DeserializeObject<GtabDocument>(File.ReadAllText(path));
				Validate(document);
				tablature = ToTablature(document);
				errorString = null;
				return true;
			}
			catch (Exception ex)
			{
				errorString = ex.Message;
				return false;
			}
		}

		private static string ResolvePath(string gtabName)
		{
			var fileName = Path.HasExtension(gtabName) ? gtabName : $"{gtabName}.gtab";
			if (Path.IsPathRooted(fileName) || fileName.Contains(Path.DirectorySeparatorChar.ToString()) || fileName.Contains(Path.AltDirectorySeparatorChar.ToString()))
			{
				return fileName;
			}

			var currentDirectoryPath = Path.Combine(Environment.CurrentDirectory, fileName);
			if (File.Exists(currentDirectoryPath))
			{
				return currentDirectoryPath;
			}

			return Path.Combine(Environment.CurrentDirectory, GtabDirectory, fileName);
		}

		private static void Validate(GtabDocument document)
		{
			if (document == null)
			{
				throw new InvalidOperationException("Invalid .gtab document.");
			}

			if (string.IsNullOrWhiteSpace(document.Title))
			{
				throw new InvalidOperationException(".gtab title is required.");
			}

			if (document.StringNotes == null || !document.StringNotes.Any())
			{
				throw new InvalidOperationException(".gtab stringNotes are required.");
			}

			if (document.TabRows == null || !document.TabRows.Any())
			{
				throw new InvalidOperationException(".gtab tabRows are required.");
			}
		}

		private static Tablature ToTablature(GtabDocument document)
		{
			var stringNotes = document.StringNotes.ToArray();
			return new Tablature
			{
				Name = document.Title,
				Speed = GetMeasureTimeFromTempo(document.Tempo),
				Tuning = GetTuningName(stringNotes),
				TabString = GetTabString(document, stringNotes.Length),
				Repeat = 1,
				NumberOfStrings = stringNotes.Length,
				TabVersions = Array.Empty<TabVersion>()
			};
		}

		private static int GetMeasureTimeFromTempo(int tempo)
		{
			return tempo > 0 ? (int)Math.Round(60000D / tempo) : 0;
		}

		private static string GetTuningName(IEnumerable<string> stringNotes)
		{
			var tuningKey = string.Join(",", stringNotes.Select(note => note?.Trim() ?? string.Empty));
			if (KnownTunings.TryGetValue(tuningKey, out var tuningName))
			{
				return tuningName;
			}

			throw new InvalidOperationException($"Unsupported .gtab tuning: {tuningKey}.");
		}

		private static string GetTabString(GtabDocument document, int numberOfStrings)
		{
			var groups = new List<string>();
			IEnumerable<string> pendingNotes = null;
			var durationColumns = 1;

			foreach (var column in GetColumns(document))
			{
				var currentNotes = GetNotes(column, numberOfStrings, document.CapoFret).ToArray();
				if (currentNotes.Any())
				{
					if (pendingNotes != null)
					{
						groups.Add(FormatGroup(pendingNotes, durationColumns));
					}

					pendingNotes = currentNotes;
					durationColumns = 1;
				}
				else if (pendingNotes != null)
				{
					durationColumns++;
				}
			}

			if (pendingNotes != null)
			{
				groups.Add(FormatGroup(pendingNotes, durationColumns));
			}

			return string.Join(",", groups);
		}

		private static IEnumerable<IEnumerable<GtabCell>> GetColumns(GtabDocument document)
		{
			return document.TabRows
				.SelectMany(row => row.Columns ?? Array.Empty<IEnumerable<GtabCell>>());
		}

		private static IEnumerable<string> GetNotes(IEnumerable<GtabCell> column, int numberOfStrings, int capoFret)
		{
			var cells = column?.ToArray() ?? Array.Empty<GtabCell>();
			for (var index = 0; index < cells.Length; index++)
			{
				var position = cells[index]?.Position;
				if (int.TryParse(position, NumberStyles.Integer, CultureInfo.InvariantCulture, out var fret))
				{
					var stringNumber = numberOfStrings - index;
					yield return $"{stringNumber}-{fret + capoFret}";
				}
			}
		}

		private static string FormatGroup(IEnumerable<string> notes, int durationColumns)
		{
			var suffix = durationColumns > 1 ? $"-{durationColumns}" : string.Empty;
			return string.Join("|", notes.Select(note => $"{note}{suffix}"));
		}
	}
}

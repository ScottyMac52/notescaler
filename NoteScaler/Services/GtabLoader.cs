namespace NoteScaler.Services
{
	using Newtonsoft.Json;
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.IO;

	public sealed class GtabLoader : IGtabLoader
	{
		private const int SupportedSchemaVersion = 1;
		private const string GtabDirectory = "GTabs";

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

			if (document.SchemaVersion != SupportedSchemaVersion)
			{
				throw new InvalidOperationException($"Unsupported .gtab schemaVersion: {document.SchemaVersion}.");
			}

			if (string.IsNullOrWhiteSpace(document.Name))
			{
				throw new InvalidOperationException(".gtab name is required.");
			}

			if (string.IsNullOrWhiteSpace(document.Tuning))
			{
				throw new InvalidOperationException(".gtab tuning is required.");
			}

			if (string.IsNullOrWhiteSpace(document.TabString))
			{
				throw new InvalidOperationException(".gtab tab is required.");
			}
		}

		private static Tablature ToTablature(GtabDocument document)
		{
			return new Tablature
			{
				Name = document.Name,
				Speed = document.Speed,
				Tuning = document.Tuning,
				TabString = document.TabString,
				Repeat = document.Repeat,
				NumberOfStrings = document.NumberOfStrings,
				TabVersions = Array.Empty<TabVersion>()
			};
		}
	}
}

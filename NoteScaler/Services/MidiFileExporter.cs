namespace NoteScaler.Services
{
	using NoteScaler.Models;
	using NoteScaler.Services.Interfaces;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;

	public sealed class MidiFileExporter : IMidiFileExporter
	{
		private const short TicksPerQuarterNote = 1000;
		private const int MicrosecondsPerQuarterNote = 1000000;
		private const int MidiChannel = 0;
		private const int DefaultVelocity = 100;

		public void Export(IEnumerable<GuitarPerformanceEvent> performanceEvents, string outputPath)
		{
			if (string.IsNullOrWhiteSpace(outputPath))
			{
				throw new ArgumentException("MIDI output path is required.", nameof(outputPath));
			}

			var events = performanceEvents?.ToArray() ?? throw new ArgumentNullException(nameof(performanceEvents));
			var directory = Path.GetDirectoryName(outputPath);
			if (!string.IsNullOrEmpty(directory))
			{
				Directory.CreateDirectory(directory);
			}

			using var outputStream = File.Create(outputPath);
			WriteHeader(outputStream);
			WriteTrack(outputStream, events);
		}

		private static void WriteHeader(Stream outputStream)
		{
			WriteAscii(outputStream, "MThd");
			WriteInt32(outputStream, 6);
			WriteInt16(outputStream, 0);
			WriteInt16(outputStream, 1);
			WriteInt16(outputStream, TicksPerQuarterNote);
		}

		private static void WriteTrack(Stream outputStream, IReadOnlyCollection<GuitarPerformanceEvent> performanceEvents)
		{
			using var trackStream = new MemoryStream();
			WriteTempoEvent(trackStream);

			var midiEvents = performanceEvents
				.SelectMany(CreateMidiEvents)
				.OrderBy(midiEvent => midiEvent.Tick)
				.ThenBy(midiEvent => midiEvent.IsNoteOn ? 1 : 0)
				.ToArray();

			var previousTick = 0;
			foreach (var midiEvent in midiEvents)
			{
				WriteVariableLengthQuantity(trackStream, midiEvent.Tick - previousTick);
				trackStream.WriteByte((byte)(midiEvent.IsNoteOn ? 0x90 + MidiChannel : 0x80 + MidiChannel));
				trackStream.WriteByte((byte)midiEvent.NoteNumber);
				trackStream.WriteByte((byte)midiEvent.Velocity);
				previousTick = midiEvent.Tick;
			}

			WriteEndOfTrackEvent(trackStream);

			WriteAscii(outputStream, "MTrk");
			WriteInt32(outputStream, (int)trackStream.Length);
			trackStream.Position = 0;
			trackStream.CopyTo(outputStream);
		}

		private static IEnumerable<MidiNoteEvent> CreateMidiEvents(GuitarPerformanceEvent performanceEvent)
		{
			var noteNumber = GetMidiNoteNumber(performanceEvent.Note);
			var velocity = performanceEvent.Velocity > 0 ? performanceEvent.Velocity : DefaultVelocity;
			var startTick = performanceEvent.StartOffsetMilliseconds;
			var endTick = performanceEvent.StartOffsetMilliseconds + performanceEvent.DurationMilliseconds;

			yield return new MidiNoteEvent(startTick, noteNumber, velocity, true);
			yield return new MidiNoteEvent(endTick, noteNumber, 0, false);
		}

		private static int GetMidiNoteNumber(string note)
		{
			var match = Regex.Match(note, "^([A-G])(#?)(-?\\d+)$");
			if (!match.Success)
			{
				throw new ArgumentException($"Unsupported MIDI note: {note}", nameof(note));
			}

			var noteName = match.Groups[1].Value + match.Groups[2].Value;
			var octave = int.Parse(match.Groups[3].Value);
			return (octave + 1) * 12 + GetSemitone(noteName);
		}

		private static int GetSemitone(string noteName)
		{
			switch (noteName)
			{
				case "C": return 0;
				case "C#": return 1;
				case "D": return 2;
				case "D#": return 3;
				case "E": return 4;
				case "F": return 5;
				case "F#": return 6;
				case "G": return 7;
				case "G#": return 8;
				case "A": return 9;
				case "A#": return 10;
				case "B": return 11;
				default:
					throw new ArgumentException($"Unsupported MIDI note name: {noteName}", nameof(noteName));
			}
		}

		private static void WriteTempoEvent(Stream stream)
		{
			WriteVariableLengthQuantity(stream, 0);
			stream.WriteByte(0xFF);
			stream.WriteByte(0x51);
			stream.WriteByte(0x03);
			stream.WriteByte((byte)((MicrosecondsPerQuarterNote >> 16) & 0xFF));
			stream.WriteByte((byte)((MicrosecondsPerQuarterNote >> 8) & 0xFF));
			stream.WriteByte((byte)(MicrosecondsPerQuarterNote & 0xFF));
		}

		private static void WriteEndOfTrackEvent(Stream stream)
		{
			WriteVariableLengthQuantity(stream, 0);
			stream.WriteByte(0xFF);
			stream.WriteByte(0x2F);
			stream.WriteByte(0x00);
		}

		private static void WriteVariableLengthQuantity(Stream stream, int value)
		{
			var buffer = value & 0x7F;
			while ((value >>= 7) > 0)
			{
				buffer <<= 8;
				buffer |= ((value & 0x7F) | 0x80);
			}

			while (true)
			{
				stream.WriteByte((byte)(buffer & 0xFF));
				if ((buffer & 0x80) != 0)
				{
					buffer >>= 8;
				}
				else
				{
					break;
				}
			}
		}

		private static void WriteAscii(Stream stream, string value)
		{
			foreach (var character in value)
			{
				stream.WriteByte((byte)character);
			}
		}

		private static void WriteInt16(Stream stream, short value)
		{
			stream.WriteByte((byte)((value >> 8) & 0xFF));
			stream.WriteByte((byte)(value & 0xFF));
		}

		private static void WriteInt32(Stream stream, int value)
		{
			stream.WriteByte((byte)((value >> 24) & 0xFF));
			stream.WriteByte((byte)((value >> 16) & 0xFF));
			stream.WriteByte((byte)((value >> 8) & 0xFF));
			stream.WriteByte((byte)(value & 0xFF));
		}

		private sealed class MidiNoteEvent
		{
			public MidiNoteEvent(int tick, int noteNumber, int velocity, bool isNoteOn)
			{
				Tick = tick;
				NoteNumber = noteNumber;
				Velocity = velocity;
				IsNoteOn = isNoteOn;
			}

			public int Tick { get; }
			public int NoteNumber { get; }
			public int Velocity { get; }
			public bool IsNoteOn { get; }
		}
	}
}

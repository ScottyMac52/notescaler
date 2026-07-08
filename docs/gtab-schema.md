# Guitar Tab Maker .gtab Schema

`.gtab` files loaded by NoteScaler are JSON documents produced by the Guitar Tab Maker app.

The loader adapts the Guitar Tab Maker shape into NoteScaler's existing `Tablature` model so string-instrument resolution, tab playback, guitar performance events, and MIDI export stay DRY.

## Root shape

```json
{
  "cFret": 0,
  "title": "Unforgiven",
  "tempo": 80,
  "stringNotes": ["E", "A", "D", "G", "B", "E"],
  "version": 5,
  "lyricSize": 100,
  "tabRows": []
}
```

## Supported root fields

| Field | Type | Description |
|---|---|---|
| `cFret` | number | Capo fret. The loader adds this value to parsed fret numbers when converting to NoteScaler tab notation. |
| `title` | string | Song or tab title. Maps to `Tablature.Name`. |
| `tempo` | number | Beats per minute from Guitar Tab Maker. The loader stores an approximate measure time as `60000 / tempo`, but current playback still uses the command-line `--speed` measure time. |
| `stringNotes` | string array | Open string notes in low-to-high order, for example `E,A,D,G,B,E`. |
| `version` | number | Guitar Tab Maker document version. The first observed sample uses version `5`. |
| `lyricSize` | number | Preserved app metadata. Not used by NoteScaler playback in this slice. |
| `tabRows` | array | Visual tab rows containing columns and optional lyric data. |

## Row shape

Each `tabRows` entry can contain:

```json
{
  "lyricLines": [],
  "columnHeaders": [
    { "name": -1, "strum": -1 }
  ],
  "columns": [],
  "lyrics": ""
}
```

The first loader slice uses `columns` for playback conversion. `lyricLines`, `columnHeaders`, and `lyrics` are recognized as part of the file shape but are not used for playback yet.

## Column and cell shape

Each row has multiple columns. Each column contains one cell per string.

```json
[
  { "p": "—", "s": "" },
  { "p": "0", "s": "" },
  { "p": "—", "s": "" },
  { "p": "—", "s": "" },
  { "p": "—", "s": "" },
  { "p": "—", "s": "" }
]
```

Cell fields:

| Field | Type | Description |
|---|---|---|
| `p` | string | Position value. Numeric values are frets. `—` means no fretted note in that string cell. Non-numeric technique markers are ignored in this first slice. |
| `s` | string | Style or decoration metadata. Not used by NoteScaler playback in this slice. |

## String ordering

Guitar Tab Maker stores `stringNotes` in low-to-high order. For standard tuning:

```json
["E", "A", "D", "G", "B", "E"]
```

NoteScaler's string instrument catalog uses normal string numbers where string `1` is the highest string. Therefore, the loader converts a cell index into a NoteScaler string number by reversing the index:

```text
index 0 -> string 6
index 1 -> string 5
index 2 -> string 4
index 3 -> string 3
index 4 -> string 2
index 5 -> string 1
```

## Supported tunings in this first slice

The loader maps known Guitar Tab Maker `stringNotes` arrays to existing NoteScaler catalog names:

| Guitar Tab Maker `stringNotes` | NoteScaler tuning |
|---|---|
| `E,A,D,G,B,E` | `Standard` |
| `D,A,D,G,B,E` | `Drop D` |
| `D,G,C,F,A,D` | `D Standard` |
| `D#,G#,C#,F#,A#,D#` | `Eb Standard` |
| `Eb,Ab,Db,Gb,Bb,Eb` | `Eb Standard` |
| `C#,F#,B,E,G#,C#` | `C# Standard` |
| `Db,Gb,B,E,Ab,Db` | `C# Standard` |

Unsupported `stringNotes` arrays return a validation error for now.

## Conversion behavior

The loader converts each Guitar Tab Maker column into existing NoteScaler tab notation.

A single fretted cell becomes one NoteScaler tab item:

```text
4-2
```

Multiple fretted cells in the same column become a chord group:

```text
5-0|4-2
```

Empty columns between fretted columns are folded into the previous note group's duration multiplier:

```text
4-2-2,5-0
```

That means the first note group lasts two command-line measure units before the next fretted group starts.

## Loader behavior

`--gtab maryhadalittlelamb` loads:

```text
GTabs/maryhadalittlelamb.gtab
```

`--gtab maryhadalittlelamb.gtab` also loads from `GTabs` when no explicit path is supplied.

An explicit path is honored:

```text
--gtab C:\Music\tabs\maryhadalittlelamb.gtab
```

After loading, the `.gtab` document is normalized into the existing `Tablature` playback path so tab playback and MIDI export stay DRY.

## Current limitations

- Lyrics are parsed as part of the document shape but not used for playback.
- Style metadata is ignored.
- Non-numeric markers such as hammer-on markers are ignored in this first loader slice.
- `tempo` is captured, but current playback timing still comes from the command-line `--speed` option.
- Unsupported tunings need a follow-up mapping or a generated supplemental instrument definition.

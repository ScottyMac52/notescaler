# .gtab Schema

`.gtab` files are JSON documents used to describe guitar tablature in a small, versioned format.

The first supported schema version is `1`.

## Version 1

```json
{
  "schemaVersion": 1,
  "name": "Mary Had A Little Lamb",
  "speed": 1500,
  "tuning": "Standard",
  "tab": "1-0,2-1,3-0",
  "repeat": 1,
  "strings": 6
}
```

## Required fields

| Field | Type | Description |
|---|---|---|
| `schemaVersion` | number | Must be `1` for the current loader. |
| `name` | string | Display name for the tab. |
| `tuning` | string | String instrument name from the string instrument catalog, such as `Standard`, `Drop D`, or `C# Standard`. |
| `tab` | string | Existing NoteScaler tab notation. |

## Optional fields

| Field | Type | Default | Description |
|---|---|---:|---|
| `speed` | number | `0` | Measure duration in milliseconds. The command-line `--speed` value still controls the playable sequence measure time in the first loader slice. |
| `repeat` | number | `1` | Number of playback repeats. |
| `strings` | number | `6` | Number of strings for informational compatibility with existing tab documents. |

## Tab notation

The `tab` field uses existing NoteScaler tab notation.

Single fretted note:

```text
1-0
```

Comma-separated groups advance time sequentially:

```text
1-0,2-1,3-0
```

Pipe-separated values form a chord group at the same start offset:

```text
1-0|2-1|3-0
```

Duration modifiers use the existing measure-time multiplier:

```text
1-0-0.5
```

With a measure time of `1000`, that note lasts `500` milliseconds.

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

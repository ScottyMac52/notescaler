# Guitar Performance Events

`GuitarPerformanceEvent` is the neutral model used to preserve guitar-specific information from tablature before any renderer consumes it.

The existing playback path can still convert tabs directly into note names and durations. This model exists so future renderers, such as MIDI export, SoundFont rendering, physical modeling, or hardware output, do not need to re-parse tab text or guess guitar metadata after it has been discarded.

## Preserved Data

Each event includes:

- note name
- string number
- fret number
- start offset in milliseconds
- duration in milliseconds
- velocity
- articulation

## Timing Rules

Comma-separated tab groups advance time sequentially.

```text
1-0,2-1,3-0
```

The events start at `0`, `measureTime`, and `measureTime * 2`.

Pipe-separated tab notes are treated as a chord group.

```text
1-0|2-1|3-0
```

All events in the group share the same start offset by default.

Duration modifiers still use the existing tab duration syntax.

```text
1-0-0.5
```

With a `measureTime` of `1000`, this event lasts `500` milliseconds.

## Renderer Use

Future renderers should consume `GuitarPerformanceEvent` rather than re-parsing tab strings.

The intended serial path is:

```text
Tablature -> GuitarPerformanceEvent -> MIDI export -> SoundFont rendering -> optional hardware output
```

# Spike: Guitar Audio Generation

Resolves #52.

## Decision

NoteScaler should not try to make the existing raw tone generator sound like a convincing guitar by only changing waveform types.

The recommended direction is:

1. Add a guitar-performance model that preserves guitar-specific information from tablature.
2. Add MIDI export as the first implementation slice.
3. Add optional SoundFont-based rendering as the first higher-quality in-app audio path.
4. Keep a physical-modeling path, such as Karplus-Strong plucked-string synthesis, as a later optional synthesizer.
5. Treat a USB hardware pickup-signal generator as an experimental side project, not the core NoteScaler playback path.

## Current NoteScaler Playback Architecture

The current audio path is intentionally simple:

1. Tablature resolves string/fret notation into note names.
2. Note names are converted into frequencies and durations.
3. `CompositeNote` passes `FrequencyDuration` values to an `IPlayer`.
4. `SignalNotePlayer` generates mono audio using NAudio `SignalGenerator`.
5. Instrument choice maps to basic waveform type:
   - `Horn` -> sawtooth
   - `Flute` -> triangle
   - `Clarinet` -> square
   - `Recorder` -> sine

This is good enough for pitch confirmation, but it discards guitar-specific performance details before playback.

## Main Limitation

The problem is not only waveform quality. The current playback model loses information that a guitar renderer needs:

- original string number
- fret number
- picking/strumming order
- velocity
- articulation
- muting
- sustain/decay behavior
- whether two identical pitches came from different strings
- chord roll/arpeggiation timing
- instrument-specific body/amp coloration

A realistic guitar renderer needs more than `frequency + duration`.

## Options Considered

### Option 1: Improve Current NAudio Tone Generation

Use the existing `SignalNotePlayer` but improve the generated tones with envelopes, filtering, and better mixing.

Pros:

- Smallest code change.
- Keeps dependencies almost unchanged.
- Easy to unit test.
- Could reduce clicks and make notes less harsh.

Cons:

- Still sounds synthetic.
- Basic oscillator waveforms do not behave like plucked strings.
- Does not solve guitar-specific string/fret behavior.
- Chords will still sound like stacked oscillators.

Verdict: useful as cleanup, not the primary realism path.

### Option 2: MIDI Export

Generate a `.mid` file from NoteScaler playback events.

Pros:

- Low implementation risk.
- Decouples NoteScaler from audio rendering quality.
- Allows playback in DAWs, notation tools, or dedicated MIDI renderers.
- Gives users freedom to choose guitar instruments, amp sims, and plugins.
- Excellent first step because it forces a better internal performance model.

Cons:

- MIDI alone does not sound like anything until rendered.
- General MIDI guitar patches can still sound cheesy.
- Guitar realism depends heavily on the target sound module or plugin.

Verdict: best first implementation slice.

### Option 3: SoundFont-Based Rendering

Render NoteScaler events using a SoundFont guitar instrument.

Pros:

- Better sound than raw oscillator synthesis.
- Still deterministic and testable.
- Can support in-app playback or WAV export.
- SoundFont libraries are interchangeable.
- Does not require hardware.

Cons:

- Quality depends entirely on the chosen SoundFont.
- Licensing of bundled SoundFonts must be handled carefully.
- Realistic guitar articulations may require better sample libraries than General MIDI.
- Adds dependency and asset-management questions.

Verdict: best first in-app audio improvement after MIDI export.

### Option 4: Karplus-Strong / Physical Modeling

Implement a plucked-string synthesizer.

Pros:

- Good conceptual match for guitar strings.
- Can be implemented directly in C#.
- Produces more string-like attack and decay than raw oscillator tones.
- No sample licensing issues.
- Parameters can be tied to string/fret/articulation metadata.

Cons:

- More DSP work.
- Requires tuning, filtering, and envelope experimentation.
- Still does not automatically sound like a miked guitar amp.
- Chords require careful mixing and timing to avoid harsh output.

Verdict: promising second or third implementation path, especially if we want a self-contained renderer.

### Option 5: Sample-Based Per-String/Fret Playback

Record or obtain guitar samples for strings/frets and play them back.

Pros:

- Potentially the most realistic direct sound.
- Preserves per-string character if samples are mapped correctly.
- Good fit for tab playback.

Cons:

- Large asset set.
- Licensing and storage problems.
- Recording a complete sample set is time-consuming.
- Needs velocity layers and round-robin samples to avoid machine-gun repetition.

Verdict: high quality, high cost. Not recommended as the first path.

### Option 6: USB Hardware Pickup-Signal Generator

Create a USB-controlled box that receives note/chord events and outputs an instrument-level analog signal into a guitar amp.

Pros:

- Very interesting project for someone with electronics experience.
- Lets a real amp, pedals, or modeler provide coloration.
- Could be built around a small MCU, USB interface, DAC/audio codec, output filter, and instrument-level analog output stage.
- Potentially fun and unique.

Cons:

- A guitar pickup is not just an audio source. It is a passive, high-impedance, inductive source interacting with the amp input.
- A DAC output is normally low-impedance and line-like, not pickup-like.
- Requires careful output level, impedance, anti-alias filtering, isolation, grounding, and noise control.
- Digital latency becomes more visible when playing into a real amp.
- It becomes a hardware product/project, not just a NoteScaler feature.
- Debugging audio noise, ground loops, clipping, and impedance mismatches can consume a lot of time.

Verdict: technically feasible as an experimental companion project, but not recommended as the core NoteScaler playback solution.

## Hardware Path Notes

A safe hardware exploration path would be:

1. First prove the sound using software audio output into a reamp box or instrument-level interface.
2. Then build a USB audio/DAC prototype.
3. Add an analog output stage that approximates instrument level.
4. Add filtering and isolation.
5. Test into a real guitar amp at low volume.
6. Only after that consider a custom PCB.

The hardware box should probably consume the same future `GuitarPerformanceEvent` model as MIDI/SoundFont rendering. That keeps NoteScaler architecture clean and prevents hardware details from leaking into tab parsing.

## Recommended Architecture Direction

Create a neutral playback/performance model before changing audio output.

Suggested model concept:

```csharp
public sealed class GuitarPerformanceEvent
{
    public string Note { get; init; }
    public int StringNumber { get; init; }
    public int Fret { get; init; }
    public int StartOffsetMilliseconds { get; init; }
    public int DurationMilliseconds { get; init; }
    public int Velocity { get; init; }
    public GuitarArticulation Articulation { get; init; }
}
```

Possible articulations:

```csharp
public enum GuitarArticulation
{
    Pick,
    StrumDown,
    StrumUp,
    LetRing,
    PalmMute,
    Mute,
    HammerOn,
    PullOff,
    Slide
}
```

This does not need to be fully implemented at once. The first slice can support only `Pick`, `StrumDown`, and default velocity.

## Proposed First TDD Implementation Slice

Create a new issue for:

> Add MIDI export from tab playback events

Acceptance criteria:

- Given a tab with a known tuning, when converted for export, then the resulting events preserve note, string, fret, duration, and ordering.
- Given a chord group, when exported to MIDI, then notes start together by default.
- Given a strummed chord group, when exported to MIDI, then notes are staggered by a configurable strum delay.
- Given a tab file, when `--export-midi` is provided, then NoteScaler writes a `.mid` file without changing current audio playback.
- Existing playback remains unchanged.

## Proposed Follow-Up Slice

Create a second issue for:

> Add optional SoundFont rendering for guitar playback

Acceptance criteria:

- Given a MIDI/performance sequence, when rendered with a configured SoundFont, then NoteScaler can produce audio without using raw oscillator waveforms.
- SoundFont path is configurable.
- No SoundFont is bundled unless its license is explicitly compatible.
- Existing `SignalNotePlayer` remains available as fallback.

## Final Recommendation

Do this in order:

1. Preserve guitar tab metadata in a new performance-event model.
2. Add MIDI export.
3. Add optional SoundFont rendering.
4. Add better generated synthesis only after the event model is clean.
5. Explore the USB pickup-signal hardware separately, using the same performance-event stream as input.

This gives NoteScaler an immediate practical upgrade path while keeping the door open for both better software sound and a real hardware experiment.

## References

- Karplus-Strong string synthesis: plucked-string physical modeling technique.
- Digital waveguide synthesis: related family of efficient string/resonance models.
- SoundFont: sample-based instrument format used to render MIDI with instrument samples.
- FluidSynth: SoundFont-based MIDI synthesizer that can render MIDI to audio.
- NAudio: current NoteScaler dependency used for generated audio playback.

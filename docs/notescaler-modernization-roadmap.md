# NoteScaler Modernization Roadmap

This roadmap captures the preferred implementation order for modernizing NoteScaler and adding GTAB / video-import support.

## Confirmed current baseline

- Repository: `ScottyMac52/notescaler`
- Default branch: `master`
- Current solution: `NoteScaler.sln`
- Current projects:
  - `NoteScaler/NoteScaler.csproj`
  - `NoteScalerTests/NoteScalerTests.csproj`
- Current target framework: `netcoreapp3.1`
- Current tab support is JSON-based and is invoked by the `--tab` option.
- Current tab playback is wired directly through `Program.PlayTabAsRequired(...)`, `PlayableSequence.LoadFromFile(...)`, and `PlayableSequence.ConvertTabsToNoteSequence(...)`.

## Desired sequence

1. Modernize the solution structure.
2. Increase test coverage.
3. Introduce `IPlayableFileLoader` and implement `GTabPlayableFileLoader` plus any transitional loaders.
4. Add `.gtab` support.
5. Add `Import from Video URL` support.

## Phase 1: Modernize

Goal: make the current project easier to test and extend before adding new import formats.

Recommended changes:

- Move the solution toward a layered structure:
  - `NoteScaler.Core`
  - `NoteScaler.Loaders`
  - `NoteScaler.GTab`
  - `NoteScaler.VideoImport`
  - `NoteScaler.Console`
  - `NoteScaler.Tests`
- Keep the existing CLI behavior working while extracting orchestration away from `Program`.
- Upgrade the target framework from `netcoreapp3.1` to a supported modern .NET target.
- Centralize package versions if the solution grows beyond a couple of projects.
- Add CI that runs restore, build, and tests on every PR.

Suggested first PR:

- Retarget the projects.
- Add/confirm CI.
- Extract a testable application service for current command execution.
- Do not change the public CLI behavior yet.

## Phase 2: Increase test coverage

Goal: lock down current behavior before changing file-loading architecture.

Coverage targets:

- `MusicNote` creation and scale behavior.
- Current song JSON loading.
- Current tab JSON loading.
- `Tablature.FixUp()` default-version behavior.
- `PlayableSequence.ConvertTabsToNoteSequence(...)` behavior.
- CLI option mapping.
- Error handling for missing files and malformed input.

Suggested PR:

- Add characterization tests around existing behavior.
- Add a coverage report to CI.
- Avoid refactoring production code unless required to make behavior testable.

## Phase 3: Introduce playable file loaders

Goal: decouple playable file import from `Program` and `PlayableSequence`.

Proposed interface:

```csharp
public interface IPlayableFileLoader
{
    bool CanLoad(PlayableFileRequest request);
    PlayableLoadResult Load(PlayableFileRequest request);
}
```

Proposed request model:

```csharp
public sealed class PlayableFileRequest
{
    public string FileNameOrPath { get; init; }
    public string Format { get; init; }
    public int MeasureTime { get; init; }
    public int Octave { get; init; }
    public int A4Reference { get; init; }
}
```

Proposed result model:

```csharp
public sealed class PlayableLoadResult
{
    public bool Success { get; init; }
    public string Error { get; init; }
    public IEnumerable<NoteGroup> Notes { get; init; }
    public int? Repeat { get; init; }
}
```

Likely loaders:

- `SongJsonPlayableFileLoader` for the existing song JSON format.
- `TabJsonPlayableFileLoader` for the existing tab JSON format.
- `GTabPlayableFileLoader` for `.gtab` files.

Suggested PR:

- Add the interface and request/result models.
- Move the existing tab JSON behavior into `TabJsonPlayableFileLoader`.
- Wire the CLI through the loader abstraction.
- Preserve current behavior.

## Phase 4: Add `.gtab` support

Goal: support a real `.gtab` file format without disrupting existing tab JSON files.

Recommended scope:

- Define the GTAB schema expected by NoteScaler.
- Parse GTAB JSON into a strongly typed model.
- Convert GTAB string/fret events into `NoteGroup` values.
- Support standard tuning first.
- Add alternate tuning once the base path is stable.
- Add fixture files under tests.

Suggested PR:

- Add `NoteScaler.GTab`.
- Add `GTabPlayableFileLoader`.
- Add tests for single notes, chords, rests, timing, and malformed GTAB input.
- Add a CLI option path such as `--gtab <file>` or support format detection by extension.

## Phase 5: Import from Video URL

Goal: create `.gtab` from a YouTube/video URL when the visible video contains guitar tablature.

Recommended architecture:

```text
Video URL
  -> video/frame provider
  -> frame sampler
  -> tab-region detector
  -> OCR / fret-number recognizer
  -> string/timing reconstructor
  -> GTAB draft
  -> user review/edit
  -> save .gtab
```

Recommended constraints:

- Keep downloading/extracting frames behind the app workflow.
- The user-facing command should feel like `import from video URL`.
- The first implementation should generate a draft GTAB, not silently claim perfection.
- Include confidence scores and review markers for uncertain fret recognition.

Suggested PR sequence:

1. Add interfaces only: `IVideoFrameProvider`, `ITabFrameAnalyzer`, `IGTabDraftWriter`.
2. Add local-video or image-sequence import for deterministic tests.
3. Add URL-based video frame extraction.
4. Add OCR/fret recognition.
5. Add review/edit workflow.

## Branch and PR strategy

Recommended branch order:

1. `modernize/solution-structure`
2. `test/coverage-baseline`
3. `feature/playable-file-loaders`
4. `feature/gtab-loader`
5. `feature/import-from-video-url`

Each PR should be small enough to review independently and should keep the command-line app runnable.

# NoteScaler Modernization Roadmap

This roadmap captures the preferred implementation order for moving NoteScaler to .NET 10 and adding GTAB / video-import support.

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

1. Move the project to .NET 10.
2. Modernize the solution structure.
3. Increase test coverage.
4. Introduce `IPlayableFileLoader` and implement `GTabPlayableFileLoader` plus any transitional loaders.
5. Add `.gtab` support.
6. Add `Import from Video URL` support.

## Phase 1: Move to .NET 10

Goal: retarget the existing app and test projects from `netcoreapp3.1` to `net10.0` before larger architectural changes.

Recommended changes:

- Update `NoteScaler/NoteScaler.csproj` from `netcoreapp3.1` to `net10.0`.
- Update `NoteScalerTests/NoteScalerTests.csproj` from `netcoreapp3.1` to `net10.0`.
- Update NuGet package versions that are too old for a clean .NET 10 build/test path.
- Keep the current solution/project layout intact for this first PR.
- Keep existing CLI behavior intact.
- Add or update CI so restore, build, and tests run with .NET 10.

Suggested first PR:

- Branch: `modernize/dotnet-10`
- Retarget the app and test projects to `net10.0`.
- Update package references only as needed to build and test cleanly.
- Add or update GitHub Actions for .NET 10.
- Do not extract architecture or change playback behavior in this PR.

## Phase 2: Modernize solution structure

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
- Centralize package versions if the solution grows beyond a couple of projects.

Suggested PR:

- Branch: `modernize/solution-structure`
- Extract a testable application service for current command execution.
- Preserve current CLI behavior.
- Avoid introducing `.gtab` behavior in this PR.

## Phase 3: Increase test coverage

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

- Branch: `test/coverage-baseline`
- Add characterization tests around existing behavior.
- Add a coverage report to CI.
- Avoid refactoring production code unless required to make behavior testable.

## Phase 4: Introduce playable file loaders

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

- Branch: `feature/playable-file-loaders`
- Add the interface and request/result models.
- Move the existing tab JSON behavior into `TabJsonPlayableFileLoader`.
- Wire the CLI through the loader abstraction.
- Preserve current behavior.

## Phase 5: Add `.gtab` support

Goal: support a real `.gtab` file format without disrupting existing tab JSON files.

Recommended scope:

- Define the GTAB schema expected by NoteScaler.
- Parse GTAB JSON into a strongly typed model.
- Convert GTAB string/fret events into `NoteGroup` values.
- Support standard tuning first.
- Add alternate tuning once the base path is stable.
- Add fixture files under tests.

Suggested PR:

- Branch: `feature/gtab-loader`
- Add `NoteScaler.GTab`.
- Add `GTabPlayableFileLoader`.
- Add tests for single notes, chords, rests, timing, and malformed GTAB input.
- Add a CLI option path such as `--gtab <file>` or support format detection by extension.

## Phase 6: Import from Video URL

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

1. `modernize/dotnet-10`
2. `modernize/solution-structure`
3. `test/coverage-baseline`
4. `feature/playable-file-loaders`
5. `feature/gtab-loader`
6. `feature/import-from-video-url`

Each PR should be small enough to review independently and should keep the command-line app runnable.

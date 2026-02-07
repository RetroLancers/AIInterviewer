# Task 13: Implement TTS Model Pre-loading AppTask

## Objective
Convert the lazy-loading logic for the Kokoro TTS model in `TtsService.cs` into a dedicated ServiceStack `AppTask`. This allows pre-loading/downloading the ~300MB model as part of the setup process rather than during the first user request, improving the initial user experience.

## Requirements
- Register a new `AppTask` in `Configure.Db.Migrations.cs` named `tts.load`.
- The task should execute `KokoroTTS.LoadModel()` to ensure the model and voices are available on disk.
- Update `README.md` to include instructions on how to run this task and why it's recommended.
- Update `TtsService.cs` to leverage this pre-loading where possible, while maintaining a safe fallback.
- Add a convenient `npm run` script to `package.json` to make running this task easy.

## Checklist
- [ ] Create `AppTask` for `tts.load` in `Configure.Db.Migrations.cs`.
- [ ] Add `npm run tts:load` script to `AIInterviewer/package.json`.
- [ ] Verify the task works correctly (runs without error and loads/downloads model).
- [ ] Update `README.md` with "TTS Model Setup" section.
- [ ] Refactor `TtsService.cs` to use the same logic or refer to the task's necessity.
- [ ] Update `clood-groups/service-interface.json` if necessary.

## Notes
- The model file is large (~300MB).
- See `TtsService.cs:L26-L35` for the current loading logic.
- See `Configure.Db.Migrations.cs:L19-L20` for examples of `AppTask` registration.

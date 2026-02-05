# Optional Task: Enable GPU Acceleration for Kokoro TTS

## Objective
Add support for NVIDIA GPU acceleration (CUDA) to speed up text-to-speech synthesis.

## Prerequisites
- NVIDIA GPU with CUDA support.
- CUDA Toolkit (v12.x) installed and in SYSTEM PATH.
- cuDNN installed and in SYSTEM PATH.

## Requirements
- Update `SiteConfig.cs`:
    - Add `string? KokoroEngine` (Optional, defaults to "CPU").
- Update `TtsService.cs`:
    - Add logic to check `KokoroEngine`.
    - If "CUDA", initialize `SessionOptions` with `AppendExecutionProvider_CUDA()`.
    - Catch failures (e.g. CUDA not found) and fallback to CPU.
- Update `SiteConfigEditor.vue`:
    - Add dropdown for Kokoro Engine (CPU, CUDA).

## Checklist
- [ ] Add `KokoroEngine` field to `SiteConfig`
- [ ] Implement CUDA initialization logic in `TtsService`
- [ ] Add error handling and fallback mechanism
- [ ] UI dropdown in `SiteConfigEditor.vue`
- [ ] Verify GPU usage during synthesis (e.g. via Task Manager)

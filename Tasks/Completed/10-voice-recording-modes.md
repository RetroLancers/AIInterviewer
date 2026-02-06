# Task: Voice Recording Modes and Transcript Review

## Objective
Reduce speech cutoff issues by adding manual start/stop recording mode and an optional transcript review flow before sending.

## Status
Not started.

## Requirements
- Add a recording mode toggle that enables manual start/stop recording.
    - Single mic button should toggle between start and stop when in manual mode.
    - Mic UI should visually indicate recording state (color/active state).
    - Manual mode should ignore any automatic "speech finished" detection until user stops.
- Add a toggle to send the captured transcript into the textbox for review/editing before submitting.
    - When enabled, speech results append to the input field instead of auto-sending.
    - Subsequent recordings in either mode should add to the textbox (not replace it).
    - Support editing then submitting with existing send behavior.
- Persist user preferences (per session or saved settings) so toggles remain consistent while chatting.

## Checklist
- [x] Add UI toggles for manual recording mode and transcript-to-textbox mode.
- [x] Update speech recording logic to respect manual start/stop behavior.
- [x] Update mic button styling to reflect recording state.
- [x] Route speech transcript to textbox when review toggle is enabled.
- [x] Confirm UX: manual mode overrides auto-stop; review mode prevents auto-send.
- [x] Update relevant clood file(s) after implementation.

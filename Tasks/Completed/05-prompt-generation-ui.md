# Task: Prompt Generation UI

## Objective
Build the frontend page where users can describe what kind of interview they need and get a generated prompt.

## Status
Work exists in `AIInterviewer.Client/src/pages/interviews/new.vue` but it needs fixes before completion.

## Issues Found
- Uses `try/catch` around `client.api(...)` even though `client.api` never throws. Handle `ApiResult` instead.
- `GenerateInterviewPrompt` sets both `targetRole` and `context` to the same value. Split fields or map correctly.
- No user-facing error state for failed prompt generation or interview creation.

## Requirements
- New page `AIInterviewer.Client/src/pages/interviews/new.vue`.
- User-friendly input for interview context (e.g., "Fullstack Developer at Google").
- Call `GenerateInterviewPrompt` API.
- Show generated prompt in an editable text area.
- "Start Interview" button that creates the `Interview` record and redirects to the chat.

## Checklist
- [x] Create `interviews/new.vue`
- [x] Implement UI design (modern, clean, AI-themed)
- [x] Fix `GenerateInterviewPrompt` request mapping (`targetRole` vs `context`)
- [x] Remove `try/catch` around `client.api` and handle `ApiResult` correctly
- [x] Add visible error state for API failures
- [x] Verify navigation flow
- [x] Update clood files for Frontend and Interview domains

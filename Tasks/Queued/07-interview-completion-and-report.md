# Task: Interview Completion and Report

## Objective
Finalize the interview process by generating a comprehensive feedback report using Gemini.

## Status
Backend and UI exist but report rendering and API handling need fixes.

## Issues Found
- `interviews/[id]/result.vue` renders markdown as plain text; it should render markdown (e.g., via `markdown-it`).
- `result.vue` uses `try/catch` around `client.api` (violates API usage rules).
- `FinishInterview` uses a nested `EvaluationResponse` class; move it to its own file to follow the one-class-per-file rule.
- Guard against null `Feedback` to avoid null `ReportText`.

## Requirements
- "End Interview" button in the chat UI.
- Backend service to:
    - Aggregate all chat messages for a specific `Interview`.
    - Send history to Gemini with a request for evaluation and scoring.
    - Save the result to `InterviewResult` table.
- Result page: `AIInterviewer.Client/src/pages/interviews/[id]/result.vue`.
- Display report markdown and score.

## Checklist
- [x] Implement `FinishInterview` service logic
- [x] Create `interviews/[id]/result.vue` page
- [x] Add "End Interview" confirmation dialog
- [ ] Render markdown for `reportText` (ensure runtime dependency)
- [ ] Remove `try/catch` around `client.api` and handle `ApiResult`
- [ ] Move `EvaluationResponse` to its own file (or justify exception)
- [ ] Verify report generation quality
- [x] Update clood files for all domains

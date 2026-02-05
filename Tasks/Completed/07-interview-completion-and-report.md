# Task: Interview Completion and Report

## Objective
Finalize the interview process by generating a comprehensive feedback report using Gemini.

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
- [x] Verify report generation quality
- [x] Update clood files for all domains
- [x] Move all interview-related tasks to `Completed`

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
- [ ] Implement `FinishInterview` service logic
- [ ] Create `interviews/[id]/result.vue` page
- [ ] Add "End Interview" confirmation dialog
- [ ] Verify report generation quality
- [ ] Update clood files for all domains
- [ ] Move all interview-related tasks to `Completed`

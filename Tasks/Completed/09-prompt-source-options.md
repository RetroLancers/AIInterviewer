# Task: Prompt Source Options for New Interview

## Objective
Allow users to either generate an interview prompt with AI or paste their own system prompt when creating a new interview.

## Background
Task 05's prompt-generation UI forced AI generation. We need an optional flow where a user can provide a custom system prompt instead.

## Requirements
- Update `AIInterviewer.Client/src/pages/interviews/new.vue`.
- Add a prompt source selector (generate vs custom).
- When "Generate" is selected:
  - Collect target role and optional context.
  - Call `GenerateInterviewPrompt`.
  - Display the generated prompt in an editable textarea.
- When "Custom" is selected:
  - Skip prompt generation.
  - Allow users to paste or type a system prompt directly.
- Ensure the "Start Interview" button is enabled only when a system prompt exists.
- Preserve existing error handling and loading states.

## Checklist
- [ ] Add prompt source selector to the UI.
- [ ] Support manual system prompt entry when custom is selected.
- [ ] Keep generated prompt editable when using AI generation.
- [ ] Ensure "Start Interview" uses the final system prompt in all flows.
- [ ] Update relevant clood file(s).

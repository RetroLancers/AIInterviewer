# Task: Update interview rules + final evaluation report

## Objective
Enforce the new base interview rules (one-question-at-a-time, firm tone, 8–12 questions, no feedback during interview, explicit closing instruction) across all interviews, and upgrade the final evaluation report format.

## Research Notes
- System prompt generation lives in `AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs` under `Post(GenerateInterviewPrompt)` and the chat flow uses `interview.Prompt` as the `systemInstruction` for Gemini. This is the best insertion point for mandatory base rules, even when users supply custom prompts.【F:AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs†L12-L118】
- The interview evaluation prompt is composed inside `Post(FinishInterview)` in the same service file; it currently asks for JSON with `Score` and `Feedback` only. This needs the updated “Final Evaluation” structure inserted into the report text.【F:AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs†L120-L183】
- The create interview flow accepts a raw `SystemPrompt` from the UI (`AIInterviewer.Client/src/pages/interviews/new.vue`) and saves it via `CreateInterview` in `InterviewService`; this is where base rules must be enforced regardless of prompt source.【F:AIInterviewer.Client/src/pages/interviews/new.vue†L1-L232】【F:AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs†L20-L41】
- The report is rendered as markdown in `AIInterviewer.Client/src/pages/interviews/[id]/result.vue`, so the upgraded report sections should be delivered as markdown-friendly headings/bullets.【F:AIInterviewer.Client/src/pages/interviews/[id]/result.vue†L1-L91】
- Evaluation response shape is defined in `AIInterviewer.ServiceModel/Types/Interview/EvaluationResponse.cs` and should remain consistent unless we need structured fields beyond `Score` and `Feedback`.【F:AIInterviewer.ServiceModel/Types/Interview/EvaluationResponse.cs†L1-L6】

## Requirements
- Add a mandatory base rules block to the system prompt for every interview, even when users supply a custom prompt.
- Include the opening line: “We’ll proceed with the interview. Answer concisely but thoroughly.”
- Force 8–12 questions, one at a time, increasing difficulty, no feedback until explicit completion.
- Require final closing message: “The interview is complete. Evaluation follows. Please press the end interview button”.
- Upgrade evaluation prompt to include the “Final Evaluation” rubric and ensure it is only produced after the final question.
- Keep the report output compatible with the markdown renderer.

## Plan
1. Implement a base interview rules template (server-side) and ensure it is appended or prepended to any generated or custom system prompt.
2. Update `FinishInterview` evaluation prompt to request the new “Final Evaluation” structure in the report body.
3. Confirm the report output is still delivered via the existing JSON schema (`Score`, `Feedback`).
4. Update any related clood file to include any additional relevant files touched by this work.

## Checklist
- [ ] Add base interview rules template to prompt generation and interview creation flow.
- [ ] Update evaluation prompt to require the new “Final Evaluation” sections.
- [ ] Confirm report renders as markdown in the result page.
- [ ] Update clood files for interview domain.

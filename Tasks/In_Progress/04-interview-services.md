# Task: Implement Interview Services

## Objective
Create the backend services to handle interview prompt generation and chat history.

## Requirements
- ServiceStack Request DTOs for:
    - `GenerateInterviewPrompt`: Input "target role/context", output "system prompt".
    - `GetInterview`: Retrieve interview details and chat history.
    - `AddChatMessage`: Store new message in history.
- Service implementation in `AIInterviewer.ServiceInterface/InterviewService.cs`.
- Use `GeminiClient` for prompt generation.

## Checklist
- [ ] Define Request/Response DTOs in `AIInterviewer.ServiceModel/Types/Interview.cs`
- [ ] Implement `InterviewService.cs`
- [ ] Add logic to `GeminiClient` or a helper to craft the initial "Interviewer" prompt
- [ ] Test services using `api.ts` or Swagger
- [ ] Update clood files for Service Interface domain

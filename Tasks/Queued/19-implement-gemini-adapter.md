# Implement Gemini Adapter

## Objective
Implement the newly created `IAiProvider` using the existing Gemini logic.

## Context
We have extracted the interface (Task 18), now we need to move the current hardcoded Gemini logic into a concrete implementation of this interface.

## Requirements
1.  **Create Service**: Create `GeminiAiProvider` implementing `IAiProvider`.
2.  **Migration**: Move the logic from the current services (e.g., `InterviewService` methods that call Gemini) into this provider.
3.  **Configuration**: The provider should accept configuration (API Key, Model) via constructor or initialization method, NOT by directly reading global static config if possible.

## Implementation Steps
1.  Create `GeminiAiProvider.cs`.
2.  Implement `IAiProvider` methods.
3.  Map `AiMessage` to Gemini's internal message types.
4.  Map Gemini's response to `AiChatResponse`.
5.  Ensure feature parity with existing logic.

## Definition of Done
*   `GeminiAiProvider` implements `IAiProvider`.
*   Passes all integration tests (if any) or functions identically to current logic.

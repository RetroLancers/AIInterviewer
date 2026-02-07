# Extract AI Interface

## Objective
Extract a common interface for AI Service communication to decouple the application from specific providers (currently tightly coupled to Gemini).

## Context
Currently, the application communicates directly with Gemini. We need to support multiple providers (OpenAI, etc.). To do this, we first need an abstraction layer.

## Requirements
1.  **Analyze Existing Usage**: Identify all points where `Gemini` is currently used (Chat, Prompt Generation, etc.).
2.  **Define Interface**: Create `IAiProvider` (or `ILlmProvider`) in `AIInterviewer.ServiceInterface`.
    *   Should cover all existing capabilities.
    *   Methods might include `ChatAsync`, `GenerateTextAsync`, `StreamChatAsync` (if used).
3.  **Define Agnostic Types**: Create input/output models that are not tied to Gemini's SDK.
    *   `AiMessage` instead of Gemini's message type.
    *   `AiChatRequest`, `AiChatResponse`.

## Implementation Steps
1.  Create `IAiProvider.cs` in `AIInterviewer.ServiceInterface/Interfaces` (or similar appropriate path).
2.  Define the agnostic data structures (`AiMessage`, `AiRole`, etc.).
3.  Ensure the interface signature is strictly generic and doesn't leak Gemini details.

## Definition of Done
*   `IAiProvider` exists.
*   Agnostic models exist.
*   No strict dependency on Gemini in the interface definition.

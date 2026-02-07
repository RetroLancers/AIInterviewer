# Implement OpenAI Adapter

## Objective
Implement the `IAiProvider` interface for OpenAI.

## Context
To support OpenAI as an alternative to Gemini.

## Requirements
1.  **Create Service**: Create `OpenAiAiProvider` implementing `IAiProvider`.
2.  **SDK**: Use an OpenAI SDK or raw HTTP client.
3.  **Mapping**: Map `AiMessage` to OpenAI's message format.
4.  **Configuration**: Must accept API Key and Model in constructor.

## Implementation Steps
1.  Add OpenAI NuGet package if needed.
2.  Create `OpenAiAiProvider.cs`.
3.  Implement methods (Chat, Generate).
4.  Handle specific OpenAI quirks (e.g. system messages vs user messages).

## Definition of Done
*   `OpenAiAiProvider` implements `IAiProvider`.
*   Can successfully send requests to OpenAI and receive responses.

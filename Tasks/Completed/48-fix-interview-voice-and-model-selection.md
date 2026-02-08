---
description: Fix issue where interview voice and model selection are not being respected
---

# Fix Interview Voice and Model Selection

The user reported that when creating a new interview, the site uses the default voice instead of the voice specified by the selected AI option. There is also suspicion that the selected model might not be used.

## Investigation Findings

1.  **Voice Issue**:
    *   The `TextToSpeechRequest` DTO has an optional `InterviewId`.
    *   The `TtsService` uses this `InterviewId` to look up the `Interview`, then the `AiConfig`, and finally the `Voice`.
    *   However, in `AIInterviewer.Client/src/composables/useInterview.ts`, the `playAiResponse` function creates a `TextToSpeechRequest` **without** passing the `interviewId`.
    *   `const api = await client.api(new TextToSpeechRequest({ text: sanitizedText }))`
    *   This causes the backend to fall back to the site's default voice.

2.  **Model Issue**:
    *   The `InterviewService` correctly looks up the `AiConfig` using the `AiConfigId` stored on the interview.
    *   When creating an interview, if an `AiConfigId` is selected, it is saved.
    *   If "Use Site Default" is selected (or nothing passed), it "bakes in" the current site default `AiConfigId` at creation time.
    *   The voice handling fix (passing `InterviewId` to TTS) ensures that the voice associated with the interview's `AiConfig` is used.
    *   The model selection logic appears correct in `InterviewService.cs`, using the same `AiConfigId`.

## Plan

1.  **Fix Frontend TTS Call**:
    *   Open `AIInterviewer.Client/src/composables/useInterview.ts`.
    *   Locate the `playAiResponse` function.
    *   Update the `TextToSpeechRequest` instantiation to include `interviewId: id.value`.

2.  **Verify Model Persistence (Optional but recommended)**:
    *   Review `InterviewService.cs` `Post(CreateInterview)` to ensure `AiConfigId` is correctly saved. (Code review confirms this, but keeping it as a check).

## Implementation Steps

1.  Open `AIInterviewer.Client/src/composables/useInterview.ts`.
2.  Find `playAiResponse`.
3.  Change:
    ```typescript
    const api = await client.api(new TextToSpeechRequest({ text: sanitizedText }))
    ```
    To:
    ```typescript
    const api = await client.api(new TextToSpeechRequest({ 
        text: sanitizedText,
        interviewId: id.value 
    }))
    ```
4.  Verify that `id.value` is available and correct in that scope. (It is computed at the top of the composable).

## Verification

*   Create a new interview with a *specific* AI Config that has a distinct voice (different from site default).
*   Start the interview.
*   Verify the initial greeting uses the correct voice.
*   Verify subsequent messages use the correct voice.

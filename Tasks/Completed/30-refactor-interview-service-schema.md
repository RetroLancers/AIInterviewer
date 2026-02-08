
# Refactor InterviewService Schema Generation & Cleanup

## Objective
Refactor `InterviewService.cs` to remove the dependency on `AiSchemaGenerator` and instead use the manual `AiSchemaDefinition` construction pattern. Subsequently, delete `AiSchemaGenerator` and its associated tests as they will no longer be needed.

## Context
Currently, `InterviewService.Post(FinishInterview request)` uses `AiSchemaGenerator.Generate(typeof(EvaluationResponse))` to automatically generate the JSON schema via reflection. We want to explicitly define this schema to have better control (e.g. descriptions, specific constraints) and match the pattern established in our generic provider tests. Since `AiSchemaGenerator` is a utility solely for this purpose and we are moving away from reflection-based generation, it should be removed.

## Files to Modify
- `AIInterviewer.ServiceInterface/Services/Interview/InterviewService.cs` (Refactor)
- `AIInterviewer.ServiceInterface/Utilities/AiSchemaGenerator.cs` (Delete)
- `AIInterviewer.Tests/AiSchemaGeneratorTests.cs` (Delete)

## References
- `AIInterviewer.Tests/GenericProviderTests.cs` (Example of the target pattern)
- `AIInterviewer.ServiceModel/Types/Interview/EvaluationResponse.cs` (Target type definition)

## Implementation Steps

1.  **Create Worktree**:
    -   Create a new worktree for this task:
        ```bash
        git worktree add ../AIInterviewer-refactor-schema -b feature/refactor-schema
        ```

2.  **Modify `InterviewService.cs`**:
    -   Locate the `Post(FinishInterview request)` method.
    -   Find the code block:
        ```csharp
        var schema = AiSchemaGenerator.Generate(typeof(EvaluationResponse));
        var evaluation = await provider.GenerateJsonAsync<EvaluationResponse>(evaluationPrompt, schema, nameof(EvaluationResponse));
        ```
    -   Replace `AiSchemaGenerator.Generate(...)` with a manually constructed `AiSchemaDefinition`.
    -   The `AiSchemaDefinition` should define the structure for `EvaluationResponse`:
        -   **Description**: "Evaluation response containing score and feedback."
        -   **Properties**:
            -   `"score"`: Type = "integer", Description = "Score from 0-100 based on the candidate's performance."
            -   `"feedback"`: Type = "string", Description = "Comprehensive markdown report including Summary, Strengths, Areas for Improvement, Role Fit, Hiring Recommendation, and Next Steps."
        -   **Required**: `["score", "feedback"]`
    -   Pass the new `schema` object to `provider.GenerateJsonAsync`.
    -   **Important**: Ensure property names in the schema match what the AI is expected to produce and what the JSON serializer expects (usually camelCase or matching the C# property names if using a specific resolver). `EvaluationResponse` has `Score` and `Feedback`. The manual definition above uses lowercase "score" and "feedback". Ensure the deserialization in `GenerateJsonAsync` handles this (it usually does with camelCase resolvers, but verifying doesn't hurt).

3.  **Delete `AiSchemaGenerator`**:
    -   Delete the file: `AIInterviewer.ServiceInterface/Utilities/AiSchemaGenerator.cs`.
    -   Remove `using AIInterviewer.ServiceInterface.Utilities;` from `InterviewService.cs` and any other files if it's no longer used.

4.  **Delete Tests**:
    -   Delete the file: `AIInterviewer.Tests/AiSchemaGeneratorTests.cs`.

5.  **Verify**:
    -   Ensure the project compiles.
    -   Run tests to ensure no regressions (specifically `GenericProviderTests` if relevant, though unrelated to this specific class).
    -   Ensure the application builds successfully without the deleted files.

6.  **Finalize**:
    -   Update strict clood files if necessary (remove the deleted files from `ai-integration.json` if present).
    -   Commit and request review/merge.

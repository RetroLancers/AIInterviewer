# Abstract AI Schema Generation & Remove Reflection

## Objective
Finalize the abstraction of AI schema generation by enforcing a tree-walking approach and removing reliance on Reflection and `System.Type` within the providers and interface.

## Current State Analysis
- `AiSchemaDefinition` exists and is structured correctly (recursive tree).
- `IAiProvider` still uses `GenerateJsonAsync<T>`.
- `GeminiAiProvider` correctly implements a tree walker (`ConvertSchema`).
- `OpenAiProvider` incorrectly uses `JsonConvert.SerializeObject` (serialization reflection) and `typeof(T).Name`.

## Requirements
1.  **Refactor Interface**:
    -   Modify `IAiProvider.GenerateJsonAsync` to **remove the generic `T` parameter**.
    -   It should return `Task<string?>` (the raw JSON string) instead of `T`.
    -   It should accept a `schemaName` string (or similar) explicitly.
2.  **Explicit Tree Walker for Both Providers**:
    -   **OpenAI**: Remove `JsonConvert` usage. Implement a manual recursive walker that constructs the JSON Schema string (or `BinaryData`) from `AiSchemaDefinition`.
    -   **Gemini**: Ensure `ConvertSchema` is a robust recursive tree walker that converts `AiSchemaDefinition` to `Google.GenAI.Types.Schema` without any hidden reflection.
    -   Both implementations must strictly avoid `System.Type` or reflection-based serialization for schema generation.
3.  **Update Consumers**:
    -   Refactor all calls to `GenerateJsonAsync` (e.g., in `AiConfigService`) to handle the deserialization themselves.

## Checklist
- [ ] Refactor `IAiProvider` interface:
    -   Change `Task<T?> GenerateJsonAsync<T>(...)` to `Task<string?> GenerateJsonAsync(string prompt, AiSchemaDefinition schema, string schemaName, ...)`
- [ ] Update `GeminiAiProvider`:
    -   Match new interface.
    -   Ensure `ConvertSchema` works as a tree walker (already exists, verify compliance).
    -   Return the raw JSON string (remove internal deserialization).
- [ ] Update `OpenAiProvider`:
    -   Match new interface.
    -   **Implement Tree Walker**: Replace `JsonConvert.SerializeObject(schema)` with a custom recursive method that builds the JSON schema string from `AiSchemaDefinition`.
    -   Remove `typeof(T)`.
- [ ] Update `AiSchemaGenerator` (if needed) or relevant call sites to ensure `AiSchemaDefinition` is passed correctly.
- [ ] Refactor call sites (e.g. `AiConfigService`) to handle deserialization manually.
- [ ] Verify both providers work with the new abstraction.

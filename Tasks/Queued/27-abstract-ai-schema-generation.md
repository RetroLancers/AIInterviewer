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
    -   Update `IAiProvider.GenerateJsonAsync` to accept `AiSchemaDefinition schema` explicitly.
    -   Signature should look like: `Task<T?> GenerateJsonAsync<T>(string prompt, AiSchemaDefinition schema, string schemaName, ...)`
    -   **Crucially**: The provider must use the passed `AiSchemaDefinition` to construct the AI request (using a tree walker), **NOT** partially inspect `typeof(T)` to build the schema.
2.  **Explicit Tree Walker for Both Providers**:
    -   **OpenAI**: Implement a manual recursive walker that constructs the JSON Schema string (or `BinaryData`) from the passed `AiSchemaDefinition`. Do not use `JsonConvert` or reflection on `T` to build this schema.
    -   **Gemini**: Ensure `ConvertSchema` uses the `AiSchemaDefinition` to build `Google.GenAI.Types.Schema`.
    -   Both providers should finally deserialize the raw JSON response to `T` using standard JSON deserialization (e.g. `JsonConvert.DeserializeObject<T>`).
3.  **Update Consumers**:
    -   Callers must construct and pass the `AiSchemaDefinition` explicitly.
    -   `AiSchemaGenerator.Generate(typeof(T))` can still be used *by the consumer* if they want to derive it from the type initially, but the *Provider* shouldn't care where it came from; it just respects the passed definition.

## Checklist
- [ ] Refactor `IAiProvider` interface:
    -   Update signature: `Task<T?> GenerateJsonAsync<T>(string prompt, AiSchemaDefinition schema, string schemaName, ...)`
- [ ] Update `GeminiAiProvider`:
    -   Ensure it uses the passed `AiSchemaDefinition` for the request.
    -   Deserialize the response to `T` before returning.
- [ ] Update `OpenAiProvider`:
    -   **Implement Tree Walker**: Replace `JsonConvert.SerializeObject(schema)` with a custom recursive method that builds the OpenAI JSON schema string from `AiSchemaDefinition`.
    -   Deserialize the response to `T` before returning.
- [ ] Update call sites to pass `AiSchemaDefinition`.
- [ ] Verify functionality.

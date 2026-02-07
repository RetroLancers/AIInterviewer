# 29. Refactor Schema Generation Logic

## Objectives
- Extract the schema generation and "tree walking" logic from `OpenAiProvider` and `GeminiAiProvider` into their own dedicated classes.
- Ensure the provider classes delegate this logic to the new classes rather than containing the implementation details.

## Context
Currently, the logic for walking the type tree and generating the schema for AI providers (e.g., for Structured Outputs) resides directly within the `GenerateSchema` methods of `OpenAiProvider` and `GeminiAiProvider`. To improve maintainability and separation of concerns, this logic should be encapsulated in separate, specialized classes.

## Requirements
1.  **Identify Logic**: Locate the schema generation and tree-walking code in both `OpenAiProvider.cs` and `GeminiAiProvider.cs`.
2.  **Create Classes**: Create new classes (e.g., `OpenAiSchemaGenerator`, `GeminiSchemaGenerator` or similar) to house this logic.
3.  **Refactor Providers**: Update `OpenAiProvider` and `GeminiAiProvider` to use these new classes for schema generation.
4.  **Verification**: Ensure that the functionality remains unchanged (tests from Task 28 should pass if applicable/implemented, otherwise manual verification).

## Outcome
Clean `OpenAiProvider` and `GeminiAiProvider` classes that delegate complex schema generation logic to specialized components.

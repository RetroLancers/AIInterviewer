# Abstract AI Schema Generation

## Objective
Create a type to abstract the way both `GeminiAiProvider` and `OpenAiProvider` can communicate their schema/structured json responses. Currently, in OpenAI we're using reflection which should be avoided. We want to create a type that the "Generate schema" can take in either implementation of it (also change the interface to not take a type) and convert it to the appropriate schema.

## Context
OpenAI example for reference:
```csharp
ChatCompletionOptions options = new()
{
    ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
        jsonSchemaFormatName: "math_reasoning",
        jsonSchema: BinaryData.FromBytes("""
            {
                "type": "object",
                "properties": {
                    "steps": {
                        "type": "array",
                        "items": {
                            "type": "object",
                            "properties": {
                                "explanation": { "type": "string" },
                                "output": { "type": "string" }
                            },
                            "required": ["explanation", "output"],
                            "additionalProperties": false
                        }
                    },
                    "final_answer": { "type": "string" }
                },
                "required": ["steps", "final_answer"],
                "additionalProperties": false
            }
            """u8.ToArray()),
        jsonSchemaIsStrict: true)
};
```

## Requirements
- Create a new type (e.g., `AiSchemaDefinition`) that can be used by both providers.
- Refactor `IAiProvider` interface to use this new type for schema generation instead of taking a Type and using reflection.
- Update `GeminiAiProvider` to support this new type.
- Update `OpenAiProvider` to support this new type.
- Ensure the schema generation logic handles the conversion to the specific provider's format (OpenAI's JSON schema vs Gemini's format).

## Checklist
- [x] Create `AiSchemaDefinition` type (or similar name).
- [x] Update `IAiProvider` interface method signature.
- [x] Implement support in `OpenAiProvider`.
- [x] Implement support in `GeminiAiProvider`.
- [x] Verify schema generation works for both providers without reflection in the provider implementation (or at least cleanly abstracted).

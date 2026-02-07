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
- Create a new recursive type structure (e.g., `AiSchemaDefinition`) that represents the schema (Object, Array, String, Number, Boolean, etc.).
- This type must be capable of defining properties, required fields, and nested schemas.
- **Strictly avoid using `System.Type` or Reflection.** The schema should be defined by manual construction of this tree or a helper fluent API, not by inspecting CLR types.
- The "Generate Schema" logic should be a tree walker that traverses this `AiSchemaDefinition` structure and outputs the provider-specific format (JSON Schema for OpenAI, appropriate format for Gemini).
- Refactor `IAiProvider` interface to accept this `AiSchemaDefinition` instead of a `Type`.
- Update `GeminiAiProvider` and `OpenAiProvider` to implement the walker for their respective formats.

## Checklist
- [ ] Design and implement `AiSchemaDefinition` (or similar) recursive tree structure.
- [ ] Implement a tree walker for OpenAI that produces JSON Schema.
- [ ] Implement a tree walker for Gemini that produces the required Gemini schema format.
- [ ] Update `IAiProvider` interface to use `AiSchemaDefinition`.
- [ ] Refactor call sites to construct `AiSchemaDefinition` explicitly instead of passing a C# type.
- [ ] Verify both providers work correctly with the new tree-based schema generation.

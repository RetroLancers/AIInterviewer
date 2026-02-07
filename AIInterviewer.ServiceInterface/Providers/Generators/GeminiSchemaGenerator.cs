using AIInterviewer.ServiceModel.Types.Ai;
using Google.GenAI.Types;

namespace AIInterviewer.ServiceInterface.Providers.Generators;

public static class GeminiSchemaGenerator
{
    public static Schema ConvertSchema(AiSchemaDefinition def)
    {
        var schema = new Schema
        {
            Type = MapType(def.Type),
            Description = def.Description
        };

        if (def.Properties != null)
        {
            schema.Properties = new Dictionary<string, Schema>();
            foreach (var prop in def.Properties)
            {
                schema.Properties[prop.Key] = ConvertSchema(prop.Value);
            }
        }

        if (def.Required != null)
        {
            schema.Required = new List<string>(def.Required);
        }

        if (def.Items != null)
        {
            schema.Items = ConvertSchema(def.Items);
        }

        if (def.Enum != null)
        {
            schema.Enum = new List<string>(def.Enum);
        }

        return schema;
    }

    private static Google.GenAI.Types.Type MapType(string type)
    {
        return type.ToLower() switch
        {
            "string" => Google.GenAI.Types.Type.STRING,
            "number" => Google.GenAI.Types.Type.NUMBER,
            "integer" => Google.GenAI.Types.Type.INTEGER,
            "boolean" => Google.GenAI.Types.Type.BOOLEAN,
            "array" => Google.GenAI.Types.Type.ARRAY,
            "object" => Google.GenAI.Types.Type.OBJECT,
            _ => Google.GenAI.Types.Type.STRING
        };
    }
}

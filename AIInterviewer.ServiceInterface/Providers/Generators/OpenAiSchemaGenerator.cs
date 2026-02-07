using AIInterviewer.ServiceModel.Types.Ai;

namespace AIInterviewer.ServiceInterface.Providers.Generators;

public static class OpenAiSchemaGenerator
{
    public static Dictionary<string, object> MapSchema(AiSchemaDefinition def)
    {
        var dict = new Dictionary<string, object>
        {
            ["type"] = def.Type
        };

        if (!string.IsNullOrEmpty(def.Description))
        {
            dict["description"] = def.Description;
        }

        if (def.Properties != null && def.Properties.Count > 0)
        {
            var props = new Dictionary<string, object>();
            foreach (var prop in def.Properties)
            {
                props[prop.Key] = MapSchema(prop.Value);
            }
            dict["properties"] = props;
        }

        if (def.Required != null && def.Required.Count > 0)
        {
            dict["required"] = def.Required;
        }

        if (def.Items != null)
        {
            dict["items"] = MapSchema(def.Items);
        }

        if (def.Enum != null && def.Enum.Count > 0)
        {
            dict["enum"] = def.Enum;
        }

        if (def.Type == "object")
        {
            dict["additionalProperties"] = def.AdditionalProperties ?? false;
        }

        return dict;
    }
}

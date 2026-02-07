using System.Reflection;
using AIInterviewer.ServiceModel.Types.Ai;

namespace AIInterviewer.ServiceInterface.Utilities;

public static class AiSchemaGenerator
{
    public static AiSchemaDefinition Generate(Type type)
    {
        if (type == typeof(string)) return new AiSchemaDefinition { Type = "string" };
        if (type == typeof(int) || type == typeof(long)) return new AiSchemaDefinition { Type = "integer" };
        if (type == typeof(bool)) return new AiSchemaDefinition { Type = "boolean" };
        if (type == typeof(double) || type == typeof(float) || type == typeof(decimal)) return new AiSchemaDefinition { Type = "number" };

        if (type.IsArray)
        {
            return new AiSchemaDefinition
            {
                Type = "array",
                Items = Generate(type.GetElementType()!)
            };
        }

        if (type.IsGenericType && typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
        {
            var args = type.GetGenericArguments();
            if (args.Length > 0)
            {
                return new AiSchemaDefinition
                {
                    Type = "array",
                    Items = Generate(args[0])
                };
            }
        }

        if (type.IsEnum)
        {
            return new AiSchemaDefinition
            {
                Type = "string",
                Enum = Enum.GetNames(type).ToList()
            };
        }

        if (type.IsClass && type != typeof(string))
        {
            var properties = new Dictionary<string, AiSchemaDefinition>();
            var required = new List<string>();

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                properties[prop.Name] = Generate(prop.PropertyType);
                required.Add(prop.Name);
            }

            return new AiSchemaDefinition
            {
                Type = "object",
                Properties = properties,
                Required = required,
                AdditionalProperties = false
            };
        }

        return new AiSchemaDefinition { Type = "string" };
    }
}

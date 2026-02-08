using System.Collections.Generic;
using System.Linq;
using AIInterviewer.ServiceModel.Tables.Configuration;

namespace AIInterviewer.ServiceModel.Types.Configuration.ExtensionMethods;

public static class AiServiceConfigExtensions
{
    public static AiServiceConfig ToTable(this CreateAiConfig request)
    {
        return new AiServiceConfig
        {
            Name = request.Name,
            ProviderType = request.ProviderType,
            ApiKey = request.ApiKey,
            ModelId = request.ModelId,
            FallbackModelId = request.FallbackModelId,
            Voice = request.Voice
        };
    }

    public static void UpdateTable(this AiServiceConfig table, UpdateAiConfig request)
    {
        table.Name = request.Name;
        table.ProviderType = request.ProviderType;
        table.ApiKey = request.ApiKey;
        table.ModelId = request.ModelId;
        table.FallbackModelId = request.FallbackModelId;
        table.Voice = request.Voice;
    }

    public static AiConfigResponse ToDto(this AiServiceConfig table)
    {
        return new AiConfigResponse
        {
            Id = table.Id,
            Name = table.Name,
            ProviderType = table.ProviderType,
            ApiKey = table.ApiKey,
            ModelId = table.ModelId,
            FallbackModelId = table.FallbackModelId,
            Voice = table.Voice
        };
    }

    public static List<AiConfigResponse> ToDto(this IEnumerable<AiServiceConfig> tables)
    {
        return tables.Select(x => x.ToDto()).ToList();
    }
}

using AIInterviewer.ServiceModel.Tables.Configuration;

namespace AIInterviewer.ServiceModel.Types.Configuration.ExtensionMethods;

public static class SiteConfigExtensions
{
    public static SiteConfig ToTable(this UpdateSiteConfigRequest request)
    {
        return new SiteConfig
        {
            Id = request.Id,
            ActiveAiConfigId = request.ActiveAiConfigId,
            GlobalFallbackModel = request.GlobalFallbackModel,
            KokoroVoice = request.KokoroVoice,
            TranscriptionProvider = request.TranscriptionProvider
        };
    }

    public static SiteConfigResponse ToDto(this SiteConfig table)
    {
        return new SiteConfigResponse
        {
            Id = table.Id,
            ActiveAiConfigId = table.ActiveAiConfigId,
            GlobalFallbackModel = table.GlobalFallbackModel,
            KokoroVoice = table.KokoroVoice,
            TranscriptionProvider = table.TranscriptionProvider
        };
    }

    public static List<SiteConfigResponse> ToDto(this IEnumerable<SiteConfig> tables)
    {
        return tables.Select(x => x.ToDto()).ToList();
    }
}

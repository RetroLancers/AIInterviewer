using TyphoonSharp.ServiceModel.Tables.Configuration;

namespace TyphoonSharp.ServiceModel.Types.Configuration.ExtensionMethods;

public static class SiteConfigExtensions
{
    public static SiteConfig ToTable(this UpdateSiteConfigRequest request)
    {
        return new SiteConfig
        {
            Id = request.Id,
            GeminiApiKey = request.GeminiApiKey,
            InterviewModel = request.InterviewModel,
            GlobalFallbackModel = request.GlobalFallbackModel
        };
    }

    public static SiteConfigResponse ToDto(this SiteConfig table)
    {
        return new SiteConfigResponse
        {
            Id = table.Id,
            GeminiApiKey = table.GeminiApiKey,
            InterviewModel = table.InterviewModel,
            GlobalFallbackModel = table.GlobalFallbackModel
        };
    }

    public static List<SiteConfigResponse> ToDto(this IEnumerable<SiteConfig> tables)
    {
        return tables.Select(x => x.ToDto()).ToList();
    }
}
using ServiceStack;
using AIInterviewer.ServiceModel.Types.AI;
using System.Linq;
using System.Threading.Tasks;
using AIInterviewer.ServiceModel.Tables.Configuration;
using System.Collections.Generic;

namespace AIInterviewer.ServiceInterface.Services.AI;

public class GeminiModelsService(SiteConfigHolder siteConfigHolder) : Service
{
    public async Task<GetGeminiModelsResponse> Get(GetGeminiModels request)
    {
        var client = siteConfigHolder.GetGeminiClient();
        var modelsPager = await client.GetModels(null);
        var models = new List<string>();
        
        await foreach (var model in modelsPager)
        {
            if (model.Name != null)
                models.Add(model.Name);
        }

        return new GetGeminiModelsResponse
        {
            Models = models
        };
    }
}
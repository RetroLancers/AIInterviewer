using System;
using System.Linq;
using ServiceStack;
using ServiceStack.OrmLite;
using AIInterviewer.ServiceModel.Tables.Configuration;
using AIInterviewer.ServiceModel.Types.Configuration;
using AIInterviewer.ServiceModel.Types.Configuration.ExtensionMethods;

namespace AIInterviewer.ServiceInterface.Services.Configuration;

public class AiConfigService : Service
{
    public object Any(ListAiConfigs request)
    {
        return new ListAiConfigsResponse
        {
            Configs = Db.Select<AiServiceConfig>().ToDto()
        };
    }

    public object Any(GetAiConfig request)
    {
        var config = Db.SingleById<AiServiceConfig>(request.Id);
        if (config == null) throw HttpError.NotFound("Config not found");
        return config.ToDto();
    }

    public object Any(CreateAiConfig request)
    {
        Validate(request.ProviderType, request.ApiKey);

        var config = request.ToTable();
        long id = Db.Insert(config, selectIdentity: true);
        config.Id = (int)id;

        return config.ToDto();
    }

    public object Any(UpdateAiConfig request)
    {
        var config = Db.SingleById<AiServiceConfig>(request.Id);
        if (config == null) throw HttpError.NotFound("Config not found");

        Validate(request.ProviderType, request.ApiKey);
        
        config.UpdateTable(request);
        Db.Update(config);

        return config.ToDto();
    }

    public void Any(DeleteAiConfig request)
    {
        Db.DeleteById<AiServiceConfig>(request.Id);
    }

    private void Validate(string providerType, string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("Api Key is required");
            
        if (providerType != "Gemini" && providerType != "OpenAI")
             throw new ArgumentException("Provider Type must be Gemini or OpenAI");
    }
}

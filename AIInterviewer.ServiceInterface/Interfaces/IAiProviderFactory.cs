using AIInterviewer.ServiceModel.Tables.Configuration;

namespace AIInterviewer.ServiceInterface.Interfaces;

public interface IAiProviderFactory
{
    IAiProvider GetProvider(AiServiceConfig config);
}

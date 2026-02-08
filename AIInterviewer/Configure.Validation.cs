using ServiceStack;
using ServiceStack.Validation;

[assembly: HostingStartup(typeof(AIInterviewer.ConfigureValidation))]

namespace AIInterviewer;

public class ConfigureValidation : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            services.AddPlugin(new ValidationFeature());
        });
}

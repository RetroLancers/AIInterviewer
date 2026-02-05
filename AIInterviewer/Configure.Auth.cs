using AIInterviewer.ServiceInterface.Data;
using ServiceStack.Auth;

[assembly: HostingStartup(typeof(AIInterviewer.ConfigureAuth))]

namespace AIInterviewer;

public class ConfigureAuth : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices(services => {
            services.AddPlugin(new AuthFeature(IdentityAuth.For<ApplicationUser>(options => {
                options.SessionFactory = () => new CustomUserSession();
                options.CredentialsAuth();
                options.AdminUsersFeature();
            })));
        });
}
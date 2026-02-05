using AIInterviewer.ServiceInterface.Data;
using Microsoft.EntityFrameworkCore;
using ServiceStack.Data;
using ServiceStack.OrmLite;
using Microsoft.EntityFrameworkCore.Diagnostics;

[assembly: HostingStartup(typeof(AIInterviewer.ConfigureDb))]

namespace AIInterviewer;

public class ConfigureDb : IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) => {
            var connectionString = context.Configuration.GetConnectionString("DefaultConnection")
                                   ?? "DataSource=App_Data/app.db;Cache=Shared";
            
            services.AddOrmLite(options => options.UseSqlite(connectionString));

            // $ dotnet ef migrations add CreateIdentitySchema
            // $ dotnet ef database update
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlite(connectionString, b => b.MigrationsAssembly(nameof(AIInterviewer)));
                options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
            });
            
            // Enable built-in Database Admin UI at /admin-ui/database
            services.AddPlugin(new AdminDatabaseFeature());
        });
}
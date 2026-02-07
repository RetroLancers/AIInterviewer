using System.Net;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using AIInterviewer.ServiceInterface;
using AIInterviewer.ServiceInterface.Data;
using AIInterviewer.ServiceInterface.Services.Configuration;
using NLog;
using NLog.Web;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

AppHost.RegisterKey();

try {
    var builder = WebApplication.CreateBuilder(args);
    
    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var services = builder.Services;

services.AddAuthorization();
services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("App_Data"));

services.AddDatabaseDeveloperPageExceptionFilter();

services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

services.AddRazorPages();

services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
// Uncomment to send emails with SMTP, configure SMTP with "SmtpConfig" in appsettings.json
// services.AddSingleton<IEmailSender<ApplicationUser>, EmailSender>();
services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, AdditionalUserClaimsPrincipalFactory>();

// Register all services
services.AddServiceStack(typeof(SiteConfigService).Assembly);

var app = builder.Build();
var nodeProxy = new NodeProxy("http://127.0.0.1:5173") {
    Log = app.Logger
};

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
        
    app.MapNotFoundToNode(nodeProxy);
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapCleanUrls();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapRazorPages();

app.UseServiceStack(new AppHost(), options => {
    options.MapEndpoints();
});

// Proxy HMR WebSocket and fallback routes to Node dev server in Development
if (app.Environment.IsDevelopment())
{
    app.RunNodeProcess(nodeProxy, "../AIInterviewer.Client"); // Start Node if not running
    app.UseWebSockets();
    app.MapViteHmr(nodeProxy);
    app.MapFallbackToNode(nodeProxy); // Fallback to Node dev server in development
}
else
{
    app.MapFallbackToFile("index.html"); // Fallback to index.html in production (AIInterviewer.Client/dist > wwwroot)
}

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}

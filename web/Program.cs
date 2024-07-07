#pragma warning disable CA1031 // Do not catch general exception types

using System.Globalization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

const string appName = "Web_Template";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Information, theme: AnsiConsoleTheme.Code, formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    var webApplication = WebApplication.CreateBuilder(args)
        .ConfigureBehavior(appName, static webApplicationBehaviorBuilder =>
        {
            webApplicationBehaviorBuilder.ConfigureServices(static serviceCollection =>
            {
                serviceCollection.AddControllersWithViews();
            });
        })
        .Build();

    webApplication.UseCacheResponseHeaders();
    webApplication.UseSecureResponseHeaders();
    webApplication.UseStatusCodePagesWithReExecute("/StatusCodes/{0}");

    if (!webApplication.Environment.IsDevelopment())
    {
        webApplication.UseExceptionHandler();
        webApplication.UseHsts();
    }

    webApplication.UseHttpsRedirection();

    webApplication.UseStaticFiles();
    webApplication.UseCookiePolicy();

    webApplication.MapHealthChecks("/healthz");
    webApplication.MapControllers();

    webApplication.Run();

    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "An unexpected error occurred during startup");
    return -1;
}
finally
{
    Log.CloseAndFlush();
}

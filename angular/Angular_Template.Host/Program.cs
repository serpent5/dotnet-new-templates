#pragma warning disable CA1031 // Do not catch general exception types

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: ApiController]

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Information, formatProvider: CultureInfo.InvariantCulture, theme: AnsiConsoleTheme.Code)
    .CreateBootstrapLogger();

try
{
    var webApplicationBuilder = WebApplication.CreateBuilder(args)
        .ConfigureAnnabelle("Angular_Template")
        .ConfigureDevelopmentServer("Frontend", "http://localhost:4200");

    webApplicationBuilder.Services.AddControllersWithViews();

    var webApplication = webApplicationBuilder.Build();

    webApplication.UseResponseCacheControl();
    webApplication.UseSecureResponseHeaders();
    webApplication.UseStatusCodePagesWithReExecute("/StatusCodes/{0}");

    if (!webApplication.Environment.IsDevelopment())
    {
        webApplication.UseExceptionHandler();
        webApplication.UseHsts();
    }

    webApplication.UseHttpsRedirection();
    webApplication.UseStaticFiles();

    webApplication.UseRouting();

    webApplication.UseAuthentication();
    webApplication.UseAuthorization();

    webApplication.MapHealthChecks("/healthz");
    webApplication.MapControllers().RequireAuthorization();
    webApplication.MapJavaScriptFramework();

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

// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program;

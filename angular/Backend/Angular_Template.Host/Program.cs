#pragma warning disable CA1031 // Do not catch general exception types

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using static System.Net.Mime.MediaTypeNames;

[assembly: ApiController]

const string appName = "Angular_Template";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Information, theme: AnsiConsoleTheme.Code, formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    var webApplication = WebApplication.CreateBuilder()
        .ConfigureBehavior(appName, static webApplicationBehaviorBuilder =>
        {
            webApplicationBehaviorBuilder
                .ConfigureWebApi(static o =>
                {
                    o.SwaggerDoc(
                        "v0.1",
                        new OpenApiInfo
                        {
                            Title = appName,
                            Description = $"An ASP.NET Core Web API for {appName}.",
                            Version = "0.1"
                        });
                })
                .ConfigureServices(static serviceCollection =>
                {
                    serviceCollection.AddControllersWithViews(static o => o.Filters.Add(new ProducesAttribute(Application.Json)));
                });
        })
        .Build();

    webApplication.UseCacheResponseHeaders();
    webApplication.UseSecureResponseHeaders();
    webApplication.UseStatusCodePagesWithReExecute("/StatusCodes/{0}");

    if (webApplication.Environment.IsDevelopment())
    {
        webApplication.UseSwagger();
        webApplication.UseSwaggerUI(static o => o.SwaggerEndpoint("/swagger/v0.1/swagger.json", $"{appName} v0.1"));
    }
    else
    {
        webApplication.UseExceptionHandler();
        webApplication.UseHsts();
    }

    webApplication.UseHttpsRedirection();

    webApplication.UseStaticFiles();
    webApplication.UseCookiePolicy();

    webApplication.UseRouting();

    webApplication.MapHealthChecks("/healthz");
    webApplication.MapControllers();

    if (webApplication.Environment.IsDevelopment())
        webApplication.MapFallbackToServer("http://localhost:4200");
    else
        webApplication.MapFallbackToIndexHTML();

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

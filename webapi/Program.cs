using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

[assembly: ApiController]

#pragma warning disable CA1852 // Seal internal types

const string appName = "WebApi_Template";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Information, theme: AnsiConsoleTheme.Code, formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    var webApplicationBuilder = WebApplication.CreateBuilder(args);

    webApplicationBuilder.ConfigureBehavior(appName, static webApplicationBehaviorBuilder =>
    {
        webApplicationBehaviorBuilder.ConfigureWebAPI();
    });

    webApplicationBuilder.Services.AddControllers();

    var webApplication = webApplicationBuilder.Build();

    webApplication.UseCacheResponseHeaders();
    webApplication.UseStatusCodePagesWithReExecute("/StatusCodes/{0}");

    if (webApplication.Environment.IsDevelopment())
    {
        webApplication.UseSwagger();
        webApplication.UseSwaggerUI();
    }
    else
    {
        webApplication.UseSecureResponseHeaders();
        webApplication.UseExceptionHandler();
        webApplication.UseHsts();
    }

    webApplication.UseHttpsRedirection();
    webApplication.UseSerilogRequestLogging();

    webApplication.MapHealthChecks("/healthz");
    webApplication.MapControllers();

    webApplication.Run();

}
#pragma warning disable CA1031 // Do not catch general exception types
catch (Exception ex)
{
    Log.Fatal(ex, "An unexpected error occurred during startup");
    Environment.ExitCode = -1;
}
#pragma warning restore CA1031 // Do not catch general exception types
finally
{
    Log.CloseAndFlush();
}

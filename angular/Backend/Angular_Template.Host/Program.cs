using System.Globalization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;
using Swashbuckle.AspNetCore.SwaggerGen;
using Yarp.ReverseProxy.Forwarder;

[assembly: ApiController]

#pragma warning disable CA1852 // Seal internal types

const string appName = "Angular_Template";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(LogEventLevel.Information, theme: AnsiConsoleTheme.Code, formatProvider: CultureInfo.InvariantCulture)
    .CreateBootstrapLogger();

try
{
    var webApplicationBuilder = WebApplication.CreateBuilder(args);

    webApplicationBuilder.ConfigureBehavior(appName, static webApplicationBehaviorBuilder =>
    {
        webApplicationBehaviorBuilder
            .ConfigureWebAPI()
            .ConfigureClientUI(new Uri("http://localhost:4200"));
    });

    webApplicationBuilder.Services.Configure<SwaggerGenOptions>(static o =>
    {
        o.SwaggerDoc(
            "v0.1",
            new OpenApiInfo
            {
                Title = appName,
                Description = $"An ASP.NET Core Web API for {appName}.",
                Version = "v0.1"
            });
    });

    webApplicationBuilder.Services.AddControllersWithViews(static o => o.Filters.Add(new ProducesAttribute("application/json")));

    var webApplication = webApplicationBuilder.Build();

    // https://github.com/microsoft/reverse-proxy/issues/1375
    if (webApplication.Environment.IsDevelopment() && webApplication.Services.GetService<IHttpForwarder>() is not null)
    {
        webApplication.Use((ctx, nextMiddleware) =>
        {
            if (ctx.Request.Method != HttpMethods.Connect || ctx.Request.Protocol == HttpProtocol.Http11)
                return nextMiddleware(ctx);

            ctx.Features.Get<IHttpResetFeature>()?.Reset(0xD); // HTTP_1_1_REQUIRED
            return Task.CompletedTask;
        });
    }

    webApplication.UseCacheResponseHeaders();
    webApplication.UseStatusCodePagesWithReExecute("/StatusCodes/{0}");

    if (webApplication.Environment.IsDevelopment())
    {
        webApplication.UseSwagger();
        webApplication.UseSwaggerUI(static o => o.SwaggerEndpoint("/swagger/v0.1/swagger.json", $"{appName} v0.1"));
    }
    else
    {
        webApplication.UseSecureResponseHeaders();
        webApplication.UseExceptionHandler();
        webApplication.UseHsts();
    }

    webApplication.UseHttpsRedirection();

    webApplication.UseStaticFiles();

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

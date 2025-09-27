using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

builder.Configuration.AddEnvironmentVariables();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.AddSingleton(builder.Configuration);

var app = builder.Build();

app.MapGet("/{**catchAll}", async (HttpContext context, IHttpClientFactory httpClientFactory, IConfiguration configuration) =>
{
    var catchAll = context.Request.RouteValues["catchAll"] as string ?? "";

    var backendUrl = $"{configuration["UrlShortnerServerUrl"]}{catchAll}";
    var httpClient = httpClientFactory.CreateClient();

    try
    {
        var backendResponse = await httpClient.GetAsync(backendUrl);

        if (!backendResponse.IsSuccessStatusCode)
        {
            return Results.StatusCode((int)backendResponse.StatusCode);
        }

        var response = await backendResponse.Content.ReadAsStringAsync();

        if (response != null)
        {
            return Results.Redirect(response);
        }

        return Results.BadRequest(new { error = "Redirect URL not found in backend response" });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});

app.Run();

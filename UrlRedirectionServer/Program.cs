using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();

var app = builder.Build();

app.MapGet("/{**catchAll}", async (HttpContext context, IHttpClientFactory httpClientFactory) =>
{
    var catchAll = context.Request.RouteValues["catchAll"] as string ?? "";

    var backendUrl = $"http://localhost:7001/UrlShortner?shortUrl={catchAll}";
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

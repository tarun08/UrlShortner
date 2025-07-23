using Microsoft.AspNetCore.Mvc;
using UrlShortnerService.Models;
using UrlShortnerService.Services;

namespace UrlShortnerService.Controllers;

[ApiController]
[Route("[controller]")]
public class UrlShortnerController : ControllerBase
{

    private readonly ILogger<UrlShortnerController> _logger;
    private readonly IUrlShortnerService _urlShortnerService;

    public UrlShortnerController(
        ILogger<UrlShortnerController> logger,
        IUrlShortnerService urlShortnerService)
    {
        _logger = logger;
        _urlShortnerService = urlShortnerService;
    }

    [HttpGet(Name = "{shortUrl}")]
    public OkObjectResult Get(string shortUrl)
    {
        try
        {
            string? longUrl = _urlShortnerService.GetLongUrl(shortUrl);
            return new OkObjectResult(longUrl);
        }
        catch(Exception ex)
        {
            return new OkObjectResult(ex.Message);
        }
    }

    [HttpPost(Name = "CreateShortUrl")]
    public OkObjectResult Post(UrlShortenerRequest urlShortenerRequest) 
    {
        _logger.LogInformation("Creating short URL for: {LongUrl}", urlShortenerRequest.LongUrl);

        try
        {
            string shortUrl = _urlShortnerService.CreateShortUrl(urlShortenerRequest);

            _logger.LogInformation("Short URL created: {ShortUrl}", shortUrl);

            return new OkObjectResult($"{shortUrl}");
        }
        catch (Exception ex)
        {
            return new OkObjectResult(ex.Message);
        }

    }
}

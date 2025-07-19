using Microsoft.AspNetCore.Mvc;
using UrlShortnerService.Services;
using static UrlShortnerService.Models;

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
        string longUrl = _urlShortnerService.GetLongUrl(shortUrl);
        return new OkObjectResult(longUrl);
    }

    [HttpPost(Name = "CreateShortUrl")]
    public OkObjectResult Post(UrlShortenerRequest urlShortenerRequest) 
    {
        _logger.LogInformation("Creating short URL for: {LongUrl}", urlShortenerRequest.LongUrl);

        string shortUrl = _urlShortnerService.CreateShortUrl(urlShortenerRequest);

        _logger.LogInformation("Short URL created: {ShortUrl}", shortUrl);

        return new OkObjectResult($"http://localhost:5172/UrlShortnerController/{shortUrl}");
    }
}

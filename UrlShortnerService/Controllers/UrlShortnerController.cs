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
    private readonly IConfiguration _config;

    public UrlShortnerController(
        ILogger<UrlShortnerController> logger,
        IUrlShortnerService urlShortnerService,
        IConfiguration config)
    {
        _logger = logger;
        _urlShortnerService = urlShortnerService;
        _config = config;
    }

    [HttpGet(Name = "{shortUrl}")]
    public IActionResult Get(string shortUrl)
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
    public IActionResult Post(UrlShortenerRequest urlShortenerRequest) 
    {
        _logger.LogInformation("Creating short URL for: {LongUrl}", urlShortenerRequest.LongUrl);

        try
        {
            string shortUrl = _urlShortnerService.CreateShortUrl(urlShortenerRequest);

            _logger.LogInformation("Short URL created: {ShortUrl}", shortUrl);

            return Ok(new { shortUrl = $"{_config["UrlRedirectionServiceUrl"]}/{shortUrl}" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

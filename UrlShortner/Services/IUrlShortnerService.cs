using static UrlShortnerService.Models;

namespace UrlShortnerService.Services
{
    public interface IUrlShortnerService
    {
        public string CreateShortUrl(UrlShortenerRequest urlShortenerRequest);

        public string GetLongUrl(string shortUrl);
    }
}

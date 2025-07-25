using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;
using UrlShortnerService.Models;

namespace UrlShortnerService.Services
{
    public class UrlShortnerService : IUrlShortnerService
    {
        private Dictionary<string, string> urlMap = new Dictionary<string, string>();
        private readonly IMongoCollection<ShortUrl> _collection;

        public UrlShortnerService(IMongoDatabase database) 
        {
            _collection = database.GetCollection<ShortUrl>("ShortUrl");
        }

        public string CreateShortUrl(Models.UrlShortenerRequest urlShortenerRequest)
        {
            string shortUrlCode = string.Empty;

            if (!string.IsNullOrWhiteSpace(urlShortenerRequest.CustomName))
            {
                if (shortCodeExists(urlShortenerRequest.CustomName))
                    throw new InvalidOperationException("Custom name already exists.");

                shortUrlCode = urlShortenerRequest.CustomName;
            }

            if (string.IsNullOrWhiteSpace(urlShortenerRequest.CustomName))
            {
                while (string.IsNullOrWhiteSpace(shortUrlCode) ||
                    (!string.IsNullOrWhiteSpace(shortUrlCode) && shortCodeExists(shortUrlCode)))
                {
                    string hash = ComputeSha256Hash(urlShortenerRequest.LongUrl + Guid.NewGuid().ToString());
                    shortUrlCode = GetBase64String(hash).Substring(0, 6);
                }
            }
            

            if (!urlMap.TryAdd(shortUrlCode, urlShortenerRequest.LongUrl))
                throw new InvalidOperationException("Failed to add short code to cache.");

            ShortUrl url = new ShortUrl()
            {
                ShortCode = shortUrlCode,
                LongUrl = urlShortenerRequest.LongUrl,
                CreatedAt = urlShortenerRequest.CreatedOn,
                ExpiryDate = urlShortenerRequest.ExpiryDate
            };
            _collection.InsertOne(url);

            return shortUrlCode;
        }

        public string? GetLongUrl(string shortCode)
        {
            urlMap.TryGetValue(shortCode, out string? longUrl);

            if (longUrl == null)
            {
                var filter = Builders<ShortUrl>.Filter.Eq(s => s.ShortCode, shortCode);
                return _collection.Find<ShortUrl>(filter)?.FirstOrDefault()?.LongUrl;
            }
            return longUrl;
        }

        private bool shortCodeExists(string shortCode)
        {
            return urlMap.ContainsKey(shortCode) || !string.IsNullOrWhiteSpace(GetLongUrl(shortCode));
        }

        private static string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));

                return builder.ToString();
            }
        }

        private static string GetBase64String(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            string base64Encoded = Convert.ToBase64String(bytes);

            return base64Encoded.ToString();
        }
    }
}

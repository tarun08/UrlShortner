using System.Security.Cryptography;
using System.Text;

namespace UrlShortnerService.Services
{
    public class UrlShortnerService : IUrlShortnerService
    {
        private Dictionary<string, string> urlMap = new Dictionary<string, string>();
        public string CreateShortUrl(Models.UrlShortenerRequest urlShortenerRequest)
        {
            if (urlShortenerRequest.CustomName != null)
            {
                if (urlMap.ContainsKey(urlShortenerRequest.CustomName))
                {
                    throw new InvalidOperationException("Custom name already exists.");
                }
                urlMap[urlShortenerRequest.CustomName] = urlShortenerRequest.LongUrl;
                return urlShortenerRequest.CustomName;
            }
            
            string hash = ComputeSha256Hash(urlShortenerRequest.LongUrl);
            string shortUrlCode = GetBase64String(hash).Substring(0, 6);

            urlMap.Add(shortUrlCode, urlShortenerRequest.LongUrl);

            return shortUrlCode;
        }

        public string GetLongUrl(string shortUrl)
        {
            urlMap.TryGetValue(shortUrl, out string? longUrl);
            return longUrl ?? throw new KeyNotFoundException("Short URL not found.");
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

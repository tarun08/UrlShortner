namespace UrlShortnerService.Models
{
    public class UrlShortenerRequest
    {
        public UrlShortenerRequest()
        {
            CreatedOn = DateTime.UtcNow;
        }

        public required string LongUrl { get; set; }
        public string? CustomName { get; set; } = null;
        public int ExpiryDate { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}

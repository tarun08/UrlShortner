using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UrlShortnerService.Models
{
    public class ShortUrl
    {
        [BsonId] 
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("longUrl")]
        public required string LongUrl { get; set; }

        [BsonElement("shortCode")]
        public required string ShortCode { get; set; }

        [BsonElement("expiryDate")]
        public int ExpiryDate { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Ecommerce.Services.DAO.Models
{
    public class Article
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? Category { get; set; }
    }
}

using Ecommerce.Services.DAO.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;

namespace Ecommerce.Services.DAO.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }

        [BsonElement("CategoryId")]
        [BsonRepresentation(BsonType.String)]
        public int CategoryId { get; set; }

        [BsonSerializer(typeof(CategorySerializer))]
        public Category Category { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public string StripeProductId { get; set; } 
        public string StripePriceId { get; set; }
        public virtual List<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    }

}
public class CategorySerializer : EnumSerializer<Category>
{
    public CategorySerializer() : base(BsonType.String) { }
}


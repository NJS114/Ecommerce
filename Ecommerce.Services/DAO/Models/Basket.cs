using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Ecommerce.Services.DAO.Models
{
    public class Basket : BaseOrder
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public virtual Book Book { get; set; }
        
    }
}

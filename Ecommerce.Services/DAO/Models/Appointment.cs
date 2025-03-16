using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Org.BouncyCastle.Asn1.Mozilla;

namespace Ecommerce.Services.DAO.Models
{
        public class Appointment
        {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public bool IsConfirmed { get; set; }
        public int CustomerId { get; set; }
        public UserSummary User { get; set; }
    }

    public class UserSummary
    {
        public string UserId { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
    }

}

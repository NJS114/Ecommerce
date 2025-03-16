using Ecommerce.Services.DAO.Enums;
using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Nest;

namespace Ecommerce.Services.DAO.Models
{
    public class User 
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public UserRole Roles { get; set; } = UserRole.Buyer;
        public string Role { get; set; }
        public string? StripeAccountId { get; set; }
        public string? BankAccountDetails { get; set; }
        public string? Address { get; set; } // Ajout de l'adresse
        public string? PhoneNumber { get; set; } // Ajout du numéro de téléphone
        public string? City { get; set; } // Ajout de la ville
        public string? PostalCode { get; set; } // Ajout du code postal
        public virtual List<Book> BooksForSaleOrRent { get; set; } = new List<Book>();
        public virtual List<PaymentMethod> Transactions { get; set; } = new List<PaymentMethod>();
    }
}

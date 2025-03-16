    using Ecommerce.Services.DAO.Enums;

    namespace Ecommerce.Services.DAO.DTOs
    {
        public class UserDTO
        {
            public string Id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Password { get; set; }
            public UserStatus Status { get; set; }
            public string Role { get; set; }
            public string? StripeAccountId { get; set; }
            public string? BankAccountDetails { get; set; }
            public string? Address { get; set; } // Ajout de l'adresse
            public string? PhoneNumber { get; set; } // Ajout du numéro de téléphone
            public string? City { get; set; } // Ajout de la ville
            public string? PostalCode { get; set; } // Ajout du code postal
            public string ConfirmationCode { get; set; }
            public Boolean? IsConfirmed { get; set; }
            public List<string> BooksForSaleOrRent { get; set; } = new List<string>();
            public List<string> Transactions { get; set; } = new List<string>();
        }
    }

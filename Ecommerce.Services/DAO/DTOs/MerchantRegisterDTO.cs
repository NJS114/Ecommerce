using Ecommerce.Services.DAO.Enums;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Services.DAO.DTOs
{
    public class MerchantRegisterDTO
    {
        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le prénom ne doit pas dépasser 50 caractères.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [StringLength(50, ErrorMessage = "Le nom ne doit pas dépasser 50 caractères.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "L'email est obligatoire.")]
        [EmailAddress(ErrorMessage = "Veuillez fournir une adresse email valide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        public string Password { get; set; }

        public Guid Id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "Un document est requis pour la vérification.")]
        public DocumentDTO Document { get; set; }
        [Required(ErrorMessage = "Le nom du titulaire du compte est obligatoire.")]
        public string AccountHolderName { get; set; }

        [Required(ErrorMessage = "Le type de compte est obligatoire.")]
        public string AccountHolderType { get; set; } // individual ou company

        [Required(ErrorMessage = "Le code IBAN ou de routage est obligatoire.")]
        public string IBAN { get; set; }
        public string RoutingNumber { get; set; }

        [Required(ErrorMessage = "Le pays est obligatoire.")]
        public string Country { get; set; }
        public Currency Currency { get; set; } = Currency.eur;
    }
}

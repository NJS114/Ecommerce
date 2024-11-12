using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Services.DAO.DTOs
{
    public class UserDTO
    {
            public int Id { get; set; }

            [Required(ErrorMessage = "Le nom complet est requis.")]
            [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Le nom complet ne peut contenir que des lettres et des espaces.")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "L'email est requis.")]
            [EmailAddress(ErrorMessage = "L'email n'est pas valide.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Le mot de passe est requis.")]
            [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Le mot de passe doit contenir au moins 8 caractères, une lettre et un chiffre.")]
            public string Password { get; set; }
        }
    }






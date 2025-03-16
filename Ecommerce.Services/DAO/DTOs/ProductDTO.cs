using System.ComponentModel.DataAnnotations;
using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.DTOs
{
    public class ProductDTO
    {
        [Required(ErrorMessage = "L'ID du produit est requis.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Le nom du produit est requis.")]
        [StringLength(100, ErrorMessage = "Le nom du produit ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La description du produit est requise.")]
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string Description { get; set; }

        // Utilisation de l'enum Category au lieu du nom ou de l'ID
        [Required(ErrorMessage = "La catégorie du produit est requise.")]

        public Category Category { get; set; }

        [Required(ErrorMessage = "Le prix du produit est requis.")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0.")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string StripeProductId { get; set; }
        public string StripePriceId { get; set; }

    }
}

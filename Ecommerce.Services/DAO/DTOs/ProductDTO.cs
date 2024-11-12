using System.ComponentModel.DataAnnotations;

namespace  Ecommerce.Services.DAO.DTOs
{
    public class ProductDTO
    {
        [Required(ErrorMessage = "L'ID du produit est requis.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom du produit est requis.")]
        [StringLength(100, ErrorMessage = "Le nom du produit ne peut pas dépasser 100 caractères.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "La description du produit est requise.")]
        [StringLength(500, ErrorMessage = "La description ne peut pas dépasser 500 caractères.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "La catégorie du produit est requise.")]
        [StringLength(50, ErrorMessage = "La catégorie ne peut pas dépasser 50 caractères.")]
        public string CategoryName { get; set; }
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Le prix du produit est requis.")]
        [Range(0.01, float.MaxValue, ErrorMessage = "Le prix doit être supérieur à 0.")]
        public float Price { get; set; }
    }
}

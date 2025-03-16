using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Services.DAO.DTOs
{
    public class DocumentDTO
    {
        [Required(ErrorMessage = "Le type de document est obligatoire.")]
        [StringLength(20, ErrorMessage = "Le type de document ne doit pas dépasser 20 caractères.")]
        public string DocumentType { get; set; } 

        [Required(ErrorMessage = "Le fichier du document est obligatoire.")]
        public byte[] FileContent { get; set; } 

        [StringLength(100, ErrorMessage = "La description ne doit pas dépasser 100 caractères.")]
        public string Description { get; set; } 
    }

}

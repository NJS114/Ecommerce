using Ecommerce.Services.DAO.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Services.DAO.Models
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        public string? Barcode { get; set; }

        [Required]
        [MaxLength(255)]
        public string Title { get; set; }

        [Required]
        [MaxLength(255)]
        public string Author { get; set; }

        [MaxLength(2000)]
        public string Synopsis { get; set; }

        [MaxLength(100)]
        public string Edition { get; set; }

        public DateTime? PublicationDate { get; set; }

        [Required]
        public BookCondition BookCondition { get; set; }

        public string? CoverImagePath { get; set; }

        public List<string> AdditionalImagePaths { get; set; } = new List<string>();

        [Required]
        public TransactionType TransactionType { get; set; }

        [Required]
        public Category Category { get; set; } 

        [Column(TypeName = "decimal(18,2)")]
        public decimal? RentalPrice { get; set; }
        [Required]
        public BookStatus Status { get; set; }

    }
}
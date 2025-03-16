using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.DTOs
{
    public class BookDTO
    {
        public string Id { get; set; }
        public string? Barcode { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Synopsis { get; set; }
        public string Edition { get; set; }
        public DateTime? PublicationDate { get; set; }
        public BookCondition BookCondition { get; set; }
        public string CoverImagePath { get; set; }
        public List<string> AdditionalImagePaths { get; set; } = new();
        public TransactionType TransactionType { get; set; }
        public string Category { get; set; }
        public decimal? RentalPrice { get; set; }
    }
}

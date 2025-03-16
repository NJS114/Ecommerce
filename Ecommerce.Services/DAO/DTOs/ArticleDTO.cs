namespace Ecommerce.Services.DAO.DTOs
{
    public class ArticleDTO
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime PublishedDate { get; set; }
        public string ImageUrl { get; set; }
        public string Category { get; set; }
    }
}

using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.Models
{
    public class BasketItem
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
        public string? ImageUrl { get; set; }
        public BasketItemStatus Status { get; set; }
        public ProductAttributes? Attributes { get; set; }
    }
    public class ProductAttributes
    {
        public string Size { get; set; }
        public string Color { get; set; }
    }

}

namespace Ecommerce.Services.DAO.Models
{
    public class BaseOrder
    {
        public string UserId { get; set; }

        public virtual List<BasketItem> Items { get; set; } = new List<BasketItem>();
        public decimal TotalPrice { get; set; }

        
    }

}

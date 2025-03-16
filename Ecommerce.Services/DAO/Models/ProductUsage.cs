using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.Models
{
    public class ProductUsage
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public string Currency { get; set; }
        public string ChargeId { get; set; }
        public DateTime ChargeDate { get; set; }
    }
}
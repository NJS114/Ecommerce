using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.Models
{
    public class Order : BaseOrder
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ShippedAt { get; set; }

        public virtual User User { get; set; }
    }
}

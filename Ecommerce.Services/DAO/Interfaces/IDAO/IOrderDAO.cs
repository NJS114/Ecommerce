using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IOrderDAO
    {
        Task<Order> CreateOrderFromBasket(Basket basket);
        Task<Order> GetOrderById(string orderId);
        Task<List<Order>> GetOrdersByUserId(string userId);
        Task<Order> UpdateOrderStatus(string orderId, OrderStatus newStatus);
        Task<Order> DeleteOrder(string orderId);
        Task<List<Order>> GetAllOrdersInPeriod(DateTime startDate, DateTime endDate);
    }
}

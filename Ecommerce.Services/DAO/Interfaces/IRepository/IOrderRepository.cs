using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Interfaces.IRepository
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderFromBasket(Basket basket);
        Task<Order> GetOrderById(string orderId);
        Task<List<Order>> GetOrdersByUserId(string userId);
        Task<Order> UpdateOrderStatus(string orderId, OrderStatus newStatus);
        Task<Order> DeleteOrder(string orderId);
        Task<List<Order>> GetAllOrders(DateTime startDate, DateTime endDate);

    }
}

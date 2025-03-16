using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly IOrderDAO _orderDAO;

        public OrderRepository(IOrderDAO orderDAO)
        {
            _orderDAO = orderDAO ?? throw new ArgumentNullException(nameof(orderDAO), "OrderDAO ne peut pas être nul.");
        }

        #region CRUD Methods
        public async Task<List<Order>> GetAllOrders(DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
            {
                throw new ArgumentException("La date de début ne peut pas être après la date de fin.");
            }

            var orders = await _orderDAO.GetAllOrdersInPeriod(startDate, endDate);
            return orders;
        }

        public async Task<Order> CreateOrderFromBasket(Basket basket)
        {
            if (basket == null)
            {
                throw new ArgumentNullException(nameof(basket), "Le panier ne peut pas être nul.");
            }

            return await _orderDAO.CreateOrderFromBasket(basket);
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ArgumentException("L'ID de la commande ne peut pas être nul ou vide.", nameof(orderId));
            }

            return await _orderDAO.GetOrderById(orderId);
        }

        public async Task<List<Order>> GetOrdersByUserId(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("L'ID de l'utilisateur ne peut pas être nul ou vide.", nameof(userId));
            }

            return await _orderDAO.GetOrdersByUserId(userId);
        }

        public async Task<Order> UpdateOrderStatus(string orderId, OrderStatus newStatus)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ArgumentException("L'ID de la commande ne peut pas être nul ou vide.", nameof(orderId));
            }

            return await _orderDAO.UpdateOrderStatus(orderId, newStatus);
        }

        public async Task<Order> DeleteOrder(string orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                throw new ArgumentException("L'ID de la commande ne peut pas être nul ou vide.", nameof(orderId));
            }

            return await _orderDAO.DeleteOrder(orderId);
        }
        #endregion
    }
}

using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IMongoDbConnection = Ecommerce.Services.DAO.Interfaces.IRepository.IMongoDbConnection;


namespace Ecommerce.Services.DAO.Implementations
{
    public class OrderDAO : IOrderDAO
    {
        private readonly IMongoCollection<Order> _orders;
        private readonly IMongoCollection<Basket> _baskets;

        public OrderDAO(IMongoDbConnection mongoConnection)
        {
            var database = mongoConnection.GetDatabase();
            _orders = database.GetCollection<Order>("Orders");
            _baskets = database.GetCollection<Basket>("Baskets");
        }

        #region CRUD Methods
        public async Task<List<Order>> GetAllOrdersInPeriod(DateTime startDate, DateTime endDate)
        {
            var filter = Builders<Order>.Filter.Gte(o => o.CreatedAt, startDate) &
                         Builders<Order>.Filter.Lte(o => o.CreatedAt, endDate);

            var orders = await _orders.Find(filter).ToListAsync();
            return orders;
        }

        public async Task<Order> CreateOrderFromBasket(Basket basket)
        {
            if (basket == null)
            {
                throw new ArgumentNullException(nameof(basket), "Le panier ne peut pas être nul.");
            }

            var order = new Order
            {
                UserId = basket.UserId,
                Items = basket.Items,
                TotalPrice = basket.TotalPrice,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _orders.InsertOneAsync(order);
            await _baskets.DeleteOneAsync(b => b.UserId == basket.UserId);

            return order;
        }

        public async Task<Order> GetOrderById(string orderId)
        {
            var order = await _orders.Find(o => o.Id == orderId).FirstOrDefaultAsync();
            if (order == null)
            {
                throw new KeyNotFoundException($"Commande avec ID {orderId} introuvable.");
            }

            return order;
        }

        public async Task<List<Order>> GetOrdersByUserId(string userId)
        {
            var orders = await _orders.Find(o => o.UserId == userId).ToListAsync();
            return orders;
        }

        public async Task<Order> UpdateOrderStatus(string orderId, OrderStatus newStatus)
        {
            var update = Builders<Order>.Update.Set(o => o.Status, newStatus).Set(o => o.UpdatedAt, DateTime.UtcNow);
            var result = await _orders.UpdateOneAsync(o => o.Id == orderId, update);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Commande avec ID {orderId} introuvable.");
            }

            return await GetOrderById(orderId);
        }

        public async Task<Order> DeleteOrder(string orderId)
        {
            var order = await GetOrderById(orderId);
            await _orders.DeleteOneAsync(o => o.Id == orderId);
            return order;
        }

        #endregion
    }
}

using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Builders
{
    public class OrderBuilder
    {
        private string _userId;
        private List<BasketItem> _items;
        private decimal _totalPrice;
        private OrderStatus _status;
        private DateTime _createdAt;
        private DateTime? _updatedAt;

        public OrderBuilder SetUserId(string userId)
        {
            _userId = userId;
            return this;
        }

        public OrderBuilder SetItems(List<BasketItem> items)
        {
            _items = items ?? new List<BasketItem>();
            return this;
        }

        public OrderBuilder SetTotalPrice(decimal totalPrice)
        {
            _totalPrice = totalPrice;
            return this;
        }

        public OrderBuilder SetStatus(OrderStatus status)
        {
            _status = status;
            return this;
        }

        public OrderBuilder SetCreatedAt(DateTime createdAt)
        {
            _createdAt = createdAt;
            return this;
        }

        public OrderBuilder SetUpdatedAt(DateTime? updatedAt)
        {
            _updatedAt = updatedAt;
            return this;
        }

        public Order Build()
        {
            return new Order
            {
                UserId = _userId,
                Items = _items ?? new List<BasketItem>(),
                TotalPrice = _totalPrice,
                Status = _status,
                CreatedAt = _createdAt,
                UpdatedAt = _updatedAt
            };
        }
    }
}

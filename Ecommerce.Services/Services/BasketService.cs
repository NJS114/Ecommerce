using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Interfaces;
using Ecommerce.Services.DAO.Models;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Interfaces.IServices;

namespace Ecommerce.Services.Services
{
    #region BasketService
    public class BasketService : IBasketService
    {
        private readonly IMongoCollection<Basket> _basketCollection;
        private readonly Dictionary<string, Basket> _baskets;

        public BasketService(IMongoDbConnection mongoConnection)
        {
            _basketCollection = mongoConnection.GetDatabase().GetCollection<Basket>("Baskets");
            _baskets = new Dictionary<string, Basket>();
        }
        #endregion

        #region Methods

        public async Task<Basket> GetBasketByUserIdAsync(string userId)
        {
            var basket = await _basketCollection.Find(b => b.UserId == userId).FirstOrDefaultAsync();
            return basket ?? new Basket { UserId = userId };
        }

        public async Task AddToBasketAsync(string userId, BasketItem basketItem )
        {
            var basket = await GetBasketByUserIdAsync(userId);
            basket.Items.Add(basketItem);
            basket.TotalPrice = CalculateTotalPrice(basket); // Recalculer le prix total
            await _basketCollection.ReplaceOneAsync(b => b.UserId == userId, basket, new ReplaceOptions { IsUpsert = true });
        }

        public async Task RemoveFromBasketAsync(string userId, string bookId)
        {
            var basket = await GetBasketByUserIdAsync(userId);
            basket.Items.RemoveAll(item => item.ProductId == bookId);
            basket.TotalPrice = CalculateTotalPrice(basket); // Recalculer le prix total
            await _basketCollection.ReplaceOneAsync(b => b.UserId == userId, basket);
        }

        public async Task UpdateBasketItemQuantityAsync(string userId, string productId, int quantity)
        {
            if (quantity <= 0)
            {
                throw new ArgumentException("La quantité doit être supérieure à zéro.");
            }

            var basket = await GetBasketByUserIdAsync(userId);
            if (basket == null)
            {
                throw new KeyNotFoundException("Panier non trouvé pour cet utilisateur.");
            }

            var item = basket.Items.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                throw new KeyNotFoundException("Article non trouvé dans le panier.");
            }

            // Mettre à jour la quantité
            item.Quantity = quantity;

            // Recalculer le prix total
            basket.TotalPrice = CalculateTotalPrice(basket);

            // Sauvegarder le panier
            await SaveBasketAsync(basket);
        }


        public async Task EmptyBasketAsync(string userId)
        {
            var basket = await GetBasketByUserIdAsync(userId);
            basket.Items.Clear();
            basket.TotalPrice = 0; // Réinitialiser le prix total
            await _basketCollection.ReplaceOneAsync(b => b.UserId == userId, basket);
        }

        public async Task ConfirmPurchaseAsync(string userId)
        {
            var basket = await GetBasketByUserIdAsync(userId);
            foreach (var item in basket.Items)
            {
                item.Status = BasketItemStatus.Purchased;
            }

            decimal totalPrice = CalculateTotalPrice(basket); // Calculer le total avant confirmation
            basket.TotalPrice = totalPrice;

            await _basketCollection.ReplaceOneAsync(b => b.UserId == userId, basket);
        }

        public async Task UpdateItemStatusAsync(string userId, string bookId, BasketItemStatus newStatus)
        {
            var basket = await GetBasketByUserIdAsync(userId);
            var item = basket.Items.FirstOrDefault(i => i.ProductId == bookId);
            if (item != null)
            {
                item.Status = newStatus;
                await _basketCollection.ReplaceOneAsync(b => b.UserId == userId, basket);
            }
        }

        public async Task ApplyDiscountAfterPurchaseAsync(string userId)
        {
            var basket = await GetBasketByUserIdAsync(userId);
            decimal totalPrice = CalculateTotalPrice(basket);

            decimal discount = totalPrice * 0.10m;
            totalPrice -= discount;

            basket.TotalPrice = totalPrice;

            await _basketCollection.ReplaceOneAsync(b => b.UserId == userId, basket);
        }

        public async Task SaveBasketAsync(Basket basket)
        {
            if (basket == null) throw new ArgumentNullException(nameof(basket));
            _baskets[basket.UserId] = basket;
            await Task.CompletedTask;
        }

        public decimal CalculateTotalPrice(Basket basket)
        {
            if (basket == null || basket.Items == null || !basket.Items.Any())
            {
                throw new ArgumentException("Le panier est vide ou invalide.");
            }
            return basket.Items.Sum(item => item.Price * item.Quantity);
        }

        #endregion
    }
}

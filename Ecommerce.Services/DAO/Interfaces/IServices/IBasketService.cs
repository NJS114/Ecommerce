using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Interfaces.IServices
{
    public interface IBasketService
    {
        Task<Basket> GetBasketByUserIdAsync(string userId);
        Task AddToBasketAsync(string userId, BasketItem basketItem);
        Task RemoveFromBasketAsync(string userId, string bookId);
        Task UpdateBasketItemQuantityAsync(string userId, string bookId, int quantity);
        Task EmptyBasketAsync(string userId);
        Task ConfirmPurchaseAsync(string userId);
        Task UpdateItemStatusAsync(string userId, string bookId, BasketItemStatus newStatus);
        Task ApplyDiscountAfterPurchaseAsync(string userId);
        Task SaveBasketAsync(Basket basket);
    }

}

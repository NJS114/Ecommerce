using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Interfaces.IServices
{
    public interface ISubscriptionService
    {
        Task<PaymentResponse> CreateSubscriptionAsync(string userId, string priceId, string paymentMethodId);
        Task<PaymentResponse> CancelSubscriptionAsync(string subscriptionId);
        Task<PaymentResponse> UpdateSubscriptionPlanAsync(string subscriptionId, string priceId);
    }
}

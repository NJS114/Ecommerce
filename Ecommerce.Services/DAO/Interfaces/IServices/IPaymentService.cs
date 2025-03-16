using Ecommerce.Services.DAO.Models;
using Ecommerce.Services.DAO.Enums;
using System.Threading.Tasks;
using Stripe;
using PaymentMethod = Ecommerce.Services.DAO.Models.PaymentMethod;
using Stripe.Issuing;
using Order = Stripe.Climate.Order;

namespace Ecommerce.Services.DAO.Interfaces.IServices
{
    public interface IPaymentService
    {
        Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest);
        Task<PaymentResponse> ProcessStripePaymentAsync(PaymentRequest paymentRequest);
        Task<PaymentResponse> ProcessCardPaymentAsync(PaymentRequest paymentRequest);
        Task<List<CustomerProductUsage>> GetCustomersByProductAsync(string productId);
        Task<PaymentResponse> ProcessPayPalPaymentAsync(PaymentRequest paymentRequest);
        Task<PaymentResponse> ProcessBankTransferAsync(PaymentRequest paymentRequest);
        Task<List<ProductDetails>> GetAllProductsAsync();
        Task<List<ProductUsage>> GetProductsByCustomerAsync(string customerId);
        Task<PaymentResponse> CreateSubscriptionAsync(string userId, decimal amount, string currency);
        Task<IEnumerable<Payout>> GetAllPayoutsAsync();
        Task SendConfirmationEmailAsync(PaymentResponse paymentResponse, string userEmail);
        Task<string> GenerateToken(PaymentMethod paymentMethod);
        //Task<IEnumerable<Transaction>> GetAllTransactionsAsync();
        Task<PaymentResponse> CreateCheckoutSessionAsync(PaymentRequest paymentRequest);
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId);
        Task<IEnumerable<Payment>> GetAllTransactionsWithDetailsAsync();
        Task<List<Payment>> GetAllPaymentsAsync();
        Task<CustomerDetails> GetCustomerDetailsAsync(string customerId);
        Task<ChargeDetails> GetChargeDetailsAsync(string paymentIntentId);
        Task<string> RefundPaymentAsync(string paymentId);
    }
}

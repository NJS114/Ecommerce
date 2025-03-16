using Ecommerce.Services.DAO.Enums;
using Stripe;

namespace Ecommerce.Services.DAO.Models
{
    public class AddCardRequest
    {
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string CVV { get; set; }
        public string CardHolderName { get; set; }
        public Card Card { get; set; }  
        public string CustomerId { get; set; }
    }

    public class RefundRequest
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
    }

    public class InvoiceRequest
    {
        public string CustomerId { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
    }

    public class SubscriptionRequest
    {
        public string CustomerId { get; set; }
        public string PriceId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class  PaymentRequest 
    {
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; } = Currency.eur;
        public string PaymentMethodId { get; set; }
        public string Description { get; set; }
        public string Token { get; set; }
    }
}

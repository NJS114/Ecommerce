using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.Models
{
    public class CustomerDetails
    {
        public string CustomerId { get; set; }
        public string Email { get; set; }
        public List<CustomerPayment> Payments { get; set; }
    }

    public class CustomerPayment
    {
        public string PaymentId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
        public string Description { get; set; }
        public string ReceiptUrl { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
    }
    public class CustomerProductUsage
    {
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal AmountPaid { get; set; }
        public string Currency { get; set; }
        public string InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
    }

    public class ProductDetails
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> Images { get; set; } // Liste des URL des images
        public Dictionary<string, string> Metadata { get; set; } // Métadonnées personnalisées
    }

    public class Payment
    {
        public string Id { get; set; } // Payment ID (e.g., pi_...)
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
        public string Status { get; set; }
        public string CustomerEmail { get; set; }
        public string Description { get; set; }
        public string CustomerAddress { get; set; } // Nouveau champ
        public string ProductId { get; set; } // Nouveau champ
        public string ProductName { get; set; }
        public string Last4CardDigits { get; set; }
        public string Nickname { get; set; }
        public ChargeDetails ChargeDetails { get; set; }
    }
    public class ChargeDetails
    {
        public string PaymentId { get; set; }
        public string PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string Status { get; set; }
        public string CustomerId { get; set; }
        public string Description { get; set; }
        public string RiskLevel { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Email { get; set; }
        public string ReceiptUrl { get; set; }
        public object ProductDetails { get; set; }
    }


    public class TransactionDetails
    {
        public string TransactionId { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
        public string CustomerId { get; set; }
        public string ProductIds { get; set; }
    }

    public class BankTransferRequest
    {
        public string MerchantId { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
    }
}

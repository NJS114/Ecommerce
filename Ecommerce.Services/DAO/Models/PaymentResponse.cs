using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.Models
{
    public class PaymentResponse
    {
        public PaymentStatus Status;
        public string ClientSecret { get; set; }
        public bool Success { get; set; }
        public string TransactionId { get; set; }
        public string ErrorMessage { get; set; }
        public string Url {get; set; }
    }
}

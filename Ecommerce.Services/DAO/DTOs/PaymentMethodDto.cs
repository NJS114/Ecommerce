using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.DTOs
{
    public class PaymentMethodDto
    {
        public PaymentMethod PaymentMethod { get; set; }
        public string CardNumber { get; set; }
        public int ExpMonth { get; set; }
        public int ExpYear { get; set; }
        public string Cvc { get; set; }
    }

}

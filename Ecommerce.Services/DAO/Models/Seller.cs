using Stripe.Issuing;

namespace Ecommerce.Services.DAO.Models
{
    public class Seller : User
    {
        public List<Transaction> Transactions { get; set; } 
        public string StripeAccountId { get; set; }  
        public string BankAccountDetails { get; set; }  
    }
}

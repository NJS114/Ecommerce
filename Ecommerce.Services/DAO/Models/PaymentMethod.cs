using Ecommerce.Services.DAO.Enums;
using Microsoft.AspNetCore.Http;
using Stripe;

namespace Ecommerce.Services.DAO.Models
{
    public class PaymentMethod
    {
        public string Id { get; set; }
        public string Type { get; set; } 
        public Card Card { get; set; } 
        public decimal Amount { get; set; }
        public Currency Currency { get; set; } = Currency.eur;
    }
    public class Card
    {
        public string Number { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Cvc { get; set; }
        public string Name { get; set; }
        public BillingAddress BillingAddress { get; set; }
    }
    public class BillingAddress
    {
        public StripeResponse StripeResponse { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }
    }
    public class StripeResponse
    {
        public List<Header> Headers { get; set; }
        public string Content { get; set; }
    }

    public class Header
    {
        public string Key { get; set; }
        public List<string> Value { get; set; }
    }
}

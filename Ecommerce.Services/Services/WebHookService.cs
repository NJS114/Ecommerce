using Ecommerce.Services.DAO.Interfaces.IRepository;
using Stripe;

namespace Ecommerce.Services.Services
{
    public class WebhookService
    {
        private readonly StripeClient _stripeClient;
        private readonly IOrderRepository _orderRepository;

        public WebhookService(StripeClient stripeClient, IOrderRepository orderRepository)
        {
            _stripeClient = stripeClient;
            _orderRepository = orderRepository;
        }

        public async Task HandleStripeWebhookAsync(HttpRequest request)
        {
            string json = await new StreamReader(request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, request.Headers["Stripe-Signature"], "<Your Stripe Webhook Secret>");

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    // Mettre à jour la commande ici
                    break;
                case "payment_intent.failed":
                    // Gérer l'échec du paiement
                    break;
                default:
                    // Autres types d'événements à gérer
                    break;
            }
        }
    }
}

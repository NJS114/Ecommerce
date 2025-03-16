using Microsoft.AspNetCore.Mvc;
using Ecommerce.Services.DAO.Models;
using Ecommerce.Services.DAO.Interfaces.IServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Ecommerce.Services.DAO.Enums;
using Microsoft.AspNetCore.Authorization;
using Stripe;
using PaymentMethod = Ecommerce.Services.DAO.Models.PaymentMethod;
using Stripe.Checkout;
using Ecommerce.Services.DAO.Interfaces.IRepository;

namespace Ecommerce.Services.Controllers
{
    public class CartRequest
    {
        public string CustomerId { get; set; }
        public List<CartItem> CartItems { get; set; }
    }

    public class CartItem
    {
        public string PriceId { get; set; }  // L'ID du prix Stripe
        public int Quantity { get; set; }    // Quantité de l'article
    }

    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userService;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IEmailService emailService, IUserRepository userService)
        {
            _paymentService = paymentService;
            _logger = logger;
            _emailService = emailService;
            _userService = userService;
        }

        [HttpPost("session")]
        public async Task<IActionResult> CreateCheckoutSession([FromBody] CartRequest cartRequest)
        {
            var paymentResponse = new PaymentResponse();

            try
            {
                var domain = "http://localhost:3000/cart/";
                var cartItems = cartRequest.CartItems;

                if (cartItems == null || cartItems.Count == 0)
                {
                    paymentResponse.Status = PaymentStatus.Failed;
                    paymentResponse.ErrorMessage = "Le panier est vide.";
                    return BadRequest(paymentResponse);
                }

                var lineItems = new List<SessionLineItemOptions>();
                foreach (var item in cartItems)
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        Price = item.PriceId,
                        Quantity = item.Quantity
                    });
                }

                string userId = cartRequest.CustomerId;
                string customerId;

                // Récupérer le client Stripe associé à l'utilisateur
                var user = await _userService.GetUserByIdAsync(userId); // Exemple de méthode pour récupérer un utilisateur
                if (user == null)
                {
                    paymentResponse.Status = PaymentStatus.Failed;
                    paymentResponse.ErrorMessage = "Utilisateur introuvable.";
                    return BadRequest(paymentResponse);
                }

                customerId = user.StripeAccountId;

                if (string.IsNullOrEmpty(customerId))
                {
                    // Créer un nouveau client Stripe si aucun CustomerId n'existe
                    var customerOptions = new CustomerCreateOptions
                    {
                        Email = user.Email,
                        Name = $"{user.FirstName} {user.LastName}"
                    };

                    var customerService = new CustomerService();
                    var customer = await customerService.CreateAsync(customerOptions);

                    user.StripeAccountId = customer.Id;
                    var id = user.Id;
                    await _userService.UpdateUserDTOAsync(id, user);

                    customerId = customer.Id;
                }

                var options = new SessionCreateOptions
                {
                    LineItems = lineItems,
                    PaymentMethodTypes = new List<string> { "card" },
                    Mode = "payment",
                    SuccessUrl = $"{domain}/success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{domain}/cancel",
                    Customer = customerId,
                    Metadata = new Dictionary<string, string>
            {
                { "userId", userId }
            }
                };

                var sessionService = new SessionService();
                var session = await sessionService.CreateAsync(options);

                // Ajouter la création et finalisation de la facture
                var invoiceOptions = new InvoiceCreateOptions
                {
                    Customer = customerId,  // ID du client Stripe
                    AutoAdvance = true,
                    CollectionMethod = "send_invoice",
                    DaysUntilDue = 30
                };

                var invoiceService = new InvoiceService();
                var invoice = await invoiceService.CreateAsync(invoiceOptions);

                // Finaliser la facture
                await invoiceService.FinalizeInvoiceAsync(invoice.Id);
                await invoiceService.SendInvoiceAsync(invoice.Id);


                paymentResponse.Url = session.Url;
                paymentResponse.TransactionId = session.Id;
                paymentResponse.Status = PaymentStatus.Completed;

                return Ok(paymentResponse);
            }
            catch (StripeException stripeEx)
            {
                paymentResponse.Status = PaymentStatus.Failed;
                paymentResponse.ErrorMessage = $"Erreur Stripe: {stripeEx.Message}";
                return BadRequest(paymentResponse);
            }
            catch (Exception ex)
            {
                paymentResponse.Status = PaymentStatus.Failed;
                paymentResponse.ErrorMessage = $"Erreur lors de la création de la session de paiement : {ex.Message}";
                return BadRequest(paymentResponse);
            }
        }


        // POST: Create Invoice
        [HttpPost("create-invoice")]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceRequest invoiceRequest)
        {
            if (string.IsNullOrEmpty(invoiceRequest.CustomerId))
                return BadRequest("CustomerId requis.");

            try
            {
                var invoiceOptions = new InvoiceCreateOptions
                {
                    Customer = invoiceRequest.CustomerId,
                    AutoAdvance = true,
                };

                var invoiceService = new InvoiceService();
                var invoice = await invoiceService.CreateAsync(invoiceOptions);

                return Ok(new { InvoiceId = invoice.Id, Status = invoice.Status });
            }
            catch (StripeException ex)
            {
                _logger.LogError($"Erreur Stripe: {ex.Message}");
                return BadRequest(new { message = "Échec de la création de la facture", error = ex.Message });
            }
        }

        // POST: Create Subscription
        [HttpPost("create-subscription")]
        public async Task<IActionResult> CreateSubscription([FromBody] SubscriptionRequest subscriptionRequest)
        {
            if (string.IsNullOrEmpty(subscriptionRequest.CustomerId) || string.IsNullOrEmpty(subscriptionRequest.PriceId))
                return BadRequest("Données invalides.");

            try
            {
                var options = new SubscriptionCreateOptions
                {
                    Customer = subscriptionRequest.CustomerId,
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = subscriptionRequest.PriceId
                        }
                    }
                };

                var subscriptionService = new SubscriptionService();
                var subscription = await subscriptionService.CreateAsync(options);

                return Ok(new { SubscriptionId = subscription.Id, Status = subscription.Status });
            }
            catch (StripeException ex)
            {
                _logger.LogError($"Erreur Stripe: {ex.Message}");
                return BadRequest(new { message = "Échec de la création de l'abonnement", error = ex.Message });
            }
        }

        // POST: Add Card to Customer
        [HttpPost("add-card")]
        public async Task<IActionResult> AddCardToCustomer([FromBody] AddCardRequest cardRequest)
        {
            if (string.IsNullOrEmpty(cardRequest.CustomerId) || cardRequest.Card == null)
                return BadRequest("Données invalides.");

            try
            {
                var cardOptions = new PaymentMethodCreateOptions
                {
                    Type = "card",
                    Card = new PaymentMethodCardOptions
                    {
                        Number = cardRequest.Card.Number,
                        ExpMonth = long.TryParse(cardRequest.Card.ExpMonth, out var expMonth) ? (long?)expMonth : null,
                        ExpYear = long.TryParse(cardRequest.Card.ExpYear, out var expYear) ? (long?)expYear : null,
                        Cvc = cardRequest.Card.Cvc,
                    },
                };

                var paymentMethodService = new PaymentMethodService();
                var paymentMethod = await paymentMethodService.CreateAsync(cardOptions);

                // Attacher la carte au client
                await paymentMethodService.AttachAsync(paymentMethod.Id, new PaymentMethodAttachOptions
                {
                    Customer = cardRequest.CustomerId
                });

                return Ok(new { CardId = paymentMethod.Id });
            }
            catch (StripeException ex)
            {
                _logger.LogError($"Erreur Stripe: {ex.Message}");
                return BadRequest(new { message = "Échec de l'ajout de la carte", error = ex.Message });
            }
        }
        [HttpGet("all-products")]
        public async Task<IActionResult> GetAllProducts()
        {
            try
            {
                var products = await _paymentService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur : {ex.Message}");
            }
        }
        [HttpGet("customers/{customerId}/products")]
        public async Task<IActionResult> GetCustomerProducts(string customerId)
        {
            var productUsageList = await _paymentService.GetProductsByCustomerAsync(customerId);

            if (productUsageList == null || !productUsageList.Any())
                return NotFound("Aucun produit trouvé pour ce client.");

            return Ok(productUsageList);
        }

        [HttpGet("customers-by-product/{productId}")]
        public async Task<IActionResult> GetCustomersByProduct(string productId)
        {
            try
            {
                var customers = await _paymentService.GetCustomersByProductAsync(productId);
                return Ok(customers);
            }
            catch (Exception ex)
            {
                return BadRequest($"Erreur : {ex.Message}");
            }
        }


        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetCustomerDetails(string customerId)
        {
            var customerDetails = await _paymentService.GetCustomerDetailsAsync(customerId);
            return Ok(customerDetails);
        }

        [HttpGet("charge/{paymentIntentId}")]
        public async Task<IActionResult> GetChargeDetails(string paymentIntentId)
        {
            try
            {
                // Appel de la méthode GetChargeDetailsAsync
                var chargeDetails = await _paymentService.GetChargeDetailsAsync(paymentIntentId);

                // Retourne les détails de la charge sous forme de réponse JSON
                return Ok(chargeDetails);
            }
            catch (Stripe.StripeException ex)
            {
                // Gestion des exceptions liées à Stripe (ex: charge non trouvée)
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Gestion des autres exceptions
                return StatusCode(500, new { message = "Une erreur est survenue.", details = ex.Message });
            }
        }

        [HttpGet("payouts")]
        public async Task<IActionResult> GetAllPayouts()
        {
            try
            {
                // Appel au service pour récupérer les payouts
                var payouts = await _paymentService.GetAllPayoutsAsync();

                // Vérification si aucun payout n'a été trouvé
                if (payouts == null || !payouts.Any())
                {
                    return NotFound("Aucun payout trouvé.");
                }

                return Ok(payouts);
            }
            catch (Exception ex)
            {
                // Journalisation de l'erreur
                _logger.LogError(ex, "Erreur lors de la récupération des payouts : {Message}", ex.Message);
                return StatusCode(500, "Une erreur est survenue lors de la récupération des payouts.");
            }
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetAllPayments()
        {
            var payments = await _paymentService.GetAllPaymentsAsync();
            return Ok(payments);
        }

        [HttpGet("payment-status/{paymentIntentId}")]
        public async Task<IActionResult> GetPaymentStatus(string paymentIntentId)
        {
            try
            {
                var paymentIntentService = new PaymentIntentService();
                var paymentIntent = await paymentIntentService.GetAsync(paymentIntentId);

                return Ok(new
                {
                    Status = paymentIntent.Status,
                    Amount = paymentIntent.Amount,
                    Currency = paymentIntent.Currency,
                    Created = paymentIntent.Created
                });
            }
            catch (StripeException ex)
            {
                _logger.LogError($"Erreur Stripe: {ex.Message}");
                return BadRequest(new { message = "Échec de la récupération du statut du paiement", error = ex.Message });
            }
        }
    }
}

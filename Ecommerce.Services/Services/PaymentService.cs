using Ecommerce.Services.DAO.Models;
using Ecommerce.Services.DAO.Enums;
using Microsoft.Extensions.Logging;
using Stripe;
using Stripe.Checkout;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Generic;
using Nest;
using PaymentMethod = Ecommerce.Services.DAO.Models.PaymentMethod;
using Stripe.Issuing;
using Stripe.Climate;
using Order = Stripe.Climate.Order;
using Microsoft.AspNetCore.Mvc;


namespace Ecommerce.Services
{
    public class Transaction
    {
        public string Id { get; set; }
        public long Amount { get; set; }
        public string Currency { get; set; }
        public DateTime Created { get; set; }
        public string CustomerId { get; set; } // Propriété pour le CustomerId
    }
    public class PaymentService : IPaymentService
    {
        private readonly StripeClient _stripeClient;
        private readonly IEmailService _emailService;
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<PaymentService> _logger;


        public PaymentService(
            IOptions<StripeSettings> stripeOptions,
            IEmailService emailService,
            IDistributedCache cache,
            IUserRepository userRepository,
            ILogger<PaymentService> logger)
        {
            var stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
            _stripeClient = new StripeClient(stripeSecretKey);
            _emailService = emailService;
            _cache = cache;
            _userRepository = userRepository;
            _logger = logger;

            _logger.LogInformation("PaymentService initialized with Stripe API.");
        }
        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId))
            {
                _logger.LogWarning("Customer ID is null or empty.");
                throw new ArgumentException("Customer ID must be provided.", nameof(customerId));
            }

            _logger.LogInformation("Fetching orders for customer ID: {CustomerId}", customerId);

            try
            {
                var service = new PaymentIntentService(_stripeClient);
                var options = new PaymentIntentListOptions
                {
                    Customer = customerId,
                    Limit = 100 // Limiter les résultats pour éviter de surcharger l'appel.
                };

                var paymentIntents = await service.ListAsync(options);

                if (paymentIntents.Data == null || !paymentIntents.Data.Any())
                {
                    _logger.LogWarning("No orders found for customer ID: {CustomerId}", customerId);
                    return Enumerable.Empty<Order>();
                }

                var orders = paymentIntents.Data.Select(paymentIntent => new Order
                {
                    Id = paymentIntent.Id,
                    AmountFees = paymentIntent.ApplicationFeeAmount ?? 0, // Utilisation de l'application fee amount (ajustez selon la logique)
                    AmountSubtotal = paymentIntent.Amount, // Exemple d'utilisation de Amount, ajustez en fonction de la logique de votre application
                    AmountTotal = paymentIntent.AmountReceived, // Ajustez si nécessaire pour représenter le total
                    Currency = paymentIntent.Currency,
                    CanceledAt = paymentIntent.CanceledAt,
                    CancellationReason = paymentIntent.CancellationReason,
                    Certificate = null, // Cette propriété semble ne pas exister dans PaymentIntent, donc vous pouvez la laisser vide ou l'adapter
                    ConfirmedAt = null, // Ajouter une logique spécifique si cette information est nécessaire
                    Created = paymentIntent.Created,
                    DelayedAt = null, // Cette propriété n'existe pas dans PaymentIntent, laissez-la vide ou ajoutez une logique
                    DeliveredAt = null, // Comme ci-dessus
                    DeliveryDetails = null, // Comme ci-dessus
                    Livemode = paymentIntent.Livemode,
                    Metadata = paymentIntent.Metadata,
                    Product = null, // Assurez-vous que vous avez une logique pour cette propriété
                    ProductSubstitutedAt = null, // Comme ci-dessus
                    Status = paymentIntent.Status // Assurez-vous que 'Status' correspond à l'état du PaymentIntent
                }).ToList();


                _logger.LogInformation("{OrderCount} orders found for customer ID: {CustomerId}", orders.Count(), customerId);
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders for customer ID: {CustomerId}", customerId);
                throw;
            }
        }
        public async Task<IEnumerable<Payout>> GetAllPayoutsAsync()
        {
            _logger.LogInformation("Fetching all payouts from Stripe.");

            try
            {
                // Initialisation du service Stripe pour les virements
                var service = new PayoutService(_stripeClient);

                // Options pour la récupération des virements
                var options = new PayoutListOptions
                {
                    Limit = 100 // Ajustez selon vos besoins
                };

                // Récupération des virements
                var payoutsList = await service.ListAsync(options);

                // Vérifie si aucune donnée n'est trouvée
                if (payoutsList.Data == null || !payoutsList.Data.Any())
                {
                    _logger.LogWarning("No payouts found.");
                    return Enumerable.Empty<Payout>();
                }

                _logger.LogInformation("{PayoutCount} payouts found.", payoutsList.Data.Count);
                return payoutsList.Data;
            }
            catch (StripeException stripeEx)
            {
                _logger.LogError(stripeEx, "Stripe API error occurred: {Message}", stripeEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching payouts.");
                throw;
            }
        }
         [HttpGet("charges")]
        public async Task<List<Session>> GetAllSessionsWithChargesAsync()
        {
            _logger.LogInformation("Fetching all checkout sessions from Stripe.");

            try
            {
                // Initialisation du service Stripe pour les sessions de paiement
                var sessionService = new SessionService(_stripeClient);

                // Options pour la récupération des sessions
                var options = new SessionListOptions
                {
                    Limit = 100 // Ajustez selon vos besoins
                };

                // Récupération des sessions
                var sessionsList = await sessionService.ListAsync(options);

                // Vérifie si aucune donnée n'est trouvée
                if (sessionsList.Data == null || !sessionsList.Data.Any())
                {
                    _logger.LogWarning("No sessions found.");
                    return new List<Session>(); // Retourner une liste vide si aucune session n'est trouvée
                }

                _logger.LogInformation("{SessionCount} sessions found.", sessionsList.Data.Count);

                // Liste pour stocker les sessions avec les charges associées
                var sessionsWithCharges = new List<Session>();

                // Pour chaque session, récupérer la charge associée
                foreach (var session in sessionsList.Data)
                {
                    var paymentIntentId = session.PaymentIntentId;  // Récupère l'ID du PaymentIntent
                    if (!string.IsNullOrEmpty(paymentIntentId))
                    {
                        // Utiliser le service ChargeService pour récupérer les charges associées au PaymentIntent
                        var chargeService = new ChargeService(_stripeClient);
                        var charges = await chargeService.ListAsync(new ChargeListOptions
                        {
                            PaymentIntent = paymentIntentId, // Spécifier le PaymentIntentId
                            Limit = 1 // Vous pouvez ajuster selon vos besoins
                        });

                        // Vérifie s'il y a des charges et les affiche dans les logs
                        if (charges.Data.Any())
                        {
                            var charge = charges.Data.First();
                            _logger.LogInformation("Charge ID: {ChargeId}, Amount: {Amount}, Status: {Status}",
                                charge.Id, charge.Amount, charge.Status);
                        }
                        else
                        {
                            _logger.LogWarning("No charges found for PaymentIntent ID: {PaymentIntentId}", paymentIntentId);
                        }
                    }

                    // Ajouter la session à la liste
                    sessionsWithCharges.Add(session);
                }

                return sessionsWithCharges; // Retourner la liste des sessions récupérées
            }
            catch (StripeException stripeEx)
            {
                _logger.LogError(stripeEx, "Stripe API error occurred: {Message}", stripeEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching sessions.");
                throw;
            }
        }

        public async Task<List<CustomerProductUsage>> GetCustomersByProductAsync(string productId)
        {
            try
            {
                var invoiceService = new Stripe.InvoiceService(_stripeClient);
                var customerService = new Stripe.CustomerService(_stripeClient);

                var customerProductUsageList = new List<CustomerProductUsage>();

                // Récupérer toutes les factures
                var invoices = await invoiceService.ListAsync(new Stripe.InvoiceListOptions { Limit = 100 });

                if (invoices.Data.Count == 0)
                {
                    Console.WriteLine("Aucune facture trouvée.");
                }

                foreach (var invoice in invoices.Data)
                {
                    Console.WriteLine($"Traitement de la facture : {invoice.Id}");

                    // Parcourir les lignes de la facture
                    foreach (var line in invoice.Lines.Data)
                    {
                        Console.WriteLine($"Traitement de la ligne de facture : {line.Id}, Produit ID : {line.Price?.ProductId}");

                        // Vérifier si le produit correspond
                        if (line.Price?.ProductId == productId)
                        {
                            Console.WriteLine($"Produit trouvé : {line.Price.ProductId}");

                            // Récupérer les informations du client
                            var customer = await customerService.GetAsync(invoice.CustomerId);

                            customerProductUsageList.Add(new CustomerProductUsage
                            {
                                CustomerId = customer.Id,
                                CustomerEmail = customer.Email,
                                ProductId = productId,
                                ProductName = line.Description,
                                AmountPaid = line.Amount / 100.0m,
                                Currency = line.Currency,
                                InvoiceId = invoice.Id,
                                InvoiceDate = invoice.Created
                            });

                            break; // Passer à la prochaine facture dès que le produit est trouvé
                        }
                    }
                }

                Console.WriteLine($"Total des clients trouvés : {customerProductUsageList.Count}");
                return customerProductUsageList;
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Erreur Stripe : {ex.Message}");
                throw new Exception($"Erreur Stripe : {ex.Message}");
            }
        }
        public async Task<List<ProductUsage>> GetProductsByCustomerAsync(string customerId)
        {
            try
            {
                var chargeService = new ChargeService(_stripeClient); // ChargeService
                var productService = new Stripe.ProductService(_stripeClient);
                var invoiceService = new InvoiceService(_stripeClient); // InvoiceService pour récupérer les invoices

                var productUsageList = new List<ProductUsage>();

                // Liste des charges
                var charges = await chargeService.ListAsync(new ChargeListOptions
                {
                    Customer = customerId,
                    Limit = 20,
                });

                if (charges.Data == null || !charges.Data.Any())
                {
                    Console.WriteLine("Aucune charge trouvée pour ce client.");
                    return productUsageList;
                }

                foreach (var charge in charges.Data)
                {
                    // Récupérer l'Invoice liée à cette charge
                    var invoice = await invoiceService.GetAsync(charge.InvoiceId);

                    if (invoice?.Lines?.Data == null || !invoice.Lines.Data.Any())
                    {
                        Console.WriteLine($"La charge {charge.Id} ne contient aucune ligne.");
                        continue;
                    }

                    foreach (var line in invoice.Lines.Data)
                    {
                        if (line.Price?.ProductId == null)
                        {
                            Console.WriteLine($"Ligne sans produit associé : {line.Id}");
                            continue;
                        }

                        // Récupérer le produit
                        var product = await productService.GetAsync(line.Price.ProductId);

                        productUsageList.Add(new ProductUsage
                        {
                            ProductId = product.Id,
                            ProductName = product.Name,
                            Price = line.Price.UnitAmount / 100.0m,
                            Currency = line.Price.Currency,
                            ChargeId = charge.Id,
                            ChargeDate = charge.Created
                        });
                    }
                }

                return productUsageList;
            }
            catch (StripeException ex)
            {
                Console.WriteLine($"Erreur Stripe : {ex.Message}, Requête ID : {ex.StripeError?.RequestLogUrl}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Une erreur est survenue : {ex.Message}");
                throw;
            }
        }






        public async Task<List<ProductDetails>> GetAllProductsAsync()
        {
            try
            {
                var productService = new Stripe.ProductService(_stripeClient);

                // Récupérer tous les produits depuis Stripe
                var products = await productService.ListAsync(new Stripe.ProductListOptions { Limit = 100 });

                // Convertir les produits en une liste d'objets ProductDetails
                var productDetailsList = products.Select(product => new ProductDetails
                {
                    ProductId = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    Images = product.Images, // Liste d'URL d'images
                    Metadata = product.Metadata // Métadonnées personnalisées
                }).ToList();

                return productDetailsList;
            }
            catch (StripeException ex)
            {
                throw new Exception($"Erreur lors de la récupération des produits : {ex.Message}");
            }
        }

        public async Task<dynamic> GetProductDetailsAsync(string productId)
        {
            try
            {
                // Initialisation du service Stripe
                var productService = new Stripe.ProductService(_stripeClient);

                // Récupération des informations du produit
                var product = await productService.GetAsync(productId);

                // Extraction des informations
                var productDetails = new
                {
                    Name = product.Name,
                    Description = product.Description,
                    Images = product.Images, // Liste d'URL d'images
                    Metadata = product.Metadata // Métadonnées personnalisées
                };

                return productDetails;
            }
            catch (StripeException ex)
            {
                throw new Exception($"Erreur lors de la récupération du produit : {ex.Message}");
            }
        }

        public async Task<ChargeDetails> GetChargeDetailsAsync(string paymentIntentId)
        {
            try
            {
                // Récupération de la charge via Stripe
                var chargeService = new ChargeService(_stripeClient);
                var charge = await chargeService.GetAsync(paymentIntentId);

                // Initialisation des détails du produit
                dynamic productDetails = null;

                // Si un produit est associé, récupérer ses détails
                if (!string.IsNullOrEmpty(charge.InvoiceId))
                {
                    var invoiceService = new InvoiceService(_stripeClient);
                    var invoice = await invoiceService.GetAsync(charge.InvoiceId);

                    // Parcourir les lignes de la facture
                    foreach (var line in invoice.Lines.Data)
                    {
                        if (line.Price?.Product.ToString() is string productId)
                        {
                            // Récupérer les détails du produit
                            productDetails = await GetProductDetailsAsync(productId);
                            break; // Supposons qu'il y a un seul produit pertinent
                        }
                    }
                }

                // Construction des détails de la charge
                var chargeDetails = new ChargeDetails
                {
                    PaymentId = charge.Id,
                    PaymentMethod = charge.PaymentMethodDetails?.Card?.Last4,
                    Amount = charge.Amount / 100.0m,
                    Currency = charge.Currency,
                    Status = charge.Status,
                    CustomerId = charge.CustomerId,
                    Description = charge.Description,
                    Email = charge.ReceiptEmail,
                    ReceiptUrl = charge.ReceiptUrl,
                    ProductDetails = productDetails
                };

                return chargeDetails;
            }
            catch (Stripe.StripeException ex)
            {
                // Gestion des erreurs Stripe
                throw new Exception($"Erreur Stripe : {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                // Gestion des autres erreurs
                throw new Exception($"Une erreur est survenue : {ex.Message}", ex);
            }
        }


        // Récupérer les détails d'un client
        public async Task<CustomerDetails> GetCustomerDetailsAsync(string customerId)
        {
            var customerService = new CustomerService(_stripeClient);
            var chargeService = new ChargeService(_stripeClient);

            try
            {
                // Récupérer les informations du client
                var customer = await customerService.GetAsync(customerId);

                // Récupérer toutes les charges associées au client
                var chargesOptions = new ChargeListOptions
                {
                    Customer = customerId,
                    Limit = 100, // Limite des résultats (max 100 par appel)
                };

                var charges = await chargeService.ListAsync(chargesOptions);

                // Construire la liste des paiements
                var payments = charges.Data.Select(charge => new CustomerPayment
                {
                    PaymentId = charge.Id,
                    Amount = charge.Amount,
                    Currency = charge.Currency,
                    Created = charge.Created,
                    Description = charge.Description,
                    ReceiptUrl = charge.ReceiptUrl



                }).ToList();

                // Retourner les détails du client et ses paiements
                return new CustomerDetails
                {
                    CustomerId = customer.Id,
                    Email = customer.Email,
                    Payments = payments
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Une erreur Stripe est survenue : {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Une erreur est survenue lors de la récupération des détails du client.");
                throw;
            }
        }

        // Rembourser un paiement
        public async Task<string> RefundPaymentAsync(string paymentId)
        {
            var refundService = new RefundService(_stripeClient);
            var refund = await refundService.CreateAsync(new RefundCreateOptions
            {
                PaymentIntent = paymentId
            });

            return refund.Status;
        }

        public async Task<IEnumerable<Payment>> GetAllTransactionsWithDetailsAsync()
        {
            var paymentIntentService = new PaymentIntentService(_stripeClient);
            var chargeService = new ChargeService(_stripeClient);
            var invoiceService = new InvoiceService(_stripeClient);

            // Récupération des PaymentIntents avec une limite de 15
            var options = new PaymentIntentListOptions { Limit = 15 };
            var paymentIntents = await paymentIntentService.ListAsync(options);

            // Liste pour stocker les résultats finaux
            var payments = new List<Payment>();

            // Parcours des PaymentIntents récupérés
            foreach (var paymentIntent in paymentIntents.Data)
            {
                // Récupération des Charges associées au PaymentIntent
                var charges = await chargeService.ListAsync(new ChargeListOptions
                {
                    PaymentIntent = paymentIntent.Id
                });

                // Extraction de la première charge (s'il y en a une)
                var charge = charges.Data.FirstOrDefault();

                // Récupération de la facture (Invoice) associée à la charge
                var invoice = await invoiceService.GetAsync(charge?.InvoiceId);

                // Parcours des lignes de la facture pour récupérer les informations produit
                var productDetails = invoice.Lines.Data.Select(lineItem =>
                {
                    var productId = lineItem.Price.ProductId;
                    var productName = lineItem.Description; // Description du produit

                    return new
                    {
                        ProductId = productId,
                        ProductName = productName
                    };
                }).FirstOrDefault(); // On prend seulement le premier produit associé à la facture

                // Création d'un objet Payment avec les informations nécessaires
                var payment = new Payment
                {
                    Id = paymentIntent.Id,
                    Amount = paymentIntent.Amount / 100m, // Conversion du montant en devise (centimes -> unité)
                    Currency = paymentIntent.Currency.ToUpper(),
                    Created = paymentIntent.Created,
                    Status = paymentIntent.Status,
                    CustomerEmail = paymentIntent.ReceiptEmail,
                    Description = paymentIntent.Description,
                    Last4CardDigits = charge?.PaymentMethodDetails?.Card?.Last4, // Vérification null-safe
                    ProductId = productDetails?.ProductId,
                    ProductName = productDetails?.ProductName
                };

                // Ajout du paiement à la liste finale
                payments.Add(payment);

                // Affichage des informations (optionnel)
                Console.WriteLine($"PaymentIntent: {paymentIntent.Id}, ProductId: {productDetails?.ProductId}, ProductName: {productDetails?.ProductName}");
            }

            return payments;
        }






        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            _logger.LogInformation("Starting payment processing for userId: {UserId}, Amount: {Amount}, Method: {Method}",
                paymentRequest.UserId, paymentRequest.Amount);

            Console.WriteLine($"Processing payment for User ID: {paymentRequest.UserId}");
            PaymentResponse paymentResponse = new PaymentResponse();
            PaymentMethod paymentMethod = new PaymentMethod();

            try
            {
                switch (paymentMethod.Type?.ToLower())
                {
                    case "card":
                        _logger.LogInformation("Processing card payment...");
                        paymentResponse = await ProcessCardPaymentAsync(paymentRequest);
                        break;
                    case "paypal":
                        _logger.LogInformation("Processing PayPal payment...");
                        paymentResponse = await ProcessPayPalPaymentAsync(paymentRequest);
                        break;
                    case "bank_transfer": // Ajout de la méthode pour le virement bancaire
                        _logger.LogInformation("Processing bank transfer...");
                        paymentResponse = await ProcessBankTransferAsync(paymentRequest);
                        break;
                    case "stripe": // Correction de l'énumération pour Stripe
                        _logger.LogInformation("Processing Stripe payment...");
                        // Assurez-vous de récupérer le token Stripe avant d'appeler cette méthode
                        string paymentToken = paymentRequest.Token; // Le token Stripe reçu depuis le frontend
                        paymentResponse = await ProcessStripePaymentAsync(paymentRequest);
                        break;
                    default:
                        _logger.LogWarning("Unsupported payment method: {Method}", paymentMethod.Type);
                        paymentResponse.Success = false;
                        paymentResponse.ErrorMessage = "Unsupported payment method.";
                        break;
                }

                if (paymentResponse.Status == PaymentStatus.Completed)
                {
                    _logger.LogInformation("Payment processed successfully. Transaction ID: {TransactionId}",
                        paymentResponse.TransactionId);
                }
                else
                {
                    _logger.LogWarning("Payment failed. Error: {Error}", paymentResponse.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing payment for User ID: {UserId}", paymentRequest.UserId);
                paymentResponse.Success = false;
                paymentResponse.ErrorMessage = $"Error processing payment: {ex.Message}";
            }

            Console.WriteLine($"Payment processing completed. Status: {paymentResponse.Status}, Transaction ID: {paymentResponse.TransactionId}");
            return paymentResponse;
        }


        public async Task<PaymentResponse> ProcessStripePaymentAsync(PaymentRequest paymentRequest)
        {
            var paymentResponse = new PaymentResponse();

            try
            {
                // Code pour créer le PaymentIntent Stripe
                var paymentIntent = await CreateStripePaymentIntent(paymentRequest);

                paymentResponse.TransactionId = paymentIntent.Id;
                paymentResponse.ClientSecret = paymentIntent.ClientSecret;
                paymentResponse.Status = paymentIntent.Status == "succeeded" ? PaymentStatus.Completed : PaymentStatus.Failed;
                paymentResponse.Success = paymentResponse.Status == PaymentStatus.Completed;
                paymentResponse.ErrorMessage = paymentResponse.Status == PaymentStatus.Completed ? "Payment successful" : "Payment failed";

                return paymentResponse;
            }
            catch (Exception ex)
            {
                paymentResponse.Status = PaymentStatus.Failed;
                paymentResponse.Success = false;
                paymentResponse.ErrorMessage = $"Error processing payment: {ex.Message}";

                return paymentResponse;
            }
        }
        private async Task<PaymentIntent> CreateStripePaymentIntent(PaymentRequest paymentRequest)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(paymentRequest.Amount * 100), // Montant en centimes
                Currency = paymentRequest.Currency.ToString().ToLower(),
                PaymentMethod = paymentRequest.PaymentMethodId, // Utilisation du PaymentMethod fourni
                Confirm = true, // Confirmation immédiate
                Description = paymentRequest.Description,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions
                {
                    Enabled = true,
                    AllowRedirects = "never",  // Désactive les redirections
                }
            };

          

            var service = new PaymentIntentService();
            PaymentIntent paymentIntent = await service.CreateAsync(options);

            return paymentIntent;
        }





        public Task<PaymentResponse> ProcessCardPaymentAsync(PaymentRequest paymentRequest)
        {
            _logger.LogInformation("Processing card payment for User ID: {UserId}", paymentRequest.UserId);

            return Task.FromResult(new PaymentResponse
            {
                Status = PaymentStatus.Completed,
                TransactionId = Guid.NewGuid().ToString(),
                ErrorMessage = "Card payment processed successfully"
            });
        }

        public Task<PaymentResponse> ProcessPayPalPaymentAsync(PaymentRequest paymentRequest)
        {
            _logger.LogInformation("Processing PayPal payment for User ID: {UserId}", paymentRequest.UserId);

            return Task.FromResult(new PaymentResponse
            {
                Status = PaymentStatus.Completed,
                TransactionId = Guid.NewGuid().ToString(),
                ErrorMessage = "PayPal payment processed successfully"
            });
        }

        public async Task<PaymentResponse> CreateSubscriptionAsync(string userId, decimal amount, string currency)
        {
            var subscriptionService = new SubscriptionService();
            var subscriptionOptions = new SubscriptionCreateOptions
            {
                Customer = userId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        PriceData = new SubscriptionItemPriceDataOptions
                        {
                            Currency = currency.ToLower(),
                            UnitAmount = (long)(amount * 100),
                            Product = "prod_xyz123",
                            Recurring = new SubscriptionItemPriceDataRecurringOptions
                            {
                                Interval = "month",
                                IntervalCount = 1
                            }
                        }
                    }
                },
                PaymentBehavior = "default_incomplete",
            };

            try
            {
                var subscription = await subscriptionService.CreateAsync(subscriptionOptions);
                return new PaymentResponse
                {
                    Status = subscription.Status == "active" ? PaymentStatus.Completed : PaymentStatus.Pending,
                    TransactionId = subscription.Id,
                    ErrorMessage = subscription.Status == "active"
                        ? "Subscription created successfully"
                        : "Subscription creation is pending"
                };
            }
            catch (StripeException ex)
            {
                return new PaymentResponse
                {
                    Status = PaymentStatus.Failed,
                    TransactionId = null,
                    ErrorMessage = $"Stripe error: {ex.Message}"
                };
            }
        }

        public async Task<List<Payment>> GetAllPaymentsAsync()
        {
            var chargeService = new ChargeService();

            try
            {
                var options = new ChargeListOptions
                {
                    Limit = 100, // Limite des résultats
                };

                var charges = chargeService.ListAutoPagingAsync(options); // Récupération des charges
                var allPayments = new List<Payment>();

                // Utilisation de `await foreach` pour itérer sur les éléments de charges
                await foreach (var charge in charges)
                {
                    if (charge != null)
                    {
                        allPayments.Add(new Payment
                        {
                            Id = charge.Id,
                            Amount = charge.Amount / 100m, // Conversion en unités monétaires
                            Currency = charge.Currency,
                            Created = charge.Created,
                            Status = charge.Status,
                            CustomerEmail = charge.BillingDetails?.Email ?? "Email non disponible",
                            CustomerAddress = charge.BillingDetails?.Address != null
                        ? $"{charge.BillingDetails.Address.Line1}, {charge.BillingDetails.Address.City}, {charge.BillingDetails.Address.PostalCode}, {charge.BillingDetails.Address.Country}"
                        : "Adresse non disponible",
                            ProductId = charge.Metadata != null && charge.Metadata.ContainsKey("product_id")
                        ? charge.Metadata["product_id"]
                        : "ID produit non disponible",
                            ProductName = charge.Metadata != null && charge.Metadata.ContainsKey("product_name")
                        ? charge.Metadata["product_name"]
                        : "Nom du produit non disponible",
                            Description = charge.Description,
                            Last4CardDigits = charge.PaymentMethodDetails?.Card?.Last4 ?? "Non disponible",
                            ChargeDetails = new ChargeDetails
                            {
                                PaymentId = charge.Id,
                                PaymentMethod = charge.PaymentMethodDetails?.Type ?? "Non spécifié",
                                Amount = charge.Amount / 100m,
                                Currency = charge.Currency,
                                Status = charge.Status,
                                CustomerId = charge.CustomerId,
                                Description = charge.Description ?? "Aucune description",
                                RiskLevel = charge.Outcome?.RiskLevel ?? "Non évalué",
                                UpdatedAt = charge.Created,
                                Email = charge.BillingDetails?.Email ?? "Email non disponible",
                                ReceiptUrl = charge.ReceiptUrl ?? "Aucun reçu"
                            }
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Charge null détectée.");
                    }
                }

                return allPayments;
            }
            catch (NullReferenceException ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des paiements : objet null.");
                throw new Exception("Un objet nécessaire était null", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur générale lors de la récupération des paiements.");
                throw;
            }

        }



        public async Task<string> GenerateToken(PaymentMethod paymentMethod)
        {
            try
            {
                var stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
                StripeConfiguration.ApiKey = stripeSecretKey;

                if (paymentMethod.Type == "card" && paymentMethod.Card != null)
                {
                    var tokenOptions = new TokenCreateOptions
                    {
                        Card = new TokenCardOptions
                        {
                            Number = paymentMethod.Card.Number, // Numéro complet de la carte
                            ExpMonth = paymentMethod.Card.ExpMonth,
                            ExpYear = paymentMethod.Card.ExpYear,
                            Cvc = paymentMethod.Card.Cvc,
                        }
                    };

                    var tokenService = new Stripe.TokenService();
                    var token = await tokenService.CreateAsync(tokenOptions);

                    return token.Id; // Retourner l'identifiant du token
                }
                else
                {
                    throw new InvalidOperationException("Le type de méthode de paiement ou les données de carte sont invalides.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la génération du token : {ex.Message}");
                throw; // Relancer l'exception après l'avoir loggée
            }
        }



        public async Task SendConfirmationEmailAsync(PaymentResponse paymentResponse, string userEmail)
        {
            var subject = paymentResponse.Status == PaymentStatus.Completed
                ? "Payment Confirmation"
                : "Payment Failed";

            await _emailService.SendEmailAsync(userEmail, subject, paymentResponse.ErrorMessage);
        }
        public async Task<PaymentResponse> CreateCheckoutSessionAsync(PaymentRequest paymentRequest)
        {
            var paymentResponse = new PaymentResponse();

            try
            {
                var domain = "http://localhost:3000/"; // Remplacez par votre domaine de production
                var options = new SessionCreateOptions
                {
                    LineItems = new List<SessionLineItemOptions>
            {
                new SessionLineItemOptions
                {
                    Price = paymentRequest.Amount.ToString(),
                    Quantity = 1 
                }
            },
                    PaymentMethodTypes = new List<string> { "card" }, // Méthodes de paiement
                    Mode = "payment", // Mode de paiement
                    SuccessUrl = $"{domain}/success?session_id={{CHECKOUT_SESSION_ID}}",
                    CancelUrl = $"{domain}/cancel"
                };

                var service = new SessionService();
                Session session = await service.CreateAsync(options);

                paymentResponse.TransactionId = session.Id;
                paymentResponse.Status = PaymentStatus.Completed; // Ajustez selon l'état de la session
                paymentResponse.Url = session.Url; // URL de redirection vers Stripe Checkout

                return paymentResponse;
            }
            catch (Exception ex)
            {
                paymentResponse.Status = PaymentStatus.Failed;
                paymentResponse.Success = false;
                paymentResponse.ErrorMessage = $"Erreur lors de la création de la session de paiement : {ex.Message}";
                return paymentResponse;
            }
        }

        public Task<PaymentResponse> ProcessBankTransferAsync(PaymentRequest paymentRequest)
        {
            _logger.LogInformation("Processing bank transfer for User ID: {UserId}", paymentRequest.UserId);

            return Task.FromResult(new PaymentResponse
            {
                Status = PaymentStatus.Completed,
                TransactionId = Guid.NewGuid().ToString(),
                ErrorMessage = "Bank transfer initiated"
            });
        }
    }
}

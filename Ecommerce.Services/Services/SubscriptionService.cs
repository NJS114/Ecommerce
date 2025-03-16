using Stripe;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Services.DAO.Models;
using Ecommerce.Services.DAO.Interfaces.IServices;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.Services
{
    public class SubscriptionServices : ISubscriptionService
    {
        private readonly StripeClient _stripeClient;
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;

        public SubscriptionServices(IUserRepository userRepository, IOrderRepository orderRepository)
        {
            _stripeClient = new StripeClient(Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY"));
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }

        public async Task<PaymentResponse> CreateSubscriptionAsync(string userId, string priceId, string paymentMethodId)
        {
            try
            {
                var user = await _userRepository.GetUserWithDetailsAsync(userId);
                if (user == null)
                {
                    return new PaymentResponse
                    {
                        Status = PaymentStatus.Failed,
                        ErrorMessage = "User not found"
                    };
                }

                var subscriptionService = new SubscriptionService(_stripeClient);
                var subscriptionOptions = new SubscriptionCreateOptions
                {
                    Customer = userId,
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = priceId,
                        }
                    },
                    DefaultPaymentMethod = paymentMethodId,
                    PaymentBehavior = "default_incomplete",
                    Expand = new List<string> { "latest_invoice.payment_intent" }
                };

                var subscription = await subscriptionService.CreateAsync(subscriptionOptions);

                return new PaymentResponse
                {
                    Status = subscription.Status == "active" ? PaymentStatus.Completed : PaymentStatus.Pending,
                    TransactionId = subscription.Id,
                    ErrorMessage = subscription.Status == "active" ? "Subscription created successfully" : "Subscription creation is pending"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Status = PaymentStatus.Failed,
                    ErrorMessage = $"Error creating subscription: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResponse> CancelSubscriptionAsync(string subscriptionId)
        {
            try
            {
                var subscriptionService = new SubscriptionService(_stripeClient);
                var subscription = await subscriptionService.CancelAsync(subscriptionId, new SubscriptionCancelOptions());

                return new PaymentResponse
                {
                    Status = subscription.Status == "canceled" ? PaymentStatus.Completed : PaymentStatus.Failed,
                    TransactionId = subscription.Id,
                    ErrorMessage = subscription.Status == "canceled" ? "Subscription canceled successfully" : "Subscription cancellation failed"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Status = PaymentStatus.Failed,
                    ErrorMessage = $"Error canceling subscription: {ex.Message}"
                };
            }
        }

        public async Task<PaymentResponse> UpdateSubscriptionPlanAsync(string subscriptionId, string priceId)
        {
            try
            {
                var subscriptionService = new SubscriptionService(_stripeClient);
                var subscription = await subscriptionService.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
                {
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions
                        {
                            Price = priceId
                        }
                    }
                });

                return new PaymentResponse
                {
                    Status = subscription.Status == "active" ? PaymentStatus.Completed : PaymentStatus.Failed,
                    TransactionId = subscription.Id,
                    ErrorMessage = subscription.Status == "active" ? "Subscription updated successfully" : "Subscription update failed"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Status = PaymentStatus.Failed,
                    ErrorMessage = $"Error updating subscription plan: {ex.Message}"
                };
            }
        }
    }

   
}

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
    public class OrderService
    {
        private readonly IBasketService _basketService;
        private readonly IOrderRepository _orderRepository;
        private readonly StripeClient _stripeClient;

        public OrderService(IBasketService basketService, IOrderRepository orderRepository)
        {
            _basketService = basketService ?? throw new ArgumentNullException(nameof(basketService));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _stripeClient = new StripeClient(Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
                                             ?? throw new InvalidOperationException("STRIPE_SECRET_KEY is not set."));
        }

        #region Create Order and Process Payment
        public async Task<Order> CreateOrderAsync(string userId)
        {
            // Step 1: Retrieve the user's basket
            var basket = await _basketService.GetBasketByUserIdAsync(userId);

            // Step 2: Create order from basket
            var order = new Order
            {
                UserId = userId,
                Items = basket.Items,
                TotalPrice = basket.TotalPrice,
                Status = OrderStatus.Pending
            };

            // Step 3: Save order to database
            await _orderRepository.CreateOrderFromBasket(basket);

            // Step 4: Create Stripe PaymentIntent
            var paymentResponse = await CreateStripePaymentIntent(basket.TotalPrice, "usd", userId);

            if (paymentResponse.Status == PaymentStatus.Completed)
            {
                order.Status = OrderStatus.Completed;
                await _orderRepository.UpdateOrderStatus(order.Id, OrderStatus.Completed);
            }
            else
            {
                order.Status = OrderStatus.Cancelled;
                await _orderRepository.UpdateOrderStatus(order.Id, OrderStatus.Cancelled);
            }

            return order;
        }

        private async Task<PaymentResponse> CreateStripePaymentIntent(decimal amount, string currency, string userId)
        {
            try
            {
                var paymentIntentService = new PaymentIntentService(_stripeClient);
                var paymentIntentOptions = new PaymentIntentCreateOptions
                {
                    Amount = (long)(amount * 100),  // Stripe needs the amount in cents
                    Currency = currency,
                    PaymentMethodTypes = new List<string> { "card" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "user_id", userId }
                    }
                };

                var paymentIntent = await paymentIntentService.CreateAsync(paymentIntentOptions);

                return new PaymentResponse
                {
                    Status = paymentIntent.Status == "succeeded" ? PaymentStatus.Completed : PaymentStatus.Failed,
                    TransactionId = paymentIntent.Id,
                    ErrorMessage = paymentIntent.Status == "succeeded" ? "Payment successful" : "Payment failed"
                };
            }
            catch (Exception ex)
            {
                return new PaymentResponse
                {
                    Status = PaymentStatus.Failed,
                    ErrorMessage = $"Error processing payment: {ex.Message}"
                };
            }
        }

        #endregion
    }
    }

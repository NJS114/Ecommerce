using Stripe;
using System;
using System.Threading.Tasks;
using Ecommerce.Services.DAO.Models;
using Ecommerce.Services.DAO.Interfaces.IServices;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Enums;
using MimeKit;
using MailKit.Net.Smtp;

namespace Ecommerce.Services.Services
{
    public class InvoiceServices : IInvoiceService
    {
        private readonly StripeClient _stripeClient;
        private readonly IOrderRepository _orderRepository;
        private readonly IEmailService _emailService;
        private readonly CustomerService _customerService;


        public InvoiceServices(IOrderRepository orderRepository, IEmailService emailService)
        {
            _stripeClient = new StripeClient(Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY")
                                             ?? throw new InvalidOperationException("STRIPE_SECRET_KEY is not set."));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _customerService = new CustomerService();
        }

        // Generate invoice after successful payment
        public async Task<Customer> CreateAsync(CustomerCreateOptions options)
        {
            return await _customerService.CreateAsync(options);
        }

        public async Task GenerateInvoiceAsync(string orderId)
        {
            try
            {
                var order = await _orderRepository.GetOrderById(orderId);

                if (order == null || order.Status != OrderStatus.Completed)
                {
                    throw new InvalidOperationException("Order is not completed yet or does not exist.");
                }

                // Step 1: Create invoice from the completed order
                var invoiceService = new InvoiceService(_stripeClient);
                var invoiceOptions = new InvoiceCreateOptions
                {
                    Customer = order.UserId,
                    ApplicationFeeAmount = (long)(order.TotalPrice * 100),  // Convert to cents
                    Currency = "usd",  // Adjust currency as needed
                    Metadata = new Dictionary<string, string>
                    {
                        { "order_id", order.Id }
                    }
                };

                var invoice = await invoiceService.CreateAsync(invoiceOptions);

                // Step 2: Finalize the invoice
                var finalizedInvoice = await invoiceService.FinalizeInvoiceAsync(invoice.Id);

                // Step 3: Send the invoice to the user via email
                var subject = "Your Invoice from Ecommerce";
                var body = $"<h1>Invoice #{finalizedInvoice.Id}</h1><p>Thank you for your purchase. Your invoice is attached below.</p>";

                await _emailService.SendEmailAsync(order.Email, subject, body);

                Console.WriteLine("Invoice generated and email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error generating invoice: {ex.Message}");
                throw;
            }
        }

        // Retrieve a list of invoices for a user
        public async Task<List<Invoice>> GetInvoicesByUserIdAsync(string userId)
        {
            try
            {
                var invoiceService = new InvoiceService(_stripeClient);
                var invoiceList = await invoiceService.ListAsync(new InvoiceListOptions
                {
                    Customer = userId
                });

                return invoiceList.Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching invoices: {ex.Message}");
                throw;
            }
        }
    }
}

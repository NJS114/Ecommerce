using Stripe;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Interfaces.IServices
{
    public interface IInvoiceService
    {
       Task GenerateInvoiceAsync(string orderId);
        Task<Customer> CreateAsync(CustomerCreateOptions options);
       Task<List<Invoice>> GetInvoicesByUserIdAsync(string userId);
    }
}

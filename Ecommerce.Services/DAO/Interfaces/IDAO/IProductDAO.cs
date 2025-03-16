using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IProductDAO
    {
        Task<Product> CreateProduct(Product product);
        Task<List<Product>> GetAllProduct();
        Task<Product> GetProductById(string id);
        Task<Product> UpdateProduct(Product product);
        Task<Product> DeleteProduct(string id);

    }
}

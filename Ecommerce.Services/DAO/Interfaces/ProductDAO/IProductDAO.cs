using  Ecommerce.Services.DAO.Models;

namespace  Ecommerce.Services.DAO.Interfaces.ProductDAO
{
    public interface IProductDAO
    {
        Task<Product> CreateProduct(Product product);
        Task<List<Product>> GetAllProduct();
        Task<Product> GetProductById(int id);
        Task<Product> UpdateProduct(Product product);
        Task<Product> DeleteProduct(int id);

    }
}

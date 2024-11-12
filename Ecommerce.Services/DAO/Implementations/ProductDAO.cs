using  Ecommerce.Services.DAO.Connexion;
using  Ecommerce.Services.DAO.Interfaces.ProductDAO;
using  Ecommerce.Services.DAO.Models;
using Microsoft.EntityFrameworkCore;

namespace  Ecommerce.Services.DAO.Implementations
{
    public class ProductDAO : IProductDAO
    {
        private readonly AppDbContext _appContext;
        public ProductDAO(AppDbContext appContext)
        {
            _appContext = appContext;
        }

        public async Task<Product> CreateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product));
            }

            await _appContext.Products.AddAsync(product);
            await _appContext.SaveChangesAsync();
            return product;
        }
        public async Task<List<Product>> GetAllProduct()
        {
            return await _appContext.Products.ToListAsync();
        }
        public async Task<Product> GetProductById(int id)
        {
            var product=  await _appContext.Products.FindAsync(id);
            if (product == null)
            {
                throw new KeyNotFoundException($"Produit avec ID {id} introuvable");
            }
            return product; 

        }
        public async Task<Product> UpdateProduct(Product product)
        {
            var existingProduct = await _appContext.Products.FindAsync(product.Id);

            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;
                existingProduct.Category = product.Category;

                await _appContext.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Produit avec ID {product.Id} introuvable");
            }

            return existingProduct;
        }
        public async Task<Product> DeleteProduct(int id)
        {
            var product= await _appContext.Products.FindAsync( id);
            if (product != null)
            {
                _appContext.Products.Remove(product);
                _appContext.SaveChanges();
            }
            return product;
        }

    }
}

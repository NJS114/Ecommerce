using Ecommerce.Services.DAO.Connexion;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Models;
using MongoDB.Driver;
using MongoFramework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IMongoDbConnection = Ecommerce.Services.DAO.Interfaces.IRepository.IMongoDbConnection;
namespace Ecommerce.Services.DAO.Implementations
{
    public class ProductDAO : IProductDAO
    {
        private readonly IMongoCollection<Product> _products;

        public ProductDAO(IMongoDbConnection mongoConnection)
        {
            var database = mongoConnection.GetDatabase();
            _products = database.GetCollection<Product>("Products");
        }

        // Créer un produit
        public async Task<Product> CreateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Le produit ne peut pas être nul.");
            }

            product.CreatedAt = DateTime.UtcNow; 
            await _products.InsertOneAsync(product);
            return product;
        }

        // Obtenir tous les produits
        public async Task<List<Product>> GetAllProduct()
        {
            return await _products.Find(_ => true).ToListAsync();
        }

        // Obtenir un produit par son ID
        public async Task<Product> GetProductById(string id)
        {
            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                throw new KeyNotFoundException($"Produit avec ID {id} introuvable.");
            }
            return product;
        }

        // Mettre à jour un produit
        public async Task<Product> UpdateProduct(Product product)
        {
            if (product == null)
            {
                throw new ArgumentNullException(nameof(product), "Le produit ne peut pas être nul.");
            }

            var update = Builders<Product>.Update
                .Set(p => p.Name, product.Name)
                .Set(p => p.Description, product.Description)
                .Set(p => p.Price, product.Price)
                .Set(p => p.Category, product.Category)
                .Set(p => p.CreatedAt, product.CreatedAt)
                .Set(p => p.Stock, product.Stock);

            var result = await _products.UpdateOneAsync(p => p.Id == product.Id, update);

            if (result.MatchedCount == 0)
            {
                throw new KeyNotFoundException($"Produit avec ID {product.Id} introuvable.");
            }

            return product;
        }

        // Supprimer un produit
        public async Task<Product> DeleteProduct(string id)
        {
            var product = await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
            if (product != null)
            {
                await _products.DeleteOneAsync(p => p.Id == id); // Suppression du produit
            }
            else
            {
                throw new KeyNotFoundException($"Produit avec ID {id} introuvable.");
            }
            return product;
        }
    }
}

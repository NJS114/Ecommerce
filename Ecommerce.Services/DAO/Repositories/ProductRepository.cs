using Ecommerce.Services.DAO.Builders;
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Interfaces.ProductDAO;
using Ecommerce.Services.DAO.Mapping;
using Ecommerce.Services.DAO.Models;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IProductDAO _productDAO;
        private readonly IElasticClient _elasticClient;

        public ProductRepository(IProductDAO productDAO, IElasticClient elasticClient)
        {
            _productDAO = productDAO;
            _elasticClient = elasticClient;  
        }


       

        // Récupère tous les produits sous forme de DTO
        public async Task<IEnumerable<ProductDTO>> GetAllProductDTO()
        {
            var products = await _productDAO.GetAllProduct();
            return ProductMapping.ToDTOList(products);
        }

        // Crée un produit, l'ajoute dans la base de données et l'indexe dans Elasticsearch
        public async Task<ProductDTO> CreateProductDTO(ProductDTO productDTO)
        {
            var category = new Category { Name = productDTO.CategoryName };
            var product = new ProductBuilder(new Product()
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Category = category
            }).Build();

            // Créer le produit dans la base de données
            await _productDAO.CreateProduct(product);

            // Indexer le produit dans Elasticsearch
            var indexResponse = await _elasticClient.IndexDocumentAsync(product);

            if (!indexResponse.IsValid)
            {
                throw new Exception("Échec de l'indexation du produit dans Elasticsearch.");
            }

            return ProductMapping.ToDTO(product);
        }

        // Met à jour un produit dans la base de données et dans Elasticsearch
        public async Task<ProductDTO> UpdateProductDTO(ProductDTO productDTO)
        {
            var category = new Category { Name = productDTO.CategoryName };
            var product = new Product
            {
                Id = productDTO.Id,
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Category = category
            };

            // Met à jour le produit dans la base de données
            await _productDAO.UpdateProduct(product);

            // Mettre à jour l'index dans Elasticsearch
            var updateResponse = await _elasticClient.UpdateAsync<Product>(product.Id, u => u
                .Doc(product)  // Mettre à jour le document dans l'index Elasticsearch
            );

            if (!updateResponse.IsValid)
            {
                throw new Exception("Échec de la mise à jour du produit dans Elasticsearch.");
            }

            return productDTO;
        }

        // Supprime un produit de la base de données et de l'index Elasticsearch
        public async Task<ProductDTO> DeleteProductDTO(int id)
        {
            var product = await _productDAO.GetProductById(id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Produit avec ID {id} introuvable.");
            }

            // Supprimer le produit de la base de données
            await _productDAO.DeleteProduct(id);

            // Supprimer le produit de l'index Elasticsearch
            var deleteResponse = await _elasticClient.DeleteAsync<Product>(id);

            if (!deleteResponse.IsValid)
            {
                throw new Exception("Échec de la suppression du produit dans Elasticsearch.");
            }

            return ProductMapping.ToDTO(product);
        }

        // Recherche des produits dans Elasticsearch par nom ou description
        public async Task<IEnumerable<ProductDTO>> SearchProducts(string query)
        {
            var searchResponse = await _elasticClient.SearchAsync<Product>(s => s
                .Index("products")
                .Query(q => q
                    .MultiMatch(m => m
                        .Fields(f => f
                            .Field(p => p.Name)
                            .Field(p => p.Description)
                        )
                        .Query(query)
                    )
                )
            );

            if (!searchResponse.IsValid)
            {
                throw new Exception("Échec de la recherche de produit dans Elasticsearch.");
            }

            return searchResponse.Documents.Select(ProductMapping.ToDTO);
        }
    }
}

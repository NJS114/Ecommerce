using Ecommerce.Services.DAO.Builders;
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Mapping;
using Ecommerce.Services.DAO.Models;
using Stripe;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Product = Ecommerce.Services.DAO.Models.Product;
using ProductService = Stripe.ProductService;


namespace Ecommerce.Services.DAO.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IProductDAO _productDAO;
        private readonly ProductService _stripeProductService;
        private readonly PriceService _stripePriceService;

        // Constructeur
        public ProductRepository(IProductDAO productDAO)
        {
            _productDAO = productDAO;
            _stripeProductService = new ProductService();
            _stripePriceService = new PriceService();
        }

        #region Méthodes CRUD
        public async Task<ProductDTO> GetProductByIdAsync(string id)
        {
            var product = await _productDAO.GetProductById(id);
            if (product == null) return null;

            var productDTO = new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Category = product.Category,
                Price = product.Price,
                Stock = product.Stock,
                StripeProductId = product.StripeProductId,
                StripePriceId = product.StripePriceId,
            };

            return productDTO;
        }

        public async Task<IEnumerable<ProductDTO>> GetAllProductDTO()
        {
            var products = await _productDAO.GetAllProduct();

            var productDTOs = products.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Category = p.Category,
                Price = p.Price,
                Stock = p.Stock
            }).ToList();

            return productDTOs;

        }

        public async Task<ProductDTO> CreateProductDTO(ProductDTO productDTO)
        {
            var product = new Product
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Category = productDTO.Category,
                Stock = productDTO.Stock,
            };

            var stripeProduct = await _stripeProductService.CreateAsync(new ProductCreateOptions
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
            });

            var priceOptions = new PriceCreateOptions
            {
                UnitAmount = (long)(productDTO.Price * 100), 
                Currency = "usd",
                Product = stripeProduct.Id,
            };

            var price = await _stripePriceService.CreateAsync(priceOptions);

            product.StripeProductId = stripeProduct.Id;
            product.StripePriceId = price.Id; 

            await _productDAO.CreateProduct(product);

            return new ProductDTO
            {
                Id = product.Id,
                Name = productDTO.Name,
                Price = productDTO.Price,
                Stock = product.Stock,
                Category = product.Category,
                Description = productDTO.Description,
                StripeProductId = stripeProduct.Id,
                StripePriceId = price.Id, 
            };
        }

        public async Task<ProductDTO> UpdateProductDTO(ProductDTO productDTO)
        {
            // Mettre à jour le produit dans votre base de données
            var product = new Product
            {
                Id = productDTO.Id.ToString(),
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Category = productDTO.Category,
                Stock = productDTO.Stock,
            };

            await _productDAO.UpdateProduct(product);

            // Mettre à jour le produit dans Stripe
            var stripeProduct = await _stripeProductService.GetAsync(productDTO.Id.ToString());
            var options = new ProductUpdateOptions
            {
                Name = productDTO.Name,
                Description = productDTO.Description,
            };

            var updatedProduct = await _stripeProductService.UpdateAsync(stripeProduct.Id, options);

            return new ProductDTO
            {
                Id = updatedProduct.Id,
                Name = updatedProduct.Name,
                Price = productDTO.Price,
                Description = updatedProduct.Description,
            };
        }

        public async Task<ProductDTO> DeleteProductDTO(string id)
        {
            var product = await _productDAO.GetProductById(id);

            if (product == null)
            {
                throw new KeyNotFoundException($"Produit avec ID {id} introuvable.");
            }

            // Supprimer le produit dans Stripe
            await _stripeProductService.DeleteAsync(id);

            // Supprimer le produit de votre base de données
            await _productDAO.DeleteProduct(id);
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = (decimal)product.Price,
                Description = product.Description,
                Category = product.Category,
                Stock = product.Stock,
            };
        }

        #endregion

        #region Méthodes de Recherche

        public async Task<IEnumerable<ProductDTO>> SearchProducts(string query)
        {
            var products = await _productDAO.GetAllProduct();
            var filteredProducts = products.Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                                        p.Description.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            return filteredProducts.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = (decimal)p.Price,
            });
        }

        #endregion
    }
}
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;
using System.Collections.Generic;
using System.Linq;

namespace Ecommerce.Services.DAO.Mapping
{
    public static class ProductMapping
    {
        #region ToDTO

        /// <summary>
        /// Convertit un objet Product en ProductDTO.
        /// </summary>
        public static ProductDTO ToDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Category = product.Category, 
                Stock= product.Stock,// Utilisation de l'enum Category
            };
        }

        #endregion

        #region ToEntity

        /// <summary>
        /// Convertit un objet ProductDTO en Product.
        /// </summary>
        public static Product ToEntity(ProductDTO productDTO)
        {
            return new Product
            {
                Id = productDTO.Id.ToString(),
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Category = productDTO.Category, // Mapping direct de l'enum Category
                Stock = productDTO.Stock,
                CategoryId = (int)productDTO.Category
            };
        }

        #endregion

        #region ToDTOList

        /// <summary>
        /// Convertit une liste de Product en une liste de ProductDTO.
        /// </summary>
        public static IEnumerable<ProductDTO> ToDTOList(IEnumerable<Product> products)
        {
            return products.Select(product => ToDTO(product)).ToList();
        }

        #endregion

        #region ToEntityList

        /// <summary>
        /// Convertit une liste de ProductDTO en une liste de Product.
        /// </summary>
        public static IEnumerable<Product> ToEntityList(IEnumerable<ProductDTO> productDTOs)
        {
            return productDTOs.Select(dto => ToEntity(dto)).ToList();
        }

        #endregion
    }
}

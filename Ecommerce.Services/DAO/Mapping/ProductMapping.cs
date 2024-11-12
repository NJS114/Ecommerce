using  Ecommerce.Services.DAO.DTOs;
using  Ecommerce.Services.DAO.Models;

namespace  Ecommerce.Services.DAO.Mapping
{
    public static class ProductMapping
    {
        public static ProductDTO ToDTO(Product product)
        {
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId 
            };
        }

        public static Product ToEntity(ProductDTO productDTO)
        {
            var category = new Category { Name = productDTO.CategoryName };
            return new Product
            {
                Id = productDTO.Id,
                Name = productDTO.Name,
                Description = productDTO.Description,
                Price = productDTO.Price,
                Category = category,
            };
        }

        public static IEnumerable<ProductDTO> ToDTOList(IEnumerable<Product> products)
        {
            return products.Select(product => ToDTO(product)).ToList();
        }

        public static IEnumerable<Product> ToEntityList(IEnumerable<ProductDTO> productDTOs)
        {
            return productDTOs.Select(dto => ToEntity(dto)).ToList();
        }
    }
}

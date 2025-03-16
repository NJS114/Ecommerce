using  Ecommerce.Services.DAO.DTOs;

namespace  Ecommerce.Services.DAO.Interfaces.IRepository
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductDTO>> GetAllProductDTO();
        Task<ProductDTO> GetProductByIdAsync(string id);
        Task<ProductDTO> CreateProductDTO(ProductDTO productDTO);
        Task<ProductDTO> UpdateProductDTO(ProductDTO productDTO);
        Task<ProductDTO> DeleteProductDTO(string id);
    }
}

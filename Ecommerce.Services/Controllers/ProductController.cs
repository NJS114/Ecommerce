using Ecommerce.Services.DAO.Repositories;
using Ecommerce.Services.DAO.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Ecommerce.Services.DAO.Interfaces.IRepository;

namespace Ecommerce.Services.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductRepository _repository;

        public ProductController(IProductRepository repository)
        {
            _repository = repository;
        }

        // GET: /product
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _repository.GetAllProductDTO();
            return Ok(products);
        }

        // POST: /product
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO productDTO)
        {
            if (productDTO == null)
            {
                return BadRequest("Données du produit invalides.");
            }

            var createdProduct = await _repository.CreateProductDTO(productDTO);
            return CreatedAtAction(nameof(GetProductById), new { id = createdProduct.Id }, createdProduct);
        }

        // GET: /product/{id}
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(string id)
        {
            var product = await _repository.GetProductByIdAsync(id); // Vous devrez implémenter une méthode pour obtenir un produit par ID dans le repository.
            

            return Ok(product);
        }

        // PUT: /product
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDTO productDTO)
        {
            if (productDTO == null || productDTO.Id == "0")
            {
                return BadRequest("Données du produit invalides.");
            }

            var updatedProduct = await _repository.UpdateProductDTO(productDTO);
            return Ok(updatedProduct);
        }

        // DELETE: /product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id)
        {
            try
            {
                var deletedProduct = await _repository.DeleteProductDTO(id.ToString());
                return Ok(deletedProduct);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

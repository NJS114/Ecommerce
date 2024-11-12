using Ecommerce.Services.DAO.Repositories;
using Ecommerce.Services.DAO.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Services.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _repository;

        public ProductController(ProductRepository repository)
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _repository.GetAllProductDTO(); // Vous devrez implémenter une méthode pour obtenir un produit par ID dans le repository.
            var foundProduct = product.FirstOrDefault(p => p.Id == id);

            if (foundProduct == null)
            {
                return NotFound($"Produit avec ID {id} introuvable.");
            }

            return Ok(foundProduct);
        }

        // PUT: /product
        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductDTO productDTO)
        {
            if (productDTO == null || productDTO.Id == 0)
            {
                return BadRequest("Données du produit invalides.");
            }

            var updatedProduct = await _repository.UpdateProductDTO(productDTO);
            return Ok(updatedProduct);
        }

        // DELETE: /product/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var deletedProduct = await _repository.DeleteProductDTO(id);
                return Ok(deletedProduct);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

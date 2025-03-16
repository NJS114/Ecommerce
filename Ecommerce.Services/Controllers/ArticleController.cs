using Ecommerce.Services.DAO.Models;
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using System.Threading.Tasks;
using System.Linq;
using Ecommerce.Services.DAO.Interfaces.IDAO;

namespace Ecommerce.Services.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _repository;

        public ArticleController(IArticleRepository repository)
        {
            _repository = repository;
        }

        #region GET Methods
        [HttpGet]
        public async Task<IActionResult> GetAllArticles()
        {
            var articles = await _repository.GetAllArticleDTO();
            return Ok(articles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticleById(string id)
        {
            var foundArticle = await _repository.GetArticleByIdAsync(id);

            if (foundArticle == null)
            {
                return NotFound($"Article with ID {id} not found.");
            }

            return Ok(foundArticle);
        }
        #endregion

        #region POST Methods
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleDTO articleDTO)
        {
            if (articleDTO == null)
            {
                return BadRequest("Invalid article data.");
            }

            var createdArticle = await _repository.CreateArticleDTO(articleDTO);
            return CreatedAtAction(nameof(GetArticleById), new { id = createdArticle.Id }, createdArticle);
        }
        #endregion

        #region PUT Methods
        [HttpPut]
        public async Task<IActionResult> UpdateArticle([FromBody] ArticleDTO articleDTO)
        {
            if (articleDTO == null || articleDTO.Id == "0")
            {
                return BadRequest("Invalid article data.");
            }

            var updatedArticle = await _repository.UpdateArticleDTO(articleDTO);
            return Ok(updatedArticle);
        }
        #endregion

        #region DELETE Methods
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            try
            {
                var deletedArticle = await _repository.DeleteArticleDTO(id);
                return Ok(deletedArticle);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        #endregion
    }
}

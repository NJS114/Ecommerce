using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly IArticleDAO _articleDAO;

        public ArticleRepository(IArticleDAO articleDAO)
        {
            _articleDAO = articleDAO;
        }

        #region Méthodes CRUD
        public async Task<ArticleDTO> GetArticleByIdAsync(string id)
        {
            var article = await _articleDAO.GetArticleById(id);
            if (article == null) return null;

            var articleDTO = new ArticleDTO
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                Author = article.Author,
                PublishedDate = article.PublishedDate,
                ImageUrl = article.ImageUrl,
                Category = article.Category
            };

            return articleDTO;
        }

        public async Task<IEnumerable<ArticleDTO>> GetAllArticleDTO()
        {
            var articles = await _articleDAO.GetAllArticles();

            var articleDTOs = articles.Select(a => new ArticleDTO
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                Author = a.Author,
                PublishedDate = a.PublishedDate,
                ImageUrl = a.ImageUrl,
                Category = a.Category
            }).ToList();

            return articleDTOs;
        }

        public async Task<ArticleDTO> CreateArticleDTO(ArticleDTO articleDTO)
        {
            var article = new Article
            {
                Title = articleDTO.Title,
                Content = articleDTO.Content,
                Author = articleDTO.Author,
                PublishedDate = articleDTO.PublishedDate,
                ImageUrl = articleDTO.ImageUrl,
                Category = articleDTO.Category
            };

            await _articleDAO.CreateArticle(article);

            return new ArticleDTO
            {
                Id = article.Id,
                Title = articleDTO.Title,
                Content = articleDTO.Content,
                Author = articleDTO.Author,
                PublishedDate = articleDTO.PublishedDate,
                ImageUrl = articleDTO.ImageUrl,
                Category = articleDTO.Category
            };
        }

        public async Task<ArticleDTO> UpdateArticleDTO(ArticleDTO articleDTO)
        {
            var article = new Article
            {
                Id = articleDTO.Id,
                Title = articleDTO.Title,
                Content = articleDTO.Content,
                Author = articleDTO.Author,
                PublishedDate = articleDTO.PublishedDate,
                ImageUrl = articleDTO.ImageUrl,
                Category = articleDTO.Category
            };

            await _articleDAO.UpdateArticle(article);

            return new ArticleDTO
            {
                Id = article.Id,
                Title = articleDTO.Title,
                Content = articleDTO.Content,
                Author = articleDTO.Author,
                PublishedDate = articleDTO.PublishedDate,
                ImageUrl = articleDTO.ImageUrl,
                Category = articleDTO.Category
            };
        }

        public async Task<ArticleDTO> DeleteArticleDTO(string id)
        {
            var article = await _articleDAO.GetArticleById(id);
            if (article == null) throw new KeyNotFoundException($"Article avec ID {id} introuvable.");

            await _articleDAO.DeleteArticle(id);

            return new ArticleDTO
            {
                Id = article.Id,
                Title = article.Title,
                Content = article.Content,
                Author = article.Author,
                PublishedDate = article.PublishedDate,
                ImageUrl = article.ImageUrl,
                Category = article.Category
            };
        }
        #endregion

        #region Méthodes de Recherche
        public async Task<IEnumerable<ArticleDTO>> SearchArticles(string query)
        {
            var articles = await _articleDAO.GetAllArticles();
            var filteredArticles = articles.Where(a => a.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                                                        a.Content.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();

            return filteredArticles.Select(a => new ArticleDTO
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                Author = a.Author,
                PublishedDate = a.PublishedDate,
                ImageUrl = a.ImageUrl,
                Category = a.Category
            });
        }
        #endregion
    }
}

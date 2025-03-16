using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Mapping
{
    public class ArticleMapping
    {
        public static ArticleDTO ToDTO(Article article)
        {
            if (article == null) return null;

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

        // Méthode pour convertir un DTO en objet Article
        public static Article ToModel(ArticleDTO articleDto)
        {
            if (articleDto == null) return null;

            return new Article
            {
                Id = articleDto.Id,
                Title = articleDto.Title,
                Content = articleDto.Content,
                Author = articleDto.Author,
                PublishedDate = articleDto.PublishedDate,
                ImageUrl = articleDto.ImageUrl,
                Category = articleDto.Category
            };
        }

        // Méthode pour convertir une liste d'Articles en une liste de DTOs
        public static IEnumerable<ArticleDTO> ToDTOList(IEnumerable<Article> articles)
        {
            return articles?.Select(article => ToDTO(article)).ToList() ?? new List<ArticleDTO>();
        }

        // Méthode pour convertir une liste de DTOs en une liste d'Articles
        public static IEnumerable<Article> ToModelList(IEnumerable<ArticleDTO> articleDtos)
        {
            return articleDtos?.Select(dto => ToModel(dto)).ToList() ?? new List<Article>();
        }
    }
}

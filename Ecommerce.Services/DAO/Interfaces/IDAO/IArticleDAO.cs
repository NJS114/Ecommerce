using Ecommerce.Services.DAO.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IArticleDAO
    {
        Task<Article> CreateArticle(Article article);
        Task<List<Article>> GetAllArticles();
        Task<Article> GetArticleById(string id);
        Task<Article> UpdateArticle(Article article);
        Task<Article> DeleteArticle(string id);
    }
}

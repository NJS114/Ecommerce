using Ecommerce.Services.DAO.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IArticleRepository
    {
        Task<ArticleDTO> GetArticleByIdAsync(string id);
        Task<IEnumerable<ArticleDTO>> GetAllArticleDTO();
        Task<ArticleDTO> CreateArticleDTO(ArticleDTO articleDTO);
        Task<ArticleDTO> UpdateArticleDTO(ArticleDTO articleDTO);
        Task<ArticleDTO> DeleteArticleDTO(string id);
        Task<IEnumerable<ArticleDTO>> SearchArticles(string query);
    }
}

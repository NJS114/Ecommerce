using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Interfaces.IDAO
{
    public interface IBookDAO
    {
        Task<IEnumerable<Book>> GetAllBooksAsync();

        Task<Book> GetBookByIdAsync(string id);
        Task<IEnumerable<Book>> GetBooksByCategoryAsync(Category category);
        Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author);
        Task<IEnumerable<Book>> GetBooksByTitleAsync(string title);
        Task<IEnumerable<Book>> GetBooksByPublisherAsync(string publisher);

        Task CreateBookAsync(Book book);

        Task UpdateBookAsync(Book book);

        Task DeleteBookAsync(string id);
    }
}

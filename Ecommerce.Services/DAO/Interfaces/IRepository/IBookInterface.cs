using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;

namespace Ecommerce.Services.DAO.Interfaces.IRepository
{
    public interface IBookRepository
    {
        Task<IEnumerable<BookDTO>> GetAllBooksAsync();
        Task<IEnumerable<BookDTO>> GetBooksByCategoryAsync(Category category);
        Task<IEnumerable<BookDTO>> GetBooksByAuthorAsync(string author);
        Task<IEnumerable<BookDTO>> GetBooksByTitleAsync(string title);
        Task<IEnumerable<BookDTO>> GetBooksByPublisherAsync(string publisher);

        Task<BookDTO> GetBookByIdAsync(string id);

        Task<BookDTO> CreateBookAsync(BookDTO bookDTO);

        Task<BookDTO> UpdateBookAsync(BookDTO bookDTO);

        Task DeleteBookAsync(string id);
    }
}

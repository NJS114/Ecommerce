using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Interfaces;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Mapping;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Repositories
{
    public class BookRepository : IBookRepository
    {
        #region Fields

        private readonly IBookDAO _bookDAO;

        #endregion

        #region Constructor

        public BookRepository(IBookDAO bookDAO)
        {
            _bookDAO = bookDAO;
        }

        #endregion

        #region Public Methods

        public async Task<IEnumerable<BookDTO>> GetAllBooksAsync()
        {
            var books = await _bookDAO.GetAllBooksAsync();
            return BookMapping.ToDTOList(books);
        }

        public async Task<BookDTO> GetBookByIdAsync(string id)
        {
            var book = await _bookDAO.GetBookByIdAsync(id);
            if (book == null)
            {
                throw new KeyNotFoundException($"Aucun livre trouvé avec l'ID {id}.");
            }

            return BookMapping.ToDTO(book);
        }

        public async Task<IEnumerable<BookDTO>> GetBooksByCategoryAsync(Category category)
        {
            var books = await _bookDAO.GetBooksByCategoryAsync(category);
            return BookMapping.ToDTOList(books);
        }
        public async Task<IEnumerable<BookDTO>> GetBooksByAuthorAsync(string author )
        {
            var books = await _bookDAO.GetBooksByAuthorAsync(author);
            return BookMapping.ToDTOList(books);
        }
        public async Task<IEnumerable<BookDTO>> GetBooksByTitleAsync(string title)
        {
            var books = await _bookDAO.GetBooksByTitleAsync(title);
            return BookMapping.ToDTOList(books);
        }
        public async Task<IEnumerable<BookDTO>> GetBooksByPublisherAsync(string publisher)
        {
            var books = await _bookDAO.GetBooksByPublisherAsync(publisher);
            return BookMapping.ToDTOList(books);
        }

        public async Task<BookDTO> CreateBookAsync(BookDTO bookDTO)
        {
            if (string.IsNullOrEmpty(bookDTO.Category) || !Enum.TryParse(bookDTO.Category, true, out Category categoryEnum))
            {
                throw new ArgumentException($"La catégorie '{bookDTO.Category}' est invalide.");
            }

            var book = new Book
            {
                Id = bookDTO.Id,
                Barcode = bookDTO.Barcode,
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Synopsis = bookDTO.Synopsis,
                Edition = bookDTO.Edition,
                PublicationDate = bookDTO.PublicationDate,
                BookCondition = bookDTO.BookCondition,
                CoverImagePath = bookDTO.CoverImagePath,
                AdditionalImagePaths = bookDTO.AdditionalImagePaths,
                TransactionType = bookDTO.TransactionType,
                Category = categoryEnum,
                RentalPrice = bookDTO.RentalPrice
            };

            await _bookDAO.CreateBookAsync(book);
            return bookDTO;
        }

        public async Task<BookDTO> UpdateBookAsync(BookDTO bookDTO)
        {
            if (string.IsNullOrEmpty(bookDTO.Category) || !Enum.TryParse(bookDTO.Category, true, out Category categoryEnum))
            {
                throw new ArgumentException($"La catégorie '{bookDTO.Category}' est invalide.");
            }

            var book = new Book
            {
                Id = bookDTO.Id,
                Barcode = bookDTO.Barcode,
                Title = bookDTO.Title,
                Author = bookDTO.Author,
                Synopsis = bookDTO.Synopsis,
                Edition = bookDTO.Edition,
                PublicationDate = bookDTO.PublicationDate,
                BookCondition = bookDTO.BookCondition,
                CoverImagePath = bookDTO.CoverImagePath,
                AdditionalImagePaths = bookDTO.AdditionalImagePaths,
                TransactionType = bookDTO.TransactionType,
                Category = categoryEnum,
                RentalPrice = bookDTO.RentalPrice
            };

            await _bookDAO.UpdateBookAsync(book);
            return bookDTO;
        }

        public async Task DeleteBookAsync(string id)
        {
            await _bookDAO.DeleteBookAsync(id);
        }

        #endregion
    }
}

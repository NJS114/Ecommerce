using Ecommerce.Services.DAO.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ecommerce.Services.DAO.Interfaces.IRepository;
using Ecommerce.Services.DAO.Interfaces.IDAO;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Mapping;

namespace Ecommerce.Services.DAO.Implementations
{
    public class BookDAO : IBookDAO
    {
        private readonly IMongoCollection<Book> _books;

        public BookDAO(IMongoDbConnection mongoDbConnection)
        {
            var database = mongoDbConnection.GetDatabase();
            _books = database.GetCollection<Book>("Books");
        }

        #region Getters

        public async Task<IEnumerable<Book>> GetAllBooksAsync()
        {
            return await _books.Find(book => true).ToListAsync();
        }
        public async Task<IEnumerable<Book>> GetBooksByCategoryAsync(Category category)
        {
            return await _books.Find(book => book.Category == category).ToListAsync();
         
        }
        public async Task<IEnumerable<Book>> GetBooksByAuthorAsync(string author)
        {
            return await _books.Find(book => book.Author == author).ToListAsync();

        }
        public async Task<IEnumerable<Book>> GetBooksByTitleAsync(string title )
        {
            return await _books.Find(book => book.Title == title).ToListAsync();
        }
        public async Task<IEnumerable<Book>> GetBooksByPublisherAsync(string publisher)
        {
            return await _books.Find(book => book.Edition == publisher).ToListAsync();
        }



        public async Task<Book> GetBookByIdAsync(string id)
        {
            return await _books.Find(book => book.Id == id).FirstOrDefaultAsync();
        }

        #endregion

        #region Create / Update

        public async Task CreateBookAsync(Book book)
        {
            await _books.InsertOneAsync(book); // Insertion dans MongoDB
        }

        public async Task UpdateBookAsync(Book book)
        {
            if (book == null)
            {
                throw new ArgumentNullException(nameof(book), "Le livre ne peut pas être nul.");
            }

            var existingBook = await _books.Find(b => b.Id == book.Id).FirstOrDefaultAsync();

            if (existingBook == null)
            {
                throw new InvalidOperationException($"Aucun livre trouvé avec l'ID {book.Id}.");
            }

            var update = Builders<Book>.Update
                .Set(b => b.Title, book.Title)
                .Set(b => b.Author, book.Author)
                .Set(b => b.Synopsis, book.Synopsis)
                .Set(b => b.Edition, book.Edition)
                .Set(b => b.PublicationDate, book.PublicationDate)
                .Set(b => b.BookCondition, book.BookCondition)
                .Set(b => b.CoverImagePath, book.CoverImagePath)
                .Set(b => b.AdditionalImagePaths, book.AdditionalImagePaths)
                .Set(b => b.TransactionType, book.TransactionType)
                .Set(b => b.Category, book.Category)  // Mise à jour de la catégorie
                .Set(b => b.RentalPrice, book.RentalPrice);

            await _books.UpdateOneAsync(b => b.Id == book.Id, update); // Mise à jour dans MongoDB
        }

        #endregion

        #region Delete

        public async Task DeleteBookAsync(string id)
        {
            var result = await _books.DeleteOneAsync(book => book.Id == id);
            if (result.DeletedCount == 0)
            {
                throw new InvalidOperationException($"Aucun livre trouvé avec l'ID {id}.");
            }
        }

        #endregion
    }
}

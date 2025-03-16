using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Builders
{
    public class BookBuilder
    {
        public readonly Book _book;

        public BookBuilder()
        {
            _book = new Book();
        }

        public BookBuilder(Book book)
        {
            _book = book;
        }

        public BookBuilder SetId(string id)
        {
            _book.Id = id;
            return this;
        }

        public BookBuilder SetBarcode(string barcode)
        {
            _book.Barcode = barcode;
            return this;
        }

        public BookBuilder SetTitle(string title)
        {
            _book.Title = title;
            return this;
        }

        public BookBuilder SetAuthor(string author)
        {
            _book.Author = author;
            return this;
        }

        public BookBuilder SetSynopsis(string synopsis)
        {
            _book.Synopsis = synopsis;
            return this;
        }

        public BookBuilder SetEdition(string edition)
        {
            _book.Edition = edition;
            return this;
        }

        public BookBuilder SetPublicationDate(DateTime publicationDate)
        {
            _book.PublicationDate = publicationDate;
            return this;
        }

        // Utilisation de l'Enum pour l'état du livre
        public BookBuilder SetBookCondition(BookCondition bookCondition)
        {
            _book.BookCondition = bookCondition;
            return this;
        }

        public BookBuilder SetCoverImagePath(string coverImagePath)
        {
            _book.CoverImagePath = coverImagePath;
            return this;
        }

        public BookBuilder SetAdditionalImagePaths(List<string> additionalImagePaths)
        {
            _book.AdditionalImagePaths = additionalImagePaths;
            return this;
        }

        public BookBuilder SetTransactionType(TransactionType transactionType)
        {
            _book.TransactionType = transactionType;
            return this;
        }

        public BookBuilder SetCategory(Category category)
        {
            _book.Category = category;
            return this;
        }

        public BookBuilder SetRentalPrice(decimal rentalPrice)
        {
            _book.RentalPrice = rentalPrice;
            return this;
        }

        public Book Build()
        {
            return _book;
        }
    }
}

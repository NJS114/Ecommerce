using Ecommerce.Services.DAO.DTOs;
using Ecommerce.Services.DAO.Enums;
using Ecommerce.Services.DAO.Models;

namespace Ecommerce.Services.DAO.Mapping
{
    public static class BookMapping
    {
            #region ToDTO (Entity -> DTO)
            public static BookDTO ToDTO(Book book)
            {
                if (book == null) throw new ArgumentNullException(nameof(book));

                return new BookDTO
                {
                    Id = book.Id,
                    Barcode = book.Barcode,
                    Title = book.Title,
                    Author = book.Author,
                    Synopsis = book.Synopsis,
                    Edition = book.Edition,
                    PublicationDate = book.PublicationDate,
                    BookCondition = book.BookCondition,
                    CoverImagePath = book.CoverImagePath ?? string.Empty,
                    AdditionalImagePaths = book.AdditionalImagePaths ?? new List<string>(),
                    TransactionType = book.TransactionType,
                    Category = book.Category.ToString(),
                    RentalPrice = book.RentalPrice
                };
            }

            public static IEnumerable<BookDTO> ToDTOList(IEnumerable<Book> books)
            {
                if (books == null) throw new ArgumentNullException(nameof(books));
            if (books is IEnumerable<BookDTO> dtoList)
            {
                return dtoList;
            }
            return books.Select(ToDTO).ToList();
        }
        #endregion

        #region ToEntity (DTO -> Entity)
        public static Book ToEntity(BookDTO bookDTO)
        {
            if (bookDTO == null) throw new ArgumentNullException(nameof(bookDTO));

            if (!Enum.TryParse(bookDTO.Category, out Category categoryEnum))
            {
                categoryEnum = Category.Fiction; // Default value
            }

            return new Book
            {
                Id = bookDTO.Id,
                Barcode = bookDTO.Barcode,
                Title = bookDTO.Title ?? throw new ArgumentNullException(nameof(bookDTO.Title)),
                Author = bookDTO.Author ?? throw new ArgumentNullException(nameof(bookDTO.Author)),
                Synopsis = bookDTO.Synopsis ?? string.Empty,
                Edition = bookDTO.Edition ?? string.Empty,
                PublicationDate = bookDTO.PublicationDate,
                BookCondition = bookDTO.BookCondition,
                CoverImagePath = bookDTO.CoverImagePath ?? string.Empty,
                AdditionalImagePaths = bookDTO.AdditionalImagePaths ?? new List<string>(),
                TransactionType = bookDTO.TransactionType,
                Category = categoryEnum,
                RentalPrice = bookDTO.RentalPrice ?? 0,
                Status = BookStatus.Available // Default value
            };
        }

        public static IEnumerable<Book> ToEntityList(IEnumerable<BookDTO> bookDTOs)
        {
            if (bookDTOs == null) throw new ArgumentNullException(nameof(bookDTOs));
            return bookDTOs.Select(ToEntity).ToList();
        }
        #endregion
    }
}
    using Ecommerce.Services.DAO.DTOs;
    using Ecommerce.Services.DAO.Enums;
    using Ecommerce.Services.DAO.Interfaces.IRepository;
    using Microsoft.AspNetCore.Mvc;

    namespace Ecommerce.Services.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class BookController : ControllerBase
        {
            private readonly IBookRepository _bookRepository;

            public BookController(IBookRepository bookRepository)
            {
                _bookRepository = bookRepository;
            }

            #region Getters

            // Récupérer tous les livres
            [HttpGet]
            public async Task<ActionResult<IEnumerable<BookDTO>>> GetAllBooks()
            {
                var books = await _bookRepository.GetAllBooksAsync();
                return Ok(books);  
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<BookDTO>> GetBookById(string id)
            {
                var book = await _bookRepository.GetBookByIdAsync(id);

                if (book == null)
                {
                    return NotFound($"Livre avec l'ID {id} non trouvé.");
                }

                return Ok(book); 
            }
        [HttpGet("category/{slug}")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByCategorySlug(string slug)
        {
            if (!Enum.TryParse<Category>(slug, true, out var categoryEnum))
            {
                return BadRequest($"Le slug {slug} ne correspond à aucune catégorie valide.");
            }

            var books = await _bookRepository.GetBooksByCategoryAsync(categoryEnum);

            if (books == null || !books.Any())
            {
                return NotFound($"Aucun livre trouvé pour la catégorie {slug}.");
            }

            return Ok(books);
        }

            [HttpGet("title/{slug}")]
            public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByTitleSlug(string slug)
            {
                
                var books = await _bookRepository.GetBooksByTitleAsync(slug);

                if (books == null || !books.Any())
                {
                    return NotFound($"Aucun livre trouvé pour la catégorie {slug}.");
                }

                return Ok(books);
            }
            [HttpGet("author/{slug}")]
            public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByAuthorSlug(string slug)
            {

                var books = await _bookRepository.GetBooksByAuthorAsync(slug);

                if (books == null || !books.Any())
                {
                    return NotFound($"Aucun livre trouvé pour la catégorie {slug}.");
                }

                return Ok(books);
            }
        [HttpGet("editor/{slug}")]
        public async Task<ActionResult<IEnumerable<BookDTO>>> GetBooksByPublisherSlug(string slug)
        {

            var books = await _bookRepository.GetBooksByPublisherAsync(slug);

            if (books == null || !books.Any())
            {
                return NotFound($"Aucun livre trouvé pour la catégorie {slug}.");
            }

            return Ok(books);
        }

        #endregion

        #region Create / Update

        // Créer un nouveau livre
        [HttpPost]
            public async Task<ActionResult> CreateBook([FromBody] BookDTO bookDTO)
            {
                if (bookDTO == null)
                {
                    return BadRequest("Les informations du livre sont invalides.");
                }

                await _bookRepository.CreateBookAsync(bookDTO);
                return CreatedAtAction(nameof(GetBookById), new { id = bookDTO.Id }, bookDTO); // Retourne un code 201 avec l'URL du livre créé
            }

            // Mettre à jour un livre existant
            [HttpPut("{id}")]
            public async Task<ActionResult> UpdateBook(string id, [FromBody] BookDTO bookDTO)
            {
                if (bookDTO == null)
                {
                    return BadRequest("Les informations du livre sont invalides.");
                }

                var existingBook = await _bookRepository.GetBookByIdAsync(id);

                if (existingBook == null)
                {
                    return NotFound($"Livre avec l'ID {id} non trouvé.");
                }

                bookDTO.Id = id; // S'assurer que l'ID correspond à celui passé dans l'URL
                await _bookRepository.UpdateBookAsync(bookDTO);
                return NoContent(); // Retourne un code 204 si la mise à jour a réussi
            }

            #endregion

            #region Delete

            // Supprimer un livre
            [HttpDelete("{id}")]
            public async Task<ActionResult> DeleteBook(string id)
            {
                var existingBook = await _bookRepository.GetBookByIdAsync(id);

                if (existingBook == null)
                {
                    return NotFound($"Livre avec l'ID {id} non trouvé.");
                }

                await _bookRepository.DeleteBookAsync(id);
                return NoContent(); // Retourne un code 204 si la suppression a réussi
            }

            #endregion
        }
    }

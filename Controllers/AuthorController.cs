using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.ModelServices.IServices;
using MiniLibraryManagementSystem.ViewModels;

namespace MiniLibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IAuthorServices _authorServices;
        private readonly ApplicationDbContext _context;
        public AuthorController(IAuthorServices authorServices,
            ApplicationDbContext context)
        {
            _authorServices = authorServices;
            _context = context; 
        }

        [HttpGet("GetAuthors")]
        public async Task<IActionResult> GetAuthors()
        {

            var authors = await _authorServices.GetAuthorsAsync();
            if (authors == null)
            {
                var AuthorsNotFoundMesssage = new
                {
                    AuthorsNotFoundMesssage = "there is no authors "
                };
                return NotFound(AuthorsNotFoundMesssage);
            }
            return Ok(authors);
        }

        [HttpGet("GetAuthorBooks")]
        public async Task<IActionResult> GetAuthorBooks(int AuthorId)
        {
            var existAuthor = await _context.Authors.FindAsync(AuthorId);
            if (existAuthor == null)
            {
                var authorNotFound = new
                {
                    authorNotFoundMessage = "there is no author with this Id"
                };
                return NotFound(authorNotFound);

            }

            var authorBooks = await _authorServices.GetAuthorBooksAsync(AuthorId);
            if (!authorBooks.Any())
            {
                var noBooksWithAuthor = new
                {
                    noBooksWithAuthorMesssage = "there is no books with this Author Yet!!"
                };
                return NotFound(noBooksWithAuthor);
            }

            return Ok(authorBooks);
        }

        [HttpPost("AddAuthor")]
        public async Task<IActionResult> AddAuthor(AuthorViewModel authorView)
        {
            if (string.IsNullOrEmpty(authorView.FullName)
                || authorView.FullName.Trim().Equals("string", StringComparison.CurrentCultureIgnoreCase))
            {
                var AddauthNullMessage = new
                {
                    AddauthNullMessage = "You have to add AuthorName"
                };
                return NotFound(AddauthNullMessage);
            }

            var author = await _authorServices.AddAuthorAsync(authorView);
            if (author == null)
            {
                var message = new
                {
                    Message = "addAuthor service not working now , please try again later!"
                };
                return BadRequest(message);
            }
            return Ok(author);
        }
        [HttpPost("EditAuthor")]
        public async Task<IActionResult> EditAuthor(int AuthorId, AuthorViewModel authorView)
        {
            var existAuthor = await _context.Authors.FindAsync(AuthorId);
            if (existAuthor == null)
            {
                var authorNotFound = new
                {
                    authorNotFoundMessage = "there is no author with this Id"
                };
                return NotFound(authorNotFound);

            }

            if (string.IsNullOrEmpty(authorView.FullName)
                || authorView.FullName.Trim().Equals("string", StringComparison.CurrentCultureIgnoreCase))
            {
                var AddauthNullMessage = new
                {
                    AddauthNullMessage = "You have to add Author"
                };
                return NotFound(AddauthNullMessage);
            }

            var editAuthor = await _authorServices.EditAuthorAsync(AuthorId, authorView);
            if (editAuthor == null)
            {
                var editError = new
                {
                    EditErrorMEssage = "there is no Author gor edit Yet!"
                };
                return NotFound(editError);
            }
            return Ok(editAuthor);
        }
        [HttpDelete("DeleteAuthor")]
        public async Task<IActionResult> DeleteAuthor(int AuthorId)
        {
            var existAuthor = await _context.Authors.FindAsync(AuthorId);
            if (existAuthor == null)
            {
                var authorNotFound = new
                {
                    authorNotFoundMessage = "there is no author with this Id"
                };
                return NotFound(authorNotFound);

            }

            var deleteAuthor = await _authorServices.DeleteAuthorAsync(AuthorId);

            if (deleteAuthor == null)
            {
                var deleteError = new
                {
                    deleteErrorMessage = "the author didnt deleted yet!!"
                };
                return NotFound(deleteError);
            }

            return Ok(deleteAuthor);
        }
        [HttpDelete("DeleteAuthorInBook")]
        public async Task<IActionResult> DeleteAuthorInBook(int BookId, int AuthorId)
        {
            var existBook = await _context.Books.FindAsync(BookId);
            if (existBook == null)
            {
                var ExistBookMessage = new
                {
                    BookNotFoundMessage = "there is no book With this Id"
                };
                return NotFound(ExistBookMessage);

            }

            var existAuthor = await _context.Authors.FindAsync(AuthorId);
            if (existAuthor == null)
            {
                var authorNotFound = new
                {
                    authorNotFoundMessage = "there is no author with this Id"
                };
                return NotFound(authorNotFound);

            }

            var existAuthorInBook = await _context.BookAuthors.Where(ab => ab.BookId == BookId
            && ab.AuthorId == AuthorId).FirstOrDefaultAsync();
            if (existAuthorInBook == null)
            {
                var Message = new
                {
                    Message = "this Author not in this Book!!"
                };

                return BadRequest(Message);
            }

            var deleteAuthorInBook = await _authorServices.DeleteAuthorInBookAsync(BookId, AuthorId);
            if (deleteAuthorInBook == false)
            {
                if (deleteAuthorInBook == false)
                {
                    var deleteError = new
                    {
                        deleteErrorMessage = "the author didnt deleted in this book yet!!"
                    };
                    return NotFound(deleteError);
                }
            }

            return Ok(deleteAuthorInBook);
        }
    }
}

using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.ModelServices.IServices;
using MiniLibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniLibraryManagementSystem.Filters;

namespace MiniLibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [ValidateModelFilter]
    [ServiceFilter(typeof(JwtExceptionFilter))]
    public class BookController : ControllerBase
    {
        private readonly IBookServices _bookServices;
        private readonly ILogger<BookController> _logger;
        private readonly ApplicationDbContext _context;
        public BookController(ApplicationDbContext context, IBookServices bookServices
            , ILogger<BookController> logger)
        {
            _context = context;
            _bookServices = bookServices;
            _logger = logger;
        }
        [HttpPost("AddBookWithSolid")]
        public async Task<IActionResult> AddBookWithSolid(AddBookViewModel bookView)
        {          
            if (bookView.Title.Trim().ToLower()
                .Equals("string", StringComparison.CurrentCultureIgnoreCase))
            {
                var Message = new
                {
                    Messsage = "you should write the title of the book you wanna add"
                };

                return BadRequest(Message);
            }

            if (bookView.AvailableCopies < 0)
            {
                var AvailableCopies = new
                {
                    BookStatusMessage = "the AvailableCopies must be positive"
                };

                return BadRequest(AvailableCopies);
            }
            if (bookView.Price <= 0)
            {
                var PriceValidation = new
                {
                    BookStatusMessage = "the price must be a positive number"
                };
                return NotFound(PriceValidation);
            }

            if (bookView.Currency.Trim().ToLower()
                .Equals("string", StringComparison.CurrentCultureIgnoreCase)
                || (bookView.Currency != "USD" && bookView.Currency != "EGP"))
            {
                var Message = new
                {
                    Message = "you must add Currency to the price of this book between USD or EGP!!"
                };
                return BadRequest(Message);
            }

            var addBook = await _bookServices.AddBookSolidAsync(bookView);
            if (addBook == null)
            {
                var message = new
                {
                    Message = "the add book srvices Not Working now , please try again later"
                };
                return BadRequest(message);
            }

            return Ok(addBook);

        }

        [HttpPut("EditBookWithSolid")]
        public async Task<IActionResult> EditBookWithSolid(int BookId, EditBookViewModel EditbookView)
        {
            var existBook = await _context.Books.FindAsync(BookId);
            if (existBook == null)
            {
                var Message = new
                {
                    message = "there is no book With this Id"
                };
                return NotFound(Message);
            }

            if (EditbookView.Title.Trim().ToLower()
                .Equals("string", StringComparison.CurrentCultureIgnoreCase))
            {
                var Message = new
                {
                    Messsage = "you should write the New title of the book you wanna Edit"
                };

                return BadRequest(Message);
            }


            if (EditbookView.AvailableCopies < 0)
            {
                var AvailableCopies = new
                {
                    BookStatusMessage = "the AvailableCopies must be positive"
                };

                return BadRequest(AvailableCopies);
            }

            if (EditbookView.Price <= 0)
            {
                var PriceValidation = new
                {
                    BookStatusMessage = "the price must be a positive number"
                };
                return NotFound(PriceValidation);
            }

            if (EditbookView.Currency.Trim().ToLower()
                .Equals("string", StringComparison.CurrentCultureIgnoreCase)
                || (EditbookView.Currency != "USD" && EditbookView.Currency != "EGP"))
            {
                var Message = new
                {
                    Message = "you must add Currency to the price of this book between USD or EGP!!"
                };
                return BadRequest(Message);
            }

            var editBookSolid = await _bookServices.EditBookSolidAsync(BookId, EditbookView);

            if (editBookSolid == null)
            {
                var message = new
                {
                    Message = "the Edit book srvices Not Working now , please try again later"
                };
                return BadRequest(message);
            }

            return Ok(editBookSolid);

        }

        [HttpDelete("DeleteBook")]
        public async Task<IActionResult> DeleteBook(int BookId)
        {
            var existBook = await _context.Books.FindAsync(BookId);
            if (existBook == null)
            {
                var existBookNotFound = new
                {
                    existBookNotFoundMessage = "there is no book with this Id"
                };
                return NotFound(existBookNotFound);
            }

            var deleteBook = await _bookServices.DeleteBookAsync(BookId);
            if (deleteBook == false)
            {
                var Message = new
                {
                    MEssage = "delete book service not working now, try again later!!"
                };

                return BadRequest(Message);
            }

            return Ok(deleteBook);
        }

        [HttpPost("AddExistAuthorsInBook")]
        public async Task<IActionResult> AddExistAuthorsInBook(int BookId, ICollection<int> AuthorsIds)
        {
            var existBook = await _context.Books.FindAsync(BookId);
            if (existBook == null)
            {
                var Message = new
                {
                    message = "there is no book With this Id"
                };
                return NotFound(Message);
            }

            if (AuthorsIds.Any())
            {
                var AuthorsNotFound = new List<int>();

                foreach (var autherId in AuthorsIds)
                {

                    var existAuthor = await _context.Authors.Where(a => a.Id == autherId).FirstOrDefaultAsync();
                    if (existAuthor == null)
                    {
                        AuthorsNotFound.Add(autherId);

                    }
                }
                if (AuthorsNotFound.Any())
                {
                    var message = new
                    {
                        message = $"there is no Author with these IDs  {String.Join(",", AuthorsNotFound)} "
                    };

                    return NotFound(message);
                }

            }


            var addExistAuthor = await _bookServices.AddExistAuthorsInBookAsync(BookId, AuthorsIds);
            if (addExistAuthor == null)
            {
                var Message = new
                {
                    Message = "add exist Author in book service not working now, please try again later!"
                };
                return BadRequest(Message);
            }

            return Ok(addExistAuthor);
        }

        [HttpGet("GetBooks")]
        public async Task<IActionResult> GetBooks()
        {
            _logger.LogInformation("GetBooks method called at {time}", DateTime.UtcNow);
            try
            {
                var books = await _bookServices.GetBooksAsync();
                if (!books.Any())
                {
                    var MEssage = new
                    {
                        Message = "there is no book or the service not working, please try again later!!"
                    };
                    return NotFound(MEssage);
                }
                return Ok(books);
            }
            catch (Exception ex)
            {          
                _logger.LogError(ex, "An error occurred in DoSomething.");
                throw;
            }
        }

        [HttpGet("GetBookById")]
        public async Task<IActionResult> GetBookById(int BookId)
        {
            var existBook = await _context.Books.FindAsync(BookId);
            if (existBook == null)
            {
                _logger.LogWarning("BookId {BookId} not found", BookId);
                var ExistBookMessage = new
                {
                    BookNotFoundMessage = "there is no book With this Id"
                };
                return NotFound(ExistBookMessage);

            }

            var GetBook = await _bookServices.GetBookByIdAsync(BookId);
            if (GetBook == null)
            {
                var Message = new
                {
                    Message = "Get book Service Not working now, please try again later!!"
                };
                return BadRequest(Message);
            }

            return Ok(GetBook);
        }

        [HttpGet("AllAuthorsInBook")]
        public async Task<IActionResult> AllAuthorsInBook(int BookId)
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

            var AuthorsInBook = await _bookServices.AllAuthorsInBook(BookId);
            if (!AuthorsInBook.Any())
            {
                var AuthorsInBookMessage = new
                {
                    AuthorNotFoundMessage = "there is no Author in this Book"
                };
                return NotFound(AuthorsInBookMessage);
            }

            return Ok(AuthorsInBook);
        }

        [HttpPost("FilterWithPrice")]
        public async Task<IActionResult> FilterWithPrice(FilterWithPriceViewModel filterWithPrice)
        {
            if (filterWithPrice.Price <= 0)
            {
                var Messsage = new
                {
                    Message = "Your have to choose price that you want filter "
                };
                return NotFound(Messsage);
            }

            var filterBooks = await _bookServices.FilterWithPriceAsync(filterWithPrice);
            if (!filterBooks.Any())
            {
                var Message = new
                {
                    Message = "there is no books with this price with this Currency"
                };
                return NotFound(Message);
            }

            return Ok(filterBooks);
        }
    }
}

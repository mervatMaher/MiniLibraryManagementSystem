using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.ModelServices.IServices;
using MiniLibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniLibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class BookController : ControllerBase
    {
        private readonly IBookServices _bookServices;
        private readonly ApplicationDbContext _context;
        public BookController(ApplicationDbContext context, IBookServices bookServices)
        {
            _context = context;
            _bookServices = bookServices;
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

    }
}

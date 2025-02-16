using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Models;
using MiniLibraryManagementSystem.ModelServices.IServices;
using MiniLibraryManagementSystem.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace MiniLibraryManagementSystem.ModelServices.Services
{
    public class BookServices : IBookServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public BookServices(ApplicationDbContext context,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<object> AddBookSolidAsync(AddBookViewModel bookView)
        {
            var existBook = await _context.Books.FirstOrDefaultAsync(b => b.ISBN == bookView.ISBN);
            if (existBook != null)
            {
                var message = new
                {
                    Message = "this book Already exist!",
                    existBook.Title,
                    existBook.Description,
                    existBook.ISBN,
                    existBook.PublicationYear,
                    existBook.AvailableCopies,
                    existBook.Price,
                    existBook.Currency
                };
                return message;
            }

            var book = new Book
            {
                Title = bookView.Title,
                Description = bookView.Description,
                ISBN = bookView.ISBN,
                PublicationYear = bookView.PublicationYear,
                AvailableCopies = bookView.AvailableCopies,
                Price = bookView.Price,
                Currency = bookView.Currency
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            var BookDto = _mapper.Map<BookDetailsDto>(book);


            return BookDto;

        }
        public async Task<object> EditBookSolidAsync(int BookId, EditBookViewModel editBookView)
        {
            var existBook = await _context.Books.FindAsync(BookId);

            existBook.Title = editBookView.Title;
            existBook.Description = editBookView.Description;
            existBook.ISBN = editBookView.ISBN;
            existBook.PublicationYear = editBookView.PublicationYear;
            existBook.AvailableCopies = editBookView.AvailableCopies;

            existBook.Price = editBookView.Price;
            existBook.Currency = editBookView.Currency;

            _context.Books.Update(existBook);
            await _context.SaveChangesAsync();

            var BookDto = _mapper.Map<BookDetailsDto>(existBook);

            return BookDto;
        }
        public async Task<bool> DeleteBookAsync(int BookId)
        {
            var book = await _context.Books.FindAsync(BookId);
            if (book != null)
            {
                _context.Remove(book);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<object> AddExistAuthorsInBookAsync(int BookId, ICollection<int> AuthorsIds)
        {
            var existBook = await _context.Books.FindAsync(BookId);


            foreach (var autherId in AuthorsIds)
            {
                var existAuthor = await _context.Authors.Where(a => a.Id == autherId).FirstOrDefaultAsync();
                if (existAuthor != null)
                {
                    var existAuthorInBook = await _context.BookAuthors.Where(ba => ba.BookId == BookId
                    && ba.AuthorId == autherId).FirstOrDefaultAsync();

                    if (existAuthorInBook != null)
                    {
                        var existAuthorWithBookMessage = new
                        {
                            Messsage = "this Author Already In This Book",
                            BookId = existBook.Id,
                            BookTitle = existBook.Title,

                            Authors = await _context.BookAuthors.Where(ba => ba.BookId == BookId)
                            .Include(ba => ba.author)
                            .Select(b => new
                            {
                                AuthorId = b.AuthorId,
                                AuthorName = b.author.FullName
                            }).ToListAsync()

                        };

                        return existAuthorWithBookMessage;
                    }

                    var BookAuthor = new BookAuthor
                    {
                        BookId = BookId,
                        AuthorId = autherId
                    };
                    _context.BookAuthors.Add(BookAuthor);
                    await _context.SaveChangesAsync();

                }

            }

            var AuthorsInBook = await _context.Books.Where(b => b.Id == BookId)
                .Include(a => a.BookAuthors)
                .ThenInclude(b => b.author)
                .FirstOrDefaultAsync();

            var AuthorsInBookDto = _mapper.Map<BookWithAllAuthorsDto>(AuthorsInBook);

            return AuthorsInBookDto;

        }
        public async Task<List<BookDetailsDto>> GetBooksAsync()
        {
            var books = await _context.Books.ToListAsync();
            var booksDto = _mapper.Map<List<BookDetailsDto>>(books);

            return booksDto;
        }
        public async Task<BookDetailsDto> GetBookByIdAsync(int BookId)
        {
            var findBook = await _context.Books.FindAsync(BookId);

            var BookDetailDto = _mapper.Map<BookDetailsDto>(findBook);

            return BookDetailDto;
        }
        public async Task<List<AuthorDto>> AllAuthorsInBook(int BookId)
        {

            var AuthorsInBooks = await _context.BookAuthors.Where(ba => ba.BookId == BookId)
                .Include(b => b.author)
                .ToListAsync();

            var AuthorsInBookDto = _mapper.Map<List<AuthorDto>>(AuthorsInBooks);

            return AuthorsInBookDto;
        }
        public async Task<List<BookDetailsDto>> FilterWithPriceAsync(FilterWithPriceViewModel filterWithPrice)
        {

            if (string.IsNullOrWhiteSpace(filterWithPrice.Currency)
                || filterWithPrice.Currency.Trim().ToLower() == "string")
            {

                var booksWithoutCurrency = await _context.Books.Where(b => b.Price == filterWithPrice.Price
                || (b.Price >= filterWithPrice.Price - 50 && b.Price <= filterWithPrice.Price + 50)
              ).ToListAsync();

                var booksDtoPriceOnly = _mapper.Map<List<BookDetailsDto>>(booksWithoutCurrency);

                return booksDtoPriceOnly;

            }


            var booksWithPrice = await _context.Books.Where(b =>
            (b.Price == filterWithPrice.Price
            || (b.Price >= filterWithPrice.Price - 50 && b.Price <= filterWithPrice.Price + 50))
            && b.Currency == filterWithPrice.Currency)
                .ToListAsync();

            var BooksDtoMAin = _mapper.Map<List<BookDetailsDto>>(booksWithPrice);

            return BooksDtoMAin;
        }

    }
}



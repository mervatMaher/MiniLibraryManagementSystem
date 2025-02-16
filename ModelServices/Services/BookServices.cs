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

    }
}



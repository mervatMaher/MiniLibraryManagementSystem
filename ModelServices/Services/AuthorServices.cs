using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.ModelServices.IServices;
using MiniLibraryManagementSystem.ViewModels;

namespace MiniLibraryManagementSystem.ModelServices.Services
{
    public class AuthorServices : IAuthorServices
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public AuthorServices(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<AuthorDto>> GetAuthorsAsync()
        {
            var authors = await _context.Authors.ToListAsync();

            var authorsDto = _mapper.Map<List<AuthorDto>>(authors);
            return authorsDto;
        }
        public async Task<List<BookDetailsDto>> GetAuthorBooksAsync(int AuthorId)
        {
            var authorBooks = await _context.BookAuthors.Where(ab => ab.AuthorId == AuthorId)
                .Include(ab => ab.book)
                .ToListAsync();

            var autherBooksDto = _mapper.Map<List<BookDetailsDto>>(authorBooks);

            return autherBooksDto;
        }
        public async Task<object> AddAuthorAsync(AuthorViewModel authorView)
        {
            var existAuthor = await _context.Authors.Where(a => a.FullName == authorView.FullName).FirstOrDefaultAsync();

            if (existAuthor != null)
            {
                var AuthorFound = new
                {
                    AuthorMssage = "there is already Author with this Name!",
                    AuthorId = existAuthor.Id,
                    FullName = existAuthor.FullName,
                };
                return AuthorFound;
            }


            var Author = new Models.Author
            {
                FullName = authorView.FullName,
            };
            _context.Authors.Add(Author);
            await _context.SaveChangesAsync();

            var AuthorDto = _mapper.Map<AuthorDto>(Author);
            return AuthorDto;

        }
        public async Task<object> EditAuthorAsync(int AuthorId, AuthorViewModel authorView)
        {
            var existAuthor = await _context.Authors.FindAsync(AuthorId);

            if (existAuthor != null)
            {
                existAuthor.FullName = authorView.FullName;
                _context.Authors.Update(existAuthor);
                await _context.SaveChangesAsync();
            }

            var authorDto = _mapper.Map<AuthorDto>(existAuthor);
            return authorDto;
        }

        public async Task<object> DeleteAuthorAsync(int AuthorId)
        {
            var deleteAuthor = await _context.Authors.FindAsync(AuthorId);
            _context.Authors.Remove(deleteAuthor);
            await _context.SaveChangesAsync();

            var DeleteSuccessful = new
            {
                DeleteSuccessfulMessage = "the Author Deleted Successfully!!"
            };
            return DeleteSuccessful;
        }

        public async Task<bool> DeleteAuthorInBookAsync(int BookId, int AuthorId)
        {
            var existAuthorInBook = await _context.BookAuthors.Where(ba => ba.BookId == BookId
            && ba.AuthorId == AuthorId).FirstOrDefaultAsync();

            if (existAuthorInBook != null)
            {
                _context.BookAuthors.Remove(existAuthorInBook);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }




    }
}

using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.ViewModels;

namespace MiniLibraryManagementSystem.ModelServices.IServices
{
    public interface IAuthorServices
    {
        public Task<List<AuthorDto>> GetAuthorsAsync();
        public Task<List<BookDetailsDto>> GetAuthorBooksAsync(int AuthorId);
        public Task<object> AddAuthorAsync(AuthorViewModel authorView);
        public Task<object> EditAuthorAsync(int AuthorId, AuthorViewModel authorView);
        public Task<object> DeleteAuthorAsync(int AuthorId);
        public Task<bool> DeleteAuthorInBookAsync(int BookId, int AuthorId);


    }
}

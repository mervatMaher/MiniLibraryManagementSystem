using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.ViewModels;

namespace MiniLibraryManagementSystem.ModelServices.IServices
{
    public interface IBookServices
    {
        public Task<object> AddBookSolidAsync(AddBookViewModel bookView);
        public Task<object> EditBookSolidAsync(int BookId, EditBookViewModel EditBookView);
        public Task<bool> DeleteBookAsync(int id);
        public Task<object> AddExistAuthorsInBookAsync(int BookId, ICollection<int> AuthorsIds);
        public Task<List<BookDetailsDto>> GetBooksAsync();
        public Task<BookDetailsDto> GetBookByIdAsync(int BookId);
        public Task<List<AuthorDto>> AllAuthorsInBook(int BookId);
        public Task<List<BookDetailsDto>> FilterWithPriceAsync(FilterWithPriceViewModel filterWithPrice);




    }
}

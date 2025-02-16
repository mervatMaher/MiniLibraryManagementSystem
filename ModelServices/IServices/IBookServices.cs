using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.ViewModels;

namespace MiniLibraryManagementSystem.ModelServices.IServices
{
    public interface IBookServices
    {
        public Task<object> AddBookSolidAsync(AddBookViewModel bookView);
        public Task<object> EditBookSolidAsync(int BookId, EditBookViewModel EditBookView);
        public Task<bool> DeleteBookAsync(int id);

    }
}

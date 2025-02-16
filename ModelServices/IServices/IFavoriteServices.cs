using MiniLibraryManagementSystem.Dtos;

namespace MiniLibraryManagementSystem.ModelServices.IServices
{
    public interface IFavoriteServices
    {
        public Task<object> AddBookInFavAsync(string UserId, int BookId);
        public Task<bool> DeleteBookFromFavListAsync(int FavId);
        public Task<List<FavsDto>> AllFavsForUserAsync(string UserId);
    }
}

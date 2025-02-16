using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniLibraryManagementSystem.ModelServices.IServices;

namespace MiniLibraryManagementSystem.ModelServices.Services
{
    public class FavoriteServices : IFavoriteServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public FavoriteServices(UserManager<ApplicationUser> userManager,
           ApplicationDbContext context,
           IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;

        }

        public async Task<object> AddBookInFavAsync(string UserId, int BookId)
        {
            var existUser = await _userManager.FindByIdAsync(UserId);
            var existBook = await _context.Books.FindAsync(BookId);

            var existBookInFavList = await _context.Favorites.Where(f => f.UserId == UserId && f.BookId == BookId).FirstOrDefaultAsync();
            if (existBookInFavList != null)
            {
                var ExistBookInFav = new
                {
                    Message = "this book Already in your Fav List!",
                    BookId = existBookInFavList.Id

                };
                return ExistBookInFav;
            }

            var BookInFav = new Favorite
            {
                UserId = UserId,
                BookId = BookId
            };

            _context.Favorites.Add(BookInFav);
            await _context.SaveChangesAsync();
    
            var favDto = _mapper.Map<FavsDto>(BookInFav);
            return favDto;

        }
        public async Task<bool> DeleteBookFromFavListAsync(int FavId)
        {
            var existFav = await _context.Favorites.FindAsync(FavId);
            _context.Favorites.Remove(existFav);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<FavsDto>> AllFavsForUserAsync(string UserId)
        {

            var FavList = await _context.Favorites.Where(f => f.UserId == UserId).ToListAsync();
            var FavListDto = new List<FavsDto>();

            foreach (var fav in FavList)
            {
                var favDto = new FavsDto
                {
                    FavId = fav.Id,
                    BookId = fav.BookId
                };
                FavListDto.Add(favDto);
            }
            return FavListDto;
        }
    }
}

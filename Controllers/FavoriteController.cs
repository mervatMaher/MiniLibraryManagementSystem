using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MiniLibraryManagementSystem.ModelServices.IServices;

namespace MiniLibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FavoriteController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFavoriteServices _favoriteServices;

        public FavoriteController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IFavoriteServices favoriteServices)
        {
            _context = context;
            _userManager = userManager;
            _favoriteServices = favoriteServices;
        }

        [HttpPost("AddBookInFav")]
        public async Task<IActionResult> AddBookInFavAsync(int BookId)
        {
            var UserId = User.FindFirstValue("uid");

            var existBook = await _context.Books.FindAsync(BookId);
            if (existBook == null)
            {
                var Message = new
                {
                    Message = "this book not in our database!"
                };
                ; return NotFound(Message);
            }

            var AddBookInFav = await _favoriteServices.AddBookInFavAsync(UserId, BookId);
            if (AddBookInFav == null)
            {
                var Message = new
                {
                    Message = "the add bookIn Fav didnt work, please try again later"
                };
                return NotFound();
            }

            return Ok(AddBookInFav);
        }

        [HttpGet("AllFavsForUser")]
        public async Task<IActionResult> AllFavsForUser()
        {
            var UserId = User.FindFirstValue("uid");
            var existUser = await _userManager.FindByIdAsync(UserId);
            if (existUser == null)
            {
                var Message = new
                {
                    Message = "this user Not in our database, please signUp"
                };

                return NotFound(Message);
            }

            var AllFav = await _favoriteServices.AllFavsForUserAsync(UserId);
            if (AllFav == null)
            {
                var Message = new
                {
                    Message = "there is no FavList For this user"
                };
                return NotFound(Message);
            }

            return Ok(AllFav);
        }

        [HttpDelete("DeleteBookFromFavList")]
        public async Task<IActionResult> DeleteBookFromFavList(int FavId)
        {
            var existBookInFav = await _context.Favorites.FindAsync(FavId);
            if (existBookInFav == null)
            {
                var Message = new
                {
                    Message = "there is no Fav with this Id"
                };
                return NotFound(Message);
            }

            var DeleteBookInFav = await _favoriteServices.DeleteBookFromFavListAsync(FavId);
            if (DeleteBookInFav == false)
            {
                var Message = new
                {
                    Message = "delete book In Library Not working now, please try again later!"
                };
                return BadRequest(Message);
            }

            return Ok(DeleteBookInFav);
        }
    }
}

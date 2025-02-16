using Microsoft.AspNetCore.Identity;

namespace MiniLibraryManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfilePhoto { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
        public ICollection<Favorite> Favorites { get; set; }

    }
}


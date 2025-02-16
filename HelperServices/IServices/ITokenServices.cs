using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

namespace MiniLibraryManagementSystem.HelperServices.IServices
{
    public interface ITokenServices
    {
        public Task<JwtSecurityToken> CreateTokenAsync(ApplicationUser user);
        //public Task<RefreshTokenDto> GenerateRefreshTokenAsync();

        public Task<RefreshTokenDto> AddRefreshTokenForUserAsync(ApplicationUser user);
    }
}

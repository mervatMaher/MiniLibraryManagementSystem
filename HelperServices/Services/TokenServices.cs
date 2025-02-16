using MiniLibraryManagementSystem.Data;
using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Helper;
using MiniLibraryManagementSystem.HelperServices.IServices;
using MiniLibraryManagementSystem.Models;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace MiniLibraryManagementSystem.HelperServices.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly JWT _jwt;
        public TokenServices(UserManager<ApplicationUser> userManager
            , IOptions<JWT> jwt,
            ApplicationDbContext context
            )
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _context = context;
        }
        public async Task<JwtSecurityToken> CreateTokenAsync(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var rolesClaims = new List<Claim>();


            foreach (var role in roles)
            {
                // كل ( claim ) مكون من type and value , so i put the type of the claim as roles and the 
                // value is the role of the user 
                rolesClaims.Add(new Claim("roles", role));
            }

            var claims = new List<Claim>()
            {
                // this code represent the .Sub mean the name of the user that we will create the token for
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                // .JTi represent the token Id , so we created unique Id of this token 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                // uid for UserId 
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.JWTSecretKey));

            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwt.ExpirationInMinutes),
                claims: claims,
                signingCredentials: signingCredentials
                );

            return jwtSecurityToken;

        }

        public async Task<RefreshTokenDto> AddRefreshTokenForUserAsync(ApplicationUser user)
        {
            var GenerateRefreshToken = await GenerateRefreshTokenAsync();
            var AddRefreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken.Token,
                ExpireOn = GenerateRefreshToken.ExpireOn,
                CreatedOn = GenerateRefreshToken.CreatedOn,
                UserId = user.Id
            };
            _context.RefreshTokens.Add(AddRefreshToken);
            await _context.SaveChangesAsync();

            var refreshTokenReturn = new RefreshTokenDto
            {
                Token = GenerateRefreshToken.Token,
                ExpireOn = GenerateRefreshToken.ExpireOn,
                CreatedOn = GenerateRefreshToken.CreatedOn
            };
            return refreshTokenReturn;
        }
        public async Task<RefreshTokenDto> GenerateRefreshTokenAsync()
        {
            var RandomNumber = new byte[32];
            RandomNumberGenerator.Fill(RandomNumber);

            var token = Convert.ToBase64String(RandomNumber);
            var refreshToken = new RefreshTokenDto
            {
                Token = token,
                ExpireOn = DateTime.UtcNow.AddDays(10),
                CreatedOn = DateTime.UtcNow
            };

            return refreshToken;

        }
    }
}

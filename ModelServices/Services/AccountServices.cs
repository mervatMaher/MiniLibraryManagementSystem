using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Helper;
using MiniLibraryManagementSystem.Models;
using MiniLibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using Azure.Core;
using System;
using Microsoft.AspNetCore.Mvc;
using MiniLibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;
using MiniLibraryManagementSystem.HelperServices.IServices;
using MiniLibraryManagementSystem.ModelServices.IServices;
namespace MiniLibraryManagementSystem.ModelServices.Services
{
    public class AccountServices : IAccountServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenServices _tokenServices;
        private readonly ApplicationDbContext _context;
        private readonly IUploadFilesServices _uploadFilesServices;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfirmationEmails _confirmationEmails;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AccountServices(UserManager<ApplicationUser> userManager,
            ITokenServices tokenServices,
            IEmailSender emailSender,
            IUploadFilesServices uploadFilesServices,
            IConfirmationEmails confirmationEmails,
            SignInManager<ApplicationUser> signInManager,
            IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context
         )
        {
            _userManager = userManager;
            _tokenServices = tokenServices;
            _uploadFilesServices = uploadFilesServices;
            _confirmationEmails = confirmationEmails;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<UserDto> RegisterAsync(RegisterViewModel registerView)
        {
            var user = new ApplicationUser
            {
                FullName = registerView.FullName,
                UserName = registerView.UserName,
                Email = registerView.Email,
                PhoneNumber = registerView.PhoneNumber
            };

            var ImagePath = await _uploadFilesServices.UploadPhotoAsync(registerView.ProfilePhoto);
            user.ProfilePhoto = ImagePath;

            var result = await _userManager.CreateAsync(user, registerView.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();

                return new UserDto
                {
                    Message = string.Join(", ", errors),
                    IsSuccess = false
                };
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            // new code for email confirmation
            var schemeRequest = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{schemeRequest.Scheme}://{schemeRequest.Host}";
            await _confirmationEmails.GenerateEmailConfirmationMethod(user, baseUrl);

            // create token and refresh token 
            var token = await _tokenServices.CreateTokenAsync(user);
            var AddRefreshTokenForUser = await _tokenServices.AddRefreshTokenForUserAsync(user);

            var UserDto = new UserDto
            {
                Message = "success",
                IsSuccess = true,
                FullName = user.FullName,
                UserName = user.UserName,
                Email = user.Email,
                ProfilePhoto = user.ProfilePhoto,
                IsAuthenticated = true,
                Roles = await _userManager.GetRolesAsync(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresOn = token.ValidTo,
                RefreshToken = AddRefreshTokenForUser.Token,
                RefreshTokenExpiresOn = AddRefreshTokenForUser.ExpireOn
            };
            return UserDto;
        }
        public async Task<UserDto> ConfirmEmailSenderAsync(string UserId, string token)
        {
            if (string.IsNullOrEmpty(UserId) || string.IsNullOrEmpty(token))
            {
                return new UserDto
                {
                    Message = "Invalid confirmation link.",
                    IsSuccess = false
                };
            }

            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                return new UserDto
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var Message = new UserDto
                {
                    Message = "Email confirmation failed.",
                    IsSuccess = false
                };

                return Message;
            }

            var ConfirmMessage = new UserDto
            {
                Message = "Email confirmed successfully.",
                IsSuccess = true
            };
            return ConfirmMessage;
        }
        public async Task<UserDto> LogInAsync(LogInViewModel logInView)
        {
            var UserDtoReturn = new UserDto();
            var existUser = await _userManager.FindByEmailAsync(logInView.Email);
            if (existUser == null)
            {
                var Message = new UserDto
                {
                    Message = "the Email is not correct!",
                    IsSuccess = false,
                };
                return Message;
            }

            var userCheckPass = await _userManager.CheckPasswordAsync(existUser, logInView.Password);
            if (userCheckPass == false)
            {
                var Message = new UserDto
                {
                    Message = "the password is not correct!",
                    IsSuccess = false,
                };
                return Message;
            };

            var UserRoles = await _userManager.GetRolesAsync(existUser);

            var token = await _tokenServices.CreateTokenAsync(existUser);
            var UserWithRefreshToken = await _context.Users.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync();


            UserDtoReturn.Message = "success";
            UserDtoReturn.IsSuccess = true;
            UserDtoReturn.FullName = existUser.FullName;
            UserDtoReturn.UserName = existUser.UserName;
            UserDtoReturn.Email = existUser.Email;
            UserDtoReturn.IsAuthenticated = true;
            UserDtoReturn.Roles = UserRoles;
            UserDtoReturn.Token = new JwtSecurityTokenHandler().WriteToken(token);
            UserDtoReturn.ExpiresOn = token.ValidTo;

            if (UserWithRefreshToken.RefreshTokens.Any(t => t.IsActive))
            {
                var refreshToken = UserWithRefreshToken.RefreshTokens.FirstOrDefault(t => t.IsActive);

                UserDtoReturn.RefreshToken = refreshToken.Token;
                UserDtoReturn.RefreshTokenExpiresOn = refreshToken.ExpireOn;
            }
            else
            {
                var AddRefreshTokenForUser = await _tokenServices.AddRefreshTokenForUserAsync(existUser);

                UserDtoReturn.RefreshToken = AddRefreshTokenForUser.Token;
                UserDtoReturn.RefreshTokenExpiresOn = AddRefreshTokenForUser.ExpireOn;
            }

            return UserDtoReturn;
        }
        public async Task<UserDto> ForgetPasswordAsync(string userEmail)
        {
            var existUser = await _userManager.FindByEmailAsync(userEmail);
            var forgetConfirmation = await _userManager.IsEmailConfirmedAsync(existUser);

            if (existUser == null || !forgetConfirmation)
            {
                var EmailNotConfirmedMessage = new UserDto
                {
                    Message = "the Email is not confirmed!",
                    IsSuccess = false,
                };
                return EmailNotConfirmedMessage;
            }

            // new code for email confirmation
            var schemeRequest = _httpContextAccessor.HttpContext.Request;
            var baseUrl = $"{schemeRequest.Scheme}://{schemeRequest.Host}";
            await _confirmationEmails.GeneratePasswordResetToken(existUser, baseUrl);

            var Message = new UserDto
            {
                Message = "please check your email to reset your password!!",
                IsSuccess = true,
            };
            return Message;
        }
        public async Task<UserDto> ResetPasswordAsync(string userId, string token, string newPassword)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return new UserDto
                {
                    Message = "Invalid confirmation link.",
                    IsSuccess = false
                };
            }

            var User = await _userManager.FindByIdAsync(userId);
            if (User == null)
            {
                return new UserDto
                {
                    Message = "User not found.",
                    IsSuccess = false
                };
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));

            var result = await _userManager.ResetPasswordAsync(User, decodedToken, newPassword);
            if (!result.Succeeded)
            {
                var Message = new UserDto
                {
                    Message = "Email confirmation failed.",
                    IsSuccess = false
                };

                return Message;
            }

            var ConfirmMessage = new UserDto
            {
                Message = "Password Changed successfully, please logIn",
                IsSuccess = true
            };
            return ConfirmMessage;
        }
        public async Task<UserDto> RefreshTokenAsync(string token)
        {
            var UserDto = new UserDto();

            var UserWithRefreshTokens = await _context.Users.Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (UserWithRefreshTokens == null)
            {
                UserDto.Message = "Invalid token";
                return UserDto;
            }

            var UserRefreshToken = UserWithRefreshTokens.RefreshTokens.FirstOrDefault(t => t.Token == token);

            if (!UserRefreshToken.IsActive)
            {
                UserDto.Message = "Inactive token";
                return UserDto;
            }

            UserRefreshToken.RevokeOn = DateTime.UtcNow;
            _context.RefreshTokens.Update(UserRefreshToken);
            await _context.SaveChangesAsync();

            var newRefreshToken = await _tokenServices.AddRefreshTokenForUserAsync(UserWithRefreshTokens);
            var MainToken = await _tokenServices.CreateTokenAsync(UserWithRefreshTokens);

            UserDto.Message = "success";
            UserDto.IsSuccess = true;
            UserDto.FullName = UserWithRefreshTokens.FullName;
            UserDto.UserName = UserWithRefreshTokens.UserName;
            UserDto.Email = UserWithRefreshTokens.Email;
            UserDto.IsAuthenticated = true;
            UserDto.Roles = await _userManager.GetRolesAsync(UserWithRefreshTokens);
            UserDto.Token = new JwtSecurityTokenHandler().WriteToken(MainToken);
            UserDto.ExpiresOn = MainToken.ValidTo;
            UserDto.RefreshToken = newRefreshToken.Token;
            UserDto.RefreshTokenExpiresOn = newRefreshToken.ExpireOn;

            return UserDto;

        }
        public async Task<bool> RevokeRefreshToken(string token)
        {
            var UserWithRefreshTokens = await _context.Users.Include(u => u.RefreshTokens)
              .FirstOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (UserWithRefreshTokens == null)
            {
                return false;
            }

            var UserRefreshToken = UserWithRefreshTokens.RefreshTokens.FirstOrDefault(t => t.Token == token);

            if (!UserRefreshToken.IsActive)
            {
                return false;
            }

            UserRefreshToken.RevokeOn = DateTime.UtcNow;
            _context.RefreshTokens.Update(UserRefreshToken);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}





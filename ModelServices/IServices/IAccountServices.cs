using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.ViewModels;

namespace MiniLibraryManagementSystem.ModelServices.IServices
{
    public interface IAccountServices
    {
        public Task<UserDto> RegisterAsync(RegisterViewModel registerView);
        public Task<UserDto> ConfirmEmailSenderAsync(string UserId, string token);
        public Task<UserDto> LogInAsync(LogInViewModel logInView);
        public Task<UserDto> ForgetPasswordAsync(string userEmail);
        public Task<UserDto> ResetPasswordAsync(string userId, string token, string newPassword);
        public Task<UserDto> RefreshTokenAsync(string token);
        public Task<bool> RevokeRefreshToken(string token);


    }
}

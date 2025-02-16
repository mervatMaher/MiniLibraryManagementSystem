using MiniLibraryManagementSystem.Dtos;
using MiniLibraryManagementSystem.Models;
using MiniLibraryManagementSystem.ModelServices.IServices;
using MiniLibraryManagementSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MiniLibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountServices _accountServices;
        private readonly UserManager<ApplicationUser> _userManager; 
        public AccountController(IAccountServices accountServices
            , UserManager<ApplicationUser> userManager)
        {
            _accountServices = accountServices;
            _userManager = userManager;
        }
        [HttpPost("Register")] 
        public async Task<IActionResult> Register([FromForm] RegisterViewModel registerView)
        {
            var existUser = await _userManager.FindByEmailAsync(registerView.Email);
            if (existUser != null)
            {

                var message = new 
                {
                    Message = "this user already have an account, you van LogIn!"
                };
                return BadRequest(message);
            }

            //var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var AddAccount = await _accountServices.RegisterAsync(registerView);
            if(!AddAccount.IsSuccess)
            {
                var Message = new
                {
                    Message = AddAccount.Message
                };
                return BadRequest(Message);
            }

            if (AddAccount == null) {
                var Message = new
                {
                    Message = "the account have not added yet, please try again later!!"
                };

                return BadRequest(Message);
            }

            SetRefreshTokenInCookieAsync(AddAccount.RefreshToken, AddAccount.ExpiresOn);

            return Ok(AddAccount);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string emailToken)
        {
            var result = await _accountServices.ConfirmEmailSenderAsync(userId, emailToken);

            if (!result.IsSuccess) 
                return BadRequest(result.Message);

            return Ok("Your email has been confirmed successfully.");
        }

        [HttpPost("LogIn")]
        public async Task<IActionResult> LogIn(LogInViewModel logInView)
        {
            var LogIn = await _accountServices.LogInAsync(logInView);
            if(!LogIn.IsSuccess)
            {
                var Message = new
                {
                    Message = LogIn.Message
                };
                return BadRequest(Message);
            }
            if (LogIn == null)
            {
                var Message = new
                {
                    Message = "the log in service not working now, please try again later!!"
                };
                return BadRequest(Message);
            }
            if (!string.IsNullOrEmpty(LogIn.RefreshToken))
            {

                SetRefreshTokenInCookieAsync(LogIn.RefreshToken, LogIn.ExpiresOn);
            }
            return Ok(LogIn);
        }

        [HttpGet("ForgetPassword")]
        public async Task<IActionResult> ForgetPassword(string userEmail)
        {
            var existUser = await _userManager.FindByEmailAsync(userEmail);
            if (existUser == null)
            {
                var Message = new
                {
                    Message = "the Email is not correct!"
                };
                return BadRequest(Message);
            }

            var forgetPassword = await _accountServices.ForgetPasswordAsync(userEmail);

            if (forgetPassword == null)
            {
                var Message = new
                {
                    Message = "the forget password service not working now, please try again later!!"
                };
                return BadRequest(Message);
            }
            if(forgetPassword.IsSuccess == false)
            {
                var Message = new
                {
                    Message = forgetPassword.Message
                };
                return BadRequest(Message);
            }

            return Ok(forgetPassword.Message);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromQuery] string UserId, [FromQuery] string Token, [FromQuery] string newPassword)
        {
            var resetPassword = await _accountServices.ResetPasswordAsync(UserId, Token, newPassword);
            if(!resetPassword.IsSuccess)
            {
                var Message = new
                {
                    Message = resetPassword.Message
                };
                return BadRequest(Message);
            }

            if (resetPassword == null)
            {
                var Message = new
                {
                    Message = "the reset password service not working now, please try again later!!"
                };
                return BadRequest(Message);
            }

            return Ok(resetPassword.Message);
        }

        [HttpGet("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                var Message = new
                {
                    Message = "there is no refresh token in the cookie!"
                };
                return BadRequest(Message);
            }

            var result = await _accountServices.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
            {
                var Message = new
                {
                    Message = result.Message
                };
                return BadRequest(Message);
            }

            SetRefreshTokenInCookieAsync(result.RefreshToken, result.RefreshTokenExpiresOn);

            return Ok(result);

        }

        [HttpPost("RevokeRefreshToken")]
        public async Task<IActionResult> RevokeRefreshToken(string? token)
        {
            var refreshToken = token?? Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                var Message = new
                {
                    Message = "Token is Required!"
                };
                return BadRequest(Message);
            }

            var result = await _accountServices.RevokeRefreshToken(refreshToken);
            if (!result)
            {
                var Message = new
                {
                    Message = "the refresh token is not revoked!"
                };
                return BadRequest(Message);
            }

            return Ok("the refresh token is revoked successfully!");
        }

        private void SetRefreshTokenInCookieAsync(string refreshToken, DateTime ExpiresOn)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = ExpiresOn,
                HttpOnly = true,
                Secure = true,
                IsEssential = true,
                SameSite = SameSiteMode.None
            };

            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        }
    }
}



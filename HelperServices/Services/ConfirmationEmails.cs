using MiniLibraryManagementSystem.HelperServices.IServices;
using MiniLibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;

namespace MiniLibraryManagementSystem.HelperServices.Services
{
    public class ConfirmationEmails : IConfirmationEmails
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        public ConfirmationEmails(UserManager<ApplicationUser> userManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }
        public async Task GenerateEmailConfirmationMethod(ApplicationUser user, string baseUrl)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var confirmationLink = $"{baseUrl}/swagger/index.html#/Account/ConfirmEmail?userId={user.Id}&emailToken={code}";

            await _emailSender.SendEmailAsync(user.Email,
               "Confirm your email",
               $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>."
               );
        }

        public async Task GeneratePasswordResetToken(ApplicationUser existUser, string baseUrl)
        {

            var code = await _userManager.GeneratePasswordResetTokenAsync(existUser);

            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var confirmationLink = $"{baseUrl}/swagger/index.html#/Account/ResetPassword?userId={existUser.Id}&emailToken={code}";

            await _emailSender.SendEmailAsync(existUser.Email,
              "Reset Password",
              $"please reset your password from this link <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'> click here</a>"
              );
        }


    }
}

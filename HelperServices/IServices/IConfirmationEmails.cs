using MiniLibraryManagementSystem.Models;

namespace MiniLibraryManagementSystem.HelperServices.IServices
{
    public interface IConfirmationEmails
    {
        public Task GenerateEmailConfirmationMethod(ApplicationUser user, string baseUrl);
        public Task GeneratePasswordResetToken(ApplicationUser existUser, string baseUrl);
    }
}

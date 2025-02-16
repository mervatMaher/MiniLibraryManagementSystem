using System.ComponentModel.DataAnnotations;

namespace MiniLibraryManagementSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }
        public IFormFile ProfilePhoto { get; set; }
        [Phone(ErrorMessage = "Invalid Phone Number")]
        public string? PhoneNumber { get; set; }
        [MinLength(5, ErrorMessage = "Password should be at least More than 5 characters")]
        public string Password { get; set; }
        [Compare("Password")]
        public string? ConfirmPassword { get; set; }
    }
}

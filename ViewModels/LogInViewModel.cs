using System.ComponentModel.DataAnnotations;

namespace MiniLibraryManagementSystem.ViewModels
{
    public class LogInViewModel
    {
        [Required]
        public string Email { get; set; }     
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace MiniLibraryManagementSystem.ViewModels
{
    public class AddBookViewModel
    {
        [Required]
        [StringLength(200, ErrorMessage = "the Title of the book Should not be more than 100 Character!")]
        public string Title { get; set; }
        public string? Description { get; set; }

        public string? ISBN { get; set; }
        public DateTime? PublicationYear { get; set; }
        public decimal Price { get; set; }
        public int? AvailableCopies { get; set; }
        public string? Currency { get; set; }
    }
}

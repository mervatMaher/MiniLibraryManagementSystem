namespace MiniLibraryManagementSystem.ViewModels
{
    public class EditBookViewModel
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public string? ISBN { get; set; }
        public DateTime? PublicationYear { get; set; }
        public decimal Price { get; set; }
        public int? AvailableCopies { get; set; }
        public string? Currency { get; set; }
    }
}

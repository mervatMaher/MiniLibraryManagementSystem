namespace MiniLibraryManagementSystem.Dtos
{
    public class BookWithAllAuthorsDto
    {
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public ICollection<AuthorDto> Authors { get; set; }
    }
}

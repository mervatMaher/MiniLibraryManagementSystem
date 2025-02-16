using MiniLibraryManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MiniLibraryManagementSystem.Configuration
{
    public class BookAuthorConfig : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            builder.HasKey(ba => new
            {
                ba.BookId,
                ba.AuthorId
            });

            builder.HasOne(b => b.book)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(b => b.BookId);

            builder.HasOne(a => a.author)
                .WithMany(ba => ba.BookAuthors)
                .HasForeignKey(a => a.AuthorId);

        }
    }
}

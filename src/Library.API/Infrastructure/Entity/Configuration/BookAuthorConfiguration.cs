using Library.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.API.Infrastructure.Entity.Configuration
{
    public class BookAuthorConfiguration : IEntityTypeConfiguration<BookAuthor>
    {
        public void Configure(EntityTypeBuilder<BookAuthor> builder)
        {
            builder.ToTable("BookAuthors");
            builder.HasKey(ba => new { ba.AuthorId, ba.BookId });
            builder.HasOne<Book>(ba => ba.Book).WithMany(b => b.BookAuthors).HasForeignKey(b => b.BookId);
            builder.HasOne<Author>(ba => ba.Author).WithMany(a => a.BookAuthors).HasForeignKey(a => a.AuthorId);
            builder.Property(ba => ba.AuthorId).HasColumnName("AuthorId").IsRequired();
            builder.Property(ba => ba.BookId).HasColumnName("BookId").IsRequired();
        }
    }
}
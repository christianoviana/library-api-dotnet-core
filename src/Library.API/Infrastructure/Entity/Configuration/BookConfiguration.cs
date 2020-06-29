using Library.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.API.Infrastructure.Entity
{
    public class BookConfiguration : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.ToTable("Books");
            builder.HasKey(b => b.Id);
            builder.Property(b => b.Id).HasColumnName("Id").IsRequired().ValueGeneratedNever();
            builder.Property(b => b.Name).HasColumnName("Name").HasMaxLength(100).IsRequired();
            builder.Property(b => b.Description).HasColumnName("Description").HasMaxLength(500).IsRequired(false);
            builder.Property(b => b.Date).HasColumnName("Date").IsRequired();
            builder.Property(b => b.Pages).HasColumnName("Pages").IsRequired();
        }
    }
}

using Library.API.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.API.Infrastructure.Entity.Configuration
{
    public class AuthorConfiguration : IEntityTypeConfiguration<Author>
    {
        public void Configure(EntityTypeBuilder<Author> builder)
        {
            builder.ToTable("Authors");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnName("Id").IsRequired().ValueGeneratedNever();
            builder.Property(b => b.FirstName).HasColumnName("FirstName").HasMaxLength(100).IsRequired();
            builder.Property(b => b.LastName).HasColumnName("LastName").HasMaxLength(100).IsRequired();
            builder.Property(b => b.Birthday).HasColumnName("Birthday").IsRequired(false);
        }
    }
}

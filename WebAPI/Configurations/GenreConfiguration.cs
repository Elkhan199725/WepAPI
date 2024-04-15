using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Entities;

namespace WebAPI.Configurations
{
    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            
            builder.ToTable("Genres");

            builder.HasKey(g => g.Id);


            builder.Property(g => g.Id)
                   .IsRequired() 
                   .ValueGeneratedOnAdd(); // This indicates that the value is generated on add, typically by the database.

            builder.Property(g => g.Name)
                   .IsRequired() 
                   .HasMaxLength(100); 

            builder.HasIndex(g => g.Name).IsUnique();

        }
    }
}

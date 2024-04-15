using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Entities;

namespace WebAPI.Configurations
{
    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {

            builder.ToTable("Movies");


            builder.HasKey(m => m.Id);

            // Configure the properties
            builder.Property(m => m.Id)
                   .IsRequired() 
                   .ValueGeneratedOnAdd(); // Indicates that the value is generated on add, typically by the database.

            builder.Property(m => m.Title)
                   .IsRequired()
                   .HasMaxLength(200); 

            builder.Property(m => m.Director)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(m => m.ReleaseDate)
                   .IsRequired(); 

            builder.Property(m => m.Description)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(m => m.Rating)
                   .IsRequired()
                   .HasColumnType("decimal(3, 2)"); 

            builder.Property(m => m.CostPrice)
                   .IsRequired()
                   .HasColumnType("decimal(10, 2)"); 

            builder.Property(m => m.SellPrice)
                   .IsRequired()
                   .HasColumnType("decimal(10, 2)");

            // Configuring many-to-many relationship with Genre
            builder.HasMany(m => m.Genres)
                   .WithMany(g => g.Movies)
                   .UsingEntity(j => j.ToTable("MovieGenres")); // Configuring the join table explicitly.

            builder.HasIndex(m => m.Title).IsUnique();
        }
    }
}

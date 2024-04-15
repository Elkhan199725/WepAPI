namespace WebAPI.Entities;

public class Genre : BaseEntity
{
    public string Name { get; set; }

    // Navigation property for the many-to-many relationship
    public ICollection<Movie>? Movies { get; set; }
}

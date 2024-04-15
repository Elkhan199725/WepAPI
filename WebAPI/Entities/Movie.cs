namespace WebAPI.Entities;

public class Movie : BaseEntity
{
    public string Title { get; set; }
    public string Director { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; }
    public double Rating { get; set; }
    public double CostPrice { get; set; }
    public double SellPrice { get; set; }

    // Navigation property for the many-to-many relationship
    public ICollection<Genre>? Genres { get; set; }
}

namespace WebAPI.DTOs.Movie;

public class MovieUpdateDto
{
    public int Id { get; set; }  // Important for identifying which movie to update
    public string Title { get; set; }
    public string Director { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; }
    public double Rating { get; set; }
    public bool IsActive { get; set; } // If updating the active status is allowed
}


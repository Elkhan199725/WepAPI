namespace WebAPI.DTOs.Movie;

public class MovieReadDto
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Director { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; }
    public double Rating { get; set; }
    public bool IsActive { get; set; }
    public List<string> Genres { get; set; }
}

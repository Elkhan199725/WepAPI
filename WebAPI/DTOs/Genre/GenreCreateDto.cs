namespace WebAPI.DTOs.Genre;

public class GenreCreateDto
{
    public string Name { get; set; }
    public bool IsActive { get; set; } = true;
}

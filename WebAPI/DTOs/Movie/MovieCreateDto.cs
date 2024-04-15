﻿namespace WebAPI.DTOs.Movie;

public class MovieCreateDto
{
    public string Title { get; set; }
    public string Director { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Description { get; set; }
    public double Rating { get; set; }
    public bool IsActive { get; set; } = true; // Default to active when creating a new movie
}

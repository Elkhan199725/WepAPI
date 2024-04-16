using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.DTOs.Movie;
using WebAPI.Entities;

namespace WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    private readonly IMovieService _movieService;
    private readonly IGenreService _genreService;
    private readonly WebAPIDbContext _context;

    public MoviesController(IMovieService movieService, IGenreService genreService, WebAPIDbContext context)
    {
        _movieService = movieService;
        _genreService = genreService;
       _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovieReadDto>>> GetAllMovies()
    {
        // Eager loading genres with movies
        var movies = await _context.Movies.Include(m => m.Genres).ToListAsync();

        var movieDtos = movies.Select(movie => new MovieReadDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Director = movie.Director,
            ReleaseDate = movie.ReleaseDate,
            Description = movie.Description,
            Rating = movie.Rating,
            IsActive = movie.IsActive,
            Genres = movie.Genres?.Select(g => g.Name).ToList() ?? new List<string>() // Safeguard against null genres
        }).ToList();

        return Ok(movieDtos);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<MovieReadDto>>> GetActiveMovies()
    {
        var movies = await _movieService.GetActiveMoviesAsync();
        var movieDtos = movies.Select(movie => new MovieReadDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Director = movie.Director,
            ReleaseDate = movie.ReleaseDate,
            Description = movie.Description,
            Rating = movie.Rating,
            IsActive = movie.IsActive,
            Genres = movie.Genres?.Select(g => g.Name).ToList() ?? new List<string>()
        }).ToList();

        return Ok(movieDtos);
    }


    [HttpGet("{id}")]
    public async Task<ActionResult<MovieReadDto>> GetMovieById(int id)
    {
        var movie = await _movieService.GetMovieByIdAsync(id);
        if (movie == null)
            return NotFound($"No movie found with ID {id}.");

        var movieDto = new MovieReadDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Director = movie.Director,
            ReleaseDate = movie.ReleaseDate,
            Description = movie.Description,
            Rating = movie.Rating,
            IsActive = movie.IsActive,
            Genres = movie.Genres?.Select(g => g.Name).ToList() ?? new List<string>() // Safeguard against null genres
        };

        return Ok(movieDto);
    }


    [HttpPost]
    public async Task<ActionResult<MovieReadDto>> CreateMovie([FromBody] MovieCreateDto movieDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var movie = new Movie
        {
            Title = movieDto.Title,
            Director = movieDto.Director,
            ReleaseDate = movieDto.ReleaseDate,
            Description = movieDto.Description,
            Rating = movieDto.Rating,
            IsActive = movieDto.IsActive
        };

        await _movieService.CreateMovieAsync(movie, movieDto.GenreIds);

        var createdMovieDto = new MovieReadDto
        {
            Id = movie.Id,
            Title = movie.Title,
            Director = movie.Director,
            ReleaseDate = movie.ReleaseDate,
            Description = movie.Description,
            Rating = movie.Rating,
            IsActive = movie.IsActive,
            Genres = movie.Genres.Select(g => g.Name).ToList()
        };

        return CreatedAtAction(nameof(GetMovieById), new { id = createdMovieDto.Id }, createdMovieDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateMovie(int id, [FromBody] MovieUpdateDto movieDto)
    {
        if (id != movieDto.Id)
            return BadRequest("Mismatch between provided ID and the payload ID");

        await _movieService.UpdateMovieAsync(new Movie
        {
            Id = id,
            Title = movieDto.Title,
            Director = movieDto.Director,
            ReleaseDate = movieDto.ReleaseDate,
            Description = movieDto.Description,
            Rating = movieDto.Rating,
            IsActive = movieDto.IsActive
        }, movieDto.GenreIds);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        try
        {
            await _movieService.SoftDeleteMovieAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"No movie found with ID {id}. Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error while deleting the movie: {ex.Message}");
        }
    }

    [HttpDelete("{id}/hard")]
    public async Task<IActionResult> HardDeleteMovie(int id)
    {
        try
        {
            await _movieService.HardDeleteMovieAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest($"Cannot delete an active movie. Error: {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"No movie found with ID {id}. Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error while attempting to hard delete the movie: {ex.Message}");
        }
    }

    [HttpPost("{id}/restore")]
    public async Task<IActionResult> RestoreMovie(int id)
    {
        try
        {
            await _movieService.RestoreMovieAsync(id);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest($"Movie is already active. Error: {ex.Message}");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound($"No movie found with ID {id}. Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error while restoring the movie: {ex.Message}");
        }
    }
}

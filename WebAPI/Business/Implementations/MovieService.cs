using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Business.Implementations;

public class MovieService : IMovieService
{
    private readonly WebAPIDbContext _context;

    public MovieService(WebAPIDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Movie>> GetAllMoviesAsync()
    {
        return await _context.Movies.ToListAsync();
    }

    public async Task<IEnumerable<Movie>> GetActiveMoviesAsync()
    {
        return await _context.Movies.Where(m => m.IsActive).ToListAsync();
    }

    public async Task<Movie> GetMovieByIdAsync(int id)
    {
        var movie = await _context.Movies
                                  .Include(m => m.Genres) // Ensure genres are loaded
                                  .FirstOrDefaultAsync(m => m.Id == id);
        if (movie == null)
            throw new KeyNotFoundException($"No movie found with ID {id}.");
        return movie;
    }

    public async Task<Movie> GetMovieByTitleAsync(string title)
    {
        var movie = await _context.Movies.FirstOrDefaultAsync(m => m.Title == title);
        if (movie == null)
            throw new KeyNotFoundException($"No movie found with the specified title.");
        return movie;
    }

    public async Task<Movie> CreateMovieAsync(Movie movie, List<int> genreIds)
    {
        if (movie == null)
            throw new ArgumentNullException(nameof(movie));

        // Fetch genres from the database
        movie.Genres = await _context.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();

        _context.Movies.Add(movie);
        movie.SetCreatedDate();
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task UpdateMovieAsync(Movie movie, List<int> genreIds)
    {
        if (movie == null)
            throw new ArgumentNullException(nameof(movie));

        var existingMovie = await _context.Movies.Include(m => m.Genres)
                                                 .FirstOrDefaultAsync(m => m.Id == movie.Id);
        if (existingMovie == null)
            throw new KeyNotFoundException("Movie to update not found.");

        // Update scalar properties
        existingMovie.Title = movie.Title;
        existingMovie.Director = movie.Director;
        existingMovie.ReleaseDate = movie.ReleaseDate;
        existingMovie.Description = movie.Description;
        existingMovie.Rating = movie.Rating;
        existingMovie.IsActive = movie.IsActive;

        // Update genres
        existingMovie.Genres.Clear();
        var genresToAdd = await _context.Genres.Where(g => genreIds.Contains(g.Id)).ToListAsync();
        existingMovie.Genres.AddRange(genresToAdd);

        await _context.SaveChangesAsync();
    }

    public async Task SoftDeleteMovieAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
            throw new KeyNotFoundException("Movie to delete not found.");
        if (!movie.IsActive)
            throw new InvalidOperationException("Movie is already inactive.");

        movie.IsActive = false;
        movie.UpdateModifiedDate();
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
    }

    public async Task HardDeleteMovieAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
            throw new KeyNotFoundException("Movie not found.");

        if (!movie.IsActive)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
        else
        {
            throw new InvalidOperationException("Only inactive movies can be permanently deleted.");
        }
    }

    public async Task RestoreMovieAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
            throw new KeyNotFoundException("Movie to restore not found.");
        if (movie.IsActive)
            throw new InvalidOperationException("Movie is already active.");

        movie.IsActive = true;
        movie.UpdateModifiedDate();
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
    }
}

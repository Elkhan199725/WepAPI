using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Entities;

public interface IMovieService
{
    Task<IEnumerable<Movie>> GetAllMoviesAsync();
    Task<IEnumerable<Movie>> GetActiveMoviesAsync(); // Only active movies
    Task<Movie> GetMovieByIdAsync(int id);
    Task<Movie> GetMovieByTitleAsync(string title);
    Task<Movie> CreateMovieAsync(Movie movie, List<int> genreIds);
    Task UpdateMovieAsync(Movie movie, List<int> genreIds);
    Task SoftDeleteMovieAsync(int id); // Soft delete (set IsActive to false)
    Task HardDeleteMovieAsync(int id); // Hard delete (remove from DB)
    Task RestoreMovieAsync(int id); // Restore a soft-deleted movie
}

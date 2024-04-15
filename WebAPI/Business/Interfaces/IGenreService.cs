using System.Collections.Generic;
using System.Threading.Tasks;
using WebAPI.Entities;

public interface IGenreService
{
    Task<IEnumerable<Genre>> GetAllGenresAsync();
    Task<IEnumerable<Genre>> GetActiveGenresAsync(); // Only active genres
    Task<Genre> GetGenreByIdAsync(int id);
    Task<Genre> GetGenreByNameAsync(string name);
    Task<Genre> CreateGenreAsync(Genre genre);
    Task UpdateGenreAsync(Genre genre);
    Task SoftDeleteGenreAsync(int id); // Soft delete (set IsActive to false)
    Task HardDeleteGenreAsync(int id); // Hard delete (remove from DB)
    Task RestoreGenreAsync(int id); // Restore a soft-deleted genre
}

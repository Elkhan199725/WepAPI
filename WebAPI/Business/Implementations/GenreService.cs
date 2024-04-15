using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Entities;

namespace WebAPI.Business.Implementations
{
    public class GenreService : IGenreService
    {
        private readonly WebAPIDbContext _context;

        public GenreService(WebAPIDbContext context)
        {
            _context = context;
        }

        public async Task<Genre> CreateGenreAsync(Genre genre)
        {
            if (genre == null)
                throw new ArgumentNullException(nameof(genre));

            _context.Genres.Add(genre);

            genre.SetCreatedDate();
            await _context.SaveChangesAsync();
            return genre;
        }

        public async Task<IEnumerable<Genre>> GetActiveGenresAsync()
        {
            return await _context.Genres.Where(g => g.IsActive).ToListAsync();
        }

        public async Task<IEnumerable<Genre>> GetAllGenresAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre> GetGenreByIdAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found.");
            return genre;
        }

        public async Task<Genre> GetGenreByNameAsync(string name)
        {
            var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == name);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found with the specified name.");
            return genre;
        }

        public async Task HardDeleteGenreAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found.");

            if (!genre.IsActive)
            {
                _context.Genres.Remove(genre);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Only inactive genres can be permanently deleted.");
            }
        }

        public async Task RestoreGenreAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null || genre.IsActive)
                throw new InvalidOperationException("Genre not found or is already active.");

            genre.IsActive = true;
            genre.UpdateModifiedDate();
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteGenreAsync(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre == null)
                throw new KeyNotFoundException("Genre not found.");
            if (genre.IsActive == false)
                throw new FileNotFoundException("Genre must be active!");
            genre.IsActive = false;
            genre.UpdateModifiedDate();
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGenreAsync(Genre genre)
        {
            if (genre == null)
                throw new ArgumentNullException(nameof(genre));

            var existingGenre = await _context.Genres.FindAsync(genre.Id);
            if (existingGenre == null)
                throw new KeyNotFoundException("Genre not found.");

            // Updating properties explicitly
            existingGenre.Name = genre.Name;
            existingGenre.IsActive = genre.IsActive;

            existingGenre.UpdateModifiedDate();
            await _context.SaveChangesAsync();
        }
    }
}

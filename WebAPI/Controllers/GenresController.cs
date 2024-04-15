using Microsoft.AspNetCore.Mvc;
using WebAPI.DTOs.Genre;
using WebAPI.Entities;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GenreReadDto>>> GetAllGenres()
        {
            var genres = await _genreService.GetAllGenresAsync();
            var genreDtos = genres.Select(g => new GenreReadDto
            {
                Id = g.Id,
                Name = g.Name,
                IsActive = g.IsActive
            }).ToList();

            return Ok(genreDtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<GenreReadDto>>> GetActiveGenres()
        {
            var genres = await _genreService.GetActiveGenresAsync();
            var genreDtos = genres.Select(g => new GenreReadDto
            {
                Id = g.Id,
                Name = g.Name,
                IsActive = g.IsActive
            }).ToList();

            return Ok(genreDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreReadDto>> GetGenreById(int id)
        {
            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
                return NotFound($"No genre found with ID {id}.");

            var genreDto = new GenreReadDto
            {
                Id = genre.Id,
                Name = genre.Name,
                IsActive = genre.IsActive
            };

            return Ok(genreDto);
        }

        [HttpGet("name/{name}")]
        public async Task<ActionResult<GenreReadDto>> GetGenreByName(string name)
        {
            var genre = await _genreService.GetGenreByNameAsync(name);
            if (genre == null)
                return NotFound($"No genre found with name '{name}'.");

            var genreDto = new GenreReadDto
            {
                Id = genre.Id,
                Name = genre.Name,
                IsActive = genre.IsActive
            };

            return Ok(genreDto);
        }

        [HttpPost]
        public async Task<ActionResult<GenreReadDto>> CreateGenre([FromBody] GenreCreateDto genreDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = new Genre
            {
                Name = genreDto.Name,
                IsActive = genreDto.IsActive
            };

            genre = await _genreService.CreateGenreAsync(genre);

            var createdGenreDto = new GenreReadDto
            {
                Id = genre.Id,
                Name = genre.Name,
                IsActive = genre.IsActive
            };

            return CreatedAtAction(nameof(GetGenreById), new { id = createdGenreDto.Id }, createdGenreDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGenre(int id, [FromBody] GenreUpdateDto genreDto)
        {
            if (id != genreDto.Id)
                return BadRequest("ID mismatch between the route and model.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var genre = await _genreService.GetGenreByIdAsync(id);
            if (genre == null)
                return NotFound($"No genre found with ID {id}.");

            genre.Name = genreDto.Name;
            genre.IsActive = genreDto.IsActive;

            await _genreService.UpdateGenreAsync(genre);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGenre(int id)
        {
            try
            {
                await _genreService.SoftDeleteGenreAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"No genre found with ID {id} to delete.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while deleting genre: {ex.Message}");
            }
        }

        [HttpPost("{id}/restore")]
        public async Task<IActionResult> RestoreGenre(int id)
        {
            try
            {
                await _genreService.RestoreGenreAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"No genre found with ID {id} to restore.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while restoring genre: {ex.Message}");
            }
        }

        [HttpDelete("{id}/hard")]
        public async Task<IActionResult> HardDeleteGenre(int id)
        {
            try
            {
                await _genreService.HardDeleteGenreAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound($"No genre found with ID {id}.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error while performing hard deletion: {ex.Message}");
            }
        }
    }
}

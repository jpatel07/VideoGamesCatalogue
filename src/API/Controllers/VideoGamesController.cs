using Core.DTO;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideoGamesController : ControllerBase
    {

        private readonly IVideoGameService _gameService;

        public VideoGamesController(IVideoGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<VideoGameDto>>> GetVideoGames(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (pageNumber < 1) return BadRequest("pageNumber must be >= 1.");
            if (pageSize < 1 || pageSize > 100) return BadRequest("pageSize must be between 1 and 100.");

            return Ok(await _gameService.GetGamesAsync(pageNumber, pageSize));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VideoGameDto>> GetVideoGame(int id)
        {
            var game = await _gameService.GetByIdAsync(id);

            if (game is null)
                return NotFound();

            return Ok(game);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, VideoGameDto game)
        {
            if (id != game.Id)
                return BadRequest();

            try
            {
                await _gameService.UpdateAsync(game);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}

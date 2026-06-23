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

        public VideoGamesController(IVideoGameService gameService )
        {
            _gameService = gameService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VideoGameDto>>> GetVideoGames()
        {
            return Ok(await _gameService.GetGamesAsync());
        }
    }
}

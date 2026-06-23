using Core.DTO;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class VideoGameService : IVideoGameService
    {
        private readonly GamesCatalogueContext _context;

        public VideoGameService(GamesCatalogueContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<VideoGameDto>> GetGamesAsync()
        {
            return await _context.VideoGames.OrderBy(g => g.Name).Select(
                d => new VideoGameDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    DatePublished = d.DatePublished,
                    Author = d.Author,
                    Description = d.Description,
                    GamePlatform = d.GamePlatform,
                    Genre = d.Genre,
                    AggregateRating = d.AggregateRating
                
                }).ToListAsync();
        }
    }
}

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

        public async Task UpdateAsync(VideoGameDto game)
        {
            ArgumentNullException.ThrowIfNull(game.Name, nameof(game.Name));
            ArgumentNullException.ThrowIfNull(game.Author, nameof(game.Author));
            ArgumentNullException.ThrowIfNull(game.Description, nameof(game.Description));
            ArgumentNullException.ThrowIfNull(game.GamePlatform, nameof(game.GamePlatform));
            ArgumentNullException.ThrowIfNull(game.Genre, nameof(game.Genre));

            var entity = await _context.VideoGames.FirstOrDefaultAsync(g => g.Id == game.Id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Video game with id {game.Id} was not found.");
            }

            entity.Name = game.Name;
            entity.DatePublished = game.DatePublished;
            entity.Author = game.Author;
            entity.Description = game.Description;
            entity.GamePlatform = game.GamePlatform;
            entity.Genre = game.Genre;
            entity.AggregateRating = game.AggregateRating;

            await _context.SaveChangesAsync();
        }
    }
}

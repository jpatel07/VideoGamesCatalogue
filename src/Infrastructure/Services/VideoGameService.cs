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

        public async Task<PagedResult<VideoGameDto>> GetGamesAsync(int pageNumber, int pageSize)
        {
            var query = _context.VideoGames.OrderBy(g => g.Name);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new VideoGameDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    DatePublished = d.DatePublished,
                    Author = d.Author,
                    Description = d.Description,
                    GamePlatform = d.GamePlatform,
                    Genre = d.Genre,
                    AggregateRating = d.AggregateRating
                })
                .ToListAsync();

            return new PagedResult<VideoGameDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<VideoGameDto?> GetByIdAsync(int id)
        {
            return await _context.VideoGames
                .AsNoTracking()
                .Where(g => g.Id == id)
                .Select(d => new VideoGameDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    DatePublished = d.DatePublished,
                    Author = d.Author,
                    Description = d.Description,
                    GamePlatform = d.GamePlatform,
                    Genre = d.Genre,
                    AggregateRating = d.AggregateRating
                })
                .FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(int id, UpdateVideoGameRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var entity = await _context.VideoGames.FirstOrDefaultAsync(g => g.Id == id);

            if (entity is null)
            {
                throw new KeyNotFoundException($"Video game with id {id} was not found.");
            }

            entity.Name = request.Name;
            entity.DatePublished = request.DatePublished;
            entity.Author = request.Author;
            entity.Description = request.Description;
            entity.GamePlatform = request.GamePlatform;
            entity.Genre = request.Genre;
            entity.AggregateRating = request.AggregateRating;

            await _context.SaveChangesAsync();
        }
    }
}

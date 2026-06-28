using Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IVideoGameService
    {
        Task<PagedResult<VideoGameDto>> GetGamesAsync(int pageNumber, int pageSize);
        Task<VideoGameDto?> GetByIdAsync(int id);
        Task UpdateAsync(VideoGameDto game);
    }
}

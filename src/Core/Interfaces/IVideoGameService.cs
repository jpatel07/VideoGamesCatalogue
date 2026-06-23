using Core.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    public interface IVideoGameService
    {
        Task<IEnumerable<VideoGameDto>> GetGamesAsync(); 
    }
}

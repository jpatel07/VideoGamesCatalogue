using System;
using System.Collections.Generic;
using System.Text;

namespace Core.DTO
{
    public record VideoGameDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required DateOnly DatePublished { get; set; }
        public required string Author { get; set; }
        public required string Description { get; set; }
        public required string GamePlatform { get; set; }
        public required string Genre { get; set; }
        public decimal? AggregateRating { get; set; }
    }
}

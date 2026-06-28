using System;
using System.ComponentModel.DataAnnotations;

namespace Core.DTO
{
    public record UpdateVideoGameRequest
    {
        [Required]
        public string Name { get; init; } = default!;

        [Required]
        public DateOnly DatePublished { get; init; }

        [Required]
        public string Author { get; init; } = default!;

        [Required]
        public string Description { get; init; } = default!;

        [Required]
        public string GamePlatform { get; init; } = default!;

        [Required]
        public string Genre { get; init; } = default!;

        public decimal? AggregateRating { get; init; }
    }
}

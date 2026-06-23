using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core.DTO
{
    public record VideoGameDto
    {
        public int Id { get; set; }
        [Required]
        public string? Name { get; set; }
        [Required]
        public DateOnly DatePublished { get; set; }
        [Required]
        public string? Author { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? GamePlatform { get; set; }
        [Required]
        public string? Genre { get; set; }
        public decimal? AggregateRating { get; set; }
    }
}

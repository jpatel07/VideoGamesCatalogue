using Core.Entities;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Infrastructure.Data.SeedData
{
    public class GamesCatalogueContextSeed
    {
        public static async Task SeedAsync(GamesCatalogueContext context)
        {
            if (await context.VideoGames.AnyAsync())
            {
                return;
            }

            var csvPath = "../Infrastructure/Data/SeedData/videogames_clean_with_schemaname.csv";
            if(!File.Exists(csvPath))
            {
                throw new FileNotFoundException(csvPath);
            }
            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                TrimOptions = TrimOptions.Trim,
                PrepareHeaderForMatch = args => args.Header.Trim().ToLowerInvariant(),
                HeaderValidated = null,
                MissingFieldFound = null,
                BadDataFound = null
            });

            var rows = csv.GetRecords<VideoGameCsvRow>().ToList();

            var games = rows
                .Where(r => !string.IsNullOrWhiteSpace(r.Name))
                .Select(r =>
                {
                    var hasDate = DateOnly.TryParseExact(
                        r.DatePublished?.Trim(),
                        "MMM dd, yyyy",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out var parsedDate);

                    if (!hasDate)
                    {
                        return null;
                    }

                    var hasRating = decimal.TryParse(
                        r.AggregateRating,
                        NumberStyles.Number,
                        CultureInfo.InvariantCulture,
                        out var rating);

                    return new VideoGame
                    {
                        Name = r.Name.Trim(),
                        DatePublished = parsedDate,
                        Author = (r.Author ?? string.Empty).Trim(),
                        Description = (r.Description ?? string.Empty).Trim(),
                        GamePlatform = (r.GamePlatform ?? string.Empty).Trim(),
                        Genre = (r.Genre ?? string.Empty).Trim(),
                        AggregateRating = hasRating ? rating : null
                    };
                })
                .Where(g => g is not null)
                .Cast<VideoGame>()
                .ToList();

            if (games.Count == 0)
            {
                return;
            }

            await context.VideoGames.AddRangeAsync(games);
            await context.SaveChangesAsync();
        }

        private sealed class VideoGameCsvRow
        {
            public string Name { get; set; } = string.Empty;
            public string DatePublished { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string GamePlatform { get; set; } = string.Empty;
            public string Genre { get; set; } = string.Empty;
            public string AggregateRating { get; set; } = string.Empty;
        }
    }
}
using Core.DTO;
using Infrastructure.Data;
using Infrastructure.Data.SeedData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Tests
{
    public class VideoGamesControllerShould : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory _factory;
        private HttpClient _client = default!;


        public VideoGamesControllerShould(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        public async Task InitializeAsync()
        {
            _client = _factory.CreateClient();

            await using var scope = _factory.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<GamesCatalogueContext>();

            await db.Database.EnsureDeletedAsync();
            await db.Database.MigrateAsync();
            await GamesCatalogueContextSeed.SeedAsync(db);
        }
        public async Task DisposeAsync()
        {
            await using var scope = _factory.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<GamesCatalogueContext>();


            await db.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task Get_list_async_returns_ok_and_seeded_games()
        {
            var response = await _client.GetAsync("/api/VideoGames");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var page = await response.Content.ReadFromJsonAsync<PagedResult<VideoGameDto>>();
            Assert.NotNull(page);
            Assert.NotNull(page.Items);
            Assert.NotEmpty(page.Items);
        }
        [Fact]
        public async Task Update_game_with_valid_payload_returns_no_content_and_persists_changes()
        {
            int gameId;

            await using (var arrangeScope = _factory.Services.CreateAsyncScope())
            {
                var db = arrangeScope.ServiceProvider.GetRequiredService<GamesCatalogueContext>();
                var existing = await db.VideoGames.AsNoTracking().FirstAsync();
                gameId = existing.Id;
            }

            var payload = new VideoGameDto
            {
                Id = gameId,
                Name = "Updated Integration Test Game",
                DatePublished = new DateOnly(2024, 1, 15),
                Author = "Integration Test Author",
                Description = "Updated description from integration test.",
                GamePlatform = "PC",
                Genre = "Action",
                AggregateRating = 4.5m
            };

            var response = await _client.PutAsJsonAsync($"/api/VideoGames/{gameId}", payload);

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            await using var assertScope = _factory.Services.CreateAsyncScope();
            var assertDb = assertScope.ServiceProvider.GetRequiredService<GamesCatalogueContext>();
            var updated = await assertDb.VideoGames.AsNoTracking().FirstAsync(g => g.Id == gameId);

            Assert.Equal(payload.Name, updated.Name);
            Assert.Equal(payload.Author, updated.Author);
            Assert.Equal(payload.Description, updated.Description);
            Assert.Equal(payload.AggregateRating, updated.AggregateRating);
        }
        [Fact]
        public async Task Update_game_with_missing_required_field_returns_bad_request()
        {
            int gameId;

            await using (var arrangeScope = _factory.Services.CreateAsyncScope())
            {
                var db = arrangeScope.ServiceProvider.GetRequiredService<GamesCatalogueContext>();
                gameId = await db.VideoGames.Select(g => g.Id).FirstAsync();
            }

            // Missing "name" on purpose
            var invalidPayload = new
            {
                id = gameId,
                datePublished = "2024-01-15",
                author = "Author",
                description = "Description",
                gamePlatform = "PC",
                genre = "Action",
                aggregateRating = 4.2
            };

            var response = await _client.PutAsJsonAsync($"/api/VideoGames/{gameId}", invalidPayload);

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
            Assert.NotNull(problem);
            Assert.True(problem?.Errors.Keys.Any(k => k.Contains("name", StringComparison.OrdinalIgnoreCase))
                    , $"Error contains: {string.Join(", ", problem?.Errors?.Keys)}");
        }

        [Fact]
        public async Task Get_list_async_second_page_returns_different_items_from_first_page()
        {
            var firstPageResponse = await _client.GetAsync("/api/VideoGames?pageNumber=1&pageSize=3");
            var secondPageResponse = await _client.GetAsync("/api/VideoGames?pageNumber=2&pageSize=3");

            Assert.Equal(HttpStatusCode.OK, firstPageResponse.StatusCode);
            Assert.Equal(HttpStatusCode.OK, secondPageResponse.StatusCode);

            var firstPage = await firstPageResponse.Content.ReadFromJsonAsync<PagedResult<VideoGameDto>>();
            var secondPage = await secondPageResponse.Content.ReadFromJsonAsync<PagedResult<VideoGameDto>>();

            Assert.NotNull(firstPage);
            Assert.NotNull(secondPage);

            var firstIds = firstPage.Items.Select(g => g.Id).ToHashSet();
            var secondIds = secondPage.Items.Select(g => g.Id).ToHashSet();

            Assert.Empty(firstIds.Intersect(secondIds));
        }

        [Fact]
        public async Task Get_list_async_returns_correct_total_count_and_total_pages()
        {
            int totalGames;

            await using (var scope = _factory.Services.CreateAsyncScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<GamesCatalogueContext>();
                totalGames = await db.VideoGames.CountAsync();
            }

            var response = await _client.GetAsync("/api/VideoGames?pageNumber=1&pageSize=5");

            var page = await response.Content.ReadFromJsonAsync<PagedResult<VideoGameDto>>();
            Assert.NotNull(page);
            Assert.Equal(totalGames, page.TotalCount);
            Assert.Equal((int)Math.Ceiling((double)totalGames / 5), page.TotalPages);
        }

        [Fact]
        public async Task Get_by_id_returns_ok_and_matching_game()
        {
            int gameId;

            await using (var arrangeScope = _factory.Services.CreateAsyncScope())
            {
                var db = arrangeScope.ServiceProvider.GetRequiredService<GamesCatalogueContext>();
                var existing = await db.VideoGames.AsNoTracking().FirstAsync();
                gameId = existing.Id;
            }

            var response = await _client.GetAsync($"/api/VideoGames/{gameId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var game = await response.Content.ReadFromJsonAsync<VideoGameDto>();
            Assert.NotNull(game);
            Assert.Equal(gameId, game!.Id);
        }

        [Fact]
        public async Task Get_by_id_with_unknown_id_returns_not_found()
        {
            var response = await _client.GetAsync("/api/VideoGames/-1");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

using Core.DTO;
using Infrastructure.Data;
using Infrastructure.Data.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

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

            var games = await response.Content.ReadFromJsonAsync<List<VideoGameDto>>();
            Assert.NotNull(games);
            Assert.NotEmpty(games);
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


    }
}

using Core.DTO;
using Infrastructure.Data;
using Infrastructure.Data.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace API.Tests
{
    public  class VideoGamesControllerShould : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
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
    }
}

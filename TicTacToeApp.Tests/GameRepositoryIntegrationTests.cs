using Microsoft.Extensions.Logging;
using TicTacToeApp.API.Data;
using TicTacToeApp.API.Entity;
using TicTacToeApp.API.Repositories;
using Moq;
using TicTacToeApp.API.Entity.Enums;

namespace TicTacToeApp.Tests;

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

public class GameRepositoryIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgreSqlContainer;
    private TicTacToeContext? _dbContext;
    private GameAsyncRepository? _repository;
    private Mock<ILogger<GameAsyncRepository>> _loggerMock;

    public GameRepositoryIntegrationTests()
    {
        _postgreSqlContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _loggerMock = new Mock<ILogger<GameAsyncRepository>>();
    }

    public async Task InitializeAsync()
    {
        await _postgreSqlContainer.StartAsync();

        var options = new DbContextOptionsBuilder<TicTacToeContext>()
            .UseNpgsql(_postgreSqlContainer.GetConnectionString())
            .Options;

        _dbContext = new TicTacToeContext(options);
        await _dbContext.Database.EnsureCreatedAsync();

        // Заполняем тестовыми данными


        for (int i = 0; i < 4; i++)
        {
            var game = new Game
            {
                Id = Guid.NewGuid(),
                Status = StatusGame.Active,
                Board = new string?[][]
                {
                },
                Result = ResultGame.None,
                CreatedAt = default,
                CurrentMove = Player.X,
                CurrentStep = 0
            };
            _dbContext.Games.Add(game);
        }

        await _dbContext.SaveChangesAsync();


        _repository = new GameAsyncRepository(_dbContext, _loggerMock.Object);
    }

    public async Task DisposeAsync()
    {
        await _dbContext!.DisposeAsync();
        await _postgreSqlContainer.DisposeAsync();
    }

    [Fact]
    public async Task GetGamesAsync_ShouldReturn5GamesFromDatabase()
    {
        // Act
        var games = await _repository!.GetGamesAsync(CancellationToken.None);

        // Assert
        Assert.Equal(5, games.Count());
    }
}
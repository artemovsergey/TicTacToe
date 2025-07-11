using TicTacToeApp.API.Repositories;

namespace TicTacToeApp.Tests;

using Xunit;

public class GameAsyncRepositoryTests
{
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(10)]
    public void CreateEmptyBoard_ShouldReturnCorrectlyInitializedBoard(int size)
    {
        // Arrange
        var repository = new GameAsyncRepository(null!, null!); // Заглушки для зависимостей
        
        // Act
        var board = repository.CreateEmptyBoard(size);
        
        // Assert
        Assert.NotNull(board);
        Assert.Equal(size, board.Length);
        
        foreach (var row in board)
        {
            Assert.NotNull(row);
            Assert.Equal(size, row.Length);
            Assert.All(row, cell => Assert.Null(cell));
        }
    }
    
    [Fact]
    public void CreateEmptyBoard_WithZeroSize_ShouldReturnEmptyBoard()
    {
        // Arrange
        var repository = new GameAsyncRepository(null!, null!);
        
        // Act
        var board = repository.CreateEmptyBoard(0);
        
        // Assert
        Assert.NotNull(board);
        Assert.Empty(board);
    }
}
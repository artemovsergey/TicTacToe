using System.Security.Cryptography;
using System.Text;
using TicTacToeApp.API.Entity;

namespace TicTacToeApp.API.Services;

internal sealed class EtagService
{
    public static string GenerateETag(Game game)
    {
        // Используем хэш важных полей игры
        var content = $"{game.Id}-{game.Board}";
        var bytes = Encoding.UTF8.GetBytes(content);
        var hash = SHA256.HashData(bytes);
        return $"\"{Convert.ToBase64String(hash)}\"";
    }
}
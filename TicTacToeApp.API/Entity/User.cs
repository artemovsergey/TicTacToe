using System.Text.Json.Serialization;

namespace TicTacToeApp.API.Entity;

public class User
{
    public int Id { get; set; } //= Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    //
    // public uint Age { get; set;}
    //
    // public uint Age2 { get; set;}
    //
    // public uint Age3 { get; set;}
    
    //
    // [JsonIgnore]
    // public ICollection<Game> Games { get; set; } = default!;
}
using System.Text.Json.Serialization;

namespace TicTacToeApp.API.Response;

public sealed class ErrorResponse
{
    public ErrorResponse(string statusCode, string? message = "", string? detail = "")
    {
        StatusCode = statusCode;
        Message = message;
        Detail = detail;
    }

    [JsonInclude]
    internal string StatusCode { get; set; } = string.Empty;
    [JsonInclude]
    internal string? Message { get; set; } = string.Empty;
    [JsonInclude]
    internal string? Detail { get; set; } = string.Empty;
}

using System.Data.SqlTypes;
using System.Net;
using System.Text.Json;
using TicTacToeApp.API.Exceptions;
using TicTacToeApp.API.Response;

namespace TicTacToeApp.API.Middleware;

public sealed class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlerMiddleware> logger,
        IHostEnvironment env
    )
    {
        _env = env;
        _logger = logger;
        _next = next;
    }

    // когда мы работает с middleware у нас есть доступ к контексту HttpContext
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // мы получим контекст http и передадим его дальше другому middleware вниз
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await ConvertExceptionAsync(context, ex);
        }
    }

    private Task ConvertExceptionAsync(HttpContext context, System.Exception exception)
    {
        context.Response.ContentType = "application/json";
        var httpStatusCode = HttpStatusCode.InternalServerError;

        var message = string.Empty;
        var messagedetail = string.Empty;

        switch (exception)
        {
            case Npgsql.PostgresException ex:
                httpStatusCode = HttpStatusCode.InternalServerError;
                message = ex.Message;
                messagedetail = ex.StackTrace?.ToString();
                break;
            case BadHttpRequestException badRequestException:
                httpStatusCode = HttpStatusCode.BadRequest;
                message = badRequestException.Message;
                messagedetail = badRequestException.StackTrace?.ToString();
                break;
            
            case NotFoundException ex:
                httpStatusCode = HttpStatusCode.NotFound;
                message = ex.Message;
                messagedetail = ex.StackTrace?.ToString();
                break;
            
            case ArgumentException ex:
                httpStatusCode = HttpStatusCode.BadRequest;
                message = ex.Message;
                messagedetail = ex.StackTrace?.ToString();
                break;
            
            case SqlTypeException sqlEx:
                httpStatusCode = HttpStatusCode.BadRequest;
                message = sqlEx.Message;
                messagedetail = sqlEx.StackTrace?.ToString();
                break;
            
            case Exception ex:
                httpStatusCode = HttpStatusCode.InternalServerError;
                message = ex.Message;
                messagedetail = ex.StackTrace?.ToString();
                break;
        }

        context.Response.StatusCode = (int)httpStatusCode;

        var response = _env.IsDevelopment()
            ? new ErrorResponse(context.Response.StatusCode.ToString(), message, messagedetail)
            : new ErrorResponse(context.Response.StatusCode.ToString(), "Internal Server Error");

        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
        var result = JsonSerializer.Serialize(response, options);
        return context.Response.WriteAsync(result);
    }
}
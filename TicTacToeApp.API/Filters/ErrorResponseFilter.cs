using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using TicTacToeApp.API.Response;

namespace TicTacToeApp.API.Filters;

// Фильтр для добавления примеров ErrorResponse
public class ErrorResponseSchemaFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(ErrorResponse))
        {
            schema.Example = new OpenApiObject
            {
                ["statusCode"] = new OpenApiString("400"),
                ["message"] = new OpenApiString("Invalid request"),
                ["detail"] = new OpenApiString("The 'id' parameter must be a positive number")
            };
        }
    }
}


public class ErrorResponseOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var statusCodes = new Dictionary<string, (string Message, string Detail)>
        {
            ["400"] = ("Invalid request", "The 'id' parameter must be a positive number"),
            ["404"] = ("Resource not found", "The requested resource was not found"),
            ["409"] = ("Conflict", "Поле занято"),
            ["412"] = ("Precondition failed", "Несогласованное состояние объекта Game"),
            ["500"] = ("Internal server error", "An unexpected error occurred")
        };

        foreach (var (code, (message, detail)) in statusCodes)
        {
            if (operation.Responses.TryGetValue(code, out var response) &&
                response.Content.TryGetValue("application/json", out var mediaType))
            {
                mediaType.Example = new OpenApiObject
                {
                    ["statusCode"] = new OpenApiInteger(int.Parse(code)),
                    ["message"] = new OpenApiString(message),
                    ["detail"] = new OpenApiString(detail)
                };
            }
        }
    }
}
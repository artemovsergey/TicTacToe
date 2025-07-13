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
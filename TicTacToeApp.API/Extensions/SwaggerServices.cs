using Microsoft.OpenApi.Models;
using TicTacToeApp.API.Filters;

namespace TicTacToeApp.API.Extensions;

public static class SwaggerServices
{
    public static IServiceCollection AddSwaggerServices(
        this IServiceCollection services
    )
    {
        services.AddOpenApi();
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "Крестики-нолики",
                    Version = "v1",
                    Description = "API для пользователей",
                    Contact = new OpenApiContact
                    {
                        Url = new Uri("https://github.com/artemovsergey/TicTacToe"),
                        Email = "artik3314@gmail.com",
                    }
                }
            );

            c.EnableAnnotations();
            c.SchemaFilter<ErrorResponseSchemaFilter>();
            c.OperationFilter<ErrorResponseOperationFilter>();
        });

        return services;
    }
}

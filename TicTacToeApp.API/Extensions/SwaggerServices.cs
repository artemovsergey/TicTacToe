using Microsoft.OpenApi.Models;

namespace TicTacToeApp.API.Extensions;

public static class SwaggerServices
{
    public static IServiceCollection AddSwaggerServices(
        this IServiceCollection services
    )
    {
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
                        Name = "Artemov Sergey",
                        Email = "artik3314@gmail.com",
                    },
                }
            );

            c.EnableAnnotations();
        });

        return services;
    }
}
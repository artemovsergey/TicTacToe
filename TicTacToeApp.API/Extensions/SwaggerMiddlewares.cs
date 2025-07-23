namespace TicTacToeApp.API.Extensions;

public static class SwaggerMiddlewares
{
    public static WebApplication UseSwaggerMiddleware(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Крестики-Нолики API");
            c.RoutePrefix = string.Empty;
        });
        
        app.MapOpenApi();
        
        return app;
    }
}
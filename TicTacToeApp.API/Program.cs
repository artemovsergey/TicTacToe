using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TicTacToeApp.API.Data;
using TicTacToeApp.API.Endpoints;
using TicTacToeApp.API.Extensions;
using TicTacToeApp.API.Interfaces;
using TicTacToeApp.API.Repositories;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOpenApi();
builder.Services.AddSwaggerServices();
builder.Services.AddScoped<IGameAsyncRepository, GameAsyncRepository>();
builder.Services.AddDbContext<TicTacToeContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

var app = builder.Build();
app.UseMiddleware<TicTacToeApp.API.Middleware.ExceptionHandlerMiddleware>();
app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Крестики-Нолики API");
    c.RoutePrefix = string.Empty;
});

app.MapOpenApi();

app.MapGet("/health", () => Results.Ok("Проверка работы"))
    .WithTags("TicTacToeApp.API")
    .WithName("CheckHealth")
    .WithSummary("Проверка здоровья приложения");


app.UseGameEndpoints();
app.Run();
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TicTacToeApp.API.Data;
using TicTacToeApp.API.Endpoints;
using TicTacToeApp.API.Interfaces;
using TicTacToeApp.API.Repositories;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOpenApi();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddDbContext<TicTacToeContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));


var app = builder.Build();
app.MapOpenApi();
app.MapGet("/health", () => Results.Ok("Проверка работы"));
app.UseGameEndpoints(cancellationToken: default!);
app.Run();












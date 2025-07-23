using Microsoft.EntityFrameworkCore;
using TicTacToeApp.API.Data;
using TicTacToeApp.API.Endpoints;
using TicTacToeApp.API.Extensions;
using TicTacToeApp.API.Interfaces;
using TicTacToeApp.API.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerializeServices();
builder.Services.AddCors();
builder.Services.AddSwaggerServices();

builder.Services.AddScoped<IGameAsyncRepository, GameAsyncRepository>();
builder.Services.AddDbContext<TicTacToeContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

var app = builder.Build();

if (app.Environment.IsProduction())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TicTacToeContext>();
    dbContext.Database.EnsureDeleted();
    dbContext.Database.Migrate();
}

app.UseMiddleware<TicTacToeApp.API.Middleware.ExceptionHandlerMiddleware>();
app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());
app.UseSwaggerMiddleware();
app.UseGameEndpoints();
app.Run();


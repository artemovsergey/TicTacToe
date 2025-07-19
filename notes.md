- launchSettings

`
 "Container (Dockerfile)": {
      "commandName": "Docker",
      "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}",
      "environmentVariables": {
        "ASPNETCORE_HTTPS_PORTS": "8081",
        "ASPNETCORE_HTTP_PORTS": "8080"
      },
      "publishAllPorts": true,
      "useSSL": true
    },
`

- можно вместо своей модели ошибки, использовать ProblemDetails:

```Csharp
    var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Неверный ход",
                    Detail = error.Message
                };
```

---

Зачем это?

```Csharp
//  Marvin.Cache.Headers для eTag
            builder.Services.AddHttpCacheHeaders(
                expirationModelOptions =>
                {
                    expirationModelOptions.MaxAge = 65; 
                    expirationModelOptions.SharedMaxAge = 65;
                },
                validationModelOptions =>
                {
                    validationModelOptions.MustRevalidate = true;
                });
            
 app.UseResponseCaching();
 app.UseHttpCacheHeaders();
            
```

- часто вижу так делают миграции

```Csharp
using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.Migrate();
            }

```


- настройка опций докера через launchSettings.json

```json
{
  "profiles": {
    "http": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "dotnetRunMessages": true,
      "applicationUrl": "http://localhost:8080"
    },
    "https": {
      "commandName": "Project",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      },
      "dotnetRunMessages": true,
      "applicationUrl": "https://localhost:8081;http://localhost:8080"
    },
    "Container (Dockerfile)": {
      "commandName": "Docker",
      "launchUrl": "{Scheme}://{ServiceHost}:{ServicePort}",
      "environmentVariables": {
        "ASPNETCORE_HTTPS_PORTS": "8081",
        "ASPNETCORE_HTTP_PORTS": "8080"
      },
      "publishAllPorts": true,
      "useSSL": true
    }
  },
  "$schema": "https://json.schemastore.org/launchsettings.json"
}

```

- способ настроек

``` 
    var gameSettings = new GameSettings();
            builder.Configuration.Bind("GameSettings", gameSettings);
            builder.Services.AddSingleton(gameSettings);
```

- доступ к переменной окружения как `ConnectionStrings__DefaultConnection`:

`
  api:
    build:
      context: .
      dockerfile: TicTacToe.Api/Dockerfile
    ports:
      - "8080:8080"
    environment:
      ConnectionStrings__DefaultConnection: "Server=mssql;Database=TicTacToe;User Id=sa;Password=q7_E+3OjIi;"
      ASPNETCORE_ENVIRONMENT: "Production"
    depends_on:
      - mssql
`

- настройки

```csharp
builder.Services.Configure<GameSettings>(builder.Configuration.GetSection("GameSettings"));
```


- интересное решение обработчика ошибок


```csharp
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        context.Response.ContentType = "application/problem+json";

        var error = new ProblemDetails
        {
            Status = 400,
            Title = "Invalid request",
            Detail = "The request was malformed or contained invalid data.",
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(error);
    });
});
```
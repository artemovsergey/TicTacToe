FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID

WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TicTacToeApp.API/TicTacToeApp.API.csproj", "TicTacToeApp.API/"]
RUN dotnet restore "TicTacToeApp.API/TicTacToeApp.API.csproj"
COPY . .
WORKDIR "/src/TicTacToeApp.API"
RUN dotnet build "TicTacToeApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "TicTacToeApp.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

COPY ./TicTacToeApp.API/appsettings.Production.json /app/appsettings.Production.json
COPY ./TicTacToeApp.API/appsettings.json /app/appsettings.json

ENTRYPOINT ["dotnet", "TicTacToeApp.API.dll"]

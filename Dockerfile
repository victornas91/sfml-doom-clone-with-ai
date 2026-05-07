FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["sfml-fps-game-1/sfml-fps-game-1.csproj", "sfml-fps-game-1/"]
RUN dotnet restore "sfml-fps-game-1/sfml-fps-game-1.csproj"
COPY . .
WORKDIR "/src/sfml-fps-game-1"
RUN dotnet build "./sfml-fps-game-1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./sfml-fps-game-1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "sfml-fps-game-1.dll"]

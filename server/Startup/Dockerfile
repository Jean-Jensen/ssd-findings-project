FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Startup/Startup.csproj", "Startup/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Core.Domain/Core.Domain.csproj", "Core.Domain/"]
COPY ["Infrastructure.Postgres/Infrastructure.Postgres.csproj", "Infrastructure.Postgres/"]
COPY ["Infrastructure.Postgres.Scaffolding/Infrastructure.Postgres.Scaffolding.csproj", "Infrastructure.Postgres.Scaffolding/"]
COPY ["Api.Rest/Api.Rest.csproj", "Api.Rest/"]
RUN dotnet restore "Startup/Startup.csproj"
COPY . .
WORKDIR "/src/Startup"
RUN dotnet build "./Startup.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Startup.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Startup.dll"]

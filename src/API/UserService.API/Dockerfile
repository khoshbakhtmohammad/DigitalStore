FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/UserService.API/UserService.API.csproj", "src/API/UserService.API/"]
COPY ["src/Application/UserService.Application/UserService.Application.csproj", "src/Application/UserService.Application/"]
COPY ["src/Domain/UserService.Domain/UserService.Domain.csproj", "src/Domain/UserService.Domain/"]
COPY ["src/Infrastructure/UserService.Infrastructure/UserService.Infrastructure.csproj", "src/Infrastructure/UserService.Infrastructure/"]
RUN dotnet restore "src/API/UserService.API/UserService.API.csproj"
COPY . .
WORKDIR "/src/src/API/UserService.API"
RUN dotnet build "UserService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserService.API.dll"]


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/OrderService.API/OrderService.API.csproj", "src/API/OrderService.API/"]
COPY ["src/Application/OrderService.Application/OrderService.Application.csproj", "src/Application/OrderService.Application/"]
COPY ["src/Domain/OrderService.Domain/OrderService.Domain.csproj", "src/Domain/OrderService.Domain/"]
COPY ["src/Infrastructure/OrderService.Infrastructure/OrderService.Infrastructure.csproj", "src/Infrastructure/OrderService.Infrastructure/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["OpenSleigh/", "OpenSleigh/"]
RUN dotnet restore "src/API/OrderService.API/OrderService.API.csproj" -p:TargetFrameworks=net8.0
COPY . .
WORKDIR "/src/src/API/OrderService.API"
RUN dotnet build "OrderService.API.csproj" -c Release -o /app/build -p:TargetFrameworks=net8.0

FROM build AS publish
RUN dotnet publish "OrderService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false -p:TargetFrameworks=net8.0

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.API.dll"]


FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Services/OrderService.Orchestrator/OrderService.Orchestrator.csproj", "src/Services/OrderService.Orchestrator/"]
COPY ["src/Shared/Shared.csproj", "src/Shared/"]
COPY ["OpenSleigh/", "OpenSleigh/"]
RUN dotnet restore "src/Services/OrderService.Orchestrator/OrderService.Orchestrator.csproj" -p:TargetFrameworks=net8.0
COPY . .
WORKDIR "/src/src/Services/OrderService.Orchestrator"
RUN dotnet build "OrderService.Orchestrator.csproj" -c Release -o /app/build -p:TargetFrameworks=net8.0

FROM build AS publish
RUN dotnet publish "OrderService.Orchestrator.csproj" -c Release -o /app/publish /p:UseAppHost=false -p:TargetFrameworks=net8.0

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.Orchestrator.dll"]


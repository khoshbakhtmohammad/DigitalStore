FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/API/ProductService.API/ProductService.API.csproj", "src/API/ProductService.API/"]
COPY ["src/Application/ProductService.Application/ProductService.Application.csproj", "src/Application/ProductService.Application/"]
COPY ["src/Domain/ProductService.Domain/ProductService.Domain.csproj", "src/Domain/ProductService.Domain/"]
COPY ["src/Infrastructure/ProductService.Infrastructure/ProductService.Infrastructure.csproj", "src/Infrastructure/ProductService.Infrastructure/"]
RUN dotnet restore "src/API/ProductService.API/ProductService.API.csproj"
COPY . .
WORKDIR "/src/src/API/ProductService.API"
RUN dotnet build "ProductService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProductService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ProductService.API.dll"]


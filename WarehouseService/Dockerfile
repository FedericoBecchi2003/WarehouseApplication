FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY WarehouseService/WarehouseService.WebApi/WarehouseService.WebApi.csproj WarehouseService/WarehouseService.WebApi/
COPY WarehouseService/WarehouseService.Business/WarehouseService.Business.csproj WarehouseService/WarehouseService.Business/
COPY WarehouseService/WarehouseService.Repository/WarehouseService.Repository.csproj WarehouseService/WarehouseService.Repository/
COPY WarehouseService/WarehouseService.Shared/WarehouseService.Shared.csproj WarehouseService/WarehouseService.Shared/
COPY WarehouseService/WarehouseService.ClientHttp/WarehouseService.ClientHttp.csproj WarehouseService/WarehouseService.ClientHttp/

RUN dotnet restore WarehouseService/WarehouseService.WebApi/WarehouseService.WebApi.csproj

COPY WarehouseService/ WarehouseService/

WORKDIR /src/WarehouseService/WarehouseService.WebApi
RUN dotnet build WarehouseService.WebApi.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish WarehouseService.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WarehouseService.WebApi.dll"]

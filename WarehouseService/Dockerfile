# Questa fase viene usata durante l'esecuzione da Visual Studio in modalità rapida (impostazione predefinita per la configurazione di debug)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Questa fase viene usata per compilare il progetto di servizio
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
ARG NUGET_TOKEN
WORKDIR /src
COPY ["WarehouseService/WarehouseService.WebApi/WarehouseService.WebApi.csproj", "WarehouseService/WarehouseService.WebApi/"]
COPY ["WarehouseService/WarehouseService.Business/WarehouseService.Business.csproj", "WarehouseService/WarehouseService.Business/"]
COPY ["WarehouseService/WarehouseService.Repository/WarehouseService.Repository.csproj", "WarehouseService/WarehouseService.Repository/"]
COPY ["WarehouseService/WarehouseService.Shared/WarehouseService.Shared.csproj", "WarehouseService/WarehouseService.Shared/"]
COPY ["WarehouseService/WarehouseService.ClientHttp/WarehouseService.ClientHttp.csproj", "WarehouseService/WarehouseService.ClientHttp/"]
COPY WarehouseService/ WarehouseService/
WORKDIR "/src/WarehouseService/WarehouseService.WebApi"
RUN dotnet build "WarehouseService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build


# Questa fase viene usata per pubblicare il progetto di servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WarehouseService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Questa fase viene usata nell'ambiente di produzione o durante l'esecuzione da Visual Studio in modalità normale (impostazione predefinita quando non si usa la configurazione di debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WarehouseService.WebApi.dll"]

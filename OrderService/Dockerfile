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
COPY ["OrderService/OrderService.WebApi/OrderService.WebApi.csproj", "OrderService/OrderService.WebApi/"]
COPY ["OrderService/OrderService.Business/OrderService.Business.csproj", "OrderService/OrderService.Business/"]
COPY ["OrderService/OrderService.Repository/OrderService.Repository.csproj", "OrderService/OrderService.Repository/"]
COPY ["OrderService/OrderService.Shared/OrderService.Shared.csproj", "OrderService/OrderService.Shared/"]
COPY ["OrderService/OrderService.ClientHttp/OrderService.ClientHttp.csproj", "OrderService/OrderService.ClientHttp/"]
WORKDIR "/src/OrderService/OrderService.WebApi"
RUN dotnet build "OrderService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Questa fase viene usata per pubblicare il progetto di servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Questa fase viene usata nell'ambiente di produzione o durante l'esecuzione da Visual Studio in modalità normale (impostazione predefinita quando non si usa la configurazione di debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.WebApi.dll"]

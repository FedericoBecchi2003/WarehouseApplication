# Fase base per il runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

# Fase di build e compilazione
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1. Copia SOLO i file di progetto (.csproj) per sfruttare la cache dei layer di Docker
COPY ["OrderService/OrderService.WebApi/OrderService.WebApi.csproj", "OrderService/OrderService.WebApi/"]
COPY ["OrderService/OrderService.Business/OrderService.Business.csproj", "OrderService/OrderService.Business/"]
COPY ["OrderService/OrderService.Repository/OrderService.Repository.csproj", "OrderService/OrderService.Repository/"]
COPY ["OrderService/OrderService.Shared/OrderService.Shared.csproj", "OrderService/OrderService.Shared/"]
COPY ["OrderService/OrderService.ClientHttp/OrderService.ClientHttp.csproj", "OrderService/OrderService.ClientHttp/"]

# 2. Ripristina i pacchetti NuGet pubblici
RUN dotnet restore "OrderService/OrderService.WebApi/OrderService.WebApi.csproj"

# 3. Copia tutto il resto del codice sorgente (.cs, configurazioni, ecc.)
COPY OrderService/ OrderService/

# 4. Compila il progetto senza rifare il restore (grazie a --no-restore)
WORKDIR "/src/OrderService/OrderService.WebApi"
RUN dotnet build "OrderService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore

# Fase di pubblicazione
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrderService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# Fase finale di produzione
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.WebApi.dll"]
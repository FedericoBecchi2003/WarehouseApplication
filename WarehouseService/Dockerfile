# Fase 1: Immagine di base per il runtime (leggera)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Fase 2: Immagine SDK per la compilazione
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Dichiaro un argomento per ricevere il PAT in fase di build
ARG GITHUB_TOKEN
# Lo imposto come variabile d'ambiente affinché nuget.config lo legga
ENV GITHUB_TOKEN=$GITHUB_TOKEN 

WORKDIR /src

# Copio il nuget.config dalla root del repository
COPY ["nuget.config", "."]

# Copio solo i file di progetto per sfruttare la cache di Docker
COPY ["WarehouseService/WarehouseService.WebApi/WarehouseService.WebApi.csproj", "WarehouseService/WarehouseService.WebApi/"]
COPY ["WarehouseService/WarehouseService.Business/WarehouseService.Business.csproj", "WarehouseService/WarehouseService.Business/"]
COPY ["WarehouseService/WarehouseService.Repository/WarehouseService.Repository.csproj", "WarehouseService/WarehouseService.Repository/"]
COPY ["WarehouseService/WarehouseService.Shared/WarehouseService.Shared.csproj", "WarehouseService/WarehouseService.Shared/"]
COPY ["WarehouseService/WarehouseService.ClientHttp/WarehouseService.ClientHttp.csproj", "WarehouseService/WarehouseService.ClientHttp/"]

# Eseguo il restore (scaricherà i pacchetti da NuGet.org e da GitHub)
RUN dotnet restore "WarehouseService/WarehouseService.WebApi/WarehouseService.WebApi.csproj"

# Copio tutto il resto del codice sorgente di WarehouseService
COPY ["WarehouseService/", "WarehouseService/"]
WORKDIR "/src/WarehouseService/WarehouseService.WebApi"

# Compilo
RUN dotnet build "WarehouseService.WebApi.csproj" -c Release -o /app/build

# Fase 3: Pubblicazione
FROM build AS publish
RUN dotnet publish "WarehouseService.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Fase 4: Immagine finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Punto di ingresso dell'applicazione
ENTRYPOINT ["dotnet", "WarehouseService.WebApi.dll"]
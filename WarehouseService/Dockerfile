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
COPY ["OrderService/Order.WebApi/Order.WebApi.csproj", "OrderService/Order.WebApi/"]
COPY ["OrderService/Order.Business/Order.Business.csproj", "OrderService/Order.Business/"]
COPY ["OrderService/Order.Repository/Order.Repository.csproj", "OrderService/Order.Repository/"]
COPY ["OrderService/Order.Shared/Order.Shared.csproj", "OrderService/Order.Shared/"]
COPY ["OrderService/Order.ClientHttp/Order.ClientHttp.csproj", "OrderService/Order.ClientHttp/"]

# Eseguo il restore (scaricherà i pacchetti da NuGet.org e da GitHub)
RUN dotnet restore "OrderService/Order.WebApi/Order.WebApi.csproj"

# Copio tutto il resto del codice sorgente di OrderService
COPY ["OrderService/", "OrderService/"]
WORKDIR "/src/OrderService/Order.WebApi"

# Compilo
RUN dotnet build "Order.WebApi.csproj" -c Release -o /app/build

# Fase 3: Pubblicazione
FROM build AS publish
RUN dotnet publish "Order.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Fase 4: Immagine finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# Punto di ingresso dell'applicazione
ENTRYPOINT ["dotnet", "Order.WebApi.dll"]
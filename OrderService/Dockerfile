# ==========================================
# Fase 1: Immagine di base per il runtime (leggera)
# ==========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# ==========================================
# Fase 2: Immagine SDK per la compilazione
# ==========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Dichiaro l'argomento per il PAT e lo passo come variabile d'ambiente
ARG GITHUB_TOKEN
ENV GITHUB_TOKEN=$GITHUB_TOKEN 

# Copio il nuget.config nella radice di lavoro (/src/nuget.config)
COPY nuget.config .

# Copio i file di progetto mantenendo la struttura delle cartelle
COPY OrderService/Order.WebApi/Order.WebApi.csproj OrderService/Order.WebApi/
COPY OrderService/Order.Business/Order.Business.csproj OrderService/Order.Business/
COPY OrderService/Order.Repository/Order.Repository.csproj OrderService/Order.Repository/
COPY OrderService/Order.Shared/Order.Shared.csproj OrderService/Order.Shared/
COPY OrderService/Order.ClientHttp/Order.ClientHttp.csproj OrderService/Order.ClientHttp/

# Eseguo il restore puntando al file di progetto principale
RUN dotnet restore OrderService/Order.WebApi/Order.WebApi.csproj

# Copio tutto il resto del codice sorgente di OrderService
COPY OrderService/ OrderService/

# Cambio directory di lavoro sulla cartella dell'eseguibile
WORKDIR /src/OrderService/Order.WebApi

# Compilo l'applicazione
RUN dotnet build Order.WebApi.csproj -c Release -o /app/build

# ==========================================
# Fase 3: Pubblicazione
# ==========================================
FROM build AS publish
RUN dotnet publish Order.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

# ==========================================
# Fase 4: Immagine finale
# ==========================================
FROM base AS final
WORKDIR /app
# Copio l'output pubblicato dallo stage 'publish'
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Order.WebApi.dll"]
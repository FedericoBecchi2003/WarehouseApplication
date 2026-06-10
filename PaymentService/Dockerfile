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
COPY ["PaymentService/PaymentService.WebApi/PaymentService.WebApi.csproj", "PaymentService/PaymentService.WebApi/"]
COPY ["PaymentService/PaymentService.Business/PaymentService.Business.csproj", "PaymentService/PaymentService.Business/"]
COPY ["PaymentService/PaymentService.Repository/PaymentService.Repository.csproj", "PaymentService/PaymentService.Repository/"]
COPY ["PaymentService/PaymentService.Shared/PaymentService.Shared.csproj", "PaymentService/PaymentService.Shared/"]
COPY ["PaymentService/PaymentService.ClientHttp/PaymentService.ClientHttp.csproj", "PaymentService/PaymentService.ClientHttp/"]
COPY PaymentService/ PaymentService/
WORKDIR "/src/PaymentService/PaymentService.WebApi"
RUN dotnet build "PaymentService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build


# Questa fase viene usata per pubblicare il progetto di servizio da copiare nella fase finale
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PaymentService.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# Questa fase viene usata nell'ambiente di produzione o durante l'esecuzione da Visual Studio in modalità normale (impostazione predefinita quando non si usa la configurazione di debug)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.WebApi.dll"]

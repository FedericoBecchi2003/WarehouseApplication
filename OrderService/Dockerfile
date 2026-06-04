FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY OrderService/OrderService.WebApi/OrderService.WebApi.csproj OrderService/OrderService.WebApi/
COPY OrderService/OrderService.Business/OrderService.Business.csproj OrderService/OrderService.Business/
COPY OrderService/OrderService.Repository/OrderService.Repository.csproj OrderService/OrderService.Repository/
COPY OrderService/OrderService.Shared/OrderService.Shared.csproj OrderService/OrderService.Shared/
COPY OrderService/OrderService.ClientHttp/OrderService.ClientHttp.csproj OrderService/OrderService.ClientHttp/

RUN dotnet restore OrderService/OrderService.WebApi/OrderService.WebApi.csproj

COPY OrderService/ OrderService/

WORKDIR /src/OrderService/OrderService.WebApi
RUN dotnet build OrderService.WebApi.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish OrderService.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OrderService.WebApi.dll"]

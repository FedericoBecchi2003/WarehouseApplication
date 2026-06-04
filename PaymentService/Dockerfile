FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

COPY PaymentService/PaymentService.WebApi/PaymentService.WebApi.csproj PaymentService/PaymentService.WebApi/
COPY PaymentService/PaymentService.Business/PaymentService.Business.csproj PaymentService/PaymentService.Business/
COPY PaymentService/PaymentService.Repository/PaymentService.Repository.csproj PaymentService/PaymentService.Repository/
COPY PaymentService/PaymentService.Shared/PaymentService.Shared.csproj PaymentService/PaymentService.Shared/
COPY PaymentService/PaymentService.ClientHttp/PaymentService.ClientHttp.csproj PaymentService/PaymentService.ClientHttp/

RUN dotnet restore PaymentService/PaymentService.WebApi/PaymentService.WebApi.csproj

COPY PaymentService/ PaymentService/

WORKDIR /src/PaymentService/PaymentService.WebApi
RUN dotnet build PaymentService.WebApi.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish PaymentService.WebApi.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PaymentService.WebApi.dll"]

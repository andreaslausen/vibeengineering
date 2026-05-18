# Dockerfile for Zeiterfassung.API
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY Zeiterfassung.API/Zeiterfassung.API.csproj Zeiterfassung.API/
COPY Zeiterfassung.Application/Zeiterfassung.Application.csproj Zeiterfassung.Application/
COPY Zeiterfassung.Domain/Zeiterfassung.Domain.csproj Zeiterfassung.Domain/
COPY Zeiterfassung.Infrastructure/Zeiterfassung.Infrastructure.csproj Zeiterfassung.Infrastructure/
RUN dotnet restore Zeiterfassung.API/Zeiterfassung.API.csproj
COPY . .
WORKDIR /src/Zeiterfassung.API
RUN dotnet publish Zeiterfassung.API.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Zeiterfassung.API.dll"]

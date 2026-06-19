# ── Etapa 1: Build ──────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de proyecto primero (mejor cache de capas)
COPY AuthService.sln ./
COPY src/AuthService.Domain/AuthService.Domain.csproj         src/AuthService.Domain/
COPY src/AuthService.Application/AuthService.Application.csproj src/AuthService.Application/
COPY src/AuthService.Persistence/AuthService.Persistence.csproj src/AuthService.Persistence/
COPY src/AuthService.Api/AuthService.Api.csproj               src/AuthService.Api/

RUN dotnet restore AuthService.sln

# Copiar el resto del código
COPY . .

# Publicar en modo Release
RUN dotnet publish src/AuthService.Api/AuthService.Api.csproj \
    -c Release \
    -o /app/publish

# ── Etapa 2: Runtime ────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Instalar curl para el healthcheck
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

# Crear directorio para logs y keys (Data Protection)
RUN mkdir -p /app/logs /app/keys

COPY --from=build /app/publish .

# Fly.io expone el puerto via variable de entorno PORT
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "AuthService.Api.dll"]
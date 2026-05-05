FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copia solo los archivos de proyecto primero para optimizar la cache
COPY inventario_herramientas.Managers/inventario_herramientas.Managers.csproj inventario_herramientas.Managers/
COPY inventario_herramientas/inventario_herramientas.Web.csproj inventario_herramientas/

# Restaura dependencias
RUN dotnet restore inventario_herramientas.Managers/inventario_herramientas.Managers.csproj
RUN dotnet restore inventario_herramientas/inventario_herramientas.Web.csproj

# Copia el resto del código fuente
COPY inventario_herramientas.Managers/ inventario_herramientas.Managers/
COPY inventario_herramientas/ inventario_herramientas/

# Publica la aplicación
RUN dotnet publish inventario_herramientas/inventario_herramientas.Web.csproj -c Release -o out

# Configura el runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

EXPOSE 443

ENTRYPOINT ["dotnet", "inventario_herramientas.Web.dll"]

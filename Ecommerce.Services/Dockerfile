# Utilisation d'une image de base pour ASP.NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Utilisation de l'image SDK pour le build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copie du fichier .csproj et restauration des dépendances
COPY ["Ecommerce.Services/Ecommerce.Services.csproj", "Ecommerce.Services/"]
RUN dotnet restore "Ecommerce.Services/Ecommerce.Services.csproj"

# Copie de tous les fichiers et compilation de l'application
COPY . .
WORKDIR "/src/Ecommerce.Services"
RUN dotnet build "Ecommerce.Services.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publication de l'application
FROM build AS publish
RUN dotnet publish "Ecommerce.Services.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Étape finale : création de l'image de runtime
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Ecommerce.Services.dll"]

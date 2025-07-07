# Faza 1: Build
# ISPRAVAK: Koristimo .NET 9.0 SDK sliku
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/InventoryService/InventoryService.csproj", "InventoryService/"]
RUN dotnet restore "InventoryService/InventoryService.csproj"
COPY src/ .
WORKDIR "/src/InventoryService"
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish

# Faza 2: Finalna slika
# ISPRAVAK: Koristimo .NET 9.0 ASP.NET Runtime sliku
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]
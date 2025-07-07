# Faza 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopiraj .csproj datoteke i restoraj pakete
COPY ["src/InventoryService/InventoryService.csproj", "InventoryService/"]
COPY ["src/Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "InventoryService/InventoryService.csproj"

# Kopiraj ostatak koda
COPY src/ .

# Napravi build i publish
WORKDIR "/src/InventoryService"
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish

# Faza 2: Finalna slika
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]
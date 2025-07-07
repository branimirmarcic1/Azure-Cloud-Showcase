FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/InventoryService/InventoryService.csproj", "InventoryService/"]
COPY ["src/Contracts/Contracts.csproj", "Contracts/"] # <-- DODAJTE OVU LINIJU
RUN dotnet restore "InventoryService/InventoryService.csproj"
COPY src/ .
WORKDIR "/src/InventoryService"
RUN dotnet publish "InventoryService.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "InventoryService.dll"]
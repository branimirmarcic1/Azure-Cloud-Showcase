# Faza 1: Build
# ISPRAVAK: Koristimo .NET 9.0 SDK sliku
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Kopiraj .csproj datoteke i restoraj pakete
# Ovo kopira sve .csproj datoteke iz src mape u odgovarajuće podmape unutar kontejnera
COPY ["src/OrderService/OrderService.csproj", "OrderService/"]
COPY ["src/Contracts/Contracts.csproj", "Contracts/"]
RUN dotnet restore "OrderService/OrderService.csproj"

# Kopiraj ostatak koda
COPY src/ .

# Napravi build i publish
WORKDIR "/src/OrderService"
RUN dotnet publish "OrderService.csproj" -c Release -o /app/publish

# Faza 2: Finalna, manja slika
# ISPRAVAK: Koristimo .NET 9.0 ASP.NET Runtime sliku
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]
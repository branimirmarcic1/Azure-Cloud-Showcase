# Faza 1: Build
# Koristimo .NET SDK sliku za kompajliranje
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
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
# Koristimo samo ASP.NET Runtime sliku koja je manja
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "OrderService.dll"]
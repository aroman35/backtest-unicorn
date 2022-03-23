FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Hermes.Connectors.MarketData/Hermes.Connectors.MarketData.csproj", "Hermes.Connectors.MarketData/"]
RUN dotnet restore "Hermes.Connectors.MarketData/Hermes.Connectors.MarketData.csproj"
COPY . .
WORKDIR "/src/Hermes.Connectors.MarketData"
RUN dotnet build "Hermes.Connectors.MarketData.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hermes.Connectors.MarketData.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hermes.Connectors.MarketData.dll"]

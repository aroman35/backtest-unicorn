FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BacktestUnicorn.SiloTestCluster/BacktestUnicorn.SiloTestCluster.csproj", "BacktestUnicorn.SiloTestCluster/"]
RUN dotnet restore "BacktestUnicorn.SiloTestCluster/BacktestUnicorn.SiloTestCluster.csproj"
COPY . .
WORKDIR "/src/BacktestUnicorn.SiloTestCluster"
RUN dotnet build "BacktestUnicorn.SiloTestCluster.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BacktestUnicorn.SiloTestCluster.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BacktestUnicorn.SiloTestCluster.dll"]

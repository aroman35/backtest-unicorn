FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Hermes.Silo/Hermes.Silo.csproj", "Hermes.Silo/"]
RUN dotnet restore "Hermes.Silo/Hermes.Silo.csproj"
COPY . .
WORKDIR "/src/Hermes.Silo"
RUN dotnet build "Hermes.Silo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Hermes.Silo.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hermes.Silo.dll"]

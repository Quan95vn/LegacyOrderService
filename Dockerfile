FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY *.sln .

COPY Directory.Build.props .
COPY Directory.Packages.props .
COPY src/LegacyOrderService/*.csproj ./src/LegacyOrderService/
COPY test/LegacyOrderService.Tests/*.csproj ./test/LegacyOrderService.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish src/LegacyOrderService/LegacyOrderService.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

RUN mkdir -p /app/LocalDb


VOLUME /app/LocalDb

ENTRYPOINT ["dotnet", "LegacyOrderService.dll"]

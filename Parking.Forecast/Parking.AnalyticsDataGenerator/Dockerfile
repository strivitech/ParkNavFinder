FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Parking.Forecast/Parking.AnalyticsDataGenerator/Parking.AnalyticsDataGenerator.csproj", "Parking.Forecast/Parking.AnalyticsDataGenerator/"]
RUN dotnet restore "Parking.Forecast/Parking.AnalyticsDataGenerator/Parking.AnalyticsDataGenerator.csproj"
COPY . .
WORKDIR "/src/Parking.Forecast/Parking.AnalyticsDataGenerator"
RUN dotnet build "Parking.AnalyticsDataGenerator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Parking.AnalyticsDataGenerator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Parking.AnalyticsDataGenerator.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["User.Location.AnalyticsService/User.Location.AnalyticsService.csproj", "User.Location.AnalyticsService/"]
RUN dotnet restore "User.Location.AnalyticsService/User.Location.AnalyticsService.csproj"
COPY . .
WORKDIR "/src/User.Location.AnalyticsService"
RUN dotnet build "User.Location.AnalyticsService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "User.Location.AnalyticsService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.Location.AnalyticsService.dll"]

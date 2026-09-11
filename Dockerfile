# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["FlightBooking.Api/FlightBooking.Api.csproj", "FlightBooking.Api/"]
RUN dotnet restore "FlightBooking.Api/FlightBooking.Api.csproj"

COPY . .
WORKDIR "/src/FlightBooking.Api"

RUN dotnet publish "FlightBooking.Api.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_USE_POLLING_FILE_WATCHER=1
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "FlightBooking.Api.dll"]
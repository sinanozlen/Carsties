FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 7002

ENV ASPNETCORE_URLS=http://+:7002

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG configuration=Release
WORKDIR /src
COPY ["src/AuctionService/AuctionService.csproj", "src/AuctionService/"]
RUN dotnet restore "src\AuctionService\AuctionService.csproj"
COPY . .
WORKDIR "/src/src/AuctionService"
RUN dotnet build "AuctionService.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "AuctionService.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuctionService.dll"]

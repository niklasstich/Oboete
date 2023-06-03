FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
WORKDIR "/src/Oboete.API"
COPY "Oboete.API.csproj" .
RUN dotnet restore "Oboete.API.csproj"
COPY . .
RUN dotnet build "Oboete.API.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "Oboete.API.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY .env/appsettings.json .
ENTRYPOINT ["dotnet", "Oboete.API.dll"]

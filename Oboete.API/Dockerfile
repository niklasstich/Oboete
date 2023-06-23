FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY "Oboete.API/Oboete.API.csproj" "Oboete.API/"
COPY "Oboete.API.Controllers/Oboete.API.Controllers.csproj" "/Oboete.API.Controllers/"
COPY "Oboete.API.Entities/Oboete.API.Entities.csproj" "/Oboete.API.Entities/"
COPY "Oboete.API.Services/Oboete.API.Services.csproj" "/Oboete.API.Services/"
WORKDIR "/src/Oboete.API"
RUN dotnet restore "Oboete.API.csproj"
WORKDIR /src
COPY . .
WORKDIR "/src/Oboete.API"
RUN dotnet build "Oboete.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Oboete.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Oboete.API/appsettings.json appsettings.json
ENTRYPOINT ["dotnet", "Oboete.API.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Oboete.API/Oboete.API.csproj", "Oboete.API/"]
RUN dotnet restore "Oboete.API/Oboete.API.csproj"
COPY . .
WORKDIR "/src/Oboete.API"
RUN dotnet build "Oboete.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Oboete.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Oboete.API.dll"]

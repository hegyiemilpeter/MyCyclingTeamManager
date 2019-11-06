FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /src
COPY ["TeamManager.Manual/TeamManager.Manual.csproj", "TeamManager.Manual/"]
RUN dotnet restore "TeamManager.Manual/TeamManager.Manual.csproj"
COPY . .
WORKDIR "/src/TeamManager.Manual"
RUN dotnet build "TeamManager.Manual.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TeamManager.Manual.csproj" -c Release -o /app/publish

FROM base AS final
ENV ASPNETCORE_URLS=http://+:8080
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TeamManager.Manual.dll"]
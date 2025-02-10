FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/HelloBlazorServer/HelloBlazorServer.csproj", "HelloBlazorServer/"]
RUN dotnet restore "HelloBlazorServer/HelloBlazorServer.csproj"

COPY src/HelloBlazorServer/ HelloBlazorServer/
WORKDIR /src/HelloBlazorServer
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HelloBlazorServer.dll"]

# Используем официальный SDK для сборки
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файлы проекта и восстанавливаем зависимости
COPY ["src/HelloBlazorServer/HelloBlazorServer.csproj", "HelloBlazorServer/"]
RUN dotnet restore "HelloBlazorServer/HelloBlazorServer.csproj"

# Копируем остальные файлы и собираем проект
COPY src/HelloBlazorServer/ HelloBlazorServer/
WORKDIR /src/HelloBlazorServer
RUN dotnet publish -c Release -o /app/publish

# Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HelloBlazorServer.dll"]

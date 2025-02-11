# 1️⃣ Базовый образ ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

# 2️⃣ Билд-образ с SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Копируем .csproj и зависимости
COPY src/HelloBlazorServer/HelloBlazorServer.csproj HelloBlazorServer/
COPY Directory.Packages.props ./ 

# Восстанавливаем зависимости
RUN dotnet restore "HelloBlazorServer/HelloBlazorServer.csproj"

# Копируем весь код
COPY . .

# 3️⃣ Сборка проекта
WORKDIR "/src/HelloBlazorServer"
RUN dotnet build "HelloBlazorServer.csproj" -c Release -o /app/build

# 4️⃣ Публикация для linux-x64, без UseAppHost для минимизации
FROM build AS publish
WORKDIR "/src/HelloBlazorServer"
RUN dotnet publish "HelloBlazorServer.csproj" -c Release -o /app/publish -r linux-x64 --no-build /p:UseAppHost=false

# 5️⃣ Финальный образ
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 6️⃣ Запуск приложения
CMD ["dotnet", "HelloBlazorServer.dll"]

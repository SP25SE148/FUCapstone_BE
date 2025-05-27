# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/FUC.API/FUC.API.csproj", "src/FUC.API/"]
COPY ["src/FUC.Common/FUC.Common.csproj", "src/FUC.Common/"]
COPY ["src/FUC.Service/FUC.Service.csproj", "src/FUC.Service/"]
COPY ["src/FUC.Data/FUC.Data.csproj", "src/FUC.Data/"]

RUN dotnet restore "src/FUC.API/FUC.API.csproj"

COPY . .
WORKDIR "/src/src/FUC.API"
RUN dotnet build "FUC.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "FUC.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
EXPOSE 8081
ENTRYPOINT ["dotnet", "FUC.API.dll"]

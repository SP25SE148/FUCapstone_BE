FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/FUC.Processor/FUC.Processor.csproj", "src/FUC.Processor/"]
COPY ["src/FUC.Common/FUC.Common.csproj", "src/FUC.Common/"]

RUN dotnet restore "src/FUC.Processor/FUC.Processor.csproj"

COPY . .
WORKDIR "/src/src/FUC.Processor"
RUN dotnet build "FUC.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "FUC.Processor.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FUC.Processor.dll"]

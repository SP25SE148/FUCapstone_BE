#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/FUC.API/FUC.API.csproj", "src/FUC.API/"]
COPY ["src/FUC.Common/FUC.Common.csproj", "src/FUC.Common/"]
COPY ["src/FUC.Service/FUC.Service.csproj", "src/FUC.Service/"]
COPY ["src/FUC.Data/FUC.Data.csproj", "src/FUC.Data/"]
RUN dotnet restore "./src/FUC.API/FUC.API.csproj"
COPY . .
WORKDIR "/src/src/FUC.API"
RUN dotnet build "./FUC.API.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./FUC.API.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FUC.API.dll"]
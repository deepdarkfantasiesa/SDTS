#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000
EXPOSE 5001
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Auth.API/Auth.API.csproj", "Auth.API/"]
COPY ["Service.Framework/Service.Framework.csproj", "Service.Framework/"]
COPY ["Auth.Domain/Auth.Domain.csproj", "Auth.Domain/"]
COPY ["Domain.Abstraction/Domain.Abstraction.csproj", "Domain.Abstraction/"]
COPY ["Infrastructure.Core/Infrastructure.Core.csproj", "Infrastructure.Core/"]
COPY ["Auth.Infrastructure/Auth.Infrastructure.csproj", "Auth.Infrastructure/"]
RUN dotnet restore "Auth.API/Auth.API.csproj"
COPY . .
WORKDIR "/src/Auth.API"
RUN dotnet build "Auth.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Auth.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["Auth.API/cert.pfx","/app"]
ENTRYPOINT ["dotnet", "Auth.API.dll"]
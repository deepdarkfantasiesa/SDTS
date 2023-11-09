#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Web.User.HttpAggregator/Web.User.HttpAggregator.csproj", "Web.User.HttpAggregator/"]
COPY ["Service.Framework/Service.Framework.csproj", "Service.Framework/"]
RUN dotnet restore "Web.User.HttpAggregator/Web.User.HttpAggregator.csproj"
COPY . .
WORKDIR "/src/Web.User.HttpAggregator"
RUN dotnet build "Web.User.HttpAggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Web.User.HttpAggregator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Web.User.HttpAggregator.dll"]
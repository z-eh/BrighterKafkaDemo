FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrighterKafkaDemo.WebApi/BrighterKafkaDemo.WebApi.csproj", "BrighterKafkaDemo.WebApi/"]
RUN dotnet restore "BrighterKafkaDemo.WebApi/BrighterKafkaDemo.WebApi.csproj"
COPY . .
WORKDIR "/src/BrighterKafkaDemo.WebApi"
RUN dotnet build "BrighterKafkaDemo.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrighterKafkaDemo.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrighterKafkaDemo.WebApi.dll"]

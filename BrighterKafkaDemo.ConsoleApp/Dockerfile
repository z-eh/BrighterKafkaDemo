FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["BrighterKafkaDemo.ConsoleApp/BrighterKafkaDemo.ConsoleApp.csproj", "BrighterKafkaDemo.ConsoleApp/"]
RUN dotnet restore "BrighterKafkaDemo.ConsoleApp/BrighterKafkaDemo.ConsoleApp.csproj"
COPY . .
WORKDIR "/src/BrighterKafkaDemo.ConsoleApp"
RUN dotnet build "BrighterKafkaDemo.ConsoleApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BrighterKafkaDemo.ConsoleApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BrighterKafkaDemo.ConsoleApp.dll"]

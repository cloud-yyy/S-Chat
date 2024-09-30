FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY SChat.Server/SChat.Server.csproj ./SChat.Server/
RUN dotnet restore SChat.Server/SChat.Server.csproj

COPY . .

RUN dotnet publish SChat.Server/SChat.Server.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "SChat.Server.dll"]

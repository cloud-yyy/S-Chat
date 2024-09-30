FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY SChat.Client/SChat.Client.csproj ./SChat.Client/
RUN dotnet restore SChat.Client/SChat.Client.csproj

COPY . .

RUN dotnet publish SChat.Client/SChat.Client.csproj -c Release -o out --no-restore

FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "SChat.Client.dll"]

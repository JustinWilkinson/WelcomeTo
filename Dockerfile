FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Server/WelcomeTo.Server.csproj", "Server/"]
COPY ["Client/WelcomeTo.Client.csproj", "Client/"]
COPY ["Shared/WelcomeTo.Shared.csproj", "Shared/"]
RUN dotnet restore "Server/WelcomeTo.Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "WelcomeTo.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WelcomeTo.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WelcomeTo.Server.dll"]
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Sprint3/Sprint3.csproj", "Sprint3/"]
RUN dotnet restore "Sprint3/Sprint3.csproj"

COPY . .
WORKDIR "/src/Sprint3"
RUN dotnet publish "Sprint3.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sprint3.dll"]

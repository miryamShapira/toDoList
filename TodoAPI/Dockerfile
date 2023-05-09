FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

ENV ASPNETCORE_URLS=http://+:80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TodoAPI.csproj", "./"]
RUN dotnet restore "TodoAPI.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TodoAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TodoAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TodoAPI.dll"]

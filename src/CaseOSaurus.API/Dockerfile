FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/CaseOSaurus.API/CaseOSaurus.API.csproj", "src/CaseOSaurus.API/"]
COPY ["src/CaseOSaurus.Application/CaseOSaurus.Application.csproj", "src/CaseOSaurus.Application/"]
COPY ["src/CaseOSaurus.Domain/CaseOSaurus.Domain.csproj", "src/CaseOSaurus.Domain/"]
COPY ["src/CaseOSaurus.Infrastructure/CaseOSaurus.Infrastructure.csproj", "src/CaseOSaurus.Infrastructure/"]
RUN dotnet restore "src/CaseOSaurus.API/CaseOSaurus.API.csproj"
COPY . .
WORKDIR "/src/src/CaseOSaurus.API"
RUN dotnet build "CaseOSaurus.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CaseOSaurus.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CaseOSaurus.API.dll"]
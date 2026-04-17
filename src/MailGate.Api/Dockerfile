FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/MailGate.Api/MailGate.Api.csproj",                         "src/MailGate.Api/"]
COPY ["src/MailGate.Core/MailGate.Core.csproj",                       "src/MailGate.Core/"]
COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj",   "src/MailGate.Infrastructure/"]
RUN dotnet restore "src/MailGate.Api/MailGate.Api.csproj"

COPY . .
RUN dotnet publish "src/MailGate.Api/MailGate.Api.csproj" \
      -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MailGate.Api.dll"]

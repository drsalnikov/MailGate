FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/MailGate.Worker/MailGate.Worker.csproj",                   "src/MailGate.Worker/"]
COPY ["src/MailGate.Core/MailGate.Core.csproj",                       "src/MailGate.Core/"]
COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj",   "src/MailGate.Infrastructure/"]
COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj",     "src/MailGate.Channels.Smtp/"]
RUN dotnet restore "src/MailGate.Worker/MailGate.Worker.csproj"

COPY . .
RUN dotnet publish "src/MailGate.Worker/MailGate.Worker.csproj" \
      -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MailGate.Worker.dll"]

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Python is required by Emscripten (WASM AOT toolchain) to compile native assets
# when publishing Blazor WASM with RunAOTCompilation/trimming in Release.
RUN apt-get update \
    && apt-get install -y --no-install-recommends python3 \
    && ln -sf /usr/bin/python3 /usr/bin/python \
    && rm -rf /var/lib/apt/lists/*

# Install wasm-tools workload (required to publish Blazor WASM).
RUN dotnet workload install wasm-tools

COPY ["src/MailGate.Api/MailGate.Api.csproj",                         "src/MailGate.Api/"]
COPY ["src/MailGate.Admin/MailGate.Admin.csproj",                     "src/MailGate.Admin/"]
COPY ["src/MailGate.Contracts/MailGate.Contracts.csproj",             "src/MailGate.Contracts/"]
COPY ["src/MailGate.Core/MailGate.Core.csproj",                       "src/MailGate.Core/"]
COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj",     "src/MailGate.Channels.Smtp/"]
COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj",   "src/MailGate.Infrastructure/"]
RUN dotnet restore "src/MailGate.Api/MailGate.Api.csproj"

COPY . .
RUN dotnet publish "src/MailGate.Api/MailGate.Api.csproj" \
      -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MailGate.Api.dll"]

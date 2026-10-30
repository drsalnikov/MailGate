FROM registry.red-soft.ru/ubi8/ubi-minimal:8.0.2-260714 AS base
WORKDIR /app

RUN dnf makecache \
    && dnf install -y ca-certificates dotnet-runtime-10.0 \
    && dnf clean all

FROM registry.red-soft.ru/ubi8/ubi:8.0.2-260714 AS build
WORKDIR /src

RUN dnf makecache \
    && dnf install -y ca-certificates dotnet-sdk-10.0 \
    && dnf clean all

COPY crt/ /etc/pki/ca-trust/source/anchors/
RUN update-ca-trust extract

COPY ["src/NuGet.Config", "./"]
COPY ["src/MailGate.Worker/MailGate.Worker.csproj",                   "MailGate.Worker/"]
COPY ["src/MailGate.Core/MailGate.Core.csproj",                       "MailGate.Core/"]
COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj",   "MailGate.Infrastructure/"]
COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj",     "MailGate.Channels.Smtp/"]
RUN dotnet restore "MailGate.Worker/MailGate.Worker.csproj"

COPY src/ .
RUN dotnet publish "MailGate.Worker/MailGate.Worker.csproj" \
      -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY crt/ /etc/pki/ca-trust/source/anchors/
RUN update-ca-trust extract
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MailGate.Worker.dll"]

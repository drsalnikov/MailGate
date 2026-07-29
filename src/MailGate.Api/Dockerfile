FROM registry.red-soft.ru/ubi8/ubi-minimal:8.0.2-260714 AS base
WORKDIR /app
EXPOSE 8080

RUN dnf makecache \
    && dnf install -y dotnet-runtime-10.0 aspnetcore-runtime-10.0 \
    && dnf clean all

FROM registry.red-soft.ru/ubi8/ubi:8.0.2-260714 AS build
WORKDIR /src

RUN dnf makecache \
    && dnf install -y dotnet-sdk-10.0 \
    && dnf clean all

COPY ["src/MailGate.Api/MailGate.Api.csproj", "src/MailGate.Api/"]
COPY ["src/MailGate.Contracts/MailGate.Contracts.csproj", "src/MailGate.Contracts/"]
COPY ["src/MailGate.Core/MailGate.Core.csproj", "src/MailGate.Core/"]
COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj", "src/MailGate.Channels.Smtp/"]
COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj", "src/MailGate.Infrastructure/"]

RUN dotnet restore "src/MailGate.Api/MailGate.Api.csproj"

COPY . .
RUN dotnet publish "src/MailGate.Api/MailGate.Api.csproj" \
    -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MailGate.Api.dll"]

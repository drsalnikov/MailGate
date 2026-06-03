# MailGate — Notification Microservice

Микросервис на **.NET 10**, который принимает запросы на рассылку уведомлений от сторонних систем через REST API и доставляет их через подключаемые провайдеры каналов связи (SMTP/Exchange, мессенджеры и др.).

## Архитектура

```
┌─────────────────┐        POST /api/notifications
│  External System│ ──────────────────────────────────────►┐
└─────────────────┘                                         │
                                                   ┌────────▼────────┐
                                                   │  MailGate.Api   │
                                                   │  (ASP.NET Core) │
                                                   └────────┬────────┘
                                                            │ IPublishEndpoint
                                                            │
                                                   ┌────────▼────────┐
                                                   │    RabbitMQ     │
                                                   │  notification-  │
                                                   │    message      │
                                                   └────────┬────────┘
                                                            │ IConsumer
                                                   ┌────────▼────────┐
                                                   │ MailGate.Worker │
                                                   │  (MassTransit   │
                                                   │   Consumer)     │
                                                   └────────┬────────┘
                                                            │ ChannelRouter
                                              ┌─────────────┼────────────────┐
                                              │             │                │
                                     ┌────────▼───┐  ┌──────▼─────┐  ┌──────▼─────┐
                                     │SmtpChannel │  │ Telegram   │  │  Teams     │
                                     │ (MailKit)  │  │  (future)  │  │  (future)  │
                                     └────────────┘  └────────────┘  └────────────┘
```

### Компоненты решения

```
MailGate/
├── src/
│   ├── MailGate.Api/             # REST API, админ-эндпоинты, хостинг Blazor WASM
│   │   └── Dockerfile
│   ├── MailGate.Worker/          # MassTransit Consumer, ChannelRouter
│   │   └── Dockerfile
│   ├── MailGate.Admin/           # Blazor WebAssembly UI (MudBlazor)
│   ├── MailGate.Contracts/       # DTO, общие между API и Admin UI
│   ├── MailGate.Core/            # Доменные модели, сущности, интерфейсы
│   ├── MailGate.Channels.Smtp/   # SMTP/Exchange провайдер (MailKit + OAuth2)
│   └── MailGate.Infrastructure/  # MassTransit, EF Core, PostgreSQL, Data Protection
├── tests/
│   ├── MailGate.Api.Tests/
│   ├── MailGate.Worker.Tests/
│   └── MailGate.Channels.Smtp.Tests/
├── docker-compose.yml            # Полный стек: API + Worker + RabbitMQ + PostgreSQL + MailHog
├── .dockerignore
└── MailGate.sln
```

---

## Быстрый старт

### Требования

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Запустить весь стек

```bash
docker compose up --build
```

Собирает образы и запускает все сервисы:

| Сервис | URL | Описание |
|--------|-----|----------|
| **Admin UI** | http://localhost:8080 | Blazor WASM админка (рассылки, получатели, SMTP) |
| **API** | http://localhost:8080/api/... | REST API |
| **Swagger UI** | http://localhost:8080/swagger | Интерактивная документация |
| **Scalar UI** | http://localhost:8080/scalar/v1 | Альтернативный API Reference |
| **Keycloak** | http://keycloak.localhost:8180 | SSO/OIDC (`admin` / `admin`) |
| **RabbitMQ UI** | http://localhost:15672 | Менеджмент (`mailgate` / `mailgate`) |
| **MailHog** | http://localhost:8025 | Перехватчик исходящей почты |
| **PostgreSQL** | `localhost:5432` | БД `mailgate` |

> **SMTP в Docker:** API и Worker подключаются к MailHog по хосту **`mailhog`**, порт **`1025`** (задаётся в `docker-compose.yml`).
> В админке не используйте `localhost` — из контейнера это не MailHog. Если в БД уже сохранено переопределение с `localhost`, измените Host на `mailhog` или удалите override на странице SMTP.

> Миграции БД применяются автоматически при старте Worker.
> Dev-вход в админку: `mailgate-admin` / `mailgate`. Пользователь без роли для проверки `403`: `mailgate-user` / `mailgate`.

### 2. Отправить тестовое уведомление

```bash
curl -X POST http://localhost:8080/api/notifications \
  -H "Content-Type: application/json" \
  -H "X-Idempotency-Key: test-001" \
  -d '{
    "channel": "Smtp",
    "recipients": ["user@example.com"],
    "subject": "Test notification",
    "body": "<h1>Hello from MailGate!</h1>",
    "bodyFormat": "Html",
    "priority": "High"
  }'
```

Письмо появится в MailHog UI: http://localhost:8025

---

### Локальная разработка (без Docker для сервисов)

Если нужно запустить API или Worker напрямую через `dotnet run`, сначала подними только инфраструктуру:

```bash
docker compose up -d rabbitmq postgres mailhog
```

Затем:

```bash
# Применить миграции
dotnet ef database update \
  --project src/MailGate.Infrastructure \
  --startup-project src/MailGate.Worker

# Worker
dotnet run --project src/MailGate.Worker

# API
dotnet run --project src/MailGate.Api --urls "http://localhost:5100"
```

---

## API

### `POST /api/notifications`

Принимает запрос на отправку уведомления и помещает его в очередь.

**Заголовки:**

| Заголовок | Обязательный | Описание |
|-----------|-------------|----------|
| `Content-Type` | Да | `application/json` |
| `X-Idempotency-Key` | Нет | Уникальный ключ для дедупликации повторных запросов |

**Тело запроса:**

```json
{
  "channel": "Smtp",
  "recipients": ["user@example.com"],
  "subject": "Alert: server down",
  "body": "<h1>Critical</h1>",
  "bodyFormat": "Html",
  "priority": "High",
  "scheduledAt": null,
  "metadata": {
    "source": "monitoring-system"
  }
}
```

| Поле | Тип | Значения | По умолчанию |
|------|-----|----------|-------------|
| `channel` | string | `Smtp`, `Telegram`, `Slack`, `Teams`, `Sms`, `Webhook` | — |
| `recipients` | string[] | Email-адреса / chat ID | — |
| `subject` | string | До 500 символов | `null` |
| `body` | string | До 1 МБ | — |
| `bodyFormat` | string | `PlainText`, `Html`, `Markdown` | `PlainText` |
| `priority` | string | `Low`, `Normal`, `High`, `Critical` | `Normal` |
| `scheduledAt` | ISO 8601 | Время отправки в будущем | `null` (немедленно) |

**Ответ `202 Accepted`:**

```json
{
  "messageId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "status": "Accepted",
  "acceptedAt": "2026-04-16T12:00:00Z"
}
```

### `GET /healthz`

Возвращает `Healthy` / `Unhealthy`.

### `GET /healthz/ready`

Проверяет готовность зависимостей (RabbitMQ).

---

## Admin UI (Blazor WebAssembly)

Админка хостится самим API по корневому пути. Откройте http://localhost:8080/.
Доступ к рабочим страницам защищён SSO через Keycloak OpenID Connect (Authorization Code + PKCE). В локальном Docker-стеке realm `mailgate` импортируется автоматически; тестовый пользователь: `mailgate-admin` / `mailgate`.

| Страница | URL | Назначение |
|----------|-----|------------|
| Панель | `/` | Сводная статистика за последние 24ч |
| Рассылки | `/notifications` | Журнал отправок с фильтрами и ретраями |
| Новая рассылка | `/notifications/new` | Ручная отправка из UI |
| Получатели | `/recipients` | Адресная книга (ФИО + email, CRUD) |
| Настройки SMTP | `/smtp` | Эффективные настройки, переопределение из БД, тест подключения |

### SSO / Авторизация

- Blazor WASM перенаправляет пользователя в Keycloak (`http://keycloak.localhost:8180/realms/mailgate`) и получает access token через Authorization Code + PKCE.
- Запросы UI к `/api/admin/*` отправляются с `Authorization: Bearer ...`.
- API валидирует JWT, audience `mailgate-api`, issuer Keycloak и роль `mailgate-admin`.
- `/api/admin/recipients`, `/api/admin/notifications`, `/api/admin/smtp` требуют авторизацию.
- `/healthz`, `/healthz/ready`, Swagger/Scalar в Development и публичный `POST /api/notifications` остаются открытыми.
- Kerberos/SPNEGO подключается на стороне Keycloak (User Federation / authentication flow). MailGate не содержит Kerberos-логики и доверяет OIDC-токенам Keycloak.

Подробная инструкция по настройке Keycloak для защищённой среды: [`config/keycloak/README.md`](config/keycloak/README.md).

### Swagger / OAuth2

Swagger UI доступен по адресу http://localhost:8080/swagger. Для проверки защищённых `/api/admin/*` ручек:

1. В Swagger UI нажмите **Authorize**.
2. Выберите схему **Keycloak OAuth2**.
3. Swagger перенаправит в Keycloak; войдите пользователем с ролью `mailgate-admin`.
4. После возврата Swagger сам получит `access_token` через Authorization Code Flow + PKCE.
5. Выполняйте защищённые запросы. Если токен принят, но не хватает роли `mailgate-admin`, API вернёт `403`; если токен невалидный или отсутствует — `401`.

Также оставлена ручная схема **Bearer**: туда можно вставить уже полученный `access_token`.

### Адресная книга

- Поиск по ФИО и email.
- При создании рассылки получателей можно выбирать из книги и/или добавлять «ad-hoc» email'ы через запятую/перенос строки.
- Email хранится в нижнем регистре, уникальность гарантируется индексом БД.

### Ретраи

- Админ может повторно отправить любую рассылку со статусом `Failed`.
- В `notification_logs` сохраняется полный JSON исходного сообщения (`PayloadJson`), поэтому ретрай не требует от клиента повторять тело.
- Ретрай публикует новое `NotificationMessage` с новым `MessageId` и заголовком `MailGate-RetryOf-MessageId`, по которому Worker связывает историю через колонку `RetryOfMessageId`.
- Ретраи, инициированные автоматически MassTransit (exponential backoff), используют тот же `MessageId` и обновляют существующую запись лога.

### Гибридная конфигурация SMTP

SMTP-настройки читаются провайдером в следующем порядке:

1. Запись в таблице `smtp_configurations` (если есть) — задаётся через UI.
2. `appsettings.json` / переменные окружения — база, если в БД ничего не переопределено.

Секреты (`Password`, `OAuth2.ClientSecret`) шифруются ASP.NET Core Data Protection при сохранении и расшифровываются только внутри процесса. В API наружу отдаётся лишь флаг `hasPassword` / `hasClientSecret`.

Эффективные значения кешируются в памяти на 30 секунд; API сбрасывает кеш сразу после сохранения, Worker подтянет новые настройки в пределах TTL кеша.

Кнопка **«Проверить подключение»** выполняет TCP+TLS handshake и авторизацию с указанными значениями без сохранения (параметры берутся прямо из формы).

---

## Ключевые абстракции

### `INotificationChannel`

Интерфейс, который должен реализовать любой канал доставки:

```csharp
public interface INotificationChannel
{
    ChannelType ChannelType { get; }
    Task<ChannelResult> SendAsync(NotificationMessage message, CancellationToken ct = default);
}
```

### Добавление нового канала

1. Создать проект `MailGate.Channels.<Name>`
2. Реализовать `INotificationChannel`
3. Зарегистрировать в DI:

```csharp
services.AddSingleton<INotificationChannel, TelegramChannel>();
```

`ChannelRouter` подхватит новый канал автоматически — никаких изменений в остальных проектах не требуется.

---

## Надёжная доставка

```
API                  RabbitMQ              Worker
 │                       │                    │
 │──publish──────────────►│                    │
 │                       │──consume───────────►│
 │                       │                    │──SendAsync──► Channel
 │                       │                    │
 │                       │          ◄──Success─┤  ACK → сообщение удалено
 │                       │                    │
 │                       │          ◄──Failure─┤  NACK → retry (x3 exponential backoff)
 │                       │                    │
 │                       │◄──Dead-letter───────┤  После 3 попыток → mailgate-notification-message_error
```

- **Retry policy**: 3 попытки, экспоненциальный backoff: 5 с → ~25 с → ~2.5 мин
- **Dead-letter queue**: `notification-message_error` — сообщения после исчерпания попыток
- **Consumer-side Outbox**: сообщения, опубликованные внутри консьюмера, сохраняются в PostgreSQL до успешного завершения обработки
- **Audit log**: каждая обработка записывается в таблицу `notification_logs` с `AttemptCount` и `Status`

---

## Конфигурация

### SMTP (appsettings.json)

```json
{
  "Smtp": {
    "Host": "localhost",
    "Port": 1025,
    "UseTls": false,
    "FromAddress": "noreply@example.com",
    "FromName": "MailGate"
  }
}
```

### SMTP через Exchange Online (OAuth2)

```json
{
  "Smtp": {
    "Host": "smtp.office365.com",
    "Port": 587,
    "UseTls": true,
    "FromAddress": "noreply@company.com",
    "FromName": "MailGate",
    "OAuth2": {
      "TenantId": "<Azure Tenant ID>",
      "ClientId": "<App Registration Client ID>",
      "ClientSecret": "<Client Secret>",
      "SmtpUserPrincipalName": "noreply@company.com"
    }
  }
}
```

> Секреты рекомендуется хранить через `dotnet user-secrets` в dev-среде и через переменные окружения / Vault в продакшне.

### MassTransit / RabbitMQ

```json
{
  "MassTransit": {
    "Host": "localhost",
    "Port": 5672,
    "VirtualHost": "/",
    "Username": "mailgate",
    "Password": "mailgate"
  }
}
```

### SSO / Keycloak

API проверяет JWT Bearer по секции `Authentication:Keycloak`:

```json
{
  "Authentication": {
    "Keycloak": {
      "Authority": "http://keycloak.localhost:8180/realms/mailgate",
      "Audience": "mailgate-api",
      "ClientId": "mailgate-admin",
      "AdminRole": "mailgate-admin",
      "RequireHttpsMetadata": false
    }
  }
}
```

Blazor WASM читает публичные OIDC-настройки из `src/MailGate.Admin/wwwroot/appsettings.json`. Там нельзя хранить client secret.

---

## Стек технологий

| Компонент | Библиотека |
|-----------|-----------|
| Фреймворк | .NET 10 / ASP.NET Core Minimal API |
| Очередь сообщений | MassTransit 8.3 + RabbitMQ |
| SMTP-клиент | MailKit 4 |
| OAuth2 (Exchange) | Microsoft.Identity.Client (MSAL) |
| SSO / IAM | Keycloak + OpenID Connect / JWT Bearer |
| ORM | EF Core 10 + Npgsql (PostgreSQL) |
| Валидация | FluentValidation 11 |
| OpenAPI UI | Scalar + Swagger UI (Swashbuckle) |
| Контейнеризация | Docker / Docker Compose |
| Тесты | xUnit + NSubstitute + Testcontainers |
| SMTP dev-сервер | MailHog |

---

## Будущие каналы

Архитектура рассчитана на добавление новых провайдеров без изменения существующего кода:

- **Telegram** — `MailGate.Channels.Telegram` (Bot API)
- **Microsoft Teams** — `MailGate.Channels.Teams` (Incoming Webhook / Graph API)
- **Slack** — `MailGate.Channels.Slack` (Webhook / Bot)
- **SMS** — `MailGate.Channels.Sms` (Twilio / SMSC.ru)
- **Push** — `MailGate.Channels.Push` (FCM / APNs)
- **Webhook** — `MailGate.Channels.Webhook` (HTTP POST на произвольный URL)

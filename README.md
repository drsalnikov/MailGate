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
│   ├── MailGate.Api/             # REST endpoint, валидация, публикация в RMQ
│   ├── MailGate.Worker/          # MassTransit Consumer, ChannelRouter
│   ├── MailGate.Core/            # Доменные модели, интерфейсы
│   ├── MailGate.Channels.Smtp/   # SMTP/Exchange провайдер (MailKit + OAuth2)
│   └── MailGate.Infrastructure/  # MassTransit, EF Core, PostgreSQL
├── tests/
│   ├── MailGate.Api.Tests/
│   ├── MailGate.Worker.Tests/
│   └── MailGate.Channels.Smtp.Tests/
├── docker-compose.yml            # RabbitMQ + PostgreSQL + MailHog
└── MailGate.sln
```

---

## Быстрый старт

### Требования

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

### 1. Поднять инфраструктуру

```bash
docker compose up -d
```

Запускает:
- **RabbitMQ** — `amqp://localhost:5672`, Management UI: http://localhost:15672 (`mailgate` / `mailgate`)
- **PostgreSQL** — `localhost:5432`, БД `mailgate`
- **MailHog** — SMTP сервер для разработки на `localhost:1025`, UI: http://localhost:8025

### 2. Применить миграции БД

```bash
dotnet ef database update \
  --project src/MailGate.Infrastructure \
  --startup-project src/MailGate.Worker
```

### 3. Запустить Worker

```bash
dotnet run --project src/MailGate.Worker
```

### 4. Запустить API

```bash
dotnet run --project src/MailGate.Api --urls "http://localhost:5100"
```

Scalar API Reference: http://localhost:5100/scalar/v1

### 5. Отправить тестовое уведомление

```bash
curl -X POST http://localhost:5100/api/notifications \
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

---

## Стек технологий

| Компонент | Библиотека |
|-----------|-----------|
| Фреймворк | .NET 10 / ASP.NET Core Minimal API |
| Очередь сообщений | MassTransit 8.3 + RabbitMQ |
| SMTP-клиент | MailKit 4 |
| OAuth2 (Exchange) | Microsoft.Identity.Client (MSAL) |
| ORM | EF Core 10 + Npgsql (PostgreSQL) |
| Валидация | FluentValidation 11 |
| OpenAPI UI | Scalar |
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

# Настройка и проверка MailGate

## SMTP и ключи

API и Worker используют `DataProtection:KeysPath` и одинаковое имя приложения `MailGate`. Compose подключает общий постоянный volume `data_protection_keys`. XML key ring в этом варианте НЕ зашифрован на диске; доступ к Docker, volume и резервным копиям должен быть ограничен. SMTP-пароли и OAuth2 secrets в БД зашифрованы. Открытые пароли не возвращаются браузеру.

Для запуска API и Worker непосредственно из PowerShell можно задать `$env:DataProtection__KeysPath = Join-Path $env:LOCALAPPDATA 'MailGate\keys'` в каждом окне запуска. Это один общий каталог для текущего пользователя Windows.

Для запуска без Compose задайте `DataProtection__KeysPath` для обоих сервисов: один и тот же доступный постоянный каталог. При разных Docker-хостах локальные volumes не являются общими. Ключи не коммитить, не копировать в образ и не удалять между деплоями. Не используйте `down -v`, если данные нужны.

После перехода на новый key ring или новое ApplicationName повторно введите и сохраните SMTP-пароль. Для восстановления старого секрета нужны исходные ключи и исходный discriminator; новый key ring не восстановит потерянный ключ. Страница настроек и сохранение читают метаданные без расшифровки, поэтому старый недоступный пароль можно заменить. Пустое поле при сохранении сохраняет прежний секрет. При отправке ошибка расшифровки явная: попытки анонимной отправки из-за этой ошибки нет.

Проверка подключения использует секрет формы, а если поле пусто — сохранённый секрет. Для нового канала вводите новый пароль явно. Проверка не отправляет письмо и не проверяет право Send As. Реальная отправка выполняется Worker. SMTP-конфигурация кешируется по `Smtp:SettingsCacheSeconds` (по умолчанию 30): обновляется при следующей отправке после истечения TTL, не по таймеру. Сохранение в API не сбрасывает память Worker немедленно.

На порту 25/587 обычно STARTTLS On, SSL Off; на 465 — SSL On, STARTTLS Off, если это подтверждено администраторами SMTP. SSL означает TLS сразу; оба режима одновременно включать не нужно. Чекбокс «Проверять отзыв сертификатов» включён по умолчанию и сохраняется в БД; он влияет на API test и Worker. Отключение разрешает сертификаты без проверки отзыва, но не отменяет проверку цепочки и имени. CRL 403 требует проверки маршрута/прокси или правил корпоративного PKI.

## Миграция и запуск

Новая миграция `AddSmtpCertificateRevocationCheck` добавляет boolean с default true для существующих записей. Сохраните migration, designer и snapshot в Git. Worker применяет миграции при запуске; для управляемого деплоя примените их заранее до обновления API:

```powershell
$env:ConnectionStrings__DefaultConnection = 'Host=localhost;Database=mailgate;Username=mailgate;Password=<ваш-пароль>'
dotnet ef database update --project src/MailGate.Infrastructure --startup-project src/MailGate.Worker
Remove-Item Env:\ConnectionStrings__DefaultConnection
docker compose up -d --build api worker frontend
```

Design-time factory читает `ConnectionStrings__DefaultConnection`; параметры не записываются в миграцию. Не публикуйте реальные пароли в командах CI или логах.

Проверка: сохранить пароль → отправить письмо → пересоздать API/Worker без удаления volumes → отправить снова без повторного ввода. Нужны доверенные УЦ и рабочие credentials SMTP. При недоступном ключе страница должна открываться, а отправка возвращать понятную ошибку. После повторного сохранения правильного пароля отправка должна восстановиться.

## Сертификаты

Публичные корпоративные CA в PEM `.crt` положите в корневой `crt/`; приватные ключи туда не добавлять. API/Worker собираются с контекстом корня, сертификаты устанавливаются в build и final, без подавления ошибок. Пустая папка допустима, но корпоративное доверие тогда не настроено. Подробнее: `crt/README.md`.

## Корпоративный npm

Обычный Compose использует публичный npm без обязательного токена. Для Nexus скопируйте `src/MailGate.Frontend/.npmrc.example` в `.npmrc`, замените только адрес и scope Nexus, оставив `${NPM_TOKEN}`. В игнорируемом корневом `.env` задайте `NPM_TOKEN` (одинарные кавычки сохраняют `$` буквально). Запускайте:

```powershell
docker compose -f docker-compose.yml -f docker-compose.npm.yml up -d --build frontend
```

В GitLab сохраните NPM_TOKEN как masked/hidden secret; protected переменная доступна только разрешённым protected pipeline. В существующем Docker build job передайте:

```sh
docker build --secret id=npmrc,src=src/MailGate.Frontend/.npmrc --secret id=npm_token,env=NPM_TOKEN -t "$CI_REGISTRY_IMAGE/frontend:$CI_COMMIT_SHA" src/MailGate.Frontend
```

Нужен современный BuildKit с secret env mounts. Реальный .npmrc игнорируется Git и Docker-контекстом; pipeline должен создать его из безопасного шаблона или получить как file secret. Токен не передавать через ARG/ENV, не печатать в лог и не публиковать как artifact. После утечки отозвать и заменить токен, отдельно обработать историю Git и старые артефакты.

В финальном frontend добавлено обновление libxml2 из репозитория Alpine. Устранение CVE-2024-56171 необходимо подтвердить повторным сканированием именно опубликованного образа. Если vendor repo не содержит исправления, нужен обновлённый базовый образ; успешная сборка не доказывает устранение CVE.

## Keycloak, CORS и RabbitMQ

Вход фронтенда настраивается runtime `/config.json` из окружения; appsettings API автоматически не переписывается. .NET environment `Authentication__Keycloak__*` переопределяет JSON. Authority API должен совпадать с iss access token, aud содержать KEYCLOAK_AUDIENCE (mailgate-api), groups — точный KEYCLOAK_ADMIN_GROUP. Audience mapper и Group Membership настраиваются у фактического клиента; после изменения войдите заново. Пустая группа запрещает доступ. Нужные корпоративные адреса не зашиты в образ. SwaggerClientId передаётся из KEYCLOAK_CLIENT_ID.

CORS задаётся CORS_ORIGIN_0/1 с полным origin. Заголовок Access-Control-Allow-Origin должен добавляться одним компонентом, без дублирования в proxy. 502 требует проверки upstream; 401 — JWT, 403 после JWT — права группы.

RABBITMQ_HOST, RABBITMQ_CONNECTION_PORT и RABBITMQ_VHOST задают подключение API/Worker. Не путайте внутренний порт 5672 с Management 15672 и опубликованным RABBITMQ_PORT. API/Worker используют один vhost. На существующем брокере создайте vhost и права отдельно: DEFAULT_* применяются при первоначальной инициализации, не мигрируют пользователей и очереди существующего volume.

## Переход на Vault

Возможны KV (пароль хранится в Vault) или Transit (шифротекст остаётся в PostgreSQL). Нужны HTTPS-доступ, сервисная аутентификация, минимальные политики, обновление токенов, таймауты и аудит без раскрытия секретов. Потребуется реализация провайдера защиты/хранения секретов и миграция старых значений, пока доступны старые ключи. Простой перенос key ring в KV не означает его защиту при выгрузке. После миграции сохраняйте старые ключи для восстановления старых backup до окончания срока их хранения. Интеграция Vault сейчас не реализована.

## Локальный Docker Desktop без RED SOFT registry

```sh
docker compose -f docker-compose.yml -f docker-compose.local.yml up -d --build
```

Override использует официальные образы .NET и отдельный тестовый Keycloak на `http://keycloak.localhost:8181`, не изменяя существующий Keycloak на 8180. Тестовый realm импортируется из `config/keycloak/mailgate-local-realm.json`, пользователь `mailgate-admin` включён в `/MailGateAdmins`. Пароль тестового пользователя указан в этом файле. Override не предназначен для корпоративного развёртывания; доверенные корпоративные УЦ из RED SOFT Dockerfile в него не устанавливаются.

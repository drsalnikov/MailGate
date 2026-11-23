# Один docker-compose.yml

Все настройки локального Docker теперь находятся в корневом `docker-compose.yml`. Дополнительных Compose-файлов для локальной сборки, SMTP-файла, npm или адресов готовых образов нет.

## Запуск

Из корня репозитория:

```bash
docker compose up -d --build
docker compose ps
```

Frontend: http://localhost:8091, API: http://localhost:8080, MailHog: http://localhost:8025. Единственный управляемый этим Compose Keycloak — `keycloak-local`, http://keycloak.localhost:8181. Используется `config/keycloak/mailgate-local-realm.json`. Пароли демонстрационных пользователей предназначены только для локального стенда. БД Keycloak теперь сохраняется в volume `keycloak_data` при пересоздании контейнера.

Существующие volumes PostgreSQL, RabbitMQ и Data Protection сохраняются. Обычное обновление не требует `down -v`. Старый контейнер `mailgate-keycloak` на 8180 больше не входит в конфигурацию; его не нужно запускать для этого стенда. Автоматически удалять его данные не требуется.

Если этот старый контейнер сохранён, Compose может вывести `Found orphan containers ([mailgate-keycloak])`. Это предупреждение о прежнем контейнере, не ошибка нового запуска. Не применяйте `--remove-orphans` без решения о сохранении его встроенной БД; в текущем окружении контейнер оставлен остановленным ради сохранности данных.

При обновлении уже существующего `keycloak-local`, который хранил H2 только внутри контейнера, **сначала перенесите его БД в новый volume**, иначе новое хранилище будет пустым. В текущей локальной среде перенос выполняется с резервной копией. Для другого компьютера: остановите контейнер, сохраните `/opt/keycloak/data` через `docker cp`, создайте контейнер с новым volume без запуска, восстановите данные с владельцем UID 1000, затем запустите. Либо согласованно создайте новый тестовый realm, если прежние пользователи не нужны. Импорт не обновляет уже существующий realm.

## Переменные вместо дополнительных файлов

Настройки можно записать в игнорируемый `.env` в корне, скопировав `.env.example`. Не перезаписывайте уже настроенный `.env` автоматически.

| Переменная | Значение по умолчанию / назначение |
| --- | --- |
| `API_DOCKERFILE` | `src/Dockerfile.local`; корпоративный вариант `src/MailGate.Api/Dockerfile` |
| `WORKER_DOCKERFILE` | `src/Dockerfile.local`; корпоративный вариант `src/MailGate.Worker/Dockerfile` |
| `SMTP_CREDENTIALS_DIRECTORY` | `./secrets/smtp`, каталог хоста с `account.json` |
| `NUGET_CONFIG_FILE` | `./src/NuGet.Config`, файл для BuildKit restore |
| `NPMRC_FILE` | `./config/npmrc.public`, публичный npm без авторизации |
| `NPM_TOKEN_FILE` | `./config/build-secret.empty`, пустой токен для публичного registry |
| `KEYCLOAK_AUTHORITY` | `http://keycloak.localhost:8181/realms/mailgate`; для корпоративного Keycloak укажите свой URL |
| `KEYCLOAK_ADMIN_GROUP` | `/MailGateAdmins`, группа тестового realm. Явно пустое значение запрещает доступ |
| `MAILGATE_API_IMAGE`, `MAILGATE_WORKER_IMAGE`, `MAILGATE_FRONTEND_IMAGE` | Адреса готовых образов для серверного развёртывания; локально можно не задавать |

Пример выбора корпоративных Dockerfile в `.env`:

```dotenv
API_DOCKERFILE=src/MailGate.Api/Dockerfile
WORKER_DOCKERFILE=src/MailGate.Worker/Dockerfile
```

Сам Compose от этого не меняется. Kubernetes по-прежнему использует свой chart/манифест, а не этот локальный стек.

## Файл почтовой учётной записи

Каталог с credentials всегда подключается read-only к API и Worker как `/var/mailgate/smtp-credentials`. Если путь не задан, Docker создаст пустой `./secrets/smtp`. В ручном режиме MailHog приложение файл не читает, поэтому отсутствие `account.json` не мешает запуску.

Для настоящей почтовой учётки задайте каталог в PowerShell:

```powershell
$env:SMTP_CREDENTIALS_DIRECTORY = Read-Host 'Абсолютный путь каталога с account.json'
docker compose up -d --build
```

Либо сохраните путь в `.env`. Затем выберите файловый режим в SMTP-форме и укажите сервер/TLS/отправителя. Содержимое JSON, тестирование и ротация описаны в [smtp-sidecar.md](smtp-sidecar.md). Файл не коммитьте.

## Корпоративный npm

Для Compose используются **файлы** секретов, чтобы чистая сборка могла работать с пустым токеном без обязательной переменной окружения. GitLab build-скрипт независимо поддерживает `NPM_TOKEN` как прежде.

1. Создайте игнорируемый `src/MailGate.Frontend/.npmrc` с настоящим registry и `${NPM_TOKEN}`.
2. Сохраните настоящий токен, например в `secrets/npm-token`, одной строкой без BOM и завершающего перевода строки.
3. В `.env` добавьте:

```dotenv
NPMRC_FILE=./src/MailGate.Frontend/.npmrc
NPM_TOKEN_FILE=./secrets/npm-token
```

4. Выполните `docker compose build frontend` и `docker compose up -d --no-deps frontend`.

Если токен у вас был только в переменной `NPM_TOKEN`, для Compose перенесите его в этот локальный файл. Не записывайте значение в Git. Публичные/пустые файлы из `config` — только безопасные defaults, они не содержат корпоративных credentials.

## Сертификаты и миграции

Публичные корпоративные CA в PEM `.crt` кладите в `crt/`. Они устанавливаются в build/final и официального локального Dockerfile, и корпоративных Dockerfile. Приватные ключи в этот каталог не добавляйте. Наличие CA не заменяет сетевой доступ к SMTP/Nexus и CRL.

Сборка: `docker compose build api worker frontend`. Обновление: `docker compose up -d --no-deps api worker frontend`. Одноразовая миграция: `docker compose run --rm --no-deps worker --migrate-only` при доступной PostgreSQL.

## Готовые образы из CI

Сервисы содержат `image` с переменными `MAILGATE_*_IMAGE`. Для запуска готового набора укажите эти три адреса через `.env` или отдельный **env-файл**, не Compose-файл:

```bash
docker compose --env-file .env --env-file images.env pull api worker frontend
docker compose --env-file .env --env-file images.env up -d --no-deps --no-build api worker frontend
```

В серверном Compose сохраняйте свои сети и runtime-настройки. Draft SSH pipeline адаптирован к этим image-переменным и больше не отправляет overlay. Для вашего Kubernetes его deploy-stage всё ещё требует отдельной адаптации.

# Дополнительные диффы MailGate — 23.11.2026

База Git: `e775c18da40a6c92000e1a66b30ff4bf04726d9f`. Инструкция: [closed-network-transfer.md](../../config/closed-network-transfer.md). Не применяйте оба patch подряд. Из набора исключены генерируемые artifacts и сама инструкция, чтобы избежать рекурсии. Переводы строк в Markdown нормализованы для чтения; `.patch` сохранён как выдал Git.

## Файлы

```text
M	.dockerignore
M	.env.example
M	.gitignore
A	.gitlab-ci.yml
M	README.md
A	config/build-secret.empty
A	config/compose.md
A	config/gitlab-ci.md
A	config/npmrc.public
M	config/operations.md
A	config/smtp-account.json.example
A	config/smtp-sidecar.md
D	docker-compose.local.yml
D	docker-compose.npm.yml
M	docker-compose.yml
A	scripts/ci/build-images.sh
A	scripts/ci/check-smtp-credentials.sh
A	scripts/ci/check-templates.sh
A	scripts/ci/deploy-dev-server.sh
A	scripts/ci/deploy-dev.sh
M	src/Dockerfile.local
M	src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs
M	src/MailGate.Api/Admin/Validators/SmtpValidators.cs
M	src/MailGate.Api/Dockerfile
M	src/MailGate.Channels.Smtp/Extensions/ServiceCollectionExtensions.cs
M	src/MailGate.Channels.Smtp/Options/SmtpOptions.cs
M	src/MailGate.Channels.Smtp/Services/OptionsSmtpSettingsProvider.cs
A	src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs
M	src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs
M	src/MailGate.Core/Entities/SmtpConfiguration.cs
A	src/MailGate.Core/Enums/SmtpCredentialSource.cs
M	src/MailGate.Frontend/.dockerignore
M	src/MailGate.Frontend/src/api/types.ts
M	src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx
M	src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs
A	src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.Designer.cs
A	src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.cs
M	src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
M	src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs
M	src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs
M	src/MailGate.Worker/Dockerfile
M	src/MailGate.Worker/Program.cs
M	src/MailGate.sln
M	src/MailGate.slnx
A	tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj
A	tests/MailGate.SmtpCredentialChecks/Program.cs
A	tests/ci/test_pipeline_scripts.py
```

## Статистика

```text
 .dockerignore                                      |   6 +
 .env.example                                       |  26 +-
 .gitignore                                         |   9 +
 .gitlab-ci.yml                                     |  70 +++
 README.md                                          |  14 +-
 config/build-secret.empty                          |   0
 config/compose.md                                  |  92 ++++
 config/gitlab-ci.md                                | 224 +++++++++
 config/npmrc.public                                |   1 +
 config/operations.md                               |   6 +-
 config/smtp-account.json.example                   |   4 +
 config/smtp-sidecar.md                             | 206 ++++++++
 docker-compose.local.yml                           |  38 --
 docker-compose.npm.yml                             |  12 -
 docker-compose.yml                                 |  71 ++-
 scripts/ci/build-images.sh                         |  43 ++
 scripts/ci/check-smtp-credentials.sh               |  14 +
 scripts/ci/check-templates.sh                      |  21 +
 scripts/ci/deploy-dev-server.sh                    |  66 +++
 scripts/ci/deploy-dev.sh                           |  43 ++
 src/Dockerfile.local                               |  14 +-
 .../Admin/Endpoints/SmtpAdminEndpoints.cs          |  51 +-
 .../Admin/Validators/SmtpValidators.cs             |   8 +
 src/MailGate.Api/Dockerfile                        |   7 +-
 .../Extensions/ServiceCollectionExtensions.cs      |   1 +
 src/MailGate.Channels.Smtp/Options/SmtpOptions.cs  |   5 +
 .../Services/OptionsSmtpSettingsProvider.cs        |  26 +-
 .../Services/SmtpCredentialResolver.cs             |  58 +++
 .../Smtp/SmtpConfigurationDto.cs                   |   6 +-
 src/MailGate.Core/Entities/SmtpConfiguration.cs    |   3 +
 src/MailGate.Core/Enums/SmtpCredentialSource.cs    |   9 +
 src/MailGate.Frontend/.dockerignore                |   3 +
 src/MailGate.Frontend/src/api/types.ts             |   4 +
 .../src/pages/SmtpSettingsPage.tsx                 | 124 ++---
 .../Extensions/ServiceCollectionExtensions.cs      |  17 +-
 ...61123090000_AddSmtpCredentialSource.Designer.cs | 524 +++++++++++++++++++++
 .../20261123090000_AddSmtpCredentialSource.cs      |  17 +
 .../Migrations/MailGateDbContextModelSnapshot.cs   |   3 +
 .../Services/DbSmtpSettingsProvider.cs             |  27 +-
 .../Services/SmtpConfigurationStore.cs             |  21 +-
 src/MailGate.Worker/Dockerfile                     |   7 +-
 src/MailGate.Worker/Program.cs                     |   8 +-
 src/MailGate.sln                                   |  14 +
 src/MailGate.slnx                                  |   1 +
 .../MailGate.SmtpCredentialChecks.csproj           |   9 +
 tests/MailGate.SmtpCredentialChecks/Program.cs     |  62 +++
 tests/ci/test_pipeline_scripts.py                  | 123 +++++
 47 files changed, 1871 insertions(+), 247 deletions(-)
```

## Диффы

### `a/.dockerignore b/.dockerignore`

`````diff
diff --git a/.dockerignore b/.dockerignore
index 61aacfd..c2da045 100644
--- a/.dockerignore
+++ b/.dockerignore
@@ -1,4 +1,7 @@
 .git/
+.mailgate-ci/
+images.env
+**/__pycache__/
 .env
 .env.*
 **/.npmrc
@@ -12,3 +15,6 @@
 **/*.key
 **/*.tsbuildinfo
 **/.DS_Store
+
+**/account.json
+secrets/
`````

### `a/.env.example b/.env.example`

`````diff
diff --git a/.env.example b/.env.example
index aef147e..2036146 100644
--- a/.env.example
+++ b/.env.example
@@ -16,16 +16,16 @@ RABBITMQ_MANAGEMENT_PORT=15672
 
 # --- Keycloak ---
 KEYCLOAK_ADMIN_USER=admin
-KEYCLOAK_ADMIN_PASSWORD=admin
-KEYCLOAK_PORT=8180
+KEYCLOAK_ADMIN_PASSWORD=local-development-only
+KEYCLOAK_PORT=8181
 # Внешний URL Keycloak (KC_HOSTNAME); issuer токенов должен совпадать с ним
-KEYCLOAK_HOSTNAME=http://keycloak.localhost:8180
-KEYCLOAK_AUTHORITY=http://keycloak.localhost:8180/realms/mailgate
+KEYCLOAK_HOSTNAME=http://keycloak.localhost:8181
+KEYCLOAK_AUTHORITY=http://keycloak.localhost:8181/realms/mailgate
 KEYCLOAK_CLIENT_ID=mailgate-admin
 KEYCLOAK_AUDIENCE=mailgate-api
 # Точный путь существующей группы из groups access token; пустое значение запрещает доступ.
 # Например: /OtherApp/Administrators
-KEYCLOAK_ADMIN_GROUP=
+KEYCLOAK_ADMIN_GROUP=/MailGateAdmins
 # true — требовать HTTPS у discovery-документа Keycloak (включить в продакшене)
 KEYCLOAK_REQUIRE_HTTPS_METADATA=false
 
@@ -80,8 +80,22 @@ RABBITMQ_HOST=rabbitmq
 RABBITMQ_CONNECTION_PORT=5672
 RABBITMQ_VHOST=/
 
-# Only for docker-compose.npm.yml; keep real token in ignored .env or CI secrets
+# Docker build secrets: public/empty defaults; set file paths for corporate registries.
+# NPMRC_FILE=./src/MailGate.Frontend/.npmrc
+# NPM_TOKEN_FILE=./secrets/npm-token
+# NUGET_CONFIG_FILE=./src/NuGet.Config
+# NPM_TOKEN is used by CI docker build, while Compose uses NPM_TOKEN_FILE.
 NPM_TOKEN=
 
 # SMTP effective settings cache lifetime in each process
 SMTP_SETTINGS_CACHE_SECONDS=30
+
+# Optional absolute HOST directory containing account.json; default ./secrets/smtp
+# SMTP_CREDENTIALS_DIRECTORY=
+
+# The same Compose supports local or corporate Dockerfiles, without overrides.
+API_DOCKERFILE=src/Dockerfile.local
+WORKER_DOCKERFILE=src/Dockerfile.local
+# Corporate alternative:
+# API_DOCKERFILE=src/MailGate.Api/Dockerfile
+# WORKER_DOCKERFILE=src/MailGate.Worker/Dockerfile
`````

### `a/.gitignore b/.gitignore`

`````diff
diff --git a/.gitignore b/.gitignore
index b2721b8..602aa39 100644
--- a/.gitignore
+++ b/.gitignore
@@ -66,3 +66,12 @@ coverage/
 !.env.example
 *.pfx
 *.key
+
+# GitLab deployment state and non-secret pipeline image manifest
+.mailgate-ci/
+images.env
+__pycache__/
+
+# SMTP account files rendered by a sidecar or created locally
+account.json
+secrets/
`````

### `a/.gitlab-ci.yml b/.gitlab-ci.yml`

`````diff
diff --git a/.gitlab-ci.yml b/.gitlab-ci.yml
new file mode 100644
index 0000000..b64e0b0
--- /dev/null
+++ b/.gitlab-ci.yml
@@ -0,0 +1,70 @@
+# Linux Shell Runner tagged "mailgate", with Docker/BuildKit, bash and OpenSSH.
+# Setup: config/gitlab-ci.md. No production deploy job is defined here.
+workflow:
+  auto_cancel:
+    on_new_commit: interruptible
+  rules:
+    - if: '$CI_COMMIT_BRANCH == "develop"'
+    - when: never
+
+stages:
+  - test
+  - build
+  - deploy
+
+default:
+  tags:
+    - mailgate
+  interruptible: true
+
+variables:
+  DOCKER_BUILDKIT: "1"
+  DOTNET_SDK_IMAGE: "mcr.microsoft.com/dotnet/sdk:10.0"
+  DEV_SSH_PORT: "22"
+  DEV_API_HEALTH_URL: "http://127.0.0.1:8080/healthz/ready"
+  DEV_FRONTEND_HEALTH_URL: "http://127.0.0.1:8091/"
+
+template_checks:
+  stage: test
+  script:
+    - bash scripts/ci/check-templates.sh
+
+smtp_credential_checks:
+  stage: test
+  script:
+    - bash scripts/ci/check-smtp-credentials.sh
+
+pipeline_script_checks:
+  stage: test
+  script:
+    - bash -n scripts/ci/check-templates.sh scripts/ci/build-images.sh scripts/ci/deploy-dev.sh scripts/ci/deploy-dev-server.sh
+    - python3 -m unittest discover -s tests/ci -v
+
+build_images:
+  stage: build
+  script:
+    - bash scripts/ci/build-images.sh
+  artifacts:
+    expire_in: 7 days
+    paths:
+      - images.env
+    reports:
+      dotenv: images.env
+
+deploy_dev:
+  stage: deploy
+  # This optional job is for Compose/SSH only. Kubernetes deployment must use
+  # your cluster's Helm/Argo CD/GitLab Agent workflow instead.
+  rules:
+    - if: '$MAILGATE_DEPLOY_TARGET == "compose-ssh"'
+    - when: never
+  interruptible: false
+  resource_group: mailgate-dev
+  environment:
+    name: development
+    deployment_tier: development
+  needs:
+    - job: build_images
+      artifacts: true
+  script:
+    - bash scripts/ci/deploy-dev.sh
`````

### `a/README.md b/README.md`

`````diff
diff --git a/README.md b/README.md
index ca6e87a..1aaa9d5 100644
--- a/README.md
+++ b/README.md
@@ -1,5 +1,9 @@
 # MailGate — Notification Microservice
 
+Полный перенос в закрытый контур: [пошаговая инструкция и ссылки на полные/дополнительные диффы](config/closed-network-transfer.md).
+
+Настройка SMTP из sidecar-файла, тестовый ручной режим и локальная работа без Vault: [config/smtp-sidecar.md](config/smtp-sidecar.md).
+
 ## Доступ по группе Keycloak
 
 MailGate проверяет точное совпадение пути группы в `groups` access token. Роль администратора больше не используется для предоставления доступа. Пустая настройка группы запрещает доступ всем пользователям.
@@ -76,6 +80,10 @@ MailGate/
 
 В меню «Рассылка → Шаблоны писем» доступен блочный конструктор. На `/notifications/new` можно выбрать шаблон, ввести основной текст и увидеть серверный предпросмотр. [Настройка, API и развёртывание](config/email-templates.md).
 
+## GitLab CI/CD
+
+В `.gitlab-ci.yml` подготовлены проверки, сборка трёх образов и автоматический deploy ветки `develop` на dev через SSH/Docker Compose. Требуется Linux Shell Runner с тегом `mailgate`. До запуска заполните GitLab Variables и проверьте серверную конфигурацию: [пошаговая настройка и объяснение YAML](config/gitlab-ci.md).
+
 ## Быстрый старт
 
 ### Локальный Docker Desktop
@@ -83,13 +91,13 @@ MailGate/
 Из корня репозитория запустите вариант на официальных .NET-образах:
 
 ```bash
-docker compose -f docker-compose.yml -f docker-compose.local.yml up -d --build
-docker compose -f docker-compose.yml -f docker-compose.local.yml ps
+docker compose up -d --build
+docker compose ps
 ```
 
 Frontend: http://localhost:8091, API: http://localhost:8080, MailHog: http://localhost:8025. Локальный Keycloak: http://keycloak.localhost:8181, realm `mailgate`. Тестовый пользователь `mailgate-admin` и его пароль заданы в `config/keycloak/mailgate-local-realm.json`; он входит в `/MailGateAdmins`. Второй тестовый пользователь не имеет административного доступа. Учётные записи и пароли этого стенда предназначены только для разработки.
 
-Override задаёт локальную группу и authority для API/frontend; корпоративные Dockerfile и настройки не заменяются. Если порты заняты или `.env` содержит настройки другого окружения, проверьте их перед запуском. Worker применяет миграции при старте. Не удаляйте PostgreSQL и общий volume ключей при обычном обновлении.
+Единый Compose задаёт локальные значения по умолчанию. Для корпоративной сборки выберите API_DOCKERFILE и WORKER_DOCKERFILE в .env; для SMTP-файла задайте SMTP_CREDENTIALS_DIRECTORY. См. [config/compose.md](config/compose.md). Если порты заняты или `.env` содержит настройки другого окружения, проверьте их перед запуском. Worker применяет миграции при старте. Не удаляйте PostgreSQL и общий volume ключей при обычном обновлении.
 
 Полная инструкция ручного переноса шаблонов, миграций и конструкторов: [artifacts/mailgate-templates-full-instructions.md](artifacts/mailgate-templates-full-instructions.md).
 
`````

### `a/config/build-secret.empty b/config/build-secret.empty`

`````diff
diff --git a/config/build-secret.empty b/config/build-secret.empty
new file mode 100644
index 0000000..e69de29
`````

### `a/config/compose.md b/config/compose.md`

`````diff
diff --git a/config/compose.md b/config/compose.md
new file mode 100644
index 0000000..7bbe3d3
--- /dev/null
+++ b/config/compose.md
@@ -0,0 +1,92 @@
+# Один docker-compose.yml
+
+Все настройки локального Docker теперь находятся в корневом `docker-compose.yml`. Дополнительных Compose-файлов для локальной сборки, SMTP-файла, npm или адресов готовых образов нет.
+
+## Запуск
+
+Из корня репозитория:
+
+```bash
+docker compose up -d --build
+docker compose ps
+```
+
+Frontend: http://localhost:8091, API: http://localhost:8080, MailHog: http://localhost:8025. Единственный управляемый этим Compose Keycloak — `keycloak-local`, http://keycloak.localhost:8181. Используется `config/keycloak/mailgate-local-realm.json`. Пароли демонстрационных пользователей предназначены только для локального стенда. БД Keycloak теперь сохраняется в volume `keycloak_data` при пересоздании контейнера.
+
+Существующие volumes PostgreSQL, RabbitMQ и Data Protection сохраняются. Обычное обновление не требует `down -v`. Старый контейнер `mailgate-keycloak` на 8180 больше не входит в конфигурацию; его не нужно запускать для этого стенда. Автоматически удалять его данные не требуется.
+
+Если этот старый контейнер сохранён, Compose может вывести `Found orphan containers ([mailgate-keycloak])`. Это предупреждение о прежнем контейнере, не ошибка нового запуска. Не применяйте `--remove-orphans` без решения о сохранении его встроенной БД; в текущем окружении контейнер оставлен остановленным ради сохранности данных.
+
+При обновлении уже существующего `keycloak-local`, который хранил H2 только внутри контейнера, **сначала перенесите его БД в новый volume**, иначе новое хранилище будет пустым. В текущей локальной среде перенос выполняется с резервной копией. Для другого компьютера: остановите контейнер, сохраните `/opt/keycloak/data` через `docker cp`, создайте контейнер с новым volume без запуска, восстановите данные с владельцем UID 1000, затем запустите. Либо согласованно создайте новый тестовый realm, если прежние пользователи не нужны. Импорт не обновляет уже существующий realm.
+
+## Переменные вместо дополнительных файлов
+
+Настройки можно записать в игнорируемый `.env` в корне, скопировав `.env.example`. Не перезаписывайте уже настроенный `.env` автоматически.
+
+| Переменная | Значение по умолчанию / назначение |
+| --- | --- |
+| `API_DOCKERFILE` | `src/Dockerfile.local`; корпоративный вариант `src/MailGate.Api/Dockerfile` |
+| `WORKER_DOCKERFILE` | `src/Dockerfile.local`; корпоративный вариант `src/MailGate.Worker/Dockerfile` |
+| `SMTP_CREDENTIALS_DIRECTORY` | `./secrets/smtp`, каталог хоста с `account.json` |
+| `NUGET_CONFIG_FILE` | `./src/NuGet.Config`, файл для BuildKit restore |
+| `NPMRC_FILE` | `./config/npmrc.public`, публичный npm без авторизации |
+| `NPM_TOKEN_FILE` | `./config/build-secret.empty`, пустой токен для публичного registry |
+| `KEYCLOAK_AUTHORITY` | `http://keycloak.localhost:8181/realms/mailgate`; для корпоративного Keycloak укажите свой URL |
+| `KEYCLOAK_ADMIN_GROUP` | `/MailGateAdmins`, группа тестового realm. Явно пустое значение запрещает доступ |
+| `MAILGATE_API_IMAGE`, `MAILGATE_WORKER_IMAGE`, `MAILGATE_FRONTEND_IMAGE` | Адреса готовых образов для серверного развёртывания; локально можно не задавать |
+
+Пример выбора корпоративных Dockerfile в `.env`:
+
+```dotenv
+API_DOCKERFILE=src/MailGate.Api/Dockerfile
+WORKER_DOCKERFILE=src/MailGate.Worker/Dockerfile
+```
+
+Сам Compose от этого не меняется. Kubernetes по-прежнему использует свой chart/манифест, а не этот локальный стек.
+
+## Файл почтовой учётной записи
+
+Каталог с credentials всегда подключается read-only к API и Worker как `/var/mailgate/smtp-credentials`. Если путь не задан, Docker создаст пустой `./secrets/smtp`. В ручном режиме MailHog приложение файл не читает, поэтому отсутствие `account.json` не мешает запуску.
+
+Для настоящей почтовой учётки задайте каталог в PowerShell:
+
+```powershell
+$env:SMTP_CREDENTIALS_DIRECTORY = Read-Host 'Абсолютный путь каталога с account.json'
+docker compose up -d --build
+```
+
+Либо сохраните путь в `.env`. Затем выберите файловый режим в SMTP-форме и укажите сервер/TLS/отправителя. Содержимое JSON, тестирование и ротация описаны в [smtp-sidecar.md](smtp-sidecar.md). Файл не коммитьте.
+
+## Корпоративный npm
+
+Для Compose используются **файлы** секретов, чтобы чистая сборка могла работать с пустым токеном без обязательной переменной окружения. GitLab build-скрипт независимо поддерживает `NPM_TOKEN` как прежде.
+
+1. Создайте игнорируемый `src/MailGate.Frontend/.npmrc` с настоящим registry и `${NPM_TOKEN}`.
+2. Сохраните настоящий токен, например в `secrets/npm-token`, одной строкой без BOM и завершающего перевода строки.
+3. В `.env` добавьте:
+
+```dotenv
+NPMRC_FILE=./src/MailGate.Frontend/.npmrc
+NPM_TOKEN_FILE=./secrets/npm-token
+```
+
+4. Выполните `docker compose build frontend` и `docker compose up -d --no-deps frontend`.
+
+Если токен у вас был только в переменной `NPM_TOKEN`, для Compose перенесите его в этот локальный файл. Не записывайте значение в Git. Публичные/пустые файлы из `config` — только безопасные defaults, они не содержат корпоративных credentials.
+
+## Сертификаты и миграции
+
+Публичные корпоративные CA в PEM `.crt` кладите в `crt/`. Они устанавливаются в build/final и официального локального Dockerfile, и корпоративных Dockerfile. Приватные ключи в этот каталог не добавляйте. Наличие CA не заменяет сетевой доступ к SMTP/Nexus и CRL.
+
+Сборка: `docker compose build api worker frontend`. Обновление: `docker compose up -d --no-deps api worker frontend`. Одноразовая миграция: `docker compose run --rm --no-deps worker --migrate-only` при доступной PostgreSQL.
+
+## Готовые образы из CI
+
+Сервисы содержат `image` с переменными `MAILGATE_*_IMAGE`. Для запуска готового набора укажите эти три адреса через `.env` или отдельный **env-файл**, не Compose-файл:
+
+```bash
+docker compose --env-file .env --env-file images.env pull api worker frontend
+docker compose --env-file .env --env-file images.env up -d --no-deps --no-build api worker frontend
+```
+
+В серверном Compose сохраняйте свои сети и runtime-настройки. Draft SSH pipeline адаптирован к этим image-переменным и больше не отправляет overlay. Для вашего Kubernetes его deploy-stage всё ещё требует отдельной адаптации.
`````

### `a/config/gitlab-ci.md b/config/gitlab-ci.md`

`````diff
diff --git a/config/gitlab-ci.md b/config/gitlab-ci.md
new file mode 100644
index 0000000..27e5dd9
--- /dev/null
+++ b/config/gitlab-ci.md
@@ -0,0 +1,224 @@
+# GitLab CI/CD: автоматический dev для MailGate
+
+**Для вашего Kubernetes:** готовый deploy через SSH/Compose не применяется. По умолчанию job `deploy_dev` отключена; включается только при `MAILGATE_DEPLOY_TARGET=compose-ssh`. Для Kubernetes оставьте её выключенной и добавьте развёртывание через используемые у вас Helm/Argo CD/GitLab Agent. До согласования типа Runner даже test/build jobs могут требовать адаптации: они рассчитаны на Linux Shell Runner с Docker.
+
+## Принятая схема и её границы
+
+Готовый `.gitlab-ci.yml` рассчитан на **Linux Shell Runner с Docker** и **Linux dev-сервер с Docker Compose**, доступный Runner по SSH. Runner помечен тегом `mailgate`. Если у вас Docker executor, Kubernetes или Windows Runner, этот YAML нельзя просто запустить без адаптации. Фактические адреса, тип Runner и переменные инфраструктуры в этой среде неизвестны.
+
+Pipeline запускается для ветки `develop`: проверяет шаблоны и SMTP credentials, собирает API/Worker/frontend и публикует три образа в GitLab Container Registry. Автоматическое обновление Compose-dev включается только отдельной переменной выше. Для `main` deploy не определён. Последний **успешно проверенный и собранный** pipeline может попасть на dev; запуск нового pipeline сам по себе ничего на сервере не меняет.
+
+**Тег образа:** `<полный-SHA>-pipeline<CI_PIPELINE_ID>`. Это фиксированный набор образов конкретной сборки, без изменяемого `latest`. Нельзя вручную перезаписывать эти теги другими образами; при наличии настройки registry включите их immutability.
+
+`resource_group` не допускает параллельные deploy jobs. На сервере дополнительно используются `flock` и номер pipeline: более старый pipeline не заменяет новый, который уже начал обновление. Задача deploy не прерывается автоматически новым коммитом. Если новая версия сломалась, pipeline становится красным; автоматического отката БД или контейнеров нет. [Сериализация deploy](https://docs.gitlab.com/ci/resource_groups/), [настройки безопасности развёртывания](https://docs.gitlab.com/ci/environments/deployment_safety/).
+
+## Какие файлы добавлены
+
+| Файл | Зачем |
+| --- | --- |
+| `.gitlab-ci.yml` | Ветка, стадии, jobs, артефакты и окружение development |
+| `scripts/ci/check-templates.sh` | 37 проверок в SDK-контейнере |
+| `scripts/ci/build-images.sh` | BuildKit-секреты, три build/push, формирование `images.env` |
+| `scripts/ci/deploy-dev.sh` | Проверка переменных, SSH и передача только списка образов |
+| `scripts/ci/deploy-dev-server.sh` | Блокировка, миграция, обновление и проверка доступности |
+
+Также Worker получил режим `--migrate-only`: применяет миграции и выходит, не запуская обработчик RabbitMQ. API/Worker Dockerfile принимают необязательный секрет `nuget_config` для restore; без него используют прежний `src/NuGet.Config`. Существующий локальный Compose не заменён.
+
+Локальный `src/Dockerfile.local` теперь передаёт аргументы приложению, чтобы тот же режим миграций работал и с локальным образом.
+
+## Шаг 1. Уточнить у администраторов инфраструктуру
+
+Попросите подтвердить:
+
+1. Адрес GitLab, наличие проекта и включённого Container Registry.
+2. Linux **Shell** Runner с тегом `mailgate`. На нём доступны Bash, Python 3, Docker Engine с BuildKit, `ssh`, `scp`. Пользователь Runner имеет право выполнять Docker.
+3. CPU-архитектуры Runner и dev совпадают. Эта реализация не делает multiarch-сборку.
+4. Runner имеет DNS/сетевой доступ к GitLab Registry, RED SOFT registry, используемым NuGet/Nexus, npm/Nexus и dev SSH.
+5. Dev-сервер имеет Docker Engine, Compose v2 с поддержкой нескольких `--env-file`, Bash, `curl`, `flock` (обычно пакет `util-linux`). Он видит GitLab Registry и свою PostgreSQL/RabbitMQ/Keycloak.
+
+Это отдельный доверенный Runner проекта/команды: доступ к Docker daemon даёт широкие права на хост. Не выдавайте deploy-секреты недоверенным fork/MR. [Варианты GitLab Runner для Docker-команд](https://docs.gitlab.com/ci/docker/using_docker_build/).
+
+Если Runner ещё нет: Settings → CI/CD → Runners → New project runner. Задайте тег `mailgate`, при необходимости ограничьте protected-ветками. На подготовленном Linux-хосте администратор выполняет `gitlab-runner register`, указывает настоящий URL GitLab, выданный runner authentication token и executor **shell**. Установку GitLab Runner/Docker выполняют по правилам вашей ОС и контура.
+
+Проверьте от имени пользователя Runner:
+
+```bash
+bash --version
+docker version
+docker buildx version
+ssh -V
+```
+
+## Шаг 2. Подготовить директорию приложения на dev
+
+Pipeline **не создаёт инфраструктуру с нуля**, не выполняет `down`, не удаляет volumes и не отправляет `.env` из GitLab. Он обновляет только `api`, `worker`, `frontend` поверх **существующего** серверного `docker-compose.yml`.
+
+На dev должны быть:
+
+- постоянная директория проекта с подходящим `docker-compose.yml`;
+- `.env` с настоящими dev-настройками, читаемый deploy-пользователем, права желательно `600`;
+- уже работающая инфраструктура: PostgreSQL и RabbitMQ, а также нужные серверные сети;
+- доступные API и Worker общие ключи `/var/mailgate/keys`;
+- корректные сертификаты, CORS и настройки корпоративного Keycloak;
+- пользователь SSH с доступом к этой директории и Docker, без интерактивного sudo.
+
+Для нового стенда сначала вручную разверните инфраструктуру по основной инструкции проекта и настройте `.env`. Внешние PostgreSQL/RabbitMQ подключайте через ваш серверный Compose. Не запускайте вторую БД с дефолтными паролями вместо существующей. Внешний Keycloak не заменяйте тестовым.
+
+**Важно сохранить имя Compose-проекта:** оно определяет имена сети и volumes. На сервере с уже работающим MailGate получите его так:
+
+```bash
+docker inspect mailgate-api --format '{{ index .Config.Labels "com.docker.compose.project" }}'
+```
+
+Результат укажите в `DEV_COMPOSE_PROJECT`. `DEV_DEPLOY_PATH` — абсолютная директория **этого же** проекта на dev, не директория Runner и не путь на Windows. В этой версии скриптов путь должен быть без пробелов и содержать только буквы ASCII, цифры, `/`, `.`, `_`, `-`, без `..`.
+
+Перед автоматизацией проверьте, что в серверном Compose уже есть все требуемые настройки новой версии. Pipeline сохраняет этот файл, поэтому старый Compose сам по себе не получит новых параметров key ring или группы Keycloak.
+
+Скрипт использует один серверный docker-compose.yml. У сервисов должны быть поля image с переменными MAILGATE_API_IMAGE, MAILGATE_WORKER_IMAGE, MAILGATE_FRONTEND_IMAGE, как в актуальном общем Compose. Только эти значения подставляются из images.env. Свои корпоративные сети и runtime-настройки сохраните в серверном файле.
+
+## Шаг 3. Дать dev-серверу право скачивать образы
+
+В GitLab создайте Deploy Token с **read_registry** для проекта/группы. От имени **того же Linux-пользователя, под которым будет SSH deploy**, один раз выполните на dev:
+
+```bash
+read -rp 'Настоящий адрес GitLab Registry без https://: ' REGISTRY_HOST
+read -rp 'Имя Deploy Token: ' REGISTRY_READER
+read -rsp 'Значение Deploy Token: ' REGISTRY_SECRET
+printf '\n'
+printf '%s' "$REGISTRY_SECRET" | docker login "$REGISTRY_HOST" --username "$REGISTRY_READER" --password-stdin
+unset REGISTRY_SECRET
+```
+
+Адрес registry возьмите из своего GitLab, не копируйте выдуманное имя сервера. Dev хранит доступ средствами Docker credential configuration/credential helper; применяйте корпоративный способ хранения. Токен должен оставаться действующим. Использовать пароль job из CI для постоянного доступа сервера не нужно: он краткоживущий.
+
+В build job GitLab автоматически предоставляет `CI_REGISTRY`, `CI_REGISTRY_IMAGE`, `CI_REGISTRY_USER`, `CI_REGISTRY_PASSWORD`. Вводить их вручную в YAML не надо.
+
+## Шаг 4. Подготовить SSH-ключ и known_hosts
+
+Создайте отдельный ключ только для dev CI. Например, на Windows PowerShell:
+
+```powershell
+ssh-keygen -t ed25519 -f "$env:USERPROFILE\.ssh\mailgate-dev-ci" -C 'mailgate-dev-ci'
+```
+
+В этой реализации используется неинтерактивный ключ без passphrase. Администратор добавляет содержимое `mailgate-dev-ci.pub` в `~/.ssh/authorized_keys` deploy-пользователя на dev. Приватный файл не добавляйте в Git.
+
+Получите SSH host key dev из доверенной сети и сверьте fingerprint с администратором. Содержимое known_hosts загрузите в GitLab как File-переменную. Для нестандартного порта должна быть запись `[имя-сервера]:порт`. Не отключайте `StrictHostKeyChecking`; pipeline не запускает `ssh-keyscan` для слепого доверия новому ключу. [Настройка SSH в GitLab CI](https://docs.gitlab.com/ci/jobs/ssh_keys/).
+
+## Шаг 5. Заполнить GitLab CI/CD Variables
+
+Settings → CI/CD → Variables → Add variable. Не вставляйте пароли в `.gitlab-ci.yml`. Переменные типа **File** содержат сам файл, а GitLab передаёт job путь к временному файлу.
+
+| Имя | Тип | Значение / откуда взять |
+| --- | --- | --- |
+| `DEV_SSH_HOST` | Variable | Настоящее DNS-имя или IPv4 dev-сервера, без протокола и пути |
+| `DEV_SSH_USER` | Variable | Linux-пользователь с доступом к Docker и директории проекта |
+| `DEV_SSH_PORT` | Variable | SSH-порт; по умолчанию 22 |
+| `DEV_DEPLOY_PATH` | Variable | Существующий абсолютный путь проекта на dev |
+| `DEV_COMPOSE_PROJECT` | Variable | Имя существующего Compose-проекта из inspect |
+| `SSH_PRIVATE_KEY` | **File** | Приватный SSH-ключ с переводом строки в конце |
+| `SSH_KNOWN_HOSTS` | **File** | Проверенные SSH host keys dev |
+| `DEV_API_HEALTH_URL` | Variable | URL проверки API, доступный **с dev-сервера**; по умолчанию `http://127.0.0.1:8080/healthz/ready` |
+| `DEV_FRONTEND_HEALTH_URL` | Variable | URL frontend **с dev-сервера**; по умолчанию `http://127.0.0.1:8091/` |
+| `NUGET_CONFIG_FILE` | **File**, необязательно | Полный действующий NuGet.Config с корпоративными sources/credentials для сборки |
+| `NPMRC_FILE` | **File**, необязательно | Действующий `.npmrc` с настоящим registry; можно использовать `${NPM_TOKEN}` |
+| `NPM_TOKEN` | Variable, masked, необязательно | Настоящий npm/Nexus token, если выбран этот способ авторизации |
+| `DOTNET_SDK_IMAGE` | Variable, необязательно | Доступный Runner .NET 10 SDK image для проверок; default — официальный SDK из YAML |
+
+Если Nexus требует Basic Authentication, используйте согласованный корпоративный npmrc, а не подставляйте обычный пароль в `_authToken`. Docker BuildKit получает файл как секрет, а не как COPY/ARG. Для NuGet секрет передаётся аналогично. Эти файлы не надо коммитить. Значения runtime-паролей PostgreSQL/SMTP не нужны Runner: они остаются в конфигурации dev.
+
+Для deploy-переменных можно установить Environment scope `development`; для npm/NuGet-переменных build job оставьте scope `*`, так как build не является environment job. Установите Protected там, где это требуется; тогда **develop тоже должна быть protected**, иначе GitLab не выдаст эти переменные. SSH-ключ с переносами строк обычно нельзя сделать Masked: используйте File + Protected, не печатайте содержимое. Для однострочных токенов включите Masked/Hidden, если версия GitLab поддерживает.
+
+На корпоративных УЦ trust должен быть настроен отдельно на Runner/Docker daemon, в build-контейнерах и dev. Сертификаты из `crt/` устанавливаются API/Worker Dockerfile, но не настраивают доверие Runner к registry и не исправляют DNS/VPN. Если frontend должен доверять корпоративному Nexus, его Node build-stage также должен иметь подходящий CA по вашей существующей конфигурации. Не отключайте TLS-проверку.
+
+## Шаг 6. Проверить YAML и запустить первый pipeline
+
+1. В GitLab откройте Build → Pipeline editor (в старых версиях CI/CD → Editor).
+2. Выберите ветку `develop`, откройте `.gitlab-ci.yml`, выполните Validate/CI Lint. Локальная проверка YAML не заменяет проверку вашей версией GitLab.
+3. Settings → CI/CD → General pipelines: включите **Prevent outdated deployment jobs**, отключите **Allow job retries for rollback deployments**, если не планируете такой путь отката.
+4. Убедитесь, что Runner online и его tag совпадает с `mailgate` в `default.tags`.
+5. После проверки переменных и dev-конфигурации отправьте изменения в GitLab `develop`:
+
+```bash
+git add .gitlab-ci.yml scripts/ci tests/ci config/gitlab-ci.md
+git add src/MailGate.Worker/Program.cs src/MailGate.Api/Dockerfile src/MailGate.Worker/Dockerfile .gitignore .dockerignore README.md
+git add src/Dockerfile.local
+git commit -m "Add GitLab CI builds and automatic dev deployment"
+git push origin develop
+```
+
+Проверьте, что **origin указывает именно на ваш GitLab**, а не на GitHub из локального репозитория: `git remote -v`. Если GitLab имеет другой remote, используйте его имя в push. Вносить изменения remote автоматически эта настройка не пытается.
+
+Если файлы уже попали в develop, используйте Build → Pipelines → New pipeline → develop. Pipeline запускается автоматически; отдельной кнопки подтверждения deploy здесь нет. Не делайте первый push до заполнения секретов и проверки, что dev — нужный сервер.
+
+## Как читать jobs
+
+1. `template_checks` запускает реальный renderer и валидацию шаблонов в SDK-контейнере. БД/SMTP он не трогает. Параллельно `pipeline_script_checks` проверяет Bash и сценарии deploy/build с заглушками, без обращения к реальному серверу.
+2. `build_images` компилирует API и Worker внутри корпоративных Dockerfile, выполняет npm ci + TypeScript/Vite build во frontend Dockerfile. Если любая сборка падает, deploy не запускается. `images.env` содержит **только три адреса образов**, не пароли.
+3. `deploy_dev` скачивает artifact, по SSH передаёт images.env в `.mailgate-ci/releases/<pipeline-id>` на dev. Он не копирует серверную `.env` и не делает git pull на сервере.
+4. Dev скачивает точные образы, запускает одноразовый Worker с `--migrate-only`. При ошибке миграции контейнеры приложения не обновляются. Неудачная миграция всё же может изменить БД до своей ошибки — перед изменениями схемы нужны backup и review SQL.
+5. После миграции обновляются только три сервиса через `up -d --no-deps --no-build`. PostgreSQL, RabbitMQ и volumes не удаляются.
+6. Проверяются HTTP API/frontend и состояние трёх контейнеров. На сервере сохраняются `.mailgate-ci/latest-successful` и `deployed-commit`. В GitLab появляется environment `development` с номером deployment.
+
+Это последовательное обновление Compose, **не zero-downtime и не транзакционный deploy**. Краткий перерыв возможен. Изменения схемы должны быть совместимы со старым приложением, которое ещё работает во время миграции. Любые изменения списка сервисов, сетей и runtime-настроек в серверном Compose выполняются отдельно, осознанно.
+
+## Как самостоятельно писать такой YAML
+
+Минимальный каркас уже виден в файле:
+
+```yaml
+stages:
+  - test
+  - build
+  - deploy
+
+some_job:
+  stage: test
+  script:
+    - echo "Команда внутри job"
+```
+
+- Отступы — пробелы, не табуляция. `-` обозначает элемент списка.
+- `workflow.rules` решает, создавать ли pipeline для ветки. Здесь это только develop.
+- `stages` задаёт порядок: следующая стадия не начинается после ошибки предыдущей.
+- Имя `template_checks`/`build_images`/`deploy_dev` — это job, который виден в интерфейсе.
+- `script` — команды на Runner. Большие программы вынесены в Bash-файлы: их проще тестировать и читать, чем экранировать в YAML.
+- `variables` — несекретные значения по умолчанию; настоящие секреты вводятся в Settings.
+- `artifacts` передаёт images.env из build в deploy; `reports.dotenv` также делает его строки переменными job.
+- `needs` объявляет, какой build artifact нужен deploy.
+- `environment` связывает deploy с dev в GitLab, `resource_group` сериализует такие обновления.
+- `interruptible: true` позволяет отменить устаревшую проверку/сборку новым коммитом. Для deploy стоит false, чтобы не оборвать миграцию или смену контейнеров.
+
+Меняйте одну часть за раз и сначала используйте CI Lint. Для изменения ветки отредактируйте `workflow.rules`, для типа Runner — архитектуру jobs, а не только tag. [Справочник GitLab YAML](https://docs.gitlab.com/ci/yaml/).
+
+## Проверка и ошибки
+
+| Симптом | Причина / действие |
+| --- | --- |
+| Pipeline не создаётся | Ветка не develop, файл не попал в GitLab, pipeline creation запрещён настройками |
+| Pending/Stuck | Нет online Shell Runner с тегом mailgate или protected-настройки не совпадают |
+| Нет обязательной переменной | Не заполнена Variables; Protected переменная на unprotected develop; неправильный environment scope |
+| Cannot connect to Docker | Docker не запущен или пользователь Runner не имеет прав; это не ошибка YAML |
+| Registry 401 | Не включён Registry или недостаточно прав job/deploy token; dev залогинен не тем Linux-пользователем |
+| npm/NuGet 401 | NPMRC_FILE/NUGET_CONFIG_FILE не настроены или содержат неверные корпоративные credentials |
+| ENOTFOUND | Настоящий DNS/сеть/прокси недоступны с Runner или build-контейнера |
+| SSH host key verification failed | Несовпадение known_hosts; проверьте сервер у администраторов, не отключайте проверку |
+| Пустая новая БД/ключи | Неверное DEV_COMPOSE_PROJECT/DEV_DEPLOY_PATH: используются другие volumes. Исправьте настройки, не удаляйте старые volumes |
+| Health check failed | Проверьте порты, логи сервисов, Keycloak/DB/RabbitMQ. GitLab не выполняет автоматический откат |
+| Старый pipeline пропущен | Защита от перезаписи нового развёртывания; это ожидаемо |
+
+Диагностика на dev из постоянной директории проекта, под правильным пользователем:
+
+```bash
+cat .mailgate-ci/latest-successful
+cat .mailgate-ci/deployed-commit
+docker ps --filter name=mailgate
+docker logs --tail 100 mailgate-api
+docker logs --tail 100 mailgate-worker
+```
+
+Не удаляйте `.mailgate-ci/latest-attempt` ради retry старого pipeline: это обходит защиту. Для отката приложения запустите **новый pipeline на нужном состоянии исходников** (например, новым revert-коммитом в develop); совместимость с текущей схемой БД проверьте отдельно. Образы и release manifests должны сохраняться в registry/server на требуемый срок. Артефакт GitLab хранится 7 дней; для более позднего развёртывания создайте новый pipeline.
+
+## Что можно проверить без доступа к вашему GitLab
+
+Локально проверяются синтаксис Bash/YAML, единого Compose, проверки renderer, сборка кода Worker и сценарии deploy с заглушками Docker/SSH. Здесь нет доступа к вашей GitLab-инсталляции, Runner и dev-секретам, поэтому фактический CI Lint, push образов в корпоративный Registry и реальный deploy проверяются первым pipeline после настройки. Никакие команды развёртывания на ваш dev при подготовке файлов не выполнялись.
`````

### `a/config/npmrc.public b/config/npmrc.public`

`````diff
diff --git a/config/npmrc.public b/config/npmrc.public
new file mode 100644
index 0000000..214c29d
--- /dev/null
+++ b/config/npmrc.public
@@ -0,0 +1 @@
+registry=https://registry.npmjs.org/
`````

### `a/config/operations.md b/config/operations.md`

`````diff
diff --git a/config/operations.md b/config/operations.md
index 42c767f..45f3e27 100644
--- a/config/operations.md
+++ b/config/operations.md
@@ -35,10 +35,10 @@ Design-time factory читает `ConnectionStrings__DefaultConnection`; пар
 
 ## Корпоративный npm
 
-Обычный Compose использует публичный npm без обязательного токена. Для Nexus скопируйте `src/MailGate.Frontend/.npmrc.example` в `.npmrc`, замените только адрес и scope Nexus, оставив `${NPM_TOKEN}`. В игнорируемом корневом `.env` задайте `NPM_TOKEN` (одинарные кавычки сохраняют `$` буквально). Запускайте:
+Обычный Compose использует публичный npm без обязательного токена. Для Nexus скопируйте `src/MailGate.Frontend/.npmrc.example` в `.npmrc`, замените адрес и scope Nexus, оставив `${NPM_TOKEN}`. Сохраните токен в отдельном игнорируемом файле без BOM и перевода строки. В корневом `.env` задайте пути `NPMRC_FILE` и `NPM_TOKEN_FILE`, как описано в [compose.md](compose.md). Сам токен в YAML не записывается. Запускайте:
 
 ```powershell
-docker compose -f docker-compose.yml -f docker-compose.npm.yml up -d --build frontend
+docker compose up -d --build frontend
 ```
 
 В GitLab сохраните NPM_TOKEN как masked/hidden secret; protected переменная доступна только разрешённым protected pipeline. В существующем Docker build job передайте:
@@ -66,7 +66,7 @@ RABBITMQ_HOST, RABBITMQ_CONNECTION_PORT и RABBITMQ_VHOST задают подк
 ## Локальный Docker Desktop без RED SOFT registry
 
 ```sh
-docker compose -f docker-compose.yml -f docker-compose.local.yml up -d --build
+docker compose up -d --build
 ```
 
 Override использует официальные образы .NET и отдельный тестовый Keycloak на `http://keycloak.localhost:8181`, не изменяя существующий Keycloak на 8180. Тестовый realm импортируется из `config/keycloak/mailgate-local-realm.json`, пользователь `mailgate-admin` включён в `/MailGateAdmins`. Пароль тестового пользователя указан в этом файле. Override не предназначен для корпоративного развёртывания; доверенные корпоративные УЦ из RED SOFT Dockerfile в него не устанавливаются.
`````

### `a/config/smtp-account.json.example b/config/smtp-account.json.example`

`````diff
diff --git a/config/smtp-account.json.example b/config/smtp-account.json.example
new file mode 100644
index 0000000..b947ad7
--- /dev/null
+++ b/config/smtp-account.json.example
@@ -0,0 +1,4 @@
+{
+  "username": "REPLACE_WITH_MAIL_ACCOUNT",
+  "password": "REPLACE_WITH_PASSWORD"
+}
`````

### `a/config/smtp-sidecar.md b/config/smtp-sidecar.md`

`````diff
diff --git a/config/smtp-sidecar.md b/config/smtp-sidecar.md
new file mode 100644
index 0000000..1694c97
--- /dev/null
+++ b/config/smtp-sidecar.md
@@ -0,0 +1,206 @@
+# SMTP: ручной тестовый режим и учётная запись из sidecar
+
+## Схема
+
+В настройках SMTP теперь выбирается источник учётных данных:
+
+- **Вручную**: хост, порт, STARTTLS/SSL, проверка отзыва, адрес и имя отправителя. SMTP-аутентификация отключена. Подходит для MailHog/тестового relay, который разрешает анонимную отправку.
+- **Существующая почтовая учётная запись**: те же параметры соединения, а username/password приложение читает из файла. STARTTLS либо SSL обязательны. Логин, пароль и OAuth2 в форме не вводятся и новые секреты не сохраняются в БД.
+
+Пока поддерживается **одна настроенная файловая учётная запись на окружение**, а не каталог нескольких аккаунтов. Хост/порт/отправитель по-прежнему задаются в форме — файл содержит только credentials. Адрес отправителя должен быть разрешён почтовым сервером для выбранной учётной записи.
+
+MailGate не обращается к Vault. Доступ к Vault и обновление файла — ответственность существующего sidecar. Ему нужны права **чтения** нужного SMTP-секрета, а не update ключей шифрования. Пароль существует в файле и памяти приложения в открытом виде; защита обеспечивается правами доступа, изолированным volume и TLS SMTP, не Data Protection.
+
+## Контракт файла
+
+Путь по умолчанию: `/var/mailgate/smtp-credentials/account.json`.
+
+```json
+{
+  "username": "ЛОГИН_ПОЧТОВОЙ_УЧЁТНОЙ_ЗАПИСИ",
+  "password": "ПАРОЛЬ_ПОЧТОВОЙ_УЧЁТНОЙ_ЗАПИСИ"
+}
+```
+
+Это обозначения значений, а не рабочие учётные данные. Пример структуры: `config/smtp-account.json.example`. Ключи JSON — строго `username` и `password` в нижнем регистре. Значения — строки; корректно экранируйте кавычки/обратные слеши средствами JSON-сериализатора sidecar, а не строковой подстановкой.
+
+Файл должен быть непустым, размером до 64 КБ. Логин — до 4096 символов, пароль — до 16384. Sidecar обновляет **весь файл атомарной заменой**, чтобы логин и пароль читались как одна версия. Монтируйте каталог, не отдельный файл/subPath: иначе приложение может продолжить видеть прежний inode после замены.
+
+Путь задаётся администратором, одинаково у API и Worker:
+
+```text
+Smtp__CredentialsFilePath=/var/mailgate/smtp-credentials/account.json
+```
+
+Браузер не может передать другой путь и читать произвольные файлы через SMTP API. GET настроек не читает файл и не возвращает его содержимое. Для проверки учётной записи используйте «Проверить подключение».
+
+## Когда читается файл и как происходит ротация
+
+API читает его при проверке подключения. Worker — при получении effective-настроек перед отправкой каждого письма. Файловые логин/пароль не попадают в кэш SMTP-настроек. После атомарного обновления следующий вызов читает новый файл; уже выполняющаяся отправка может завершиться со старой парой.
+
+Хост/порт/режим из БД по-прежнему кэшируются в каждом процессе на `Smtp:SettingsCacheSeconds` (обычно 30 секунд). Поэтому смена режима/хоста в интерфейсе может дойти до Worker с такой задержкой; ротация содержимого файла при уже выбранном файловом режиме этой задержки не имеет. Полное время ротации включает также интервал обновления sidecar и момент фактической смены пароля на SMTP-сервере.
+
+При отсутствии файла, ошибке доступа, некорректном JSON или пустых credentials отправка завершается ошибкой. **Fallback к прежнему паролю в БД/appsettings или к анонимной отправке не выполняется.** Сообщение ошибки не включает путь, содержимое файла или исходную ошибку JSON-парсера.
+
+Проверка соединения использует тот же resolver, что и отправка. Она проверяет соединение/TLS/аутентификацию, но не отправляет письмо и не гарантирует право MAIL FROM или доставку конкретному адресату.
+
+## Kubernetes
+
+Настройте монтирование и sidecar **в Pod API и в Pod Worker**. У них могут быть отдельные `emptyDir` с одинаковыми данными из одной записи Vault; общий PVC между Pod не нужен. Локальный volume name Kubernetes произволен, его совпадение с Docker volume name не связывает хранилища.
+
+Рекомендуется отдельное имя `smtp-credentials`, а не старое `data_protection_keys`: там теперь другой формат данных. Можно сохранить существующее имя volume, если этого требует ваш chart, но mountPath приложения должен соответствовать новому пути и каталог не должен смешиваться с XML-ключами.
+
+Пример **фрагментов для вашего chart**, не готовый манифест для kubectl apply:
+
+```yaml
+# spec.template.spec.volumes
+- name: smtp-credentials
+  emptyDir:
+    medium: Memory
+
+# env контейнера MailGate.Api / MailGate.Worker
+- name: Smtp__CredentialsFilePath
+  value: /var/mailgate/smtp-credentials/account.json
+
+# volumeMounts контейнера MailGate.Api / MailGate.Worker
+- name: smtp-credentials
+  mountPath: /var/mailgate/smtp-credentials
+  readOnly: true
+
+# volumeMounts вашего уже настроенного sidecar
+- name: smtp-credentials
+  mountPath: /var/mailgate/smtp-credentials
+```
+
+Согласуйте UID/GID, fsGroup и permissions: sidecar пишет, приложение читает, прочие контейнеры файла не видят. Не устанавливайте 0777. Нужны первоначальная отрисовка до старта отправок и работающий sidecar для последующих обновлений. При Vault Agent Injector используйте его механизм общего volume и аннотации вместо ручного создания второго конфликтующего volume. По контракту итогом должен быть именно JSON выше, без внешней оболочки `data`/`metadata`.
+
+В этой доработке нет правок вашего неизвестного Helm chart и нет зависимости MailGate от Vault token. Политики Vault, Kubernetes ServiceAccount и наблюдение за sidecar настраиваются существующей инфраструктурой. [Как Vault Agent Injector предоставляет файлы приложениям](https://developer.hashicorp.com/vault/docs/deploy/kubernetes/injector), [шаблоны и обновление секретов](https://developer.hashicorp.com/vault/docs/agent-and-proxy/agent/template).
+
+Если sidecar перестал обновлять, но последний файл остался, MailGate продолжит читать его. Здесь не реализованы проверка срока lease или принудительное истечение файла; здоровье sidecar и ротацию нужно мониторить средствами инфраструктуры.
+
+## Локальный Docker — без Vault
+
+Для обычного MailHog никаких файлов и Vault не требуется:
+
+```bash
+docker compose up -d --build
+```
+
+В интерфейсе выберите «Вручную», задайте хост `mailhog`, порт `1025`, отключите STARTTLS/SSL, укажите тестового отправителя. Сохраните, проверьте подключение и отправьте письмо. MailHog UI доступен по порту из вашего Compose (по умолчанию http://localhost:8025).
+
+Чтобы проверить **файловый режим** локально, Vault поднимать не нужно. Создайте отдельный каталог на хосте, в нём `account.json` с действительной тестовой парой. Не коммитьте его. Можно скопировать `config/smtp-account.json.example` и заменить значения. В `.gitignore` и Docker build context исключены `account.json` и `secrets/`.
+
+PowerShell из корня проекта:
+
+```powershell
+$env:SMTP_CREDENTIALS_DIRECTORY = Read-Host 'Абсолютный путь каталога с account.json'
+docker compose up -d --build
+```
+
+Linux/macOS:
+
+```bash
+read -rp 'Абсолютный путь каталога с account.json: ' SMTP_CREDENTIALS_DIRECTORY
+export SMTP_CREDENTIALS_DIRECTORY
+docker compose up -d --build
+```
+
+Один каталог монтируется read-only в API и Worker. Выберите файловый режим в интерфейсе, задайте доступный SMTP с STARTTLS/SSL. Для корпоративного SMTP нужны также сеть и доверенный корпоративный УЦ внутри соответствующих образов. Обычный локальный MailHog без TLS не подходит для проверки файловой аутентификации: для него есть ручной режим. Для автономного теста файлов/TLS нужен отдельный тестовый SMTP с TLS и аутентификацией.
+
+Изменение JSON в этом каталоге имитирует ротацию sidecar. Сам Vault локально имеет смысл поднимать только для тестирования именно инфраструктурной интеграции, а не чтения файла MailGate.
+
+### Пошаговая отправка с настоящей почтовой учётной записью (Windows)
+
+В этом сценарии приложение работает в Docker Desktop, а письмо отправляется через настоящий SMTP вашей почты. Vault и sidecar не запускаются: их результат заменяет файл на компьютере. Используйте разрешённую для тестирования учётную запись и получателя.
+
+**1. Создайте каталог для файла вне репозитория.** Например, `C:\MailGateSecrets`. В PowerShell из корня MailGate:
+
+```powershell
+$smtpDirectory = 'C:\MailGateSecrets'
+New-Item -ItemType Directory -Force -Path $smtpDirectory | Out-Null
+if (-not (Test-Path (Join-Path $smtpDirectory 'account.json'))) {
+    Copy-Item .\config\smtp-account.json.example (Join-Path $smtpDirectory 'account.json')
+}
+notepad (Join-Path $smtpDirectory 'account.json')
+```
+
+В редакторе замените обозначения на **настоящие** SMTP-логин и пароль, сохраните файл в UTF-8 без BOM:
+
+```json
+{
+  "username": "ТВОЙ_НАСТОЯЩИЙ_SMTP_ЛОГИН",
+  "password": "ТВОЙ_НАСТОЯЩИЙ_SMTP_ПАРОЛЬ"
+}
+```
+
+Не копируйте эти обозначения как значения учётной записи. Логин может отличаться от адреса отправителя — используйте формат, выданный почтовыми администраторами. Пароль с кавычками или обратными слешами должен быть корректно экранирован в JSON. Файл содержит открытый пароль: ограничьте доступ к каталогу своим пользователем, не добавляйте его в Git и не прикладывайте к логам/скриншотам.
+
+**2. Запустите контейнеры с каталогом учётной записи:**
+
+```powershell
+$env:SMTP_CREDENTIALS_DIRECTORY = $smtpDirectory
+
+docker compose up -d --build
+
+if ($LASTEXITCODE -ne 0) { throw 'Не удалось запустить MailGate' }
+```
+
+Переменная указывает на **каталог**, не на `account.json`. API и Worker получают один каталог read-only и читают `/var/mailgate/smtp-credentials/account.json`. Переменная действует в текущем PowerShell; после открытия нового терминала задайте её снова. Для последующих команд изменения конфигурации контейнеров используйте один docker-compose.yml.
+
+**3. Проверьте доступ к корпоративной почте.** Компьютер и контейнеры должны видеть настоящий SMTP-хост, в том числе через корпоративную сеть/VPN, если это требуется. Для TLS внутри **API и Worker** должны быть доверены корпоративные УЦ. Положите публичные PEM `.crt` в корневой `crt/` и пересоберите `docker compose build api worker`: локальный `src/Dockerfile.local` теперь устанавливает их в build/final. Подробнее в [operations.md](operations.md). Проверка соединения выполняется API, отправка — Worker, поэтому доверие только в Windows не заменяет настройку обоих контейнеров.
+
+**4. Откройте настройки SMTP в локальном интерфейсе** (обычно http://localhost:8091) и выберите «Существующая почтовая учётная запись». Заполните:
+
+| Поле | Что указать |
+| --- | --- |
+| Хост | Настоящее DNS-имя SMTP-сервера вашей почты, без `https://` |
+| Порт | Порт, выданный почтовыми администраторами |
+| STARTTLS / SSL | Режим, соответствующий этому серверу и порту; не оба одновременно |
+| Адрес отправителя | Почтовый адрес, с которого данной учётке разрешено отправлять |
+| Имя отправителя | Желаемое отображаемое имя |
+
+Для ранее обсуждавшегося подключения к корпоративному SMTP на порту 25 используйте STARTTLS включённым, SSL выключенным, **если сервер настроен именно на STARTTLS**. Для implicit TLS на 465 обычно используется SSL; окончательные параметры берите у администраторов. В файловом режиме полностью отключить TLS нельзя.
+
+**5. Нажмите «Проверить подключение».** При успехе сохраните настройки. Подождите время кэша Worker — по умолчанию до 30 секунд. Перейдите на `/notifications/new`, укажите разрешённый тестовый адрес, тему и текст, отправьте письмо. Проверьте журнал MailGate и фактическое получение в почте, включая папку нежелательной почты.
+
+Успешная проверка подтверждает соединение, TLS и аутентификацию. Она не проверяет право отправлять с выбранного адреса и не гарантирует конечную доставку. Реальное письмо проверяет эти следующие этапы.
+
+**6. При ошибке посмотрите свежие логи:**
+
+```powershell
+docker compose logs --since 5m api worker
+```
+
+| Ошибка | Что проверить |
+| --- | --- |
+| Не удалось прочитать SMTP-учётную запись | Каталог в переменной, точное имя `account.json` (не `.json.txt`), JSON, кодировка без BOM, права чтения и монтирование в оба сервиса |
+| Timeout / Connection refused / ошибка DNS | Настоящий SMTP-хост/порт и доступ именно из Docker, сеть/VPN/firewall |
+| Unable to get local issuer certificate | Корпоративный корневой/промежуточный УЦ в доверенном хранилище обоих контейнеров |
+| Unable to get certificate CRL | Доступ контейнеров к CRL либо согласованная политика проверки отзыва; пароль в JSON это не исправит |
+| 535 Authentication unsuccessful | Настоящие логин/пароль, разрешённый для учётки способ SMTP AUTH и отсутствие блокировки на почтовом сервере |
+| Проверка успешна, отправка запрещена | Разрешения MAIL FROM / Send As / relay и выбранный адрес отправителя |
+
+Не выводите содержимое `account.json` командой в общий лог. Для проверки смены пароля замените файл целиком новым корректным JSON: следующий вызов прочитает новую пару без пересборки образов. Сначала пароль должен быть согласован с почтовым сервером — изменение одного локального файла не меняет пароль в самой почте.
+
+**7. Чтобы вернуться к MailHog**, выберите «Вручную», задайте `mailhog:1025`, отключите STARTTLS/SSL и сохраните. После обновления кэша Worker отправка будет идти в MailHog. Если локальная почтовая учётка больше не нужна, удалите свой файл с паролем согласно правилам вашего контура. Каталог остаётся смонтированным, но в ручном режиме приложение его не читает. Вручную удалять PostgreSQL или key ring для переключения режимов не нужно.
+
+## БД, миграция и прежние пароли
+
+Добавлена миграция `20261123090000_AddSmtpCredentialSource`. Она добавляет колонку `CredentialSource` в `smtp_configurations`; существующие записи получают `Legacy` (число 2), чтобы обновление не сломало прежнюю отправку и не удалило секреты. Новые режимы — `Manual` (0) и `Sidecar` (1).
+
+Старые записи продолжают работать прежним способом, но поля ввода credentials скрыты. В UI показывается предупреждение с предложением выбрать новый режим. **При сохранении Manual/Sidecar прежние username, зашифрованный пароль и поля OAuth2 этой записи очищаются.** Сначала проверьте новый режим. Он не расшифровывает старый секрет и может быть выбран даже при утраченном старом ключе.
+
+Для двух новых режимов Data Protection не используется. `DataProtection:KeysPath` больше не обязателен: при незаданном пути автоматическое создание ключей отключено. В существующем Docker Compose старый volume оставлен ради совместимости — он не удаляется и не очищается автоматически. На новом Kubernetes-развёртывании с Manual/Sidecar путь и volume ключей можно не задавать. Для Legacy с зашифрованными данными прежние ключи всё ещё нужны.
+
+Новые runtime-настройки можно задать без БД: `Smtp__CredentialSource=Manual` либо `Sidecar`. Значение по умолчанию в appsettings-модели оставлено `Legacy` для обратной совместимости; выбор в интерфейсе сохраняется в БД и переопределяет его. Если хотите полностью запретить старую конфигурацию через appsettings, явно установите `Manual`. API больше не принимает сохранение/проверку с присланными username/password/OAuth2 и не принимает запись Legacy из новой формы.
+
+Применяйте миграцию до обновления API. Используйте ваш обычный migration Job/`worker --migrate-only` или `dotnet ef database update` с действительным подключением. Не генерируйте вторую миграцию с той же колонкой, если переносите готовую. Миграция не очищает секреты; это делает только явное сохранение нового режима.
+
+## Проверки
+
+```bash
+dotnet run --project tests/MailGate.SmtpCredentialChecks
+npm run build --prefix src/MailGate.Frontend
+```
+
+Проверяются ручной режим без файла, совместимость Legacy, чтение и атомарная ротация, некорректные/пустые/слишком большие файлы, отсутствие fallback, безопасный текст ошибок, требование TLS и отмена операции. Дополнительно на стенде проверьте сохранение обоих режимов, миграцию прежней записи, тестирование/отправку и отсутствие credentials в JSON настроек, БД и логах.
`````

### `a/docker-compose.local.yml b/docker-compose.local.yml`

`````diff
diff --git a/docker-compose.local.yml b/docker-compose.local.yml
deleted file mode 100644
index 4afdc0b..0000000
--- a/docker-compose.local.yml
+++ /dev/null
@@ -1,38 +0,0 @@
-# Official .NET images for local Docker Desktop, without changing corporate builds.
-services:
-  api:
-    depends_on:
-      keycloak-local:
-        condition: service_started
-    environment:
-      Authentication__Keycloak__AdminGroup: /MailGateAdmins
-      Authentication__Keycloak__Authority: http://keycloak.localhost:8181/realms/mailgate
-    build:
-      context: .
-      dockerfile: src/Dockerfile.local
-      args:
-        PROJECT: Api
-  frontend:
-    environment:
-      KEYCLOAK_ADMIN_GROUP: /MailGateAdmins
-      KEYCLOAK_AUTHORITY: http://keycloak.localhost:8181/realms/mailgate
-  worker:
-    build:
-      context: .
-      dockerfile: src/Dockerfile.local
-      args:
-        PROJECT: Worker
-  keycloak-local:
-    image: quay.io/keycloak/keycloak:26.2
-    container_name: mailgate-keycloak-local
-    command: ["start-dev", "--import-realm"]
-    ports:
-      - "8181:8080"
-    environment:
-      KC_HOSTNAME: http://keycloak.localhost:8181
-      KC_HOSTNAME_STRICT: "false"
-      KC_HTTP_ENABLED: "true"
-      KC_BOOTSTRAP_ADMIN_USERNAME: admin
-      KC_BOOTSTRAP_ADMIN_PASSWORD: local-development-only
-    volumes:
-      - ./config/keycloak/mailgate-local-realm.json:/opt/keycloak/data/import/mailgate-realm.json:ro
`````

### `a/docker-compose.npm.yml b/docker-compose.npm.yml`

`````diff
diff --git a/docker-compose.npm.yml b/docker-compose.npm.yml
deleted file mode 100644
index d076e9e..0000000
--- a/docker-compose.npm.yml
+++ /dev/null
@@ -1,12 +0,0 @@
-services:
-  frontend:
-    build:
-      secrets:
-        - npmrc
-        - npm_token
-
-secrets:
-  npmrc:
-    file: ./src/MailGate.Frontend/.npmrc
-  npm_token:
-    environment: NPM_TOKEN
`````

### `a/docker-compose.yml b/docker-compose.yml`

`````diff
diff --git a/docker-compose.yml b/docker-compose.yml
index 64812cb..ece4f5b 100644
--- a/docker-compose.yml
+++ b/docker-compose.yml
@@ -41,36 +41,39 @@ services:
       - "${MAILHOG_SMTP_PORT:-1025}:1025"
       - "${MAILHOG_UI_PORT:-8025}:8025"
 
-  keycloak:
+  keycloak-local:
     image: quay.io/keycloak/keycloak:26.2
-    container_name: mailgate-keycloak
-    command:
-      - start-dev
-      - --import-realm
+    container_name: mailgate-keycloak-local
+    command: ["start-dev", "--import-realm"]
     ports:
-      - "${KEYCLOAK_PORT:-8180}:8080"
+      - "${KEYCLOAK_PORT:-8181}:8080"
     environment:
       KC_BOOTSTRAP_ADMIN_USERNAME: ${KEYCLOAK_ADMIN_USER:-admin}
-      KC_BOOTSTRAP_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD:-admin}
-      # Внешний URL Keycloak (issuer токенов). За nginx укажите https://keycloak.<домен>
-      KC_HOSTNAME: ${KEYCLOAK_HOSTNAME:-http://keycloak.localhost:8180}
+      KC_BOOTSTRAP_ADMIN_PASSWORD: ${KEYCLOAK_ADMIN_PASSWORD:-local-development-only}
+      KC_HOSTNAME: ${KEYCLOAK_HOSTNAME:-http://keycloak.localhost:8181}
       KC_HOSTNAME_STRICT: "false"
       KC_HTTP_ENABLED: "true"
-      # Доверять заголовкам X-Forwarded-* от реверс-прокси
-      KC_PROXY_HEADERS: xforwarded
     volumes:
-      - ./config/keycloak/mailgate-realm.json:/opt/keycloak/data/import/mailgate-realm.json:ro
+      - keycloak_data:/opt/keycloak/data
+      - ./config/keycloak/mailgate-local-realm.json:/opt/keycloak/data/import/mailgate-realm.json:ro
 
   api:
+    image: ${MAILGATE_API_IMAGE:-mailgate-api:local}
     build:
       context: .
-      dockerfile: src/MailGate.Api/Dockerfile
+      dockerfile: ${API_DOCKERFILE:-src/Dockerfile.local}
+      args:
+        PROJECT: Api
+      secrets:
+        - nuget_config
     container_name: mailgate-api
     ports:
       - "${API_HTTP_PORT:-8080}:8080"
       - "${API_HTTPS_PORT:-8443}:8443"
     environment:
       DataProtection__KeysPath: /var/mailgate/keys
+      Smtp__CredentialsFilePath: /var/mailgate/smtp-credentials/account.json
+      Smtp__CredentialSource: ${SMTP_CREDENTIAL_SOURCE:-Legacy}
       Smtp__SettingsCacheSeconds: ${SMTP_SETTINGS_CACHE_SECONDS:-30}
       ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-Development}
       ASPNETCORE_HTTP_PORTS: "8080"
@@ -85,11 +88,11 @@ services:
       MassTransit__VirtualHost: ${RABBITMQ_VHOST:-/}
       MassTransit__Username: ${RABBITMQ_USER:-mailgate}
       MassTransit__Password: ${RABBITMQ_PASSWORD:-mailgate}
-      Authentication__Keycloak__Authority: ${KEYCLOAK_AUTHORITY:-http://keycloak.localhost:8180/realms/mailgate}
+      Authentication__Keycloak__Authority: ${KEYCLOAK_AUTHORITY:-http://keycloak.localhost:8181/realms/mailgate}
       Authentication__Keycloak__Audience: ${KEYCLOAK_AUDIENCE:-mailgate-api}
       Authentication__Keycloak__ClientId: ${KEYCLOAK_CLIENT_ID:-mailgate-admin}
       Authentication__Keycloak__SwaggerClientId: ${KEYCLOAK_CLIENT_ID:-mailgate-admin}
-      Authentication__Keycloak__AdminGroup: ${KEYCLOAK_ADMIN_GROUP:-}
+      Authentication__Keycloak__AdminGroup: ${KEYCLOAK_ADMIN_GROUP-/MailGateAdmins}
       Authentication__Keycloak__RequireHttpsMetadata: ${KEYCLOAK_REQUIRE_HTTPS_METADATA:-false}
       Cors__AllowedOrigins__0: ${CORS_ORIGIN_0:-http://localhost:8091}
       Cors__AllowedOrigins__1: ${CORS_ORIGIN_1:-http://localhost:5173}
@@ -104,41 +107,60 @@ services:
     volumes:
       - ~/.aspnet/https:/https:ro
       - data_protection_keys:/var/mailgate/keys
+      - type: bind
+        source: ${SMTP_CREDENTIALS_DIRECTORY:-./secrets/smtp}
+        target: /var/mailgate/smtp-credentials
+        read_only: true
     depends_on:
       rabbitmq:
         condition: service_healthy
       postgres:
         condition: service_healthy
-      keycloak:
+      keycloak-local:
         condition: service_started
       mailhog:
         condition: service_started
 
   frontend:
+    image: ${MAILGATE_FRONTEND_IMAGE:-mailgate-frontend:local}
     build:
       context: src/MailGate.Frontend
       dockerfile: Dockerfile
+      secrets:
+        - npmrc
+        - npm_token
     container_name: mailgate-frontend
     ports:
       - "${FRONTEND_PORT:-8091}:80"
     environment:
       API_BASE_URL: ${API_BASE_URL:-http://localhost:8080}
-      KEYCLOAK_AUTHORITY: ${KEYCLOAK_AUTHORITY:-http://keycloak.localhost:8180/realms/mailgate}
+      KEYCLOAK_AUTHORITY: ${KEYCLOAK_AUTHORITY:-http://keycloak.localhost:8181/realms/mailgate}
       KEYCLOAK_CLIENT_ID: ${KEYCLOAK_CLIENT_ID:-mailgate-admin}
-      KEYCLOAK_ADMIN_GROUP: ${KEYCLOAK_ADMIN_GROUP:-}
+      KEYCLOAK_ADMIN_GROUP: ${KEYCLOAK_ADMIN_GROUP-/MailGateAdmins}
     depends_on:
       api:
         condition: service_started
 
   worker:
+    image: ${MAILGATE_WORKER_IMAGE:-mailgate-worker:local}
     build:
       context: .
-      dockerfile: src/MailGate.Worker/Dockerfile
+      dockerfile: ${WORKER_DOCKERFILE:-src/Dockerfile.local}
+      args:
+        PROJECT: Worker
+      secrets:
+        - nuget_config
     container_name: mailgate-worker
     volumes:
       - data_protection_keys:/var/mailgate/keys
+      - type: bind
+        source: ${SMTP_CREDENTIALS_DIRECTORY:-./secrets/smtp}
+        target: /var/mailgate/smtp-credentials
+        read_only: true
     environment:
       DataProtection__KeysPath: /var/mailgate/keys
+      Smtp__CredentialsFilePath: /var/mailgate/smtp-credentials/account.json
+      Smtp__CredentialSource: ${SMTP_CREDENTIAL_SOURCE:-Legacy}
       Smtp__SettingsCacheSeconds: ${SMTP_SETTINGS_CACHE_SECONDS:-30}
       DOTNET_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-Development}
       ConnectionStrings__DefaultConnection: "Host=postgres;Database=${POSTGRES_DB:-mailgate};Username=${POSTGRES_USER:-mailgate};Password=${POSTGRES_PASSWORD:-mailgate}"
@@ -165,3 +187,14 @@ volumes:
   rabbitmq_data:
   postgres_data:
   data_protection_keys:
+  keycloak_data:
+
+# Empty/public defaults let a clean checkout build without corporate credentials.
+# Set FILE paths in .env for private registries; never COPY tokens into images.
+secrets:
+  nuget_config:
+    file: ${NUGET_CONFIG_FILE:-./src/NuGet.Config}
+  npmrc:
+    file: ${NPMRC_FILE:-./config/npmrc.public}
+  npm_token:
+    file: ${NPM_TOKEN_FILE:-./config/build-secret.empty}
`````

### `a/scripts/ci/build-images.sh b/scripts/ci/build-images.sh`

`````diff
diff --git a/scripts/ci/build-images.sh b/scripts/ci/build-images.sh
new file mode 100644
index 0000000..ec23e95
--- /dev/null
+++ b/scripts/ci/build-images.sh
@@ -0,0 +1,43 @@
+#!/usr/bin/env bash
+set -euo pipefail
+: "${CI_REGISTRY:?Enable GitLab Container Registry}"
+: "${CI_REGISTRY_IMAGE:?Missing GitLab image namespace}"
+: "${CI_REGISTRY_USER:?Missing GitLab registry user}"
+: "${CI_REGISTRY_PASSWORD:?Missing GitLab registry password}"
+: "${CI_COMMIT_SHA:?Missing commit SHA}"
+: "${CI_PIPELINE_ID:?Missing pipeline ID}"
+
+export DOCKER_CONFIG
+DOCKER_CONFIG=$(mktemp -d)
+trap 'cleanup_status=$?; rm -rf -- "$DOCKER_CONFIG" || true; exit "$cleanup_status"' EXIT
+printf '%s' "$CI_REGISTRY_PASSWORD" | docker login "$CI_REGISTRY" --username "$CI_REGISTRY_USER" --password-stdin
+
+tag="${CI_COMMIT_SHA}-pipeline${CI_PIPELINE_ID}"
+backend_args=(--label "org.opencontainers.image.revision=$CI_COMMIT_SHA")
+if [[ -n ${NUGET_CONFIG_FILE:-} ]]; then
+  test -f "$NUGET_CONFIG_FILE"
+  backend_args+=(--secret "id=nuget_config,src=$NUGET_CONFIG_FILE")
+fi
+npm_args=(--label "org.opencontainers.image.revision=$CI_COMMIT_SHA")
+if [[ -n ${NPMRC_FILE:-} ]]; then
+  test -f "$NPMRC_FILE"
+  npm_args+=(--secret "id=npmrc,src=$NPMRC_FILE")
+fi
+if [[ -n ${NPM_TOKEN:-} ]]; then
+  npm_args+=(--secret id=npm_token,env=NPM_TOKEN)
+fi
+
+docker build "${backend_args[@]}" \
+  --file src/MailGate.Api/Dockerfile --tag "$CI_REGISTRY_IMAGE/api:$tag" .
+docker build "${backend_args[@]}" \
+  --file src/MailGate.Worker/Dockerfile --tag "$CI_REGISTRY_IMAGE/worker:$tag" .
+# npm ci and TypeScript/Vite compilation happen inside this Dockerfile.
+docker build "${npm_args[@]}" \
+  --file src/MailGate.Frontend/Dockerfile --tag "$CI_REGISTRY_IMAGE/frontend:$tag" src/MailGate.Frontend
+
+# Publish only after all three builds succeed. Deployment is a separate job.
+for component in api worker frontend; do
+  docker push "$CI_REGISTRY_IMAGE/$component:$tag"
+done
+printf 'MAILGATE_API_IMAGE=%s/api:%s\nMAILGATE_WORKER_IMAGE=%s/worker:%s\nMAILGATE_FRONTEND_IMAGE=%s/frontend:%s\n' \
+  "$CI_REGISTRY_IMAGE" "$tag" "$CI_REGISTRY_IMAGE" "$tag" "$CI_REGISTRY_IMAGE" "$tag" > images.env
`````

### `a/scripts/ci/check-smtp-credentials.sh b/scripts/ci/check-smtp-credentials.sh`

`````diff
diff --git a/scripts/ci/check-smtp-credentials.sh b/scripts/ci/check-smtp-credentials.sh
new file mode 100644
index 0000000..ff81faf
--- /dev/null
+++ b/scripts/ci/check-smtp-credentials.sh
@@ -0,0 +1,14 @@
+#!/usr/bin/env bash
+set -euo pipefail
+: "${CI_PROJECT_DIR:?Run from GitLab CI}"
+: "${DOTNET_SDK_IMAGE:?Set DOTNET_SDK_IMAGE}"
+docker run --rm --env DOTNET_CLI_TELEMETRY_OPTOUT=1 \
+  --volume "$CI_PROJECT_DIR:/source:ro" "$DOTNET_SDK_IMAGE" sh -ec '
+    mkdir -p /tmp/mailgate-checks
+    tar -C /source --exclude=bin --exclude=obj -cf - \
+      tests/MailGate.SmtpCredentialChecks src/MailGate.Channels.Smtp/Options \
+      src/MailGate.Channels.Smtp/Services src/MailGate.Core/Enums \
+      | tar -C /tmp/mailgate-checks -xf -
+    cd /tmp/mailgate-checks
+    dotnet run --project tests/MailGate.SmtpCredentialChecks -p:NuGetAudit=false
+  '
`````

### `a/scripts/ci/check-templates.sh b/scripts/ci/check-templates.sh`

`````diff
diff --git a/scripts/ci/check-templates.sh b/scripts/ci/check-templates.sh
new file mode 100644
index 0000000..aacc461
--- /dev/null
+++ b/scripts/ci/check-templates.sh
@@ -0,0 +1,21 @@
+#!/usr/bin/env bash
+set -euo pipefail
+: "${CI_PROJECT_DIR:?Run from GitLab CI}"
+: "${DOTNET_SDK_IMAGE:?Set DOTNET_SDK_IMAGE}"
+
+# A shell executor is intentional: the Docker daemon sees CI_PROJECT_DIR.
+docker run --rm \
+  --env DOTNET_CLI_HOME=/tmp/dotnet-home \
+  --env DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
+  --env DOTNET_CLI_TELEMETRY_OPTOUT=1 \
+  --volume "$CI_PROJECT_DIR:/source:ro" \
+  "$DOTNET_SDK_IMAGE" \
+  sh -ec '
+    mkdir -p /tmp/mailgate-checks
+    tar -C /source --exclude=bin --exclude=obj -cf - \
+      tests/MailGate.TemplateChecks src/MailGate.Api/Templates \
+      src/MailGate.Contracts/Templates src/MailGate.Core/Enums src/MailGate.Core/Models \
+      | tar -C /tmp/mailgate-checks -xf -
+    cd /tmp/mailgate-checks
+    dotnet run --project tests/MailGate.TemplateChecks -p:NuGetAudit=false
+  '
`````

### `a/scripts/ci/deploy-dev-server.sh b/scripts/ci/deploy-dev-server.sh`

`````diff
diff --git a/scripts/ci/deploy-dev-server.sh b/scripts/ci/deploy-dev-server.sh
new file mode 100644
index 0000000..a14cf35
--- /dev/null
+++ b/scripts/ci/deploy-dev-server.sh
@@ -0,0 +1,66 @@
+#!/usr/bin/env bash
+set -euo pipefail
+umask 077
+root=${1:?}; pipeline=${2:?}; project=${3:?}; commit=${4:?}; gitlab_project=${5:?}
+api_health=${6:?}; frontend_health=${7:?}
+[[ $pipeline =~ ^[0-9]+$ && $gitlab_project =~ ^[0-9]+$ ]]
+cd "$root"
+test -f docker-compose.yml
+test -f .env
+command -v flock >/dev/null
+command -v curl >/dev/null
+docker compose version
+
+state="$root/.mailgate-ci"
+release="$state/releases/$pipeline"
+mkdir -p "$state"
+exec 9>"$state/deploy.lock"
+flock -x 9
+
+if [[ -f $state/gitlab-project && $(cat "$state/gitlab-project") != "$gitlab_project" ]]; then
+  echo 'This deployment directory belongs to another GitLab project.' >&2
+  exit 1
+fi
+printf '%s\n' "$gitlab_project" > "$state/gitlab-project"
+previous=0
+if [[ -f $state/latest-attempt ]]; then previous=$(cat "$state/latest-attempt"); fi
+[[ $previous =~ ^[0-9]+$ ]]
+if (( pipeline < previous )); then
+  echo "Skip outdated pipeline $pipeline; pipeline $previous has already started deployment."
+  exit 0
+fi
+
+compose=(docker compose --project-name "$project" --project-directory "$root" \
+  --env-file "$root/.env" --env-file "$release/images.env" \
+  -f "$root/docker-compose.yml")
+# Validate without printing resolved environment values/passwords.
+"${compose[@]}" config --quiet
+"${compose[@]}" pull api worker frontend
+printf '%s\n' "$pipeline" > "$state/latest-attempt"
+
+# Applies migrations and exits before starting MassTransit or consuming mail.
+# Fail before replacing the running app if the migration fails.
+"${compose[@]}" run --rm --no-deps worker --migrate-only
+"${compose[@]}" up -d --no-deps --no-build api worker frontend
+
+wait_http() {
+  local url=$1
+  for (( attempt=1; attempt<=30; attempt++ )); do
+    if curl --fail --silent --show-error --connect-timeout 3 --max-time 8 --output /dev/null "$url" 2>/dev/null; then
+      return 0
+    fi
+    sleep 3
+  done
+  echo "Health check failed: $url" >&2
+  return 1
+}
+wait_http "$api_health"
+wait_http "$frontend_health"
+for component in api worker frontend; do
+  container=$("${compose[@]}" ps -q "$component")
+  test -n "$container"
+  test "$(docker inspect --format '{{.State.Running}}' "$container")" = true
+done
+printf '%s\n' "$pipeline" > "$state/latest-successful"
+printf '%s\n' "$commit" > "$state/deployed-commit"
+echo "Deployed pipeline $pipeline, commit $commit."
`````

### `a/scripts/ci/deploy-dev.sh b/scripts/ci/deploy-dev.sh`

`````diff
diff --git a/scripts/ci/deploy-dev.sh b/scripts/ci/deploy-dev.sh
new file mode 100644
index 0000000..96b3735
--- /dev/null
+++ b/scripts/ci/deploy-dev.sh
@@ -0,0 +1,43 @@
+#!/usr/bin/env bash
+set -euo pipefail
+: "${DEV_SSH_HOST:?Set real dev server hostname or IPv4 address}"
+: "${DEV_SSH_USER:?Set dev SSH user}"
+: "${DEV_DEPLOY_PATH:?Set existing absolute server project directory}"
+: "${DEV_COMPOSE_PROJECT:?Set existing Compose project name}"
+: "${SSH_PRIVATE_KEY:?Set a GitLab File variable}"
+: "${SSH_KNOWN_HOSTS:?Set a GitLab File variable}"
+: "${CI_PIPELINE_ID:?Missing pipeline ID}"
+: "${CI_PROJECT_ID:?Missing project ID}"
+: "${CI_COMMIT_SHA:?Missing commit SHA}"
+: "${DEV_API_HEALTH_URL:?Set server-side API health URL}"
+: "${DEV_FRONTEND_HEALTH_URL:?Set server-side frontend URL}"
+port=${DEV_SSH_PORT:-22}
+
+# Values are also used in remote shell commands. Accept simple, explicit paths.
+[[ $DEV_SSH_HOST =~ ^[a-zA-Z0-9][a-zA-Z0-9.-]*$ ]]
+[[ $DEV_SSH_USER =~ ^[a-zA-Z_][a-zA-Z0-9_-]*$ ]]
+[[ $port =~ ^[0-9]+$ ]]
+[[ $DEV_DEPLOY_PATH =~ ^/[a-zA-Z0-9_./-]+$ && $DEV_DEPLOY_PATH != / && $DEV_DEPLOY_PATH != *..* ]]
+[[ $DEV_COMPOSE_PROJECT =~ ^[a-z0-9][a-z0-9_-]*$ ]]
+[[ $CI_PIPELINE_ID =~ ^[0-9]+$ && $CI_PROJECT_ID =~ ^[0-9]+$ && $CI_COMMIT_SHA =~ ^[0-9a-f]{40}$ ]]
+test -f images.env
+test -f "$SSH_PRIVATE_KEY"
+test -f "$SSH_KNOWN_HOSTS"
+
+ssh_dir=$(mktemp -d)
+trap 'cleanup_status=$?; rm -rf -- "$ssh_dir" || true; exit "$cleanup_status"' EXIT
+cp -- "$SSH_PRIVATE_KEY" "$ssh_dir/key"
+cp -- "$SSH_KNOWN_HOSTS" "$ssh_dir/known_hosts"
+chmod 600 "$ssh_dir/key" "$ssh_dir/known_hosts"
+ssh_options=(-i "$ssh_dir/key" -o BatchMode=yes -o IdentitiesOnly=yes \
+  -o StrictHostKeyChecking=yes -o "UserKnownHostsFile=$ssh_dir/known_hosts" -o ConnectTimeout=15)
+destination="$DEV_SSH_USER@$DEV_SSH_HOST"
+release="$DEV_DEPLOY_PATH/.mailgate-ci/releases/$CI_PIPELINE_ID"
+ssh "${ssh_options[@]}" -p "$port" "$destination" "mkdir -p -- '$release'"
+scp "${ssh_options[@]}" -P "$port" images.env "$destination:$release/images.env"
+
+# Quote URLs as shell arguments rather than interpolating them as shell code.
+printf -v remote_command 'bash -s -- %q %q %q %q %q %q %q' \
+  "$DEV_DEPLOY_PATH" "$CI_PIPELINE_ID" "$DEV_COMPOSE_PROJECT" "$CI_COMMIT_SHA" "$CI_PROJECT_ID" \
+  "$DEV_API_HEALTH_URL" "$DEV_FRONTEND_HEALTH_URL"
+ssh "${ssh_options[@]}" -p "$port" "$destination" "$remote_command" < scripts/ci/deploy-dev-server.sh
`````

### `a/src/Dockerfile.local b/src/Dockerfile.local`

`````diff
diff --git a/src/Dockerfile.local b/src/Dockerfile.local
index e77cc40..d4a1ad0 100644
--- a/src/Dockerfile.local
+++ b/src/Dockerfile.local
@@ -1,14 +1,24 @@
 # Local development build; corporate deployment keeps its existing Dockerfiles.
 FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
 WORKDIR /src
+COPY crt/ /usr/local/share/ca-certificates/mailgate/
+RUN update-ca-certificates
 COPY src/ ./
 ARG PROJECT
-RUN dotnet publish "MailGate.${PROJECT}/MailGate.${PROJECT}.csproj" -c Release -o /app/publish
+RUN --mount=type=secret,id=nuget_config \
+    if [ -f /run/secrets/nuget_config ]; then \
+      dotnet restore "MailGate.${PROJECT}/MailGate.${PROJECT}.csproj" --configfile /run/secrets/nuget_config; \
+    else \
+      dotnet restore "MailGate.${PROJECT}/MailGate.${PROJECT}.csproj"; \
+    fi
+RUN dotnet publish "MailGate.${PROJECT}/MailGate.${PROJECT}.csproj" -c Release -o /app/publish --no-restore
 
 FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
 WORKDIR /app
+COPY crt/ /usr/local/share/ca-certificates/mailgate/
+RUN update-ca-certificates
 ENV ASPNETCORE_HTTP_PORTS=8080
 COPY --from=build /app/publish ./
 ARG PROJECT
 ENV MAILGATE_PROJECT=${PROJECT}
-ENTRYPOINT ["/bin/sh", "-c", "exec dotnet MailGate.${MAILGATE_PROJECT}.dll"]
+ENTRYPOINT ["/bin/sh", "-c", "exec dotnet \"MailGate.${MAILGATE_PROJECT}.dll\" \"$@\"", "--"]
`````

### `a/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs`

`````diff
diff --git a/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs
index f3b7b5a..eab8ef3 100644
--- a/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs
+++ b/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs
@@ -1,4 +1,5 @@
 using System.Diagnostics;
+using MailGate.Core.Enums;
 using FluentValidation;
 using MailGate.Channels.Smtp.Options;
 using MailGate.Channels.Smtp.Services;
@@ -48,7 +49,7 @@ public static class SmtpAdminEndpoints
             return TypedResults.Ok(new SmtpConfigurationDto(
                 s.Host, s.Port, s.UseSsl, s.UseTls, s.CheckCertificateRevocation, s.FromAddress, s.FromName,
                 s.Username, !string.IsNullOrEmpty(s.Password),
-                oauth, SmtpConfigurationSource.AppSettings, null));
+                oauth, SmtpConfigurationSource.AppSettings, null, s.CredentialSource));
         }
 
         return TypedResults.Ok(ToDto(dbConfig, SmtpConfigurationSource.Database));
@@ -77,6 +78,7 @@ public static class SmtpAdminEndpoints
 
         var entity = new SmtpConfiguration
         {
+            CredentialSource = dto.CredentialSource,
             Host = dto.Host.Trim(),
             Port = dto.Port,
             UseSsl = dto.UseSsl,
@@ -109,9 +111,8 @@ public static class SmtpAdminEndpoints
 
     private static async Task<Ok<SmtpTestResultDto>> TestConnectionAsync(
         UpdateSmtpConfigurationDto dto,
-        SmtpConfigurationStore store,
         IValidator<UpdateSmtpConfigurationDto> validator,
-        IOptionsMonitor<SmtpOptions> appSettings,
+        SmtpCredentialResolver credentials,
         CancellationToken cancellationToken)
     {
         var validation = await validator.ValidateAsync(dto, cancellationToken);
@@ -126,21 +127,8 @@ public static class SmtpAdminEndpoints
         var sw = Stopwatch.StartNew();
         try
         {
-            var effective = BuildEffectiveOptions(dto, appSettings.CurrentValue);
-            var needsSavedSecret = dto.OAuth2 is null
-                ? string.IsNullOrEmpty(dto.Password)
-                : string.IsNullOrEmpty(dto.OAuth2.ClientSecret);
-            if (needsSavedSecret)
-            {
-                var saved = await store.GetMetadataAsync(cancellationToken);
-                if (saved is not null)
-                {
-                    if (dto.OAuth2 is null)
-                        effective.Password = store.DecryptPassword(saved) ?? effective.Password;
-                    else if (effective.OAuth2 is not null)
-                        effective.OAuth2.ClientSecret = store.DecryptOAuth2ClientSecret(saved) ?? effective.OAuth2.ClientSecret;
-                }
-            }
+            var effective = BuildEffectiveOptions(dto);
+            await credentials.ApplyAsync(effective, cancellationToken);
             if (effective.OAuth2 is null && !string.IsNullOrWhiteSpace(effective.Username) && string.IsNullOrEmpty(effective.Password))
                 throw new InvalidOperationException("Указан SMTP-логин, но пароль отсутствует.");
             using var client = new SmtpClient();
@@ -153,17 +141,8 @@ public static class SmtpAdminEndpoints
 
             await client.ConnectAsync(effective.Host, effective.Port, secureSocketOptions, cancellationToken);
 
-            if (effective.OAuth2 is not null)
-            {
-                var token = await new OAuth2TokenProvider(effective).GetAccessTokenAsync(cancellationToken);
-                await client.AuthenticateAsync(
-                    new SaslMechanismOAuth2(effective.OAuth2.SmtpUserPrincipalName, token),
-                    cancellationToken);
-            }
-            else if (!string.IsNullOrEmpty(effective.Username) && !string.IsNullOrEmpty(effective.Password))
-            {
+            if (!string.IsNullOrEmpty(effective.Username) && !string.IsNullOrEmpty(effective.Password))
                 await client.AuthenticateAsync(effective.Username, effective.Password, cancellationToken);
-            }
 
             await client.DisconnectAsync(quit: true, cancellationToken);
             sw.Stop();
@@ -176,24 +155,16 @@ public static class SmtpAdminEndpoints
         }
     }
 
-    private static SmtpOptions BuildEffectiveOptions(UpdateSmtpConfigurationDto dto, SmtpOptions fallback) => new()
+    private static SmtpOptions BuildEffectiveOptions(UpdateSmtpConfigurationDto dto) => new()
     {
+        CredentialSource = dto.CredentialSource,
         Host = dto.Host,
         Port = dto.Port,
         UseSsl = dto.UseSsl,
         UseTls = dto.UseTls,
         CheckCertificateRevocation = dto.CheckCertificateRevocation,
         FromAddress = dto.FromAddress,
-        FromName = dto.FromName,
-        Username = dto.Username ?? fallback.Username,
-        Password = dto.Password ?? fallback.Password,
-        OAuth2 = dto.OAuth2 is null ? null : new OAuth2Options
-        {
-            TenantId = dto.OAuth2.TenantId,
-            ClientId = dto.OAuth2.ClientId,
-            ClientSecret = dto.OAuth2.ClientSecret ?? fallback.OAuth2?.ClientSecret ?? string.Empty,
-            SmtpUserPrincipalName = dto.OAuth2.SmtpUserPrincipalName
-        }
+        FromName = dto.FromName
     };
 
     private static SmtpConfigurationDto ToDto(SmtpConfiguration e, SmtpConfigurationSource source)
@@ -207,6 +178,6 @@ public static class SmtpAdminEndpoints
         return new SmtpConfigurationDto(
             e.Host, e.Port, e.UseSsl, e.UseTls, e.CheckCertificateRevocation, e.FromAddress, e.FromName,
             e.Username, !string.IsNullOrEmpty(e.EncryptedPassword),
-            oauth, source, e.UpdatedAt);
+            oauth, source, e.UpdatedAt, e.CredentialSource);
     }
 }
`````

### `a/src/MailGate.Api/Admin/Validators/SmtpValidators.cs b/src/MailGate.Api/Admin/Validators/SmtpValidators.cs`

`````diff
diff --git a/src/MailGate.Api/Admin/Validators/SmtpValidators.cs b/src/MailGate.Api/Admin/Validators/SmtpValidators.cs
index 137872f..650d9e8 100644
--- a/src/MailGate.Api/Admin/Validators/SmtpValidators.cs
+++ b/src/MailGate.Api/Admin/Validators/SmtpValidators.cs
@@ -1,4 +1,5 @@
 using FluentValidation;
+using MailGate.Core.Enums;
 using MailGate.Contracts.Smtp;
 
 namespace MailGate.Api.Admin.Validators;
@@ -7,6 +8,13 @@ public sealed class UpdateSmtpConfigurationValidator : AbstractValidator<UpdateS
 {
     public UpdateSmtpConfigurationValidator()
     {
+        RuleFor(x => x.CredentialSource).Must(x => x is SmtpCredentialSource.Manual or SmtpCredentialSource.Sidecar)
+            .WithMessage("Выберите ручную настройку или учётную запись из файла.");
+        RuleFor(x => x).Must(x => x.CredentialSource != SmtpCredentialSource.Sidecar || x.UseTls || x.UseSsl)
+            .WithMessage("Для учётной записи из файла требуется STARTTLS или SSL.");
+        RuleFor(x => x.Username).Empty().WithMessage("Логин вводится только через файл учётной записи.");
+        RuleFor(x => x.Password).Empty().WithMessage("Пароль вводится только через файл учётной записи.");
+        RuleFor(x => x.OAuth2).Null().WithMessage("Настройка OAuth2 временно недоступна.");
         RuleFor(x => x.Host).NotEmpty().MaximumLength(256);
         RuleFor(x => x.UseSsl).Must((dto, useSsl) => !useSsl || !dto.UseTls)
             .WithMessage("Выберите один режим: STARTTLS или SSL при подключении.");
`````

### `a/src/MailGate.Api/Dockerfile b/src/MailGate.Api/Dockerfile`

`````diff
diff --git a/src/MailGate.Api/Dockerfile b/src/MailGate.Api/Dockerfile
index c047e57..c50d249 100644
--- a/src/MailGate.Api/Dockerfile
+++ b/src/MailGate.Api/Dockerfile
@@ -23,7 +23,12 @@ COPY ["src/MailGate.Core/MailGate.Core.csproj", "MailGate.Core/"]
 COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj", "MailGate.Channels.Smtp/"]
 COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj", "MailGate.Infrastructure/"]
 
-RUN dotnet restore "MailGate.Api/MailGate.Api.csproj"
+RUN --mount=type=secret,id=nuget_config \
+    if [ -f /run/secrets/nuget_config ]; then \
+      dotnet restore "MailGate.Api/MailGate.Api.csproj" --configfile /run/secrets/nuget_config; \
+    else \
+      dotnet restore "MailGate.Api/MailGate.Api.csproj"; \
+    fi
 
 COPY src/ .
 RUN dotnet publish "MailGate.Api/MailGate.Api.csproj" \
`````

### `a/src/MailGate.Channels.Smtp/Extensions/ServiceCollectionExtensions.cs b/src/MailGate.Channels.Smtp/Extensions/ServiceCollectionExtensions.cs`

`````diff
diff --git a/src/MailGate.Channels.Smtp/Extensions/ServiceCollectionExtensions.cs b/src/MailGate.Channels.Smtp/Extensions/ServiceCollectionExtensions.cs
index dba541a..f90fb62 100644
--- a/src/MailGate.Channels.Smtp/Extensions/ServiceCollectionExtensions.cs
+++ b/src/MailGate.Channels.Smtp/Extensions/ServiceCollectionExtensions.cs
@@ -14,6 +14,7 @@ public static class ServiceCollectionExtensions
         IConfiguration configuration)
     {
         services.Configure<SmtpOptions>(configuration.GetSection(SmtpOptions.SectionName));
+        services.TryAddSingleton<SmtpCredentialResolver>();
         services.TryAddSingleton<ISmtpSettingsProvider, OptionsSmtpSettingsProvider>();
         services.AddSingleton<INotificationChannel, SmtpChannel>();
         return services;
`````

### `a/src/MailGate.Channels.Smtp/Options/SmtpOptions.cs b/src/MailGate.Channels.Smtp/Options/SmtpOptions.cs`

`````diff
diff --git a/src/MailGate.Channels.Smtp/Options/SmtpOptions.cs b/src/MailGate.Channels.Smtp/Options/SmtpOptions.cs
index fb71530..2f2fa41 100644
--- a/src/MailGate.Channels.Smtp/Options/SmtpOptions.cs
+++ b/src/MailGate.Channels.Smtp/Options/SmtpOptions.cs
@@ -1,9 +1,14 @@
+using MailGate.Core.Enums;
+
 namespace MailGate.Channels.Smtp.Options;
 
 public sealed class SmtpOptions
 {
     public const string SectionName = "Smtp";
 
+    public SmtpCredentialSource CredentialSource { get; set; } = SmtpCredentialSource.Legacy;
+    public string CredentialsFilePath { get; set; } = "/var/mailgate/smtp-credentials/account.json";
+
     public int SettingsCacheSeconds { get; set; } = 30;
 
     public string Host { get; set; } = "localhost";
`````

### `a/src/MailGate.Channels.Smtp/Services/OptionsSmtpSettingsProvider.cs b/src/MailGate.Channels.Smtp/Services/OptionsSmtpSettingsProvider.cs`

`````diff
diff --git a/src/MailGate.Channels.Smtp/Services/OptionsSmtpSettingsProvider.cs b/src/MailGate.Channels.Smtp/Services/OptionsSmtpSettingsProvider.cs
index 9d1263e..e8e86d5 100644
--- a/src/MailGate.Channels.Smtp/Services/OptionsSmtpSettingsProvider.cs
+++ b/src/MailGate.Channels.Smtp/Services/OptionsSmtpSettingsProvider.cs
@@ -3,12 +3,24 @@ using Microsoft.Extensions.Options;
 
 namespace MailGate.Channels.Smtp.Services;
 
-/// <summary>
-/// Default provider that returns <see cref="SmtpOptions"/> from appsettings via <see cref="IOptionsMonitor{TOptions}"/>.
-/// Used when no database override mechanism is configured.
-/// </summary>
-public sealed class OptionsSmtpSettingsProvider(IOptionsMonitor<SmtpOptions> options) : ISmtpSettingsProvider
+public sealed class OptionsSmtpSettingsProvider(
+    IOptionsMonitor<SmtpOptions> options,
+    SmtpCredentialResolver credentials) : ISmtpSettingsProvider
 {
-    public Task<SmtpOptions> GetEffectiveAsync(CancellationToken cancellationToken = default)
-        => Task.FromResult(options.CurrentValue);
+    public async Task<SmtpOptions> GetEffectiveAsync(CancellationToken cancellationToken = default)
+    {
+        var source = options.CurrentValue;
+        var effective = new SmtpOptions
+        {
+            CredentialSource = source.CredentialSource,
+            CredentialsFilePath = source.CredentialsFilePath,
+            Host = source.Host, Port = source.Port,
+            UseSsl = source.UseSsl, UseTls = source.UseTls,
+            CheckCertificateRevocation = source.CheckCertificateRevocation,
+            FromAddress = source.FromAddress, FromName = source.FromName,
+            Username = source.Username, Password = source.Password, OAuth2 = source.OAuth2
+        };
+        await credentials.ApplyAsync(effective, cancellationToken);
+        return effective;
+    }
 }
`````

### `a/src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs b/src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs`

`````diff
diff --git a/src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs b/src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs
new file mode 100644
index 0000000..c2d0601
--- /dev/null
+++ b/src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs
@@ -0,0 +1,58 @@
+using System.Text.Json;
+using MailGate.Channels.Smtp.Options;
+using MailGate.Core.Enums;
+using Microsoft.Extensions.Options;
+
+namespace MailGate.Channels.Smtp.Services;
+
+/// <summary>Reads one atomically rendered credential file; never falls back on failure.</summary>
+public sealed class SmtpCredentialResolver(IOptionsMonitor<SmtpOptions> settings)
+{
+    public async Task ApplyAsync(SmtpOptions options, CancellationToken ct = default)
+    {
+        ct.ThrowIfCancellationRequested();
+        if (options.CredentialSource == SmtpCredentialSource.Legacy) return;
+        options.Username = null;
+        options.Password = null;
+        options.OAuth2 = null;
+        if (options.CredentialSource == SmtpCredentialSource.Manual) return;
+        if (options.CredentialSource != SmtpCredentialSource.Sidecar)
+            throw new InvalidOperationException("Неизвестный источник учётных данных SMTP.");
+        if (!options.UseTls && !options.UseSsl)
+            throw new InvalidOperationException("Для почтовой учётной записи включите STARTTLS или SSL.");
+        var path = settings.CurrentValue.CredentialsFilePath;
+        try
+        {
+            if (string.IsNullOrWhiteSpace(path) || !Path.IsPathFullyQualified(path))
+                throw new IOException();
+            await using var file = new FileStream(path, FileMode.Open, FileAccess.Read,
+                FileShare.ReadWrite | FileShare.Delete, 4096, FileOptions.Asynchronous);
+            if (file.Length is <= 0 or > 65536) throw new IOException();
+            // Bound the stream even if a producer writes in-place. Atomic rename is required.
+            var buffer = new byte[65537];
+            var count = 0;
+            while (count < buffer.Length)
+            {
+                var read = await file.ReadAsync(buffer.AsMemory(count), ct);
+                if (read == 0) break;
+                count += read;
+            }
+            if (count > 65536) throw new IOException();
+            using var document = JsonDocument.Parse(buffer.AsMemory(0, count));
+            var root = document.RootElement;
+            var username = root.GetProperty("username").GetString();
+            var password = root.GetProperty("password").GetString();
+            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(password)
+                || username.Length > 4096 || password.Length > 16384)
+                throw new IOException();
+            options.Username = username;
+            options.Password = password;
+        }
+        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException
+            or KeyNotFoundException or InvalidOperationException or ArgumentException)
+        {
+            // Never include the file, its path or parser exceptions in user-facing logs.
+            throw new InvalidOperationException("Не удалось прочитать SMTP-учётную запись из файла. Проверьте наличие файла, права доступа и поля username/password.");
+        }
+    }
+}
`````

### `a/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs b/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs`

`````diff
diff --git a/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs b/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs
index 33966e2..afd46a3 100644
--- a/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs
+++ b/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs
@@ -1,3 +1,5 @@
+using MailGate.Core.Enums;
+
 namespace MailGate.Contracts.Smtp;
 
 public enum SmtpConfigurationSource
@@ -18,7 +20,8 @@ public sealed record SmtpConfigurationDto(
     bool HasPassword,
     SmtpOAuth2Dto? OAuth2,
     SmtpConfigurationSource Source,
-    DateTimeOffset? UpdatedAt);
+    DateTimeOffset? UpdatedAt,
+    SmtpCredentialSource CredentialSource = SmtpCredentialSource.Legacy);
 
 public sealed record SmtpOAuth2Dto(
     string TenantId,
@@ -28,6 +31,7 @@ public sealed record SmtpOAuth2Dto(
 
 public sealed class UpdateSmtpConfigurationDto
 {
+    public SmtpCredentialSource CredentialSource { get; set; } = SmtpCredentialSource.Manual;
     public string Host { get; set; } = string.Empty;
     public int Port { get; set; } = 587;
     public bool UseSsl { get; set; }
`````

### `a/src/MailGate.Core/Entities/SmtpConfiguration.cs b/src/MailGate.Core/Entities/SmtpConfiguration.cs`

`````diff
diff --git a/src/MailGate.Core/Entities/SmtpConfiguration.cs b/src/MailGate.Core/Entities/SmtpConfiguration.cs
index 6757e3b..2f158e2 100644
--- a/src/MailGate.Core/Entities/SmtpConfiguration.cs
+++ b/src/MailGate.Core/Entities/SmtpConfiguration.cs
@@ -1,3 +1,5 @@
+using MailGate.Core.Enums;
+
 namespace MailGate.Core.Entities;
 
 /// <summary>
@@ -7,6 +9,7 @@ namespace MailGate.Core.Entities;
 /// </summary>
 public sealed class SmtpConfiguration
 {
+    public SmtpCredentialSource CredentialSource { get; set; } = SmtpCredentialSource.Legacy;
     public Guid Id { get; set; } = Guid.NewGuid();
     public string Host { get; set; } = string.Empty;
     public int Port { get; set; } = 587;
`````

### `a/src/MailGate.Core/Enums/SmtpCredentialSource.cs b/src/MailGate.Core/Enums/SmtpCredentialSource.cs`

`````diff
diff --git a/src/MailGate.Core/Enums/SmtpCredentialSource.cs b/src/MailGate.Core/Enums/SmtpCredentialSource.cs
new file mode 100644
index 0000000..8e2a90e
--- /dev/null
+++ b/src/MailGate.Core/Enums/SmtpCredentialSource.cs
@@ -0,0 +1,9 @@
+namespace MailGate.Core.Enums;
+
+public enum SmtpCredentialSource
+{
+    Manual = 0,
+    Sidecar = 1,
+    // Read existing installations without decrypting or deleting them on upgrade.
+    Legacy = 2
+}
`````

### `a/src/MailGate.Frontend/.dockerignore b/src/MailGate.Frontend/.dockerignore`

`````diff
diff --git a/src/MailGate.Frontend/.dockerignore b/src/MailGate.Frontend/.dockerignore
index 96dcf61..bde2488 100644
--- a/src/MailGate.Frontend/.dockerignore
+++ b/src/MailGate.Frontend/.dockerignore
@@ -4,3 +4,6 @@
 node_modules/
 dist/
 *.tsbuildinfo
+
+**/account.json
+secrets/
`````

### `a/src/MailGate.Frontend/src/api/types.ts b/src/MailGate.Frontend/src/api/types.ts`

`````diff
diff --git a/src/MailGate.Frontend/src/api/types.ts b/src/MailGate.Frontend/src/api/types.ts
index 16480ab..f787ae9 100644
--- a/src/MailGate.Frontend/src/api/types.ts
+++ b/src/MailGate.Frontend/src/api/types.ts
@@ -143,7 +143,10 @@ export interface SmtpOAuth2Dto {
   smtpUserPrincipalName: string;
 }
 
+export type SmtpCredentialSource = 'Manual' | 'Sidecar' | 'Legacy';
+
 export interface SmtpConfigurationDto {
+  credentialSource: SmtpCredentialSource;
   host: string;
   port: number;
   useSsl: boolean;
@@ -166,6 +169,7 @@ export interface UpdateSmtpOAuth2Dto {
 }
 
 export interface UpdateSmtpConfigurationDto {
+  credentialSource: SmtpCredentialSource;
   host: string;
   port: number;
   useSsl: boolean;
`````

### `a/src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx b/src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx`

`````diff
diff --git a/src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx b/src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx
index 108dbb6..11b8b0a 100644
--- a/src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx
+++ b/src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx
@@ -5,7 +5,7 @@ import {
   Button,
   Card,
   Checkbox,
-  Collapse,
+  Radio,
   Descriptions,
   Form,
   Input,
@@ -19,7 +19,7 @@ import {
 import { ApiOutlined, DeleteOutlined, SaveOutlined } from '@ant-design/icons';
 import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
 import { smtpApi } from '../api/endpoints';
-import type { SmtpTestResultDto, UpdateSmtpConfigurationDto } from '../api/types';
+import type { SmtpCredentialSource, SmtpTestResultDto, UpdateSmtpConfigurationDto } from '../api/types';
 
 interface FormValues {
   host: string;
@@ -29,13 +29,7 @@ interface FormValues {
   checkCertificateRevocation: boolean;
   fromAddress: string;
   fromName: string;
-  username?: string;
-  password?: string;
-  oauthEnabled: boolean;
-  oauthTenantId?: string;
-  oauthClientId?: string;
-  oauthClientSecret?: string;
-  oauthUpn?: string;
+  credentialSource: SmtpCredentialSource;
 }
 
 function toDto(values: FormValues): UpdateSmtpConfigurationDto {
@@ -47,16 +41,10 @@ function toDto(values: FormValues): UpdateSmtpConfigurationDto {
     checkCertificateRevocation: values.checkCertificateRevocation,
     fromAddress: values.fromAddress.trim(),
     fromName: values.fromName.trim(),
-    username: values.username?.trim() || null,
-    password: values.password || null,
-    oAuth2: values.oauthEnabled
-      ? {
-          tenantId: values.oauthTenantId?.trim() ?? '',
-          clientId: values.oauthClientId?.trim() ?? '',
-          clientSecret: values.oauthClientSecret || null,
-          smtpUserPrincipalName: values.oauthUpn?.trim() ?? '',
-        }
-      : null,
+    credentialSource: values.credentialSource,
+    username: null,
+    password: null,
+    oAuth2: null,
   };
 }
 
@@ -83,11 +71,7 @@ export function SmtpSettingsPage() {
         checkCertificateRevocation: effective.checkCertificateRevocation ?? true,
         fromAddress: effective.fromAddress,
         fromName: effective.fromName,
-        username: effective.username ?? undefined,
-        oauthEnabled: effective.oAuth2 !== null,
-        oauthTenantId: effective.oAuth2?.tenantId,
-        oauthClientId: effective.oAuth2?.clientId,
-        oauthUpn: effective.oAuth2?.smtpUserPrincipalName,
+        credentialSource: effective.credentialSource === 'Legacy' && !effective.username && !effective.hasPassword && !effective.oAuth2 ? 'Manual' : effective.credentialSource,
       });
     }
   }, [effective, form]);
@@ -122,7 +106,8 @@ export function SmtpSettingsPage() {
     onError: (error: Error) => message.error(error.message),
   });
 
-  const oauthEnabled = Form.useWatch('oauthEnabled', form);
+  const credentialSource = Form.useWatch('credentialSource', form);
+  const pending = saveMutation.isPending || testMutation.isPending || deleteMutation.isPending;
 
   if (effectiveQuery.isLoading) {
     return <Spin size="large" style={{ display: 'block', margin: '80px auto' }} />;
@@ -132,6 +117,7 @@ export function SmtpSettingsPage() {
     <div style={{ maxWidth: 760 }}>
       <Typography.Title level={3}>Настройки SMTP</Typography.Title>
 
+      {effectiveQuery.isError && <Alert type="error" title={effectiveQuery.error.message} style={{ marginBottom: 16 }} />}
       {effective && (
         <Card size="small" style={{ marginBottom: 16 }}>
           <Descriptions column={2} size="small" title="Действующая конфигурация">
@@ -152,8 +138,25 @@ export function SmtpSettingsPage() {
           form={form}
           layout="vertical"
           onFinish={(values) => saveMutation.mutate(values)}
-          initialValues={{ port: 587, useTls: true, useSsl: false, checkCertificateRevocation: true, oauthEnabled: false }}
+          initialValues={{ port: 587, useTls: true, useSsl: false, checkCertificateRevocation: true, credentialSource: 'Manual' }}
+          disabled={pending}
+          onValuesChange={() => setTestResult(null)}
         >
+          <Form.Item name="credentialSource" label="Источник SMTP-настроек" rules={[{ required: true }]}>
+            <Radio.Group options={[
+              { value: 'Manual', label: 'Вручную (тестовый SMTP без авторизации)' },
+              { value: 'Sidecar', label: 'Существующая почтовая учётная запись' },
+            ]} />
+          </Form.Item>
+          <Alert style={{ marginBottom: 16 }} type={credentialSource === 'Legacy' ? 'warning' : 'info'} title={
+            credentialSource === 'Sidecar'
+              ? 'Логин и пароль берутся из файла, предоставленного sidecar. Они не сохраняются в БД. Укажите параметры сервера ниже и включите STARTTLS или SSL.'
+              : credentialSource === 'Legacy'
+                ? 'Прежняя конфигурация продолжает работать. Выберите новый режим, чтобы изменить настройки. При сохранении прежние логин, пароль и OAuth2 будут удалены из этой записи.'
+                : 'Режим для MailHog и других тестовых серверов без аутентификации. Ввод логина, пароля и OAuth2 отключён.'
+          } />
+          {effective?.source === 'Database' && effective.credentialSource === 'Legacy' && credentialSource !== 'Legacy' &&
+            <Alert type="warning" style={{ marginBottom: 16 }} title="Сохранение нового режима очистит старые учётные данные из БД. Проверьте новый режим перед сохранением." />}
           <Space.Compact block>
             <Form.Item
               name="host"
@@ -205,71 +208,6 @@ export function SmtpSettingsPage() {
             <Input placeholder="MailGate" />
           </Form.Item>
 
-          <Form.Item name="username" label="Логин">
-            <Input autoComplete="off" />
-          </Form.Item>
-
-          <Form.Item
-            name="password"
-            label="Пароль"
-            tooltip={effective?.hasPassword ? 'Пароль уже сохранён. Оставьте пустым, чтобы не менять.' : undefined}
-          >
-            <Input.Password autoComplete="new-password" placeholder={effective?.hasPassword ? '••••••••' : ''} />
-          </Form.Item>
-
-          <Form.Item name="oauthEnabled" valuePropName="checked">
-            <Checkbox>OAuth2 (Microsoft Exchange Online)</Checkbox>
-          </Form.Item>
-
-          {oauthEnabled && (
-            <Collapse
-              defaultActiveKey={['oauth']}
-              items={[
-                {
-                  key: 'oauth',
-                  label: 'Параметры OAuth2',
-                  children: (
-                    <>
-                      <Form.Item
-                        name="oauthTenantId"
-                        label="Tenant ID"
-                        rules={[{ required: true, message: 'Введите Tenant ID' }]}
-                      >
-                        <Input />
-                      </Form.Item>
-                      <Form.Item
-                        name="oauthClientId"
-                        label="Client ID"
-                        rules={[{ required: true, message: 'Введите Client ID' }]}
-                      >
-                        <Input />
-                      </Form.Item>
-                      <Form.Item
-                        name="oauthClientSecret"
-                        label="Client Secret"
-                        tooltip={
-                          effective?.oAuth2?.hasClientSecret
-                            ? 'Секрет уже сохранён. Оставьте пустым, чтобы не менять.'
-                            : undefined
-                        }
-                      >
-                        <Input.Password autoComplete="new-password" />
-                      </Form.Item>
-                      <Form.Item
-                        name="oauthUpn"
-                        label="SMTP User Principal Name"
-                        rules={[{ required: true, message: 'Введите UPN' }]}
-                      >
-                        <Input placeholder="mailer@tenant.onmicrosoft.com" />
-                      </Form.Item>
-                    </>
-                  ),
-                },
-              ]}
-              style={{ marginBottom: 16 }}
-            />
-          )}
-
           {testResult && (
             <Alert
               style={{ marginBottom: 16 }}
@@ -285,10 +223,10 @@ export function SmtpSettingsPage() {
           )}
 
           <Space wrap>
-            <Button type="primary" htmlType="submit" icon={<SaveOutlined />} loading={saveMutation.isPending}>
+            <Button type="primary" htmlType="submit" icon={<SaveOutlined />} loading={saveMutation.isPending} disabled={pending || credentialSource === 'Legacy'}>
               Сохранить override
             </Button>
-            <Button icon={<ApiOutlined />} loading={testMutation.isPending} onClick={() => testMutation.mutate()}>
+            <Button icon={<ApiOutlined />} loading={testMutation.isPending} disabled={pending || credentialSource === 'Legacy'} onClick={() => testMutation.mutate()}>
               Проверить подключение
             </Button>
             {effective?.source === 'Database' && (
`````

### `a/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs b/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs b/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs
index 6439f43..e383299 100644
--- a/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs
+++ b/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs
@@ -30,12 +30,17 @@ public static class ServiceCollectionExtensions
 
         services.AddMemoryCache();
         var keysPath = configuration["DataProtection:KeysPath"];
-        if (string.IsNullOrWhiteSpace(keysPath))
-            throw new InvalidOperationException("DataProtection:KeysPath must point to a persistent shared directory.");
-        Directory.CreateDirectory(keysPath);
-        services.AddDataProtection()
-            .SetApplicationName("MailGate")
-            .PersistKeysToFileSystem(new DirectoryInfo(keysPath));
+        var protection = services.AddDataProtection().SetApplicationName("MailGate");
+        if (!string.IsNullOrWhiteSpace(keysPath))
+        {
+            Directory.CreateDirectory(keysPath);
+            protection.PersistKeysToFileSystem(new DirectoryInfo(keysPath));
+        }
+        else
+        {
+            // New SMTP modes do not encrypt anything. Never generate local fallback keys.
+            protection.DisableAutomaticKeyGeneration();
+        }
         services.AddScoped<SmtpConfigurationStore>();
 
         var rmqOptions = configuration.GetSection(MassTransitOptions.SectionName).Get<MassTransitOptions>()
`````

### `a/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.Designer.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.Designer.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.Designer.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.Designer.cs
new file mode 100644
index 0000000..3234804
--- /dev/null
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.Designer.cs
@@ -0,0 +1,524 @@
+// <auto-generated />
+using System;
+using MailGate.Infrastructure.Persistence;
+using Microsoft.EntityFrameworkCore;
+using Microsoft.EntityFrameworkCore.Infrastructure;
+using Microsoft.EntityFrameworkCore.Migrations;
+using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
+using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
+
+#nullable disable
+
+namespace MailGate.Infrastructure.Persistence.Migrations
+{
+    [DbContext(typeof(MailGateDbContext))]
+    [Migration("20261123090000_AddSmtpCredentialSource")]
+    partial class AddSmtpCredentialSource
+    {
+        protected override void BuildTargetModel(ModelBuilder modelBuilder)
+        {
+#pragma warning disable 612, 618
+            modelBuilder
+                .HasAnnotation("ProductVersion", "10.0.6")
+                .HasAnnotation("Relational:MaxIdentifierLength", 63);
+
+            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
+
+            modelBuilder.Entity("MailGate.Core.Entities.EmailTemplate", b =>
+                {
+                    b.Property<Guid>("Id")
+                        .HasColumnType("uuid");
+
+                    b.Property<DateTimeOffset>("CreatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<string>("DefinitionJson")
+                        .IsRequired()
+                        .HasColumnType("jsonb");
+
+                    b.Property<string>("Name")
+                        .IsRequired()
+                        .HasMaxLength(200)
+                        .HasColumnType("character varying(200)");
+
+                    b.Property<Guid>("Revision")
+                        .IsConcurrencyToken()
+                        .HasColumnType("uuid");
+
+                    b.Property<DateTimeOffset>("UpdatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.HasKey("Id");
+
+                    b.HasIndex("UpdatedAt");
+
+                    b.ToTable("email_templates", (string)null);
+                });
+
+            modelBuilder.Entity("MailGate.Core.Entities.NotificationLog", b =>
+                {
+                    b.Property<Guid>("Id")
+                        .HasColumnType("uuid");
+
+                    b.Property<int>("AttemptCount")
+                        .HasColumnType("integer");
+
+                    b.Property<string>("Channel")
+                        .IsRequired()
+                        .HasMaxLength(50)
+                        .HasColumnType("character varying(50)");
+
+                    b.Property<DateTimeOffset>("CreatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<string>("ErrorMessage")
+                        .HasMaxLength(2000)
+                        .HasColumnType("character varying(2000)");
+
+                    b.Property<string>("IdempotencyKey")
+                        .HasMaxLength(256)
+                        .HasColumnType("character varying(256)");
+
+                    b.Property<Guid>("MessageId")
+                        .HasColumnType("uuid");
+
+                    b.Property<string>("PayloadJson")
+                        .HasColumnType("jsonb");
+
+                    b.Property<DateTimeOffset?>("ProcessedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<string>("RecipientsJson")
+                        .IsRequired()
+                        .HasMaxLength(4096)
+                        .HasColumnType("character varying(4096)");
+
+                    b.Property<Guid?>("RetryOfMessageId")
+                        .HasColumnType("uuid");
+
+                    b.Property<string>("Status")
+                        .IsRequired()
+                        .HasMaxLength(50)
+                        .HasColumnType("character varying(50)");
+
+                    b.Property<string>("Subject")
+                        .HasMaxLength(500)
+                        .HasColumnType("character varying(500)");
+
+                    b.HasKey("Id");
+
+                    b.HasIndex("CreatedAt");
+
+                    b.HasIndex("IdempotencyKey")
+                        .IsUnique()
+                        .HasFilter("\"IdempotencyKey\" IS NOT NULL");
+
+                    b.HasIndex("MessageId");
+
+                    b.HasIndex("Status");
+
+                    b.ToTable("notification_logs", (string)null);
+                });
+
+            modelBuilder.Entity("MailGate.Core.Entities.NotificationSystem", b =>
+                {
+                    b.Property<Guid>("Id")
+                        .HasColumnType("uuid");
+
+                    b.Property<string>("Code")
+                        .IsRequired()
+                        .HasMaxLength(100)
+                        .HasColumnType("character varying(100)");
+
+                    b.Property<DateTimeOffset>("CreatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<string>("Description")
+                        .HasMaxLength(1000)
+                        .HasColumnType("character varying(1000)");
+
+                    b.Property<string>("Name")
+                        .IsRequired()
+                        .HasMaxLength(200)
+                        .HasColumnType("character varying(200)");
+
+                    b.Property<DateTimeOffset?>("UpdatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.HasKey("Id");
+
+                    b.HasIndex("Code")
+                        .IsUnique();
+
+                    b.ToTable("systems", (string)null);
+                });
+
+            modelBuilder.Entity("MailGate.Core.Entities.Recipient", b =>
+                {
+                    b.Property<Guid>("Id")
+                        .HasColumnType("uuid");
+
+                    b.Property<DateTimeOffset>("CreatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<string>("Email")
+                        .IsRequired()
+                        .HasMaxLength(320)
+                        .HasColumnType("character varying(320)");
+
+                    b.Property<string>("FullName")
+                        .IsRequired()
+                        .HasMaxLength(200)
+                        .HasColumnType("character varying(200)");
+
+                    b.Property<DateTimeOffset?>("UpdatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.HasKey("Id");
+
+                    b.HasIndex("Email")
+                        .IsUnique();
+
+                    b.ToTable("recipients", (string)null);
+                });
+
+            modelBuilder.Entity("MailGate.Core.Entities.RecipientGroup", b =>
+                {
+                    b.Property<Guid>("Id")
+                        .HasColumnType("uuid");
+
+                    b.Property<DateTimeOffset>("CreatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<string>("Description")
+                        .HasMaxLength(1000)
+                        .HasColumnType("character varying(1000)");
+
+                    b.Property<string>("Name")
+                        .IsRequired()
+                        .HasMaxLength(200)
+                        .HasColumnType("character varying(200)");
+
+                    b.Property<DateTimeOffset?>("UpdatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.HasKey("Id");
+
+                    b.HasIndex("Name")
+                        .IsUnique();
+
+                    b.ToTable("recipient_groups", (string)null);
+                });
+
+            modelBuilder.Entity("MailGate.Core.Entities.SmtpConfiguration", b =>
+                {
+                    b.Property<bool>("CheckCertificateRevocation")
+                        .ValueGeneratedOnAdd()
+                        .HasColumnType("boolean")
+                        .HasDefaultValue(true);
+
+                    b.Property<int>("CredentialSource")
+                        .HasColumnType("integer");
+
+                    b.Property<Guid>("Id")
+                        .HasColumnType("uuid");
+
+                    b.Property<string>("EncryptedPassword")
+                        .HasMaxLength(4096)
+                        .HasColumnType("character varying(4096)");
+
+                    b.Property<string>("FromAddress")
+                        .IsRequired()
+                        .HasMaxLength(320)
+                        .HasColumnType("character varying(320)");
+
+                    b.Property<string>("FromName")
+                        .IsRequired()
+                        .HasMaxLength(200)
+                        .HasColumnType("character varying(200)");
+
+                    b.Property<string>("Host")
+                        .IsRequired()
+                        .HasMaxLength(256)
+                        .HasColumnType("character varying(256)");
+
+                    b.Property<string>("OAuth2ClientId")
+                        .HasMaxLength(128)
+                        .HasColumnType("character varying(128)");
+
+                    b.Property<string>("OAuth2EncryptedClientSecret")
+                        .HasMaxLength(4096)
+                        .HasColumnType("character varying(4096)");
+
+                    b.Property<string>("OAuth2SmtpUserPrincipalName")
+                        .HasMaxLength(320)
+                        .HasColumnType("character varying(320)");
+
+                    b.Property<string>("OAuth2TenantId")
+                        .HasMaxLength(128)
+                        .HasColumnType("character varying(128)");
+
+                    b.Property<int>("Port")
+                        .HasColumnType("integer");
+
+                    b.Property<DateTimeOffset>("UpdatedAt")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<bool>("UseSsl")
+                        .HasColumnType("boolean");
+
+                    b.Property<bool>("UseTls")
+                        .HasColumnType("boolean");
+
+                    b.Property<string>("Username")
+                        .HasMaxLength(320)
+                        .HasColumnType("character varying(320)");
+
+                    b.HasKey("Id");
+
+                    b.ToTable("smtp_configurations", (string)null);
+                });
+
+            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.InboxState", b =>
+                {
+                    b.Property<long>("Id")
+                        .ValueGeneratedOnAdd()
+                        .HasColumnType("bigint");
+
+                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));
+
+                    b.Property<DateTime?>("Consumed")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<Guid>("ConsumerId")
+                        .HasColumnType("uuid");
+
+                    b.Property<DateTime?>("Delivered")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<DateTime?>("ExpirationTime")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<long?>("LastSequenceNumber")
+                        .HasColumnType("bigint");
+
+                    b.Property<Guid>("LockId")
+                        .HasColumnType("uuid");
+
+                    b.Property<Guid>("MessageId")
+                        .HasColumnType("uuid");
+
+                    b.Property<int>("ReceiveCount")
+                        .HasColumnType("integer");
+
+                    b.Property<DateTime>("Received")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<byte[]>("RowVersion")
+                        .IsConcurrencyToken()
+                        .ValueGeneratedOnAddOrUpdate()
+                        .HasColumnType("bytea");
+
+                    b.HasKey("Id");
+
+                    b.HasIndex("Delivered");
+
+                    b.ToTable("InboxState");
+                });
+
+            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxMessage", b =>
+                {
+                    b.Property<long>("SequenceNumber")
+                        .ValueGeneratedOnAdd()
+                        .HasColumnType("bigint");
+
+                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("SequenceNumber"));
+
+                    b.Property<string>("Body")
+                        .IsRequired()
+                        .HasColumnType("text");
+
+                    b.Property<string>("ContentType")
+                        .IsRequired()
+                        .HasMaxLength(256)
+                        .HasColumnType("character varying(256)");
+
+                    b.Property<Guid?>("ConversationId")
+                        .HasColumnType("uuid");
+
+                    b.Property<Guid?>("CorrelationId")
+                        .HasColumnType("uuid");
+
+                    b.Property<string>("DestinationAddress")
+                        .HasMaxLength(256)
+                        .HasColumnType("character varying(256)");
+
+                    b.Property<DateTime?>("EnqueueTime")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<DateTime?>("ExpirationTime")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<string>("FaultAddress")
+                        .HasMaxLength(256)
+                        .HasColumnType("character varying(256)");
+
+                    b.Property<string>("Headers")
+                        .HasColumnType("text");
+
+                    b.Property<Guid?>("InboxConsumerId")
+                        .HasColumnType("uuid");
+
+                    b.Property<Guid?>("InboxMessageId")
+                        .HasColumnType("uuid");
+
+                    b.Property<Guid?>("InitiatorId")
+                        .HasColumnType("uuid");
+
+                    b.Property<Guid>("MessageId")
+                        .HasColumnType("uuid");
+
+                    b.Property<string>("MessageType")
+                        .IsRequired()
+                        .HasColumnType("text");
+
+                    b.Property<Guid?>("OutboxId")
+                        .HasColumnType("uuid");
+
+                    b.Property<string>("Properties")
+                        .HasColumnType("text");
+
+                    b.Property<Guid?>("RequestId")
+                        .HasColumnType("uuid");
+
+                    b.Property<string>("ResponseAddress")
+                        .HasMaxLength(256)
+                        .HasColumnType("character varying(256)");
+
+                    b.Property<DateTime>("SentTime")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<string>("SourceAddress")
+                        .HasMaxLength(256)
+                        .HasColumnType("character varying(256)");
+
+                    b.HasKey("SequenceNumber");
+
+                    b.HasIndex("EnqueueTime");
+
+                    b.HasIndex("ExpirationTime");
+
+                    b.HasIndex("OutboxId", "SequenceNumber")
+                        .IsUnique();
+
+                    b.HasIndex("InboxMessageId", "InboxConsumerId", "SequenceNumber")
+                        .IsUnique();
+
+                    b.ToTable("OutboxMessage");
+                });
+
+            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxState", b =>
+                {
+                    b.Property<Guid>("OutboxId")
+                        .ValueGeneratedOnAdd()
+                        .HasColumnType("uuid");
+
+                    b.Property<DateTime>("Created")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<DateTime?>("Delivered")
+                        .HasColumnType("timestamp with time zone");
+
+                    b.Property<long?>("LastSequenceNumber")
+                        .HasColumnType("bigint");
+
+                    b.Property<Guid>("LockId")
+                        .HasColumnType("uuid");
+
+                    b.Property<byte[]>("RowVersion")
+                        .IsConcurrencyToken()
+                        .ValueGeneratedOnAddOrUpdate()
+                        .HasColumnType("bytea");
+
+                    b.HasKey("OutboxId");
+
+                    b.HasIndex("Created");
+
+                    b.ToTable("OutboxState");
+                });
+
+            modelBuilder.Entity("RecipientGroupMember", b =>
+                {
+                    b.Property<Guid>("GroupId")
+                        .HasColumnType("uuid");
+
+                    b.Property<Guid>("RecipientId")
+                        .HasColumnType("uuid");
+
+                    b.HasKey("GroupId", "RecipientId");
+
+                    b.HasIndex("RecipientId");
+
+                    b.ToTable("recipient_group_members", (string)null);
+                });
+
+            modelBuilder.Entity("RecipientSystem", b =>
+                {
+                    b.Property<Guid>("RecipientId")
+                        .HasColumnType("uuid");
+
+                    b.Property<Guid>("SystemId")
+                        .HasColumnType("uuid");
+
+                    b.HasKey("RecipientId", "SystemId");
+
+                    b.HasIndex("SystemId");
+
+                    b.ToTable("recipient_systems", (string)null);
+                });
+
+            modelBuilder.Entity("MassTransit.EntityFrameworkCoreIntegration.OutboxMessage", b =>
+                {
+                    b.HasOne("MassTransit.EntityFrameworkCoreIntegration.OutboxState", null)
+                        .WithMany()
+                        .HasForeignKey("OutboxId");
+
+                    b.HasOne("MassTransit.EntityFrameworkCoreIntegration.InboxState", null)
+                        .WithMany()
+                        .HasForeignKey("InboxMessageId", "InboxConsumerId")
+                        .HasPrincipalKey("MessageId", "ConsumerId");
+                });
+
+            modelBuilder.Entity("RecipientGroupMember", b =>
+                {
+                    b.HasOne("MailGate.Core.Entities.RecipientGroup", null)
+                        .WithMany()
+                        .HasForeignKey("GroupId")
+                        .OnDelete(DeleteBehavior.Cascade)
+                        .IsRequired();
+
+                    b.HasOne("MailGate.Core.Entities.Recipient", null)
+                        .WithMany()
+                        .HasForeignKey("RecipientId")
+                        .OnDelete(DeleteBehavior.Cascade)
+                        .IsRequired();
+                });
+
+            modelBuilder.Entity("RecipientSystem", b =>
+                {
+                    b.HasOne("MailGate.Core.Entities.Recipient", null)
+                        .WithMany()
+                        .HasForeignKey("RecipientId")
+                        .OnDelete(DeleteBehavior.Cascade)
+                        .IsRequired();
+
+                    b.HasOne("MailGate.Core.Entities.NotificationSystem", null)
+                        .WithMany()
+                        .HasForeignKey("SystemId")
+                        .OnDelete(DeleteBehavior.Cascade)
+                        .IsRequired();
+                });
+#pragma warning restore 612, 618
+        }
+    }
+}
`````

### `a/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.cs
new file mode 100644
index 0000000..f8ddf03
--- /dev/null
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.cs
@@ -0,0 +1,17 @@
+using Microsoft.EntityFrameworkCore.Migrations;
+
+#nullable disable
+namespace MailGate.Infrastructure.Persistence.Migrations;
+
+public partial class AddSmtpCredentialSource : Migration
+{
+    protected override void Up(MigrationBuilder migrationBuilder)
+    {
+        // Preserve existing configurations as Legacy; new UI explicitly saves Manual/Sidecar.
+        migrationBuilder.AddColumn<int>(name: "CredentialSource", table: "smtp_configurations",
+            type: "integer", nullable: false, defaultValue: 2);
+        migrationBuilder.Sql("""ALTER TABLE smtp_configurations ALTER COLUMN "CredentialSource" DROP DEFAULT;""");
+    }
+    protected override void Down(MigrationBuilder migrationBuilder) =>
+        migrationBuilder.DropColumn(name: "CredentialSource", table: "smtp_configurations");
+}
`````

### `a/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs b/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs b/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
index e4bbd60..51945b6 100644
--- a/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
@@ -215,6 +215,9 @@ namespace MailGate.Infrastructure.Persistence.Migrations
                         .HasColumnType("boolean")
                         .HasDefaultValue(true);
 
+                    b.Property<int>("CredentialSource")
+                        .HasColumnType("integer");
+
                     b.Property<Guid>("Id")
                         .HasColumnType("uuid");
 
`````

### `a/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs b/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs b/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs
index 59352a5..25fddf1 100644
--- a/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs
+++ b/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs
@@ -13,21 +13,25 @@ namespace MailGate.Infrastructure.Services;
 public sealed class DbSmtpSettingsProvider(
     IServiceScopeFactory scopeFactory,
     IOptionsMonitor<SmtpOptions> appSettings,
-    IMemoryCache cache) : ISmtpSettingsProvider
+    IMemoryCache cache,
+    SmtpCredentialResolver credentials) : ISmtpSettingsProvider
 {
     private const string CacheKey = "MailGate.Smtp.EffectiveOptions";
 
     public async Task<SmtpOptions> GetEffectiveAsync(CancellationToken cancellationToken = default)
     {
-        if (cache.TryGetValue(CacheKey, out SmtpOptions? cached) && cached is not null)
-            return cached;
-
-        var merged = await LoadAsync(cancellationToken);
-        var seconds = appSettings.CurrentValue.SettingsCacheSeconds;
-        if (seconds <= 0)
-            throw new InvalidOperationException("Smtp:SettingsCacheSeconds must be greater than zero.");
-        cache.Set(CacheKey, merged, TimeSpan.FromSeconds(seconds));
-        return merged;
+        if (!cache.TryGetValue(CacheKey, out SmtpOptions? cached) || cached is null)
+        {
+            cached = await LoadAsync(cancellationToken);
+            var seconds = appSettings.CurrentValue.SettingsCacheSeconds;
+            if (seconds <= 0)
+                throw new InvalidOperationException("Smtp:SettingsCacheSeconds must be greater than zero.");
+            cache.Set(CacheKey, cached, TimeSpan.FromSeconds(seconds));
+        }
+        var effective = Clone(cached);
+        // Read sidecar credentials each time, independently of metadata cache/rotation.
+        await credentials.ApplyAsync(effective, cancellationToken);
+        return effective;
     }
 
     public void Invalidate() => cache.Remove(CacheKey);
@@ -47,6 +51,7 @@ public sealed class DbSmtpSettingsProvider(
 
         var merged = new SmtpOptions
         {
+            CredentialSource = entity.CredentialSource,
             Host = entity.Host,
             Port = entity.Port,
             UseSsl = entity.UseSsl,
@@ -74,6 +79,8 @@ public sealed class DbSmtpSettingsProvider(
 
     private static SmtpOptions Clone(SmtpOptions src) => new()
     {
+        CredentialSource = src.CredentialSource,
+        CredentialsFilePath = src.CredentialsFilePath,
         Host = src.Host,
         Port = src.Port,
         UseSsl = src.UseSsl,
`````

### `a/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs b/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs b/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs
index ec517c9..9d8ad92 100644
--- a/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs
+++ b/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs
@@ -1,4 +1,5 @@
 using System.Security.Cryptography;
+using MailGate.Core.Enums;
 using MailGate.Core.Entities;
 using MailGate.Infrastructure.Persistence;
 using Microsoft.AspNetCore.DataProtection;
@@ -30,6 +31,9 @@ public sealed class SmtpConfigurationStore(
         if (entity is null)
             return null;
 
+        if (entity.CredentialSource != SmtpCredentialSource.Legacy)
+            return new DecryptedConfiguration(entity, null, null);
+
         var oauthEnabled = !string.IsNullOrWhiteSpace(entity.OAuth2TenantId);
         var password = oauthEnabled ? null : DecryptPassword(entity);
         var clientSecret = oauthEnabled ? DecryptOAuth2ClientSecret(entity) : null;
@@ -54,8 +58,20 @@ public sealed class SmtpConfigurationStore(
         var existing = await dbContext.SmtpConfigurations
             .FirstOrDefaultAsync(cancellationToken);
 
-        incoming.EncryptedPassword = Protect(password) ?? existing?.EncryptedPassword;
-        incoming.OAuth2EncryptedClientSecret = Protect(oauth2ClientSecret) ?? existing?.OAuth2EncryptedClientSecret;
+        if (incoming.CredentialSource == SmtpCredentialSource.Legacy)
+        {
+            incoming.EncryptedPassword = Protect(password) ?? existing?.EncryptedPassword;
+            incoming.OAuth2EncryptedClientSecret = Protect(oauth2ClientSecret) ?? existing?.OAuth2EncryptedClientSecret;
+        }
+        else
+        {
+            incoming.Username = null;
+            incoming.EncryptedPassword = null;
+            incoming.OAuth2TenantId = null;
+            incoming.OAuth2ClientId = null;
+            incoming.OAuth2SmtpUserPrincipalName = null;
+            incoming.OAuth2EncryptedClientSecret = null;
+        }
         incoming.UpdatedAt = DateTimeOffset.UtcNow;
 
         if (existing is null)
@@ -64,6 +80,7 @@ public sealed class SmtpConfigurationStore(
         }
         else
         {
+            existing.CredentialSource = incoming.CredentialSource;
             existing.Host = incoming.Host;
             existing.Port = incoming.Port;
             existing.UseSsl = incoming.UseSsl;
`````

### `a/src/MailGate.Worker/Dockerfile b/src/MailGate.Worker/Dockerfile`

`````diff
diff --git a/src/MailGate.Worker/Dockerfile b/src/MailGate.Worker/Dockerfile
index 63e0e34..0d34147 100644
--- a/src/MailGate.Worker/Dockerfile
+++ b/src/MailGate.Worker/Dockerfile
@@ -20,7 +20,12 @@ COPY ["src/MailGate.Worker/MailGate.Worker.csproj",                   "MailGate.
 COPY ["src/MailGate.Core/MailGate.Core.csproj",                       "MailGate.Core/"]
 COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj",   "MailGate.Infrastructure/"]
 COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj",     "MailGate.Channels.Smtp/"]
-RUN dotnet restore "MailGate.Worker/MailGate.Worker.csproj"
+RUN --mount=type=secret,id=nuget_config \
+    if [ -f /run/secrets/nuget_config ]; then \
+      dotnet restore "MailGate.Worker/MailGate.Worker.csproj" --configfile /run/secrets/nuget_config; \
+    else \
+      dotnet restore "MailGate.Worker/MailGate.Worker.csproj"; \
+    fi
 
 COPY src/ .
 RUN dotnet publish "MailGate.Worker/MailGate.Worker.csproj" \
`````

### `a/src/MailGate.Worker/Program.cs b/src/MailGate.Worker/Program.cs`

`````diff
diff --git a/src/MailGate.Worker/Program.cs b/src/MailGate.Worker/Program.cs
index ebf83b2..b2e6c4e 100644
--- a/src/MailGate.Worker/Program.cs
+++ b/src/MailGate.Worker/Program.cs
@@ -6,7 +6,8 @@ using MailGate.Worker.Consumers;
 using MailGate.Worker.Routing;
 using Microsoft.EntityFrameworkCore;
 
-var builder = Host.CreateApplicationBuilder(args);
+var migrateOnly = args.Contains("--migrate-only", StringComparer.Ordinal);
+var builder = Host.CreateApplicationBuilder(args.Where(arg => arg != "--migrate-only").ToArray());
 
 builder.Services.AddInfrastructure(builder.Configuration, bus =>
 {
@@ -18,7 +19,7 @@ builder.Services.AddDbSmtpSettingsProvider();
 
 builder.Services.AddSingleton<IChannelRouter, ChannelRouter>();
 
-var host = builder.Build();
+using var host = builder.Build();
 
 using (var scope = host.Services.CreateScope())
 {
@@ -26,4 +27,5 @@ using (var scope = host.Services.CreateScope())
     await db.Database.MigrateAsync();
 }
 
-await host.RunAsync();
+if (!migrateOnly)
+    await host.RunAsync();
`````

### `a/src/MailGate.sln b/src/MailGate.sln`

`````diff
diff --git a/src/MailGate.sln b/src/MailGate.sln
index fc83e61..0a809fc 100644
--- a/src/MailGate.sln
+++ b/src/MailGate.sln
@@ -19,6 +19,8 @@ Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Contracts", "MailG
 EndProject
 Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.TemplateChecks", "..\tests\MailGate.TemplateChecks\MailGate.TemplateChecks.csproj", "{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}"
 EndProject
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.SmtpCredentialChecks", "..\tests\MailGate.SmtpCredentialChecks\MailGate.SmtpCredentialChecks.csproj", "{66B75348-CA4D-4F6C-B415-E31C9A46305A}"
+EndProject
 Global
 	GlobalSection(SolutionConfigurationPlatforms) = preSolution
 		Debug|Any CPU = Debug|Any CPU
@@ -113,6 +115,18 @@ Global
 		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|x64.Build.0 = Release|Any CPU
 		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|x86.ActiveCfg = Release|Any CPU
 		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|x86.Build.0 = Release|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Debug|Any CPU.Build.0 = Debug|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Debug|x64.ActiveCfg = Debug|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Debug|x64.Build.0 = Debug|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Debug|x86.ActiveCfg = Debug|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Debug|x86.Build.0 = Debug|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Release|Any CPU.ActiveCfg = Release|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Release|Any CPU.Build.0 = Release|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Release|x64.ActiveCfg = Release|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Release|x64.Build.0 = Release|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Release|x86.ActiveCfg = Release|Any CPU
+		{66B75348-CA4D-4F6C-B415-E31C9A46305A}.Release|x86.Build.0 = Release|Any CPU
 	EndGlobalSection
 	GlobalSection(SolutionProperties) = preSolution
 		HideSolutionNode = FALSE
`````

### `a/src/MailGate.slnx b/src/MailGate.slnx`

`````diff
diff --git a/src/MailGate.slnx b/src/MailGate.slnx
index d63c8c1..ec21ab6 100644
--- a/src/MailGate.slnx
+++ b/src/MailGate.slnx
@@ -13,4 +13,5 @@
     <Project Path="MailGate.Contracts/MailGate.Contracts.csproj" />
   </Folder>
   <Project Path="../tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj" />
+  <Project Path="../tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj" />
 </Solution>
`````

### `a/tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj b/tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj`

`````diff
diff --git a/tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj b/tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj
new file mode 100644
index 0000000..498a57e
--- /dev/null
+++ b/tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj
@@ -0,0 +1,9 @@
+<Project Sdk="Microsoft.NET.Sdk">
+  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
+  <ItemGroup>
+    <FrameworkReference Include="Microsoft.AspNetCore.App" />
+    <Compile Include="../../src/MailGate.Core/Enums/SmtpCredentialSource.cs" Link="SmtpCredentialSource.cs" />
+    <Compile Include="../../src/MailGate.Channels.Smtp/Options/SmtpOptions.cs" Link="SmtpOptions.cs" />
+    <Compile Include="../../src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs" Link="SmtpCredentialResolver.cs" />
+  </ItemGroup>
+</Project>
`````

### `a/tests/MailGate.SmtpCredentialChecks/Program.cs b/tests/MailGate.SmtpCredentialChecks/Program.cs`

`````diff
diff --git a/tests/MailGate.SmtpCredentialChecks/Program.cs b/tests/MailGate.SmtpCredentialChecks/Program.cs
new file mode 100644
index 0000000..ac2e668
--- /dev/null
+++ b/tests/MailGate.SmtpCredentialChecks/Program.cs
@@ -0,0 +1,62 @@
+using System.Text.Json;
+using MailGate.Channels.Smtp.Options;
+using MailGate.Channels.Smtp.Services;
+using MailGate.Core.Enums;
+using Microsoft.Extensions.Options;
+
+var checks = 0;
+void Check(bool ok, string name) { if (!ok) throw new Exception(name); checks++; }
+var directory = Path.Combine(Path.GetTempPath(), "mailgate-credentials-" + Guid.NewGuid());
+Directory.CreateDirectory(directory);
+try
+{
+    var path = Path.Combine(directory, "account.json");
+    var config = new SmtpOptions { CredentialsFilePath = path };
+    var resolver = new SmtpCredentialResolver(new Monitor(config));
+    SmtpOptions Sidecar() => new() { CredentialSource = SmtpCredentialSource.Sidecar, UseTls = true, Username = "old", Password = "must-not-fallback" };
+    async Task Reject(SmtpOptions options, string name)
+    {
+        try { await resolver.ApplyAsync(options); throw new Exception("Accepted: " + name); }
+        catch (InvalidOperationException e)
+        {
+            Check(!e.Message.Contains("private-test-value") && e.InnerException is null, name + ": sanitized error");
+            Check(options.Username is null && options.Password is null, name + ": no fallback");
+        }
+    }
+    var manual = new SmtpOptions { CredentialSource = SmtpCredentialSource.Manual, Username = "old", Password = "old", OAuth2 = new() };
+    await resolver.ApplyAsync(manual);
+    Check(manual.Username is null && manual.Password is null && manual.OAuth2 is null, "Manual clears all auth without reading a file");
+    var legacy = new SmtpOptions { CredentialSource = SmtpCredentialSource.Legacy, Username = "old", Password = "legacy" };
+    await resolver.ApplyAsync(legacy);
+    Check(legacy.Password == "legacy", "Legacy remains compatible");
+    await Reject(Sidecar(), "Missing file");
+    await File.WriteAllTextAsync(path, JsonSerializer.Serialize(new { username = "first", password = "private-test-value" }));
+    var account = Sidecar(); await resolver.ApplyAsync(account);
+    Check(account.Username == "first" && account.Password == "private-test-value", "Read complete account");
+    var replacement = path + ".new";
+    await File.WriteAllTextAsync(replacement, JsonSerializer.Serialize(new { username = "second", password = "rotated" }));
+    File.Move(replacement, path, overwrite: true);
+    account = Sidecar(); await resolver.ApplyAsync(account);
+    Check(account.Username == "second" && account.Password == "rotated", "Atomic file rotation is observed on next call");
+    foreach (var json in new[] { "not json private-test-value", "{}", "[]", "{\"username\":\"a\",\"password\":null}", "{\"username\":1,\"password\":\"private-test-value\"}", "{\"username\":\"a\",\"password\":\"\"}" })
+    {
+        await File.WriteAllTextAsync(path, json); await Reject(Sidecar(), "Invalid account");
+    }
+    await File.WriteAllTextAsync(path, new string('x', 65537)); await Reject(Sidecar(), "Oversized file");
+    var insecure = Sidecar(); insecure.UseTls = false;
+    await Reject(insecure, "TLS required");
+    config.CredentialsFilePath = "relative-account.json"; await Reject(Sidecar(), "Absolute path required");
+    using var cancellation = new CancellationTokenSource(); cancellation.Cancel();
+    config.CredentialsFilePath = path;
+    try { await resolver.ApplyAsync(Sidecar(), cancellation.Token); throw new Exception("Cancellation ignored"); }
+    catch (OperationCanceledException) { checks++; }
+    Console.WriteLine($"Passed {checks} SMTP credential checks.");
+}
+finally { Directory.Delete(directory, recursive: true); }
+
+sealed class Monitor(SmtpOptions value) : IOptionsMonitor<SmtpOptions>
+{
+    public SmtpOptions CurrentValue => value;
+    public SmtpOptions Get(string? name) => value;
+    public IDisposable? OnChange(Action<SmtpOptions, string?> listener) => null;
+}
`````

### `a/tests/ci/test_pipeline_scripts.py b/tests/ci/test_pipeline_scripts.py`

`````diff
diff --git a/tests/ci/test_pipeline_scripts.py b/tests/ci/test_pipeline_scripts.py
new file mode 100644
index 0000000..a49ddcc
--- /dev/null
+++ b/tests/ci/test_pipeline_scripts.py
@@ -0,0 +1,123 @@
+"""Offline regression checks: no registry, SSH server or Docker daemon is used."""
+import os
+from pathlib import Path
+import subprocess
+import tempfile
+import unittest
+
+ROOT = Path(__file__).resolve().parents[2]
+
+
+class PipelineScripts(unittest.TestCase):
+    def setUp(self):
+        self.temp = tempfile.TemporaryDirectory()
+        self.addCleanup(self.temp.cleanup)
+        self.base = Path(self.temp.name)
+        self.bin = self.base / 'bin'
+        self.bin.mkdir()
+        self.log = self.base / 'commands.log'
+        self.dev = self.base / 'dev'
+        self.dev.mkdir()
+        (self.dev / '.env').write_text('RUNTIME_SECRET=must-not-appear-in-logs\n')
+        (self.dev / 'docker-compose.yml').write_text('services: {}\n')
+        self.command('docker', '''printf '%s\\n' "$*" >> "$MOCK_LOG"
+if [[ $1 == login ]]; then cat >/dev/null; fi
+if [[ " $* " == *" run --rm --no-deps worker --migrate-only "* && ${FAIL_MIGRATION:-0} == 1 ]]; then exit 1; fi
+if [[ $1 == build && ${FAIL_BUILD:-0} == 1 ]]; then exit 1; fi
+if [[ " $* " == *" ps -q "* ]]; then printf 'container-%s\\n' "${!#}"; fi
+if [[ $1 == inspect ]]; then echo true; fi
+''')
+        self.command('curl', 'exit "${FAIL_HEALTH:-0}"\n')
+        self.command('sleep', 'exit 0\n')
+        # The test verifies deployment decisions, not OS locking implementation.
+        self.command('flock', 'exit 0\n')
+        self.env = dict(os.environ, PATH=str(self.bin) + os.pathsep + os.environ['PATH'], MOCK_LOG=str(self.log))
+
+    def command(self, name, body):
+        p = self.bin / name
+        p.write_text('#!/usr/bin/env bash\nset -eu\n' + body)
+        p.chmod(0o755)
+
+    def deploy(self, pipeline=20, **extra):
+        release = self.dev / '.mailgate-ci' / 'releases' / str(pipeline)
+        release.mkdir(parents=True, exist_ok=True)
+        (release / 'images.env').write_text('MAILGATE_API_IMAGE=registry/api:test\n')
+        return subprocess.run(['bash', str(ROOT / 'scripts/ci/deploy-dev-server.sh'), str(self.dev),
+                               str(pipeline), 'mailgate', 'a' * 40, '123',
+                               'http://127.0.0.1:8080/healthz/ready', 'http://127.0.0.1:8091/'],
+                              env=dict(self.env, **extra), text=True, capture_output=True)
+
+    def commands(self):
+        return self.log.read_text() if self.log.exists() else ''
+
+    def test_success_migrates_before_up_and_preserves_runtime_files(self):
+        original = (self.dev / '.env').read_text()
+        result = self.deploy()
+        self.assertEqual(result.returncode, 0, result.stderr)
+        commands = self.commands()
+        self.assertLess(commands.index('run --rm --no-deps worker --migrate-only'), commands.index('up -d --no-deps --no-build api worker frontend'))
+        self.assertNotIn(' down', commands)
+        self.assertEqual((self.dev / '.env').read_text(), original)
+        self.assertNotIn('must-not-appear', result.stdout + result.stderr)
+        self.assertEqual((self.dev / '.mailgate-ci/latest-successful').read_text().strip(), '20')
+
+    def test_older_pipeline_cannot_replace_newer_deployment(self):
+        self.assertEqual(self.deploy(30).returncode, 0)
+        self.log.write_text('')
+        result = self.deploy(20)
+        self.assertEqual(result.returncode, 0, result.stderr)
+        self.assertIn('Skip outdated', result.stdout)
+        self.assertNotIn(' pull ', self.commands())
+        self.assertNotIn(' up ', self.commands())
+
+    def test_failed_migration_does_not_replace_containers(self):
+        result = self.deploy(20, FAIL_MIGRATION='1')
+        self.assertNotEqual(result.returncode, 0)
+        self.assertNotIn(' up ', self.commands())
+        self.assertFalse((self.dev / '.mailgate-ci/latest-successful').exists())
+        self.log.write_text('')
+        self.assertEqual(self.deploy(19).returncode, 0)
+        self.assertNotIn(' up ', self.commands())
+
+    def test_retry_same_pipeline_is_allowed(self):
+        self.assertNotEqual(self.deploy(20, FAIL_MIGRATION='1').returncode, 0)
+        self.assertEqual(self.deploy(20).returncode, 0)
+
+    def test_failed_health_does_not_mark_deployment_successful(self):
+        result = self.deploy(20, FAIL_HEALTH='1')
+        self.assertNotEqual(result.returncode, 0)
+        self.assertIn('Health check failed', result.stderr)
+        self.assertFalse((self.dev / '.mailgate-ci/latest-successful').exists())
+
+    def test_rejects_shared_directory_from_another_gitlab_project(self):
+        state = self.dev / '.mailgate-ci'
+        state.mkdir()
+        (state / 'gitlab-project').write_text('456\n')
+        result = self.deploy()
+        self.assertNotEqual(result.returncode, 0)
+        self.assertNotIn(' pull ', self.commands())
+
+    def build(self, **extra):
+        return subprocess.run(['bash', str(ROOT / 'scripts/ci/build-images.sh')], cwd=self.base,
+                              env=dict(self.env, CI_REGISTRY='registry', CI_REGISTRY_IMAGE='registry/project',
+                                       CI_REGISTRY_USER='ci-user', CI_REGISTRY_PASSWORD='secret-token',
+                                       CI_COMMIT_SHA='b' * 40, CI_PIPELINE_ID='42', **extra), text=True, capture_output=True)
+
+    def test_build_uses_exact_pipeline_tag_and_no_secret_in_manifest(self):
+        result = self.build()
+        self.assertEqual(result.returncode, 0, result.stderr)
+        manifest = (self.base / 'images.env').read_text()
+        self.assertEqual(len(manifest.splitlines()), 3)
+        self.assertEqual(manifest.count('b' * 40 + '-pipeline42'), 3)
+        self.assertNotIn('secret-token', manifest + self.commands() + result.stdout + result.stderr)
+        self.assertEqual(sum(line.startswith('push ') for line in self.commands().splitlines()), 3)
+
+    def test_failed_build_does_not_push_or_create_manifest(self):
+        result = self.build(FAIL_BUILD='1')
+        self.assertNotEqual(result.returncode, 0)
+        self.assertNotIn('push ', self.commands())
+        self.assertFalse((self.base / 'images.env').exists())
+
+
+if __name__ == '__main__':
+    unittest.main()
`````


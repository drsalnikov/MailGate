# Полные диффы MailGate — 23.11.2026

База Git: `3536209b1c154c7b449c6348d1cc0d71fa9ff9f6`. Инструкция: [closed-network-transfer.md](../../config/closed-network-transfer.md). Не применяйте оба patch подряд. Из набора исключены генерируемые artifacts и сама инструкция, чтобы избежать рекурсии. Переводы строк в Markdown нормализованы для чтения; `.patch` сохранён как выдал Git.

## Файлы

```text
A	.dockerignore
M	.env.example
M	.gitignore
A	.gitlab-ci.yml
D	MailGate.slnx
M	README.md
A	config/build-secret.empty
A	config/compose.md
A	config/email-templates.md
A	config/gitlab-ci.md
M	config/keycloak/README.md
A	config/keycloak/mailgate-local-realm.json
A	config/npmrc.public
A	config/operations.md
A	config/smtp-account.json.example
A	config/smtp-sidecar.md
A	crt/README.md
M	docker-compose.yml
D	frontend/src/pages/NotificationCreatePage.tsx
D	frontend/tsconfig.tsbuildinfo
A	scripts/ci/build-images.sh
A	scripts/ci/check-smtp-credentials.sh
A	scripts/ci/check-templates.sh
A	scripts/ci/deploy-dev-server.sh
A	scripts/ci/deploy-dev.sh
M	scripts/clean-for-transfer.sh
A	src/.dockerignore
A	src/Dockerfile.local
A	src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs
M	src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs
M	src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs
M	src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs
M	src/MailGate.Api/Admin/Validators/SmtpValidators.cs
A	src/MailGate.Api/Auth/KeycloakGroupAccess.cs
D	src/MailGate.Api/Auth/KeycloakRoleClaimsTransformation.cs
M	src/MailGate.Api/Dockerfile
D	src/MailGate.Api/MailGate.Api.http
M	src/MailGate.Api/Program.cs
A	src/MailGate.Api/Templates/EmailTemplateRenderer.cs
A	src/MailGate.Api/Templates/NotificationComposer.cs
M	src/MailGate.Api/appsettings.json
M	src/MailGate.Channels.Smtp/Extensions/ServiceCollectionExtensions.cs
M	src/MailGate.Channels.Smtp/Options/SmtpOptions.cs
M	src/MailGate.Channels.Smtp/Services/OptionsSmtpSettingsProvider.cs
A	src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs
M	src/MailGate.Channels.Smtp/SmtpChannel.cs
M	src/MailGate.Contracts/Notifications/NotificationLogDto.cs
M	src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs
A	src/MailGate.Contracts/Templates/EmailTemplateDto.cs
A	src/MailGate.Core/Entities/EmailTemplate.cs
M	src/MailGate.Core/Entities/SmtpConfiguration.cs
A	src/MailGate.Core/Enums/SmtpCredentialSource.cs
A	src/MailGate.Core/Models/EmbeddedEmailImage.cs
A	src/MailGate.Frontend/.dockerignore
A	src/MailGate.Frontend/.npmrc.example
R071	frontend/Dockerfile	src/MailGate.Frontend/Dockerfile
R088	frontend/docker-entrypoint.sh	src/MailGate.Frontend/docker-entrypoint.sh
R100	frontend/index.html	src/MailGate.Frontend/index.html
R100	frontend/nginx.conf	src/MailGate.Frontend/nginx.conf
R100	frontend/package-lock.json	src/MailGate.Frontend/package-lock.json
R100	frontend/package.json	src/MailGate.Frontend/package.json
R083	frontend/public/config.json	src/MailGate.Frontend/public/config.json
R088	frontend/src/App.tsx	src/MailGate.Frontend/src/App.tsx
R100	frontend/src/api/client.ts	src/MailGate.Frontend/src/api/client.ts
R077	frontend/src/api/endpoints.ts	src/MailGate.Frontend/src/api/endpoints.ts
R073	frontend/src/api/types.ts	src/MailGate.Frontend/src/api/types.ts
R077	frontend/src/auth/RequireAuth.tsx	src/MailGate.Frontend/src/auth/RequireAuth.tsx
R064	frontend/src/auth/oidc.ts	src/MailGate.Frontend/src/auth/oidc.ts
A	src/MailGate.Frontend/src/auth/returnPath.ts
A	src/MailGate.Frontend/src/components/EmailPreview.tsx
A	src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx
R096	frontend/src/config.ts	src/MailGate.Frontend/src/config.ts
A	src/MailGate.Frontend/src/hooks/useDebounced.ts
R096	frontend/src/layout/AppLayout.tsx	src/MailGate.Frontend/src/layout/AppLayout.tsx
R100	frontend/src/main.tsx	src/MailGate.Frontend/src/main.tsx
R100	frontend/src/pages/DashboardPage.tsx	src/MailGate.Frontend/src/pages/DashboardPage.tsx
R081	frontend/src/pages/DebugClaimsPage.tsx	src/MailGate.Frontend/src/pages/DebugClaimsPage.tsx
A	src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx
A	src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx
R100	frontend/src/pages/NotFoundPage.tsx	src/MailGate.Frontend/src/pages/NotFoundPage.tsx
A	src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx
R100	frontend/src/pages/NotificationsPage.tsx	src/MailGate.Frontend/src/pages/NotificationsPage.tsx
R100	frontend/src/pages/RecipientGroupEditPage.tsx	src/MailGate.Frontend/src/pages/RecipientGroupEditPage.tsx
R100	frontend/src/pages/RecipientGroupsPage.tsx	src/MailGate.Frontend/src/pages/RecipientGroupsPage.tsx
R100	frontend/src/pages/RecipientsPage.tsx	src/MailGate.Frontend/src/pages/RecipientsPage.tsx
R063	frontend/src/pages/SmtpSettingsPage.tsx	src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx
R100	frontend/src/pages/SystemsPage.tsx	src/MailGate.Frontend/src/pages/SystemsPage.tsx
R100	frontend/src/theme.tsx	src/MailGate.Frontend/src/theme.tsx
R100	frontend/tsconfig.json	src/MailGate.Frontend/tsconfig.json
R100	frontend/vite.config.ts	src/MailGate.Frontend/vite.config.ts
M	src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs
M	src/MailGate.Infrastructure/MailGate.Infrastructure.csproj
M	src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs
M	src/MailGate.Infrastructure/Persistence/MailGateDbContextFactory.cs
A	src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.Designer.cs
A	src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.cs
A	src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs
A	src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs
A	src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.Designer.cs
A	src/MailGate.Infrastructure/Persistence/Migrations/20261123090000_AddSmtpCredentialSource.cs
M	src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
M	src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs
M	src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs
M	src/MailGate.Worker/Dockerfile
M	src/MailGate.Worker/Program.cs
M	src/MailGate.Worker/appsettings.json
R066	MailGate.sln	src/MailGate.sln
A	src/MailGate.slnx
A	src/NuGet.Config
D	tests/MailGate.Api.Tests/MailGate.Api.Tests.csproj
D	tests/MailGate.Channels.Smtp.Tests/MailGate.Channels.Smtp.Tests.csproj
A	tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj
A	tests/MailGate.SmtpCredentialChecks/Program.cs
A	tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj
A	tests/MailGate.TemplateChecks/Program.cs
D	tests/MailGate.Worker.Tests/MailGate.Worker.Tests.csproj
A	tests/ci/test_pipeline_scripts.py
```

## Статистика

```text
 .dockerignore                                      |  20 +
 .env.example                                       |  37 +-
 .gitignore                                         |  20 +-
 .gitlab-ci.yml                                     |  70 +++
 MailGate.slnx                                      |   2 -
 README.md                                          |  80 +++-
 config/build-secret.empty                          |   0
 config/compose.md                                  |  92 ++++
 config/email-templates.md                          |  66 +++
 config/gitlab-ci.md                                | 224 +++++++++
 config/keycloak/README.md                          | 408 +---------------
 config/keycloak/mailgate-local-realm.json          | 144 ++++++
 config/npmrc.public                                |   1 +
 config/operations.md                               |  72 +++
 config/smtp-account.json.example                   |   4 +
 config/smtp-sidecar.md                             | 206 ++++++++
 crt/README.md                                      |   5 +
 docker-compose.yml                                 |  95 +++-
 frontend/src/pages/NotificationCreatePage.tsx      | 172 -------
 frontend/tsconfig.tsbuildinfo                      |   1 -
 scripts/ci/build-images.sh                         |  43 ++
 scripts/ci/check-smtp-credentials.sh               |  14 +
 scripts/ci/check-templates.sh                      |  21 +
 scripts/ci/deploy-dev-server.sh                    |  66 +++
 scripts/ci/deploy-dev.sh                           |  43 ++
 scripts/clean-for-transfer.sh                      |   3 +-
 src/.dockerignore                                  |  12 +
 src/Dockerfile.local                               |  24 +
 .../Admin/Endpoints/EmailTemplatesEndpoints.cs     |  81 ++++
 .../Admin/Endpoints/NotificationsAdminEndpoints.cs |  29 +-
 .../Admin/Endpoints/SmtpAdminEndpoints.cs          |  54 +--
 .../SendNotificationFromAdminValidator.cs          |   4 +
 .../Admin/Validators/SmtpValidators.cs             |  10 +
 src/MailGate.Api/Auth/KeycloakGroupAccess.cs       |  37 ++
 .../Auth/KeycloakRoleClaimsTransformation.cs       | 128 -----
 src/MailGate.Api/Dockerfile                        |  31 +-
 src/MailGate.Api/MailGate.Api.http                 |   6 -
 src/MailGate.Api/Program.cs                        |  17 +-
 .../Templates/EmailTemplateRenderer.cs             |  92 ++++
 src/MailGate.Api/Templates/NotificationComposer.cs |  39 ++
 src/MailGate.Api/appsettings.json                  |   7 +-
 .../Extensions/ServiceCollectionExtensions.cs      |   1 +
 src/MailGate.Channels.Smtp/Options/SmtpOptions.cs  |   8 +
 .../Services/OptionsSmtpSettingsProvider.cs        |  26 +-
 .../Services/SmtpCredentialResolver.cs             |  58 +++
 src/MailGate.Channels.Smtp/SmtpChannel.cs          |  34 +-
 .../Notifications/NotificationLogDto.cs            |   3 +
 .../Smtp/SmtpConfigurationDto.cs                   |   8 +-
 .../Templates/EmailTemplateDto.cs                  |  45 ++
 src/MailGate.Core/Entities/EmailTemplate.cs        |  11 +
 src/MailGate.Core/Entities/SmtpConfiguration.cs    |   4 +
 src/MailGate.Core/Enums/SmtpCredentialSource.cs    |   9 +
 src/MailGate.Core/Models/EmbeddedEmailImage.cs     |  35 ++
 src/MailGate.Frontend/.dockerignore                |   9 +
 src/MailGate.Frontend/.npmrc.example               |   2 +
 {frontend => src/MailGate.Frontend}/Dockerfile     |   5 +-
 .../MailGate.Frontend}/docker-entrypoint.sh        |   2 +-
 {frontend => src/MailGate.Frontend}/index.html     |   0
 {frontend => src/MailGate.Frontend}/nginx.conf     |   0
 .../MailGate.Frontend}/package-lock.json           |   0
 {frontend => src/MailGate.Frontend}/package.json   |   0
 .../MailGate.Frontend}/public/config.json          |   2 +-
 {frontend => src/MailGate.Frontend}/src/App.tsx    |   5 +
 .../MailGate.Frontend}/src/api/client.ts           |   0
 .../MailGate.Frontend}/src/api/endpoints.ts        |  11 +
 .../MailGate.Frontend}/src/api/types.ts            |  39 ++
 .../MailGate.Frontend}/src/auth/RequireAuth.tsx    |  10 +-
 .../MailGate.Frontend}/src/auth/oidc.ts            |  28 +-
 src/MailGate.Frontend/src/auth/returnPath.ts       |   9 +
 .../src/components/EmailPreview.tsx                |  29 ++
 .../src/components/EmailTemplateEditor.tsx         |  75 +++
 {frontend => src/MailGate.Frontend}/src/config.ts  |   2 +-
 src/MailGate.Frontend/src/hooks/useDebounced.ts    |  10 +
 .../MailGate.Frontend}/src/layout/AppLayout.tsx    |   2 +
 {frontend => src/MailGate.Frontend}/src/main.tsx   |   0
 .../MailGate.Frontend}/src/pages/DashboardPage.tsx |   0
 .../src/pages/DebugClaimsPage.tsx                  |   6 +-
 .../src/pages/EmailTemplateEditPage.tsx            |  65 +++
 .../src/pages/EmailTemplatesPage.tsx               |  35 ++
 .../MailGate.Frontend}/src/pages/NotFoundPage.tsx  |   0
 .../src/pages/NotificationCreatePage.tsx           | 227 +++++++++
 .../src/pages/NotificationsPage.tsx                |   0
 .../src/pages/RecipientGroupEditPage.tsx           |   0
 .../src/pages/RecipientGroupsPage.tsx              |   0
 .../src/pages/RecipientsPage.tsx                   |   0
 .../src/pages/SmtpSettingsPage.tsx                 | 137 ++----
 .../MailGate.Frontend}/src/pages/SystemsPage.tsx   |   0
 {frontend => src/MailGate.Frontend}/src/theme.tsx  |   0
 {frontend => src/MailGate.Frontend}/tsconfig.json  |   0
 {frontend => src/MailGate.Frontend}/vite.config.ts |   0
 .../Extensions/ServiceCollectionExtensions.cs      |  13 +-
 .../MailGate.Infrastructure.csproj                 |   5 +-
 .../Persistence/MailGateDbContext.cs               |  13 +
 .../Persistence/MailGateDbContextFactory.cs        |   3 +-
 ...0_AddSmtpCertificateRevocationCheck.Designer.cs | 490 +++++++++++++++++++
 ...1026090000_AddSmtpCertificateRevocationCheck.cs |  25 +
 .../20261028090000_AddEmailTemplates.Designer.cs   | 521 ++++++++++++++++++++
 .../Migrations/20261028090000_AddEmailTemplates.cs |  28 ++
 ...61123090000_AddSmtpCredentialSource.Designer.cs | 524 +++++++++++++++++++++
 .../20261123090000_AddSmtpCredentialSource.cs      |  17 +
 .../Migrations/MailGateDbContextModelSnapshot.cs   |  41 +-
 .../Services/DbSmtpSettingsProvider.cs             |  29 +-
 .../Services/SmtpConfigurationStore.cs             |  45 +-
 src/MailGate.Worker/Dockerfile                     |  29 +-
 src/MailGate.Worker/Program.cs                     |   8 +-
 src/MailGate.Worker/appsettings.json               |   5 +
 MailGate.sln => src/MailGate.sln                   |  83 ++--
 src/MailGate.slnx                                  |  17 +
 src/NuGet.Config                                   |   7 +
 tests/MailGate.Api.Tests/MailGate.Api.Tests.csproj |  28 --
 .../MailGate.Channels.Smtp.Tests.csproj            |  28 --
 .../MailGate.SmtpCredentialChecks.csproj           |   9 +
 tests/MailGate.SmtpCredentialChecks/Program.cs     |  62 +++
 .../MailGate.TemplateChecks.csproj                 |  14 +
 tests/MailGate.TemplateChecks/Program.cs           |  80 ++++
 .../MailGate.Worker.Tests.csproj                   |  27 --
 tests/ci/test_pipeline_scripts.py                  | 123 +++++
 117 files changed, 4612 insertions(+), 1094 deletions(-)
```

## Диффы

### `a/.dockerignore b/.dockerignore`

`````diff
diff --git a/.dockerignore b/.dockerignore
new file mode 100644
index 0000000..c2da045
--- /dev/null
+++ b/.dockerignore
@@ -0,0 +1,20 @@
+.git/
+.mailgate-ci/
+images.env
+**/__pycache__/
+.env
+.env.*
+**/.npmrc
+**/bin/
+**/obj/
+**/node_modules/
+**/dist/
+**/publish/
+**/TestResults/
+**/*.pfx
+**/*.key
+**/*.tsbuildinfo
+**/.DS_Store
+
+**/account.json
+secrets/
`````

### `a/.env.example b/.env.example`

`````diff
diff --git a/.env.example b/.env.example
index 647d8bc..2036146 100644
--- a/.env.example
+++ b/.env.example
@@ -16,14 +16,16 @@ RABBITMQ_MANAGEMENT_PORT=15672
 
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
-KEYCLOAK_ADMIN_ROLE=mailgate-admin
+# Точный путь существующей группы из groups access token; пустое значение запрещает доступ.
+# Например: /OtherApp/Administrators
+KEYCLOAK_ADMIN_GROUP=/MailGateAdmins
 # true — требовать HTTPS у discovery-документа Keycloak (включить в продакшене)
 KEYCLOAK_REQUIRE_HTTPS_METADATA=false
 
@@ -72,3 +74,28 @@ API_BASE_URL=http://localhost:8080
 # CORS при same-origin не задействуется, origin'ы можно оставить дефолтными
 # В Keycloak realm добавьте redirect URI https://mailgate.example.local/*
 # и web origin https://mailgate.example.local (см. config/keycloak/README.md)
+
+# RabbitMQ: connection settings (published port is configured separately)
+RABBITMQ_HOST=rabbitmq
+RABBITMQ_CONNECTION_PORT=5672
+RABBITMQ_VHOST=/
+
+# Docker build secrets: public/empty defaults; set file paths for corporate registries.
+# NPMRC_FILE=./src/MailGate.Frontend/.npmrc
+# NPM_TOKEN_FILE=./secrets/npm-token
+# NUGET_CONFIG_FILE=./src/NuGet.Config
+# NPM_TOKEN is used by CI docker build, while Compose uses NPM_TOKEN_FILE.
+NPM_TOKEN=
+
+# SMTP effective settings cache lifetime in each process
+SMTP_SETTINGS_CACHE_SECONDS=30
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
index 60d7a24..602aa39 100644
--- a/.gitignore
+++ b/.gitignore
@@ -4,8 +4,9 @@ obj/
 out/
 
 # Frontend
+*.tsbuildinfo
 node_modules/
-frontend/dist/
+src/MailGate.Frontend/dist/
 
 # .NET
 *.user
@@ -48,7 +49,6 @@ appsettings.*.json
 !appsettings.Development.json
 
 # Docker
-.dockerignore
 
 # OS
 .DS_Store
@@ -59,3 +59,19 @@ TestResults/
 coverage/
 *.trx
 *.coveragexml
+
+# Local build credentials and private keys
+.npmrc
+.env.*
+!.env.example
+*.pfx
+*.key
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

### `a/MailGate.slnx b/MailGate.slnx`

`````diff
diff --git a/MailGate.slnx b/MailGate.slnx
deleted file mode 100644
index ba788ff..0000000
--- a/MailGate.slnx
+++ /dev/null
@@ -1,2 +0,0 @@
-<Solution>
-</Solution>
`````

### `a/README.md b/README.md`

`````diff
diff --git a/README.md b/README.md
index f93f2c6..1aaa9d5 100644
--- a/README.md
+++ b/README.md
@@ -1,5 +1,20 @@
 # MailGate — Notification Microservice
 
+Полный перенос в закрытый контур: [пошаговая инструкция и ссылки на полные/дополнительные диффы](config/closed-network-transfer.md).
+
+Настройка SMTP из sidecar-файла, тестовый ручной режим и локальная работа без Vault: [config/smtp-sidecar.md](config/smtp-sidecar.md).
+
+## Доступ по группе Keycloak
+
+MailGate проверяет точное совпадение пути группы в `groups` access token. Роль администратора больше не используется для предоставления доступа. Пустая настройка группы запрещает доступ всем пользователям.
+
+В новом клиенте настройте mapper **Group Membership**: Token Claim Name — `groups`, Full group path — On, Add to access token — On. Добавьте Audience mapper с audience `mailgate-api` в access token.
+
+В корневом `.env` задайте `KEYCLOAK_ADMIN_GROUP=/OtherApp/Administrators` (замените на фактический путь), `KEYCLOAK_AUTHORITY`, `KEYCLOAK_CLIENT_ID` и `KEYCLOAK_AUDIENCE`. Значение группы должно точно совпадать с токеном, включая регистр. Для локального запуска API настройка называется `Authentication:Keycloak:AdminGroup`, а фронтенда — `keycloak.adminGroup` в `public/config.json`.
+
+Пересоберите: `docker compose up -d --build api frontend`, затем выйдите и войдите снова. Проверьте доступ участником группы и пользователем без неё: последнему защищённый API должен возвращать 403.
+
+
 Микросервис на **.NET 10**, который принимает запросы на рассылку уведомлений от сторонних систем через REST API и доставляет их через подключаемые провайдеры каналов связи (SMTP/Exchange, мессенджеры и др.).
 
 ## Архитектура
@@ -29,7 +44,7 @@
                                               ┌─────────────┼────────────────┐
                                               │             │                │
                                      ┌────────▼───┐  ┌──────▼─────┐  ┌──────▼─────┐
-                                     │SmtpChannel │  │ IvaConnect │  │   Other    │
+                                     │SmtpChannel │  │ Мессенджер │  │   Other    │
                                      │ (MailKit)  │  │  (future)  │  │  (future)  │
                                      └────────────┘  └────────────┘  └────────────┘
 ```
@@ -38,7 +53,12 @@
 
 ```
 MailGate/
+├── .dockerignore                # Исключения контекста API/Worker
+├── crt/                         # Публичные корпоративные УЦ
 ├── src/
+│   ├── MailGate.sln              # Решение .NET
+│   ├── MailGate.slnx             # То же решение в XML-формате
+│   ├── NuGet.Config             # Источники NuGet для сборки
 │   ├── MailGate.Api/             # REST API, админ-эндпоинты
 │   │   └── Dockerfile
 │   ├── MailGate.Worker/          # MassTransit Consumer, ChannelRouter
@@ -46,22 +66,52 @@ MailGate/
 │   ├── MailGate.Contracts/       # DTO админ-эндпоинтов API
 │   ├── MailGate.Core/            # Доменные модели, сущности, интерфейсы
 │   ├── MailGate.Channels.Smtp/   # SMTP/Exchange провайдер (MailKit + OAuth2)
-│   └── MailGate.Infrastructure/  # MassTransit, EF Core, PostgreSQL, Data Protection
-├── frontend/                     # React + Ant Design админка (Vite, TypeScript)
-│   └── Dockerfile                # node build → nginx статика
-├── tests/
-│   ├── MailGate.Api.Tests/
-│   ├── MailGate.Worker.Tests/
-│   └── MailGate.Channels.Smtp.Tests/
-├── docker-compose.yml            # Полный стек: API + Worker + RabbitMQ + PostgreSQL + MailHog
-├── .dockerignore
-└── MailGate.sln
+│   ├── MailGate.Infrastructure/  # MassTransit, EF Core, PostgreSQL, Data Protection
+│   └── MailGate.Frontend/        # React + Ant Design админка (Vite, TypeScript)
+│       └── Dockerfile            # node build → nginx статика
+└── docker-compose.yml            # Полный стек: API + Worker + RabbitMQ + PostgreSQL + MailHog
 ```
 
 ---
 
+Практические настройки SMTP, key ring, сертификатов, npm, миграций и pipeline: [config/operations.md](config/operations.md).
+
+## Шаблоны писем
+
+В меню «Рассылка → Шаблоны писем» доступен блочный конструктор. На `/notifications/new` можно выбрать шаблон, ввести основной текст и увидеть серверный предпросмотр. [Настройка, API и развёртывание](config/email-templates.md).
+
+## GitLab CI/CD
+
+В `.gitlab-ci.yml` подготовлены проверки, сборка трёх образов и автоматический deploy ветки `develop` на dev через SSH/Docker Compose. Требуется Linux Shell Runner с тегом `mailgate`. До запуска заполните GitLab Variables и проверьте серверную конфигурацию: [пошаговая настройка и объяснение YAML](config/gitlab-ci.md).
+
 ## Быстрый старт
 
+### Локальный Docker Desktop
+
+Из корня репозитория запустите вариант на официальных .NET-образах:
+
+```bash
+docker compose up -d --build
+docker compose ps
+```
+
+Frontend: http://localhost:8091, API: http://localhost:8080, MailHog: http://localhost:8025. Локальный Keycloak: http://keycloak.localhost:8181, realm `mailgate`. Тестовый пользователь `mailgate-admin` и его пароль заданы в `config/keycloak/mailgate-local-realm.json`; он входит в `/MailGateAdmins`. Второй тестовый пользователь не имеет административного доступа. Учётные записи и пароли этого стенда предназначены только для разработки.
+
+Единый Compose задаёт локальные значения по умолчанию. Для корпоративной сборки выберите API_DOCKERFILE и WORKER_DOCKERFILE в .env; для SMTP-файла задайте SMTP_CREDENTIALS_DIRECTORY. См. [config/compose.md](config/compose.md). Если порты заняты или `.env` содержит настройки другого окружения, проверьте их перед запуском. Worker применяет миграции при старте. Не удаляйте PostgreSQL и общий volume ключей при обычном обновлении.
+
+Полная инструкция ручного переноса шаблонов, миграций и конструкторов: [artifacts/mailgate-templates-full-instructions.md](artifacts/mailgate-templates-full-instructions.md).
+
+### Сборка .NET
+
+Команды из корня репозитория (.NET SDK 10):
+
+```bash
+dotnet restore src/MailGate.sln --configfile src/NuGet.Config
+dotnet build src/MailGate.sln --no-restore
+```
+
+Для XML-формата решения замените `MailGate.sln` на `MailGate.slnx`. При запуске из `src` указывайте имя решения явно, поскольку в каталоге находятся оба формата. Docker-сборка API и Worker использует контекст корня репозитория и тот же `NuGet.Config`. Для закрытого контура замените источник в этом файле на внутренний NuGet-сервер.
+
 ### Требования
 
 - [Docker Desktop](https://www.docker.com/products/docker-desktop/)
@@ -201,7 +251,7 @@ dotnet run --project src/MailGate.Api --urls "http://localhost:5100"
 
 ## Admin UI (React + Ant Design)
 
-Админка — отдельное SPA (`frontend/`, Vite + React + TypeScript + Ant Design), раздаётся nginx-контейнером на своём origin. В Docker-стеке откройте http://localhost:8091/.
+Админка — отдельное SPA (`src/MailGate.Frontend/`, Vite + React + TypeScript + Ant Design), раздаётся nginx-контейнером на своём origin. В Docker-стеке откройте http://localhost:8091/.
 Доступ защищён SSO через Keycloak OpenID Connect (Authorization Code + PKCE). В локальном Docker-стеке realm `mailgate` импортируется автоматически; тестовый пользователь: `mailgate-admin` / `mailgate`.
 
 | Страница | URL | Назначение |
@@ -217,12 +267,12 @@ dotnet run --project src/MailGate.Api --urls "http://localhost:5100"
 ### Локальная разработка фронтенда
 
 ```bash
-cd frontend
+cd src/MailGate.Frontend
 npm install
 npm run dev   # http://localhost:5173, API берётся из public/config.json
 ```
 
-Runtime-конфигурация фронтенда (URL API, Keycloak) читается из `config.json`: в dev — `frontend/public/config.json`, в Docker генерируется entrypoint-скриптом из переменных окружения `API_BASE_URL`, `KEYCLOAK_AUTHORITY`, `KEYCLOAK_CLIENT_ID`, `KEYCLOAK_ADMIN_ROLE`.
+Runtime-конфигурация фронтенда (URL API, Keycloak) читается из `config.json`: в dev — `src/MailGate.Frontend/public/config.json`, в Docker генерируется entrypoint-скриптом из переменных окружения `API_BASE_URL`, `KEYCLOAK_AUTHORITY`, `KEYCLOAK_CLIENT_ID`, `KEYCLOAK_ADMIN_GROUP`.
 
 Так как фронтенд живёт на отдельном origin, API отдаёт CORS для origin'ов из настройки `Cors:AllowedOrigins` (`appsettings.json` / env `Cors__AllowedOrigins__N`).
 
@@ -391,7 +441,7 @@ API проверяет JWT Bearer по секции `Authentication:Keycloak`:
       "Authority": "http://keycloak.localhost:8180/realms/mailgate",
       "Audience": "mailgate-api",
       "ClientId": "mailgate-admin",
-      "AdminRole": "mailgate-admin",
+      "AdminGroup": "",
       "RequireHttpsMetadata": false
     }
   }
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

### `a/config/email-templates.md b/config/email-templates.md`

`````diff
diff --git a/config/email-templates.md b/config/email-templates.md
new file mode 100644
index 0000000..5c0df51
--- /dev/null
+++ b/config/email-templates.md
@@ -0,0 +1,66 @@
+# Шаблоны писем
+
+На `/notifications/new` после выбора шаблона доступен тот же конструктор блоков и оформления. Правки применяются только к текущему письму и предпросмотру, сохранённый шаблон не меняется. Кнопка «Сбросить изменения макета» возвращает исходный вариант. Смена шаблона сбрасывает правки; при закрытии страницы они теряются.
+
+Preview и отправка принимают необязательный `templateDefinition` вместе с `templateId` и `templateRevision`. API проверяет исходную версию, валидирует макет и формирует HTML-снимок с правками. Worker отправляет этот снимок. Если исходный шаблон изменился или был удалён, обновите выбор перед отправкой. Миграция БД не требуется.
+
+## Работа в интерфейсе
+
+1. В меню «Рассылка → Шаблоны писем» (`/templates`) создайте шаблон.
+2. Начните с одной колонки или макета с двумя колонками. Задайте название, тему по умолчанию, прехедер, цвета и подпись.
+3. Добавляйте блоки: заголовок, текст, кнопка (http/https), разделитель, две колонки, изображение (загрузка файла). Кнопками со стрелками меняйте порядок, дублируйте или удаляйте блоки.
+4. Один обязательный блок «Основное сообщение» обозначает место вставки ключевого текста. Он перемещается, но не дублируется и не удаляется. Текст для проверки макета не сохраняется.
+5. Предпросмотр обновляется с небольшой задержкой. Переключите ширину «Компьютер/Телефон», затем сохраните шаблон.
+6. На `/notifications/new` выберите сохранённый шаблон и введите основной текст. Пустая тема берётся из шаблона; непустая переопределяет её. Текст внутри шаблона — обычный текст, HTML-теги экранируются, переводы строк сохраняются.
+7. Справа отображается результат серверного рендеринга. После проверки выберите получателей и отправьте. Без шаблона доступны прежние форматы, также с предпросмотром.
+
+Предпросмотр — браузерное представление. Outlook и другие клиенты могут отображать HTML иначе; для окончательной проверки отправьте тестовое письмо себе. Скрипты и формы в предпросмотре заблокированы sandbox/CSP, загруженные изображения встроены в письмо; старые внешние изображения разрешены по HTTPS. Шаблоны не поддерживают произвольный HTML/JavaScript и выполнение выражений. Кнопка содержит только явно заданную HTTP(S)-ссылку.
+
+## Хранение и отправка
+
+Сущность `EmailTemplate` хранится в PostgreSQL, таблица `email_templates`: название, JSON-описание, UUID версии, даты создания и изменения. Все административные endpoints защищены политикой `MailGateAdmin`.
+
+API использует один `EmailTemplateRenderer` для draft-preview конструктора, preview рассылки и фактической отправки. В RabbitMQ публикуются уже сформированные тема/HTML (`BodyFormat=Html`); Worker не читает шаблон. Изменение или удаление шаблона не меняет HTML готовых сообщений и повторной отправки из журнала. Внешние картинки остаются ссылками: изменение файла по тому же адресу повлияет на их отображение.
+
+При изменении шаблона версия меняется. Конструктор не позволяет незаметно перезаписать чужое редактирование (409). При отправке устаревшая версия отклоняется: обновите выбранный шаблон и проверьте письмо заново. Несохранённые изменения на странице шаблона не используются страницей рассылки; её собственный конструктор редактирует отдельную копию макета для письма.
+
+API:
+
+- `GET/POST /api/admin/email-templates` — список/создание.
+- `GET/PUT/DELETE /api/admin/email-templates/{id}` — просмотр/изменение/удаление.
+- `POST /api/admin/email-templates/preview` — `{ definition, body, subject? }`, без сохранения и отправки.
+- `POST /api/admin/notifications/preview` — `{ templateId?, templateRevision?, body, subject?, bodyFormat }`.
+- `POST /api/admin/notifications` — прежние поля плюс `templateId` и `templateRevision`; при шаблоне основной текст обрабатывается как plain text, итог — HTML.
+
+Не более 40 блоков, ровно один content-блок, имя до 200 символов, тема/прехедер до 500, подпись до 4000, текст блока до 10000, основной текст до 1 МБ символов. Цвета — #RRGGBB. Ссылки с userinfo или javascript/data схемами запрещены. Названия шаблонов могут повторяться.
+
+## Развёртывание
+
+Добавлена миграция `20261028090000_AddEmailTemplates`, её designer и snapshot. Worker выполняет `Database.MigrateAsync()` при запуске. В управляемом pipeline применяйте миграции до обновления API, с резервной копией БД:
+
+```powershell
+dotnet ef database update --project src/MailGate.Infrastructure --startup-project src/MailGate.Worker
+```
+
+Design-time factory читает `ConnectionStrings__DefaultConnection`. Пересоберите API, Worker (новая миграция) и frontend. Откатывать таблицу с сохранёнными шаблонами следует только с учётом потери этих шаблонов.
+
+## Проверки
+
+```powershell
+dotnet run --project tests/MailGate.TemplateChecks
+npm run build --prefix src/MailGate.Frontend
+```
+
+Проверки renderer не требуют NuGet-пакетов, БД или SMTP. Дополнительно вручную проверьте CRUD после перезапуска, mobile-preview, отсутствие подстановки HTML из основного текста, конфликт двух редакторов, удаление выбранного шаблона, отправку без шаблона и получение HTML-письма через MailHog/корпоративный SMTP.
+
+## Antwort
+
+[Antwort](https://github.com/internations/antwort) — примеры адаптивных email-макетов, а не редактор. Здесь использован подход таблиц представления, inline-стилей и fallback для Outlook. Внешняя библиотека/сервис не подключены, код Antwort не копируется. Конструктор и renderer реализованы в MailGate.
+
+### Изображения
+
+Выберите блок «Изображение» и нажмите «Загрузить с компьютера». Поддерживаются PNG, JPEG и GIF до 128 КБ каждый, общий размер картинок в шаблоне — до 512 КБ. Затем сохраните шаблон. Альтернативное описание необязательно, оно помогает, если клиент не показывает картинку. Блок перемещается в начало (шапка) или конец (подвал).
+
+Байты файла хранятся в PostgreSQL внутри JSON шаблона в base64 (примерно на треть больше исходного файла). Миграция БД и отдельный файловый volume не нужны. API проверяет размер, MIME-тип и сигнатуру файла, SVG не поддерживается. Удаление шаблона удаляет его копию картинки; в журнале и очереди остаются снимки уже сформированных писем.
+
+В предпросмотре используется встроенное изображение data URL. Worker преобразует его в MIME inline-вложение с Content-ID, HTML ссылается на `cid:`. Доступ к внешнему серверу для картинки не требуется. Резервное копирование БД сохраняет также изображения. Старые HTTPS-блоки остаются совместимыми, их можно заменить загрузкой файла.
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

### `a/config/keycloak/README.md b/config/keycloak/README.md`

`````diff
diff --git a/config/keycloak/README.md b/config/keycloak/README.md
index 7b2d479..3ce4c28 100644
--- a/config/keycloak/README.md
+++ b/config/keycloak/README.md
@@ -1,404 +1,42 @@
-# Keycloak для MailGate
+# Keycloak: доступ к MailGate по группе
 
-Документ описывает настройки Keycloak для защищённой среды. Локальный файл `mailgate-realm.json` можно использовать как dev-пример, но в production/stage лучше создать realm и clients вручную или через IaC, без dev-пользователей и тестовых паролей.
+MailGate использует существующий realm и существующую группу пользователей. Менять роли или состав группы не требуется. Можно настроить только новый клиент.
 
-## Целевая схема
+## Клиент фронтенда
 
-MailGate использует Keycloak как OpenID Connect provider:
+Создайте OpenID Connect клиент: Client authentication — Off, Standard flow — On, PKCE — S256. Для локального Docker укажите redirect URI `http://localhost:8091/*`, post logout redirect URI `http://localhost:8091/*` и Web origins `http://localhost:8091`. Для сервера используйте фактический origin фронтенда.
 
-- **Admin UI**: React SPA, Authorization Code Flow + PKCE.
-- **MailGate API**: JWT Bearer validation.
-- **Внешние системы**: client credentials flow через отдельный confidential client.
-- **Решение о доступе**: роль `mailgate-admin`.
+В Client scopes → dedicated scope → Mappers добавьте:
 
-MailGate не проверяет пароль пользователя и не работает напрямую с Kerberos/LDAP. Он доверяет только OIDC access token от Keycloak.
+- **Group Membership**: Name — `groups`, Token Claim Name — `groups`, Full group path — On, Add to access token — On. Остальные переключатели можно оставить Off при использовании обычных access tokens.
+- **Audience**: Name — `mailgate-api-audience`, Included Custom Audience — `mailgate-api`, Add to access token — On, Add to ID token — Off. Если клиент API уже существует, вместо Custom Audience можно выбрать его в Included Client Audience.
 
-## Realm
+Group Membership передаёт группы вошедшего пользователя. Название разрешённой группы задаётся в MailGate, а не в mapper.
 
-Создайте realm:
+## Настройка MailGate
 
-- **Realm name**: `mailgate`
-- **Enabled**: `On`
-- **SSL required**:
-  - `External requests` или `All requests` для защищённой среды с HTTPS.
-  - `None` допустимо только для локального dev-стека.
+В корневом `.env` задайте:
 
-Публичный issuer должен быть стабильным и совпадать с настройкой API:
-
-```text
-https://<keycloak-host>/realms/mailgate
-```
-
-Например:
-
-```text
-https://sso.company.local/realms/mailgate
-```
-
-## Realm Role
-
-Создайте realm role:
-
-- **Role name**: `mailgate-admin`
-- **Description**: `Allows access to MailGate Admin UI and admin API.`
-
-Эта роль нужна:
-
-- пользователям, которые заходят в Admin UI;
-- service account внешней системы, если ей нужен доступ к `/api/admin/*`;
-- группам AD/LDAP, если роли выдаются через federation/group mapping.
-
-## Client: MailGate API
-
-Клиент API нужен как audience для access token.
-
-Создайте client:
-
-- **Client ID**: `mailgate-api`
-- **Client type**: `OpenID Connect`
-- **Access type / Capability**: bearer-only/resource server, если доступно в вашей версии Keycloak.
-- **Standard flow**: `Off`
-- **Direct access grants**: `Off`
-- **Service accounts**: `Off`
-
-Если в вашей версии Keycloak нет режима `bearer-only`, создайте обычный confidential client без интерактивных flows и используйте его только как audience.
-
-## Client: Admin UI
-
-Клиент для React SPA (frontend/).
-
-Создайте client:
-
-- **Client ID**: `mailgate-admin`
-- **Client type**: `OpenID Connect`
-- **Client authentication**: `Off`  
-  Это public client, secret в браузере хранить нельзя.
-- **Standard flow**: `On`
-- **Implicit flow**: `Off`
-- **Direct access grants**: `Off` для защищённой среды  
-  В dev можно включать только для ручной проверки.
-- **Service accounts**: `Off`
-- **PKCE**: `S256`
-
-Настройте URLs под реальный адрес фронтенда MailGate (SPA живёт на отдельном origin от API):
-
-```text
-Root URL:              https://<frontend-host>
-Home URL:              https://<frontend-host>/
-Valid redirect URIs:   https://<frontend-host>/*
-                       https://<api-host>/swagger/oauth2-redirect.html
-Valid post logout URIs:https://<frontend-host>/*
-Web origins:           https://<frontend-host>
-                       https://<api-host>
-```
-
-Для локального Docker-стека это:
-
-```text
-http://localhost:8091/*          (frontend в Docker)
-http://localhost:5173/*          (Vite dev server)
-http://localhost:8080/swagger/oauth2-redirect.html
-Web origins: http://localhost:8091, http://localhost:5173, http://localhost:8080
-```
-
-## Mappers для Admin UI
-
-Для client `mailgate-admin` добавьте protocol mappers.
-
-### Audience mapper
-
-Нужен, чтобы access token для UI принимался MailGate API.
-
-- **Mapper type**: Audience
-- **Name**: `mailgate-api-audience`
-- **Included Client Audience**: `mailgate-api`
-- **Add to access token**: `On`
-- **Add to ID token**: `Off`
-
-После этого access token должен содержать:
-
-```json
-{
-  "aud": ["mailgate-api"]
-}
+```dotenv
+KEYCLOAK_AUTHORITY=https://keycloak.example.ru/realms/your-realm
+KEYCLOAK_CLIENT_ID=mailgate-ui
+KEYCLOAK_AUDIENCE=mailgate-api
+KEYCLOAK_ADMIN_GROUP=/OtherApp/Administrators
+KEYCLOAK_REQUIRE_HTTPS_METADATA=true
 ```
 
-Допустим и строковый вариант `"aud": "mailgate-api"` в зависимости от версии Keycloak/настроек.
+Замените значения на свои. `KEYCLOAK_ADMIN_GROUP` должен точно совпадать с элементом массива `groups` в access token, включая регистр и полный путь. Доступ закрыт при пустом значении. Подгруппа с другим полным путём автоматически не разрешается.
 
-### Realm roles mapper
+API проверяет подпись, issuer, audience, срок действия JWT и разрешённую группу. Фронтенд проверяет группу для отображения интерфейса; защита API действует независимо от фронтенда.
 
-Нужен, чтобы роли попадали в токены.
-
-Рекомендуемый mapper:
-
-- **Mapper type**: User Realm Role
-- **Name**: `realm roles`
-- **Multivalued**: `On`
-- **Token Claim Name**: `realm_access.roles`
-- **Claim JSON Type**: `String`
-- **Add to access token**: `On`
-- **Add to ID token**: `On` желательно для UI
-
-Дополнительно можно добавить top-level mapper для UI:
-
-- **Mapper type**: User Realm Role
-- **Name**: `realm roles (top-level)`
-- **Multivalued**: `On`
-- **Token Claim Name**: `roles`
-- **Claim JSON Type**: `String`
-- **Add to access token**: `On`
-- **Add to ID token**: `On`
-
-MailGate API умеет читать роли из:
-
-- `realm_access.roles`
-- `resource_access.<client>.roles`
-- `roles`
-
-Admin UI также умеет подмешивать роли из access token, но корректнее всё равно отдавать роли в ID token или top-level `roles`.
-
-## Пользователи и группы
-
-Назначайте `mailgate-admin` только администраторам MailGate.
-
-Варианты:
-
-- вручную на пользователя: `Users` → пользователь → `Role mapping` → `Assign role` → `mailgate-admin`;
-- через группу Keycloak: создайте группу, назначьте ей role `mailgate-admin`, добавляйте пользователей в группу;
-- через LDAP/AD federation: маппинг AD-группы в Keycloak group/realm role.
-
-Для защищённой среды не создавайте dev-пользователей:
-
-- `mailgate-admin` / `mailgate`
-- `mailgate-user` / `mailgate`
-
-Они нужны только для локального Docker-стека.
-
-## Client: внешняя интеграция
-
-Для сторонних систем создайте отдельный confidential client, например `mailgate-integration`.
-
-Настройки:
-
-- **Client ID**: `mailgate-integration`
-- **Client authentication**: `On`
-- **Standard flow**: `Off`
-- **Direct access grants**: `Off`
-- **Service accounts roles**: `On`
-- **Web origins**: URL MailGate/Swagger UI, например `https://<mailgate-host>`  
-  Это нужно, если Swagger UI сам получает токен через browser request к Keycloak.
-
-После создания:
-
-1. Откройте вкладку **Credentials** и сохраните `Client secret` в защищённом хранилище внешней системы.
-2. Откройте **Service accounts roles**.
-3. Нажмите **Assign role**.
-4. Выберите фильтр **Filter by realm roles**.
-5. Назначьте роль `mailgate-admin`, если интеграции нужен доступ к `/api/admin/*`.
-
-Добавьте mapper audience для `mailgate-integration`:
-
-- **Mapper type**: Audience
-- **Included Client Audience**: `mailgate-api`
-- **Add to access token**: `On`
-
-Получение токена:
+Пересоберите контейнеры:
 
 ```bash
-curl -X POST "https://<keycloak-host>/realms/mailgate/protocol/openid-connect/token" \
-  -H "Content-Type: application/x-www-form-urlencoded" \
-  -d "grant_type=client_credentials" \
-  -d "client_id=mailgate-integration" \
-  -d "client_secret=<client-secret>"
-```
-
-Вызов API:
-
-```bash
-curl -H "Authorization: Bearer <access_token>" \
-  https://<mailgate-host>/api/admin/notifications/stats
-```
-
-## Настройки MailGate API
-
-В `MailGate.Api` должны совпадать issuer, audience и роль:
-
-```json
-{
-  "Authentication": {
-    "Keycloak": {
-      "Authority": "https://<keycloak-host>/realms/mailgate",
-      "Audience": "mailgate-api",
-      "ClientId": "mailgate-admin",
-      "AdminRole": "mailgate-admin",
-      "RequireHttpsMetadata": true
-    }
-  }
-}
+docker compose up -d --build api frontend
 ```
 
-Для Docker/env:
+Выйдите и войдите заново. Проверьте участника разрешённой группы и пользователя без неё: второй должен получить отказ во фронтенде и 403 от защищённых API-эндпоинтов. Диагностика токена в MailGate показывает группы и claims.
 
-```bash
-Authentication__Keycloak__Authority=https://<keycloak-host>/realms/mailgate
-Authentication__Keycloak__Audience=mailgate-api
-Authentication__Keycloak__ClientId=mailgate-admin
-Authentication__Keycloak__AdminRole=mailgate-admin
-Authentication__Keycloak__RequireHttpsMetadata=true
-```
-
-`Authority` должен совпадать с `iss` в токене. Если Keycloak стоит за reverse proxy, настройте hostname/proxy headers так, чтобы discovery document и `iss` возвращали внешний HTTPS URL.
-
-## Настройки Admin UI
-
-React SPA читает публичные OIDC-настройки из `config.json` (в dev — `frontend/public/config.json`, в Docker генерируется из env-переменных):
-
-```json
-{
-  "apiBaseUrl": "https://<api-host>",
-  "keycloak": {
-    "authority": "https://<keycloak-host>/realms/mailgate",
-    "clientId": "mailgate-admin",
-    "adminRole": "mailgate-admin"
-  }
-}
-```
-
-Не храните client secret в Admin UI. Клиент `mailgate-admin` должен быть public.
-
-## Reverse Proxy / HTTPS
-
-Для защищённой среды рекомендуется:
-
-- Keycloak доступен только по HTTPS.
-- MailGate Admin/API доступны только по HTTPS.
-- В Keycloak настроен корректный hostname, например:
-
-```bash
-KC_HOSTNAME=https://sso.company.local
-KC_PROXY_HEADERS=xforwarded
-KC_HTTP_ENABLED=true
-```
-
-Конкретные параметры зависят от способа деплоя Keycloak и reverse proxy. Важно, чтобы:
-
-- `/.well-known/openid-configuration` возвращал внешний HTTPS issuer;
-- `issuer` совпадал с `Authentication:Keycloak:Authority`;
-- redirect URI у `mailgate-admin` совпадал с внешним URL MailGate.
-
-## Kerberos / AD / LDAP
-
-Kerberos/AD подключается на стороне Keycloak, не в MailGate.
-
-Типовой вариант:
-
-1. Настроить User Federation к LDAP/AD.
-2. Настроить Kerberos/SPNEGO flow в Keycloak, если нужен бесшовный вход.
-3. Смаппить AD-группу администраторов MailGate в realm role `mailgate-admin`.
-4. Проверить, что после входа пользователь получает `mailgate-admin` в токене.
-
-MailGate продолжает проверять только OIDC/JWT.
-
-## Проверка токена
-
-Проверьте payload `access_token`. Для admin API нужны:
-
-```json
-{
-  "iss": "https://<keycloak-host>/realms/mailgate",
-  "aud": "mailgate-api",
-  "realm_access": {
-    "roles": ["mailgate-admin"]
-  }
-}
-```
-
-Возможен вариант:
-
-```json
-{
-  "aud": ["account", "mailgate-api"],
-  "roles": ["mailgate-admin"]
-}
-```
-
-Проверка API:
-
-```bash
-curl -i -H "Authorization: Bearer <access_token>" \
-  https://<mailgate-host>/api/admin/notifications/stats
-```
-
-Ожидаемые статусы:
-
-- `200` — токен валиден и роль есть.
-- `401` — токена нет, токен невалиден, неверный issuer/audience, API не может получить metadata/JWKS.
-- `403` — токен валиден, но нет роли `mailgate-admin`.
-
-## Swagger
-
-Swagger UI:
-
-```text
-https://<mailgate-host>/swagger
-```
-
-1. Нажмите **Authorize**.
-2. Выберите **Keycloak OAuth2**.
-3. Swagger перенаправит в Keycloak.
-4. Войдите пользователем с ролью `mailgate-admin`.
-5. После возврата Swagger получит `access_token` через Authorization Code Flow + PKCE.
-6. Выполняйте защищённые `/api/admin/*` endpoints.
-
-Схема **Bearer** оставлена как ручной режим: туда вставляется уже полученный `access_token`, а не `client_secret`.
-
-## Troubleshooting
-
-### 401 от API
-
-Проверьте:
-
-- `Authorization: Bearer <token>` действительно отправляется.
-- `iss` в токене совпадает с `Authentication:Keycloak:Authority`.
-- `aud` содержит `mailgate-api`.
-- `RequireHttpsMetadata=true` используется только с HTTPS Keycloak.
-- API имеет сетевой доступ к discovery endpoint:
-
-```text
-https://<keycloak-host>/realms/mailgate/.well-known/openid-configuration
-```
-
-### 403 от API
-
-Токен принят, но не хватает роли. Проверьте:
-
-- пользователь или service account имеет realm role `mailgate-admin`;
-- role mapper добавляет роли в access token;
-- роль называется точно `mailgate-admin`.
-
-### Admin UI вошёл, но пишет «нет доступа»
-
-Проверьте страницу диагностики:
-
-```text
-https://<mailgate-host>/debug/claims
-```
-
-Должно быть:
-
-```text
-RoleClaimType: roles
-IsInRole("mailgate-admin"): True
-```
-
-Если роли нет, проверьте mappers `realm_access.roles` / `roles` и назначение роли пользователю.
-
-### Keycloak требует HTTPS в dev
-
-В защищённой среде это правильно. Для локального dev можно временно:
-
-```bash
-kcadm.sh update realms/mailgate -s sslRequired=NONE
-```
+Для запуска без Docker настройка API называется `Authentication:Keycloak:AdminGroup` (переменная окружения `Authentication__Keycloak__AdminGroup`). В `src/MailGate.Frontend/public/config.json` укажите `keycloak.adminGroup`, `keycloak.clientId` и `keycloak.authority`.
 
-Не используйте это в production.
+`mailgate-realm.json` — прежний пример realm для локального стенда с ролями. Он не настраивает новый доступ по группе автоматически. Для существующего realm импортировать его не нужно.
`````

### `a/config/keycloak/mailgate-local-realm.json b/config/keycloak/mailgate-local-realm.json`

`````diff
diff --git a/config/keycloak/mailgate-local-realm.json b/config/keycloak/mailgate-local-realm.json
new file mode 100644
index 0000000..07023a3
--- /dev/null
+++ b/config/keycloak/mailgate-local-realm.json
@@ -0,0 +1,144 @@
+{
+  "realm": "mailgate",
+  "enabled": true,
+  "sslRequired": "NONE",
+  "displayName": "MailGate",
+  "roles": {
+    "realm": [
+      {
+        "name": "mailgate-admin",
+        "description": "Allows access to MailGate Admin UI and admin API."
+      }
+    ]
+  },
+  "clients": [
+    {
+      "clientId": "mailgate-api",
+      "name": "MailGate API",
+      "enabled": true,
+      "protocol": "openid-connect",
+      "bearerOnly": true
+    },
+    {
+      "clientId": "mailgate-admin",
+      "name": "MailGate Admin UI",
+      "enabled": true,
+      "publicClient": true,
+      "protocol": "openid-connect",
+      "standardFlowEnabled": true,
+      "implicitFlowEnabled": false,
+      "directAccessGrantsEnabled": true,
+      "serviceAccountsEnabled": false,
+      "redirectUris": [
+        "http://localhost:5173/*",
+        "http://localhost:8091/*",
+        "http://localhost:8080/swagger/oauth2-redirect.html"
+      ],
+      "webOrigins": [
+        "http://localhost:5173",
+        "http://localhost:8091",
+        "http://localhost:8080"
+      ],
+      "attributes": {
+        "pkce.code.challenge.method": "S256",
+        "post.logout.redirect.uris": "http://localhost:5173/*##http://localhost:8091/*"
+      },
+      "protocolMappers": [
+        {
+          "name": "mailgate-api-audience",
+          "protocol": "openid-connect",
+          "protocolMapper": "oidc-audience-mapper",
+          "consentRequired": false,
+          "config": {
+            "included.client.audience": "mailgate-api",
+            "access.token.claim": "true"
+          }
+        },
+        {
+          "name": "realm roles",
+          "protocol": "openid-connect",
+          "protocolMapper": "oidc-usermodel-realm-role-mapper",
+          "consentRequired": false,
+          "config": {
+            "multivalued": "true",
+            "user.attribute": "foo",
+            "claim.name": "realm_access.roles",
+            "jsonType.label": "String",
+            "access.token.claim": "true",
+            "id.token.claim": "true"
+          }
+        },
+        {
+          "name": "realm roles (top-level)",
+          "protocol": "openid-connect",
+          "protocolMapper": "oidc-usermodel-realm-role-mapper",
+          "consentRequired": false,
+          "config": {
+            "multivalued": "true",
+            "user.attribute": "foo",
+            "claim.name": "roles",
+            "jsonType.label": "String",
+            "access.token.claim": "true",
+            "id.token.claim": "true"
+          }
+        },
+        {
+          "name": "groups",
+          "protocol": "openid-connect",
+          "protocolMapper": "oidc-group-membership-mapper",
+          "consentRequired": false,
+          "config": {
+            "full.path": "true",
+            "claim.name": "groups",
+            "access.token.claim": "true",
+            "id.token.claim": "false",
+            "userinfo.token.claim": "false"
+          }
+        }
+      ]
+    }
+  ],
+  "users": [
+    {
+      "username": "mailgate-admin",
+      "enabled": true,
+      "emailVerified": true,
+      "firstName": "MailGate",
+      "lastName": "Administrator",
+      "email": "mailgate-admin@example.local",
+      "credentials": [
+        {
+          "type": "password",
+          "value": "mailgate",
+          "temporary": false
+        }
+      ],
+      "realmRoles": [
+        "mailgate-admin"
+      ],
+      "groups": [
+        "/MailGateAdmins"
+      ]
+    },
+    {
+      "username": "mailgate-user",
+      "enabled": true,
+      "emailVerified": true,
+      "firstName": "MailGate",
+      "lastName": "User",
+      "email": "mailgate-user@example.local",
+      "credentials": [
+        {
+          "type": "password",
+          "value": "mailgate",
+          "temporary": false
+        }
+      ]
+    }
+  ],
+  "groups": [
+    {
+      "name": "MailGateAdmins"
+    }
+  ]
+}
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
new file mode 100644
index 0000000..45f3e27
--- /dev/null
+++ b/config/operations.md
@@ -0,0 +1,72 @@
+# Настройка и проверка MailGate
+
+## SMTP и ключи
+
+API и Worker используют `DataProtection:KeysPath` и одинаковое имя приложения `MailGate`. Compose подключает общий постоянный volume `data_protection_keys`. XML key ring в этом варианте НЕ зашифрован на диске; доступ к Docker, volume и резервным копиям должен быть ограничен. SMTP-пароли и OAuth2 secrets в БД зашифрованы. Открытые пароли не возвращаются браузеру.
+
+Для запуска API и Worker непосредственно из PowerShell можно задать `$env:DataProtection__KeysPath = Join-Path $env:LOCALAPPDATA 'MailGate\keys'` в каждом окне запуска. Это один общий каталог для текущего пользователя Windows.
+
+Для запуска без Compose задайте `DataProtection__KeysPath` для обоих сервисов: один и тот же доступный постоянный каталог. При разных Docker-хостах локальные volumes не являются общими. Ключи не коммитить, не копировать в образ и не удалять между деплоями. Не используйте `down -v`, если данные нужны.
+
+После перехода на новый key ring или новое ApplicationName повторно введите и сохраните SMTP-пароль. Для восстановления старого секрета нужны исходные ключи и исходный discriminator; новый key ring не восстановит потерянный ключ. Страница настроек и сохранение читают метаданные без расшифровки, поэтому старый недоступный пароль можно заменить. Пустое поле при сохранении сохраняет прежний секрет. При отправке ошибка расшифровки явная: попытки анонимной отправки из-за этой ошибки нет.
+
+Проверка подключения использует секрет формы, а если поле пусто — сохранённый секрет. Для нового канала вводите новый пароль явно. Проверка не отправляет письмо и не проверяет право Send As. Реальная отправка выполняется Worker. SMTP-конфигурация кешируется по `Smtp:SettingsCacheSeconds` (по умолчанию 30): обновляется при следующей отправке после истечения TTL, не по таймеру. Сохранение в API не сбрасывает память Worker немедленно.
+
+На порту 25/587 обычно STARTTLS On, SSL Off; на 465 — SSL On, STARTTLS Off, если это подтверждено администраторами SMTP. SSL означает TLS сразу; оба режима одновременно включать не нужно. Чекбокс «Проверять отзыв сертификатов» включён по умолчанию и сохраняется в БД; он влияет на API test и Worker. Отключение разрешает сертификаты без проверки отзыва, но не отменяет проверку цепочки и имени. CRL 403 требует проверки маршрута/прокси или правил корпоративного PKI.
+
+## Миграция и запуск
+
+Новая миграция `AddSmtpCertificateRevocationCheck` добавляет boolean с default true для существующих записей. Сохраните migration, designer и snapshot в Git. Worker применяет миграции при запуске; для управляемого деплоя примените их заранее до обновления API:
+
+```powershell
+$env:ConnectionStrings__DefaultConnection = 'Host=localhost;Database=mailgate;Username=mailgate;Password=<ваш-пароль>'
+dotnet ef database update --project src/MailGate.Infrastructure --startup-project src/MailGate.Worker
+Remove-Item Env:\ConnectionStrings__DefaultConnection
+docker compose up -d --build api worker frontend
+```
+
+Design-time factory читает `ConnectionStrings__DefaultConnection`; параметры не записываются в миграцию. Не публикуйте реальные пароли в командах CI или логах.
+
+Проверка: сохранить пароль → отправить письмо → пересоздать API/Worker без удаления volumes → отправить снова без повторного ввода. Нужны доверенные УЦ и рабочие credentials SMTP. При недоступном ключе страница должна открываться, а отправка возвращать понятную ошибку. После повторного сохранения правильного пароля отправка должна восстановиться.
+
+## Сертификаты
+
+Публичные корпоративные CA в PEM `.crt` положите в корневой `crt/`; приватные ключи туда не добавлять. API/Worker собираются с контекстом корня, сертификаты устанавливаются в build и final, без подавления ошибок. Пустая папка допустима, но корпоративное доверие тогда не настроено. Подробнее: `crt/README.md`.
+
+## Корпоративный npm
+
+Обычный Compose использует публичный npm без обязательного токена. Для Nexus скопируйте `src/MailGate.Frontend/.npmrc.example` в `.npmrc`, замените адрес и scope Nexus, оставив `${NPM_TOKEN}`. Сохраните токен в отдельном игнорируемом файле без BOM и перевода строки. В корневом `.env` задайте пути `NPMRC_FILE` и `NPM_TOKEN_FILE`, как описано в [compose.md](compose.md). Сам токен в YAML не записывается. Запускайте:
+
+```powershell
+docker compose up -d --build frontend
+```
+
+В GitLab сохраните NPM_TOKEN как masked/hidden secret; protected переменная доступна только разрешённым protected pipeline. В существующем Docker build job передайте:
+
+```sh
+docker build --secret id=npmrc,src=src/MailGate.Frontend/.npmrc --secret id=npm_token,env=NPM_TOKEN -t "$CI_REGISTRY_IMAGE/frontend:$CI_COMMIT_SHA" src/MailGate.Frontend
+```
+
+Нужен современный BuildKit с secret env mounts. Реальный .npmrc игнорируется Git и Docker-контекстом; pipeline должен создать его из безопасного шаблона или получить как file secret. Токен не передавать через ARG/ENV, не печатать в лог и не публиковать как artifact. После утечки отозвать и заменить токен, отдельно обработать историю Git и старые артефакты.
+
+В финальном frontend добавлено обновление libxml2 из репозитория Alpine. Устранение CVE-2024-56171 необходимо подтвердить повторным сканированием именно опубликованного образа. Если vendor repo не содержит исправления, нужен обновлённый базовый образ; успешная сборка не доказывает устранение CVE.
+
+## Keycloak, CORS и RabbitMQ
+
+Вход фронтенда настраивается runtime `/config.json` из окружения; appsettings API автоматически не переписывается. .NET environment `Authentication__Keycloak__*` переопределяет JSON. Authority API должен совпадать с iss access token, aud содержать KEYCLOAK_AUDIENCE (mailgate-api), groups — точный KEYCLOAK_ADMIN_GROUP. Audience mapper и Group Membership настраиваются у фактического клиента; после изменения войдите заново. Пустая группа запрещает доступ. Нужные корпоративные адреса не зашиты в образ. SwaggerClientId передаётся из KEYCLOAK_CLIENT_ID.
+
+CORS задаётся CORS_ORIGIN_0/1 с полным origin. Заголовок Access-Control-Allow-Origin должен добавляться одним компонентом, без дублирования в proxy. 502 требует проверки upstream; 401 — JWT, 403 после JWT — права группы.
+
+RABBITMQ_HOST, RABBITMQ_CONNECTION_PORT и RABBITMQ_VHOST задают подключение API/Worker. Не путайте внутренний порт 5672 с Management 15672 и опубликованным RABBITMQ_PORT. API/Worker используют один vhost. На существующем брокере создайте vhost и права отдельно: DEFAULT_* применяются при первоначальной инициализации, не мигрируют пользователей и очереди существующего volume.
+
+## Переход на Vault
+
+Возможны KV (пароль хранится в Vault) или Transit (шифротекст остаётся в PostgreSQL). Нужны HTTPS-доступ, сервисная аутентификация, минимальные политики, обновление токенов, таймауты и аудит без раскрытия секретов. Потребуется реализация провайдера защиты/хранения секретов и миграция старых значений, пока доступны старые ключи. Простой перенос key ring в KV не означает его защиту при выгрузке. После миграции сохраняйте старые ключи для восстановления старых backup до окончания срока их хранения. Интеграция Vault сейчас не реализована.
+
+## Локальный Docker Desktop без RED SOFT registry
+
+```sh
+docker compose up -d --build
+```
+
+Override использует официальные образы .NET и отдельный тестовый Keycloak на `http://keycloak.localhost:8181`, не изменяя существующий Keycloak на 8180. Тестовый realm импортируется из `config/keycloak/mailgate-local-realm.json`, пользователь `mailgate-admin` включён в `/MailGateAdmins`. Пароль тестового пользователя указан в этом файле. Override не предназначен для корпоративного развёртывания; доверенные корпоративные УЦ из RED SOFT Dockerfile в него не устанавливаются.
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

### `a/crt/README.md b/crt/README.md`

`````diff
diff --git a/crt/README.md b/crt/README.md
new file mode 100644
index 0000000..d676c7b
--- /dev/null
+++ b/crt/README.md
@@ -0,0 +1,5 @@
+# Корпоративные удостоверяющие центры
+
+Положите сюда публичные сертификаты доверенных корпоративных УЦ в PEM-формате с расширением `.crt`. Приватные ключи, PFX и пароли сюда не добавлять. Проверяйте источник сертификатов с администраторами УЦ. Сертификат веб-почты не обязательно соответствует SMTP. Наличие УЦ не заменяет доступ к CRL.
+
+API и Worker устанавливают сертификаты в финальные образы. Если УЦ нужен для NuGet, он также устанавливается в стадии build. Пустой каталог допустим.
`````

### `a/docker-compose.yml b/docker-compose.yml`

`````diff
diff --git a/docker-compose.yml b/docker-compose.yml
index b845c0f..ece4f5b 100644
--- a/docker-compose.yml
+++ b/docker-compose.yml
@@ -6,6 +6,7 @@ services:
       - "${RABBITMQ_PORT:-5672}:5672"
       - "${RABBITMQ_MANAGEMENT_PORT:-15672}:15672"
     environment:
+      RABBITMQ_DEFAULT_VHOST: ${RABBITMQ_VHOST:-/}
       RABBITMQ_DEFAULT_USER: ${RABBITMQ_USER:-mailgate}
       RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASSWORD:-mailgate}
     volumes:
@@ -40,35 +41,40 @@ services:
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
-      - ./config/keycloak:/opt/keycloak/data/import:ro
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
+      DataProtection__KeysPath: /var/mailgate/keys
+      Smtp__CredentialsFilePath: /var/mailgate/smtp-credentials/account.json
+      Smtp__CredentialSource: ${SMTP_CREDENTIAL_SOURCE:-Legacy}
+      Smtp__SettingsCacheSeconds: ${SMTP_SETTINGS_CACHE_SECONDS:-30}
       ASPNETCORE_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-Development}
       ASPNETCORE_HTTP_PORTS: "8080"
       # HTTPS выключен по умолчанию; включается, если задать API_HTTPS_ENABLED=true
@@ -77,15 +83,16 @@ services:
       ASPNETCORE_Kestrel__Certificates__Default__Path: ${API_HTTPS_ENABLED:+${KESTREL_CERT_PATH:-/https/mailgate.pfx}}
       ASPNETCORE_Kestrel__Certificates__Default__Password: ${API_HTTPS_ENABLED:+${KESTREL_CERT_PASSWORD:-mailgate_dev}}
       ConnectionStrings__DefaultConnection: "Host=postgres;Database=${POSTGRES_DB:-mailgate};Username=${POSTGRES_USER:-mailgate};Password=${POSTGRES_PASSWORD:-mailgate}"
-      MassTransit__Host: rabbitmq
-      MassTransit__Port: "5672"
-      MassTransit__VirtualHost: "/"
+      MassTransit__Host: ${RABBITMQ_HOST:-rabbitmq}
+      MassTransit__Port: ${RABBITMQ_CONNECTION_PORT:-5672}
+      MassTransit__VirtualHost: ${RABBITMQ_VHOST:-/}
       MassTransit__Username: ${RABBITMQ_USER:-mailgate}
       MassTransit__Password: ${RABBITMQ_PASSWORD:-mailgate}
-      Authentication__Keycloak__Authority: ${KEYCLOAK_AUTHORITY:-http://keycloak.localhost:8180/realms/mailgate}
+      Authentication__Keycloak__Authority: ${KEYCLOAK_AUTHORITY:-http://keycloak.localhost:8181/realms/mailgate}
       Authentication__Keycloak__Audience: ${KEYCLOAK_AUDIENCE:-mailgate-api}
       Authentication__Keycloak__ClientId: ${KEYCLOAK_CLIENT_ID:-mailgate-admin}
-      Authentication__Keycloak__AdminRole: ${KEYCLOAK_ADMIN_ROLE:-mailgate-admin}
+      Authentication__Keycloak__SwaggerClientId: ${KEYCLOAK_CLIENT_ID:-mailgate-admin}
+      Authentication__Keycloak__AdminGroup: ${KEYCLOAK_ADMIN_GROUP-/MailGateAdmins}
       Authentication__Keycloak__RequireHttpsMetadata: ${KEYCLOAK_REQUIRE_HTTPS_METADATA:-false}
       Cors__AllowedOrigins__0: ${CORS_ORIGIN_0:-http://localhost:8091}
       Cors__AllowedOrigins__1: ${CORS_ORIGIN_1:-http://localhost:5173}
@@ -99,43 +106,67 @@ services:
       - "keycloak.localhost:host-gateway"
     volumes:
       - ~/.aspnet/https:/https:ro
+      - data_protection_keys:/var/mailgate/keys
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
-      context: frontend
+      context: src/MailGate.Frontend
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
-      KEYCLOAK_ADMIN_ROLE: ${KEYCLOAK_ADMIN_ROLE:-mailgate-admin}
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
+    volumes:
+      - data_protection_keys:/var/mailgate/keys
+      - type: bind
+        source: ${SMTP_CREDENTIALS_DIRECTORY:-./secrets/smtp}
+        target: /var/mailgate/smtp-credentials
+        read_only: true
     environment:
+      DataProtection__KeysPath: /var/mailgate/keys
+      Smtp__CredentialsFilePath: /var/mailgate/smtp-credentials/account.json
+      Smtp__CredentialSource: ${SMTP_CREDENTIAL_SOURCE:-Legacy}
+      Smtp__SettingsCacheSeconds: ${SMTP_SETTINGS_CACHE_SECONDS:-30}
       DOTNET_ENVIRONMENT: ${ASPNETCORE_ENVIRONMENT:-Development}
       ConnectionStrings__DefaultConnection: "Host=postgres;Database=${POSTGRES_DB:-mailgate};Username=${POSTGRES_USER:-mailgate};Password=${POSTGRES_PASSWORD:-mailgate}"
-      MassTransit__Host: rabbitmq
-      MassTransit__Port: "5672"
-      MassTransit__VirtualHost: "/"
+      MassTransit__Host: ${RABBITMQ_HOST:-rabbitmq}
+      MassTransit__Port: ${RABBITMQ_CONNECTION_PORT:-5672}
+      MassTransit__VirtualHost: ${RABBITMQ_VHOST:-/}
       MassTransit__Username: ${RABBITMQ_USER:-mailgate}
       MassTransit__Password: ${RABBITMQ_PASSWORD:-mailgate}
       Smtp__Host: ${SMTP_HOST:-mailhog}
@@ -155,3 +186,15 @@ services:
 volumes:
   rabbitmq_data:
   postgres_data:
+  data_protection_keys:
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

### `a/frontend/src/pages/NotificationCreatePage.tsx b/frontend/src/pages/NotificationCreatePage.tsx`

`````diff
diff --git a/frontend/src/pages/NotificationCreatePage.tsx b/frontend/src/pages/NotificationCreatePage.tsx
deleted file mode 100644
index 3bf3390..0000000
--- a/frontend/src/pages/NotificationCreatePage.tsx
+++ /dev/null
@@ -1,172 +0,0 @@
-import { App, Button, Card, Form, Input, Radio, Select, Space, Typography } from 'antd';
-import { SendOutlined } from '@ant-design/icons';
-import { useMutation, useQuery } from '@tanstack/react-query';
-import { useNavigate } from 'react-router-dom';
-import { notificationsApi, recipientGroupsApi, recipientsApi } from '../api/endpoints';
-import type { BodyFormat, NotificationPriority, SendNotificationFromAdminDto } from '../api/types';
-
-interface FormValues {
-  recipientIds: string[];
-  recipientGroupIds: string[];
-  adHocEmails: string[];
-  subject?: string;
-  body: string;
-  bodyFormat: BodyFormat;
-  priority: NotificationPriority;
-}
-
-const EMAIL_RE = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
-
-export function NotificationCreatePage() {
-  const { message } = App.useApp();
-  const navigate = useNavigate();
-  const [form] = Form.useForm<FormValues>();
-
-  const recipientsQuery = useQuery({
-    queryKey: ['recipients', 'all'],
-    queryFn: () => recipientsApi.list({ pageSize: 500 }),
-  });
-
-  const groupsQuery = useQuery({
-    queryKey: ['recipient-groups', 'all'],
-    queryFn: () => recipientGroupsApi.list({ pageSize: 500 }),
-  });
-
-  const sendMutation = useMutation({
-    mutationFn: (dto: SendNotificationFromAdminDto) => notificationsApi.send(dto),
-    onSuccess: () => {
-      message.success('Рассылка поставлена в очередь');
-      navigate('/notifications');
-    },
-    onError: (error: Error) => message.error(error.message),
-  });
-
-  const onFinish = (values: FormValues) => {
-    const invalidEmails = (values.adHocEmails ?? []).filter((e) => !EMAIL_RE.test(e.trim()));
-    if (invalidEmails.length > 0) {
-      message.error(`Некорректные адреса: ${invalidEmails.join(', ')}`);
-      return;
-    }
-
-    if (
-      (values.recipientIds?.length ?? 0) === 0 &&
-      (values.recipientGroupIds?.length ?? 0) === 0 &&
-      (values.adHocEmails?.length ?? 0) === 0
-    ) {
-      message.error('Укажите хотя бы одного получателя, группу или адрес');
-      return;
-    }
-
-    sendMutation.mutate({
-      channel: 'Smtp',
-      recipientIds: values.recipientIds ?? [],
-      recipientGroupIds: values.recipientGroupIds ?? [],
-      adHocEmails: (values.adHocEmails ?? []).map((e) => e.trim()),
-      subject: values.subject?.trim() || null,
-      body: values.body,
-      bodyFormat: values.bodyFormat,
-      priority: values.priority,
-    });
-  };
-
-  return (
-    <div style={{ maxWidth: 800 }}>
-      <Typography.Title level={3}>Новая рассылка</Typography.Title>
-      <Card>
-        <Form<FormValues>
-          form={form}
-          layout="vertical"
-          onFinish={onFinish}
-          initialValues={{ bodyFormat: 'PlainText', priority: 'Normal' }}
-        >
-          <Form.Item name="recipientIds" label="Получатели из адресной книги">
-            <Select
-              mode="multiple"
-              allowClear
-              placeholder="Выберите получателей"
-              loading={recipientsQuery.isLoading}
-              optionFilterProp="label"
-              options={(recipientsQuery.data?.items ?? []).map((r) => ({
-                value: r.id,
-                label: `${r.fullName} <${r.email}>`,
-              }))}
-            />
-          </Form.Item>
-
-          <Form.Item name="recipientGroupIds" label="Группы получателей">
-            <Select
-              mode="multiple"
-              allowClear
-              placeholder="Выберите группы"
-              loading={groupsQuery.isLoading}
-              optionFilterProp="label"
-              options={(groupsQuery.data?.items ?? []).map((g) => ({
-                value: g.id,
-                label: `${g.name} (${g.recipientCount})`,
-              }))}
-            />
-          </Form.Item>
-
-          <Form.Item
-            name="adHocEmails"
-            label="Дополнительные адреса"
-            tooltip="Введите email и нажмите Enter"
-          >
-            <Select mode="tags" allowClear placeholder="user@example.com" tokenSeparators={[',', ';', ' ']} open={false} suffixIcon={null} />
-          </Form.Item>
-
-          <Form.Item name="subject" label="Тема">
-            <Input placeholder="Тема письма" maxLength={500} />
-          </Form.Item>
-
-          <Form.Item
-            name="body"
-            label="Текст сообщения"
-            rules={[{ required: true, message: 'Введите текст сообщения' }]}
-          >
-            <Input.TextArea rows={10} placeholder="Текст письма" />
-          </Form.Item>
-
-          <Space size="large" wrap>
-            <Form.Item name="bodyFormat" label="Формат">
-              <Radio.Group
-                options={[
-                  { value: 'PlainText', label: 'Текст' },
-                  { value: 'Html', label: 'HTML' },
-                  { value: 'Markdown', label: 'Markdown' },
-                ]}
-                optionType="button"
-              />
-            </Form.Item>
-
-            <Form.Item name="priority" label="Приоритет">
-              <Select
-                style={{ width: 160 }}
-                options={[
-                  { value: 'Low', label: 'Низкий' },
-                  { value: 'Normal', label: 'Обычный' },
-                  { value: 'High', label: 'Высокий' },
-                  { value: 'Critical', label: 'Критический' },
-                ]}
-              />
-            </Form.Item>
-          </Space>
-
-          <Form.Item>
-            <Space>
-              <Button
-                type="primary"
-                htmlType="submit"
-                icon={<SendOutlined />}
-                loading={sendMutation.isPending}
-              >
-                Отправить
-              </Button>
-              <Button onClick={() => navigate('/notifications')}>Отмена</Button>
-            </Space>
-          </Form.Item>
-        </Form>
-      </Card>
-    </div>
-  );
-}
`````

### `a/frontend/tsconfig.tsbuildinfo b/frontend/tsconfig.tsbuildinfo`

`````diff
diff --git a/frontend/tsconfig.tsbuildinfo b/frontend/tsconfig.tsbuildinfo
deleted file mode 100644
index 6be365a..0000000
--- a/frontend/tsconfig.tsbuildinfo
+++ /dev/null
@@ -1 +0,0 @@
-{"root":["./src/app.tsx","./src/config.ts","./src/main.tsx","./src/theme.tsx","./src/api/client.ts","./src/api/endpoints.ts","./src/api/types.ts","./src/auth/requireauth.tsx","./src/auth/oidc.ts","./src/layout/applayout.tsx","./src/pages/dashboardpage.tsx","./src/pages/debugclaimspage.tsx","./src/pages/notfoundpage.tsx","./src/pages/notificationcreatepage.tsx","./src/pages/notificationspage.tsx","./src/pages/recipientgroupeditpage.tsx","./src/pages/recipientgroupspage.tsx","./src/pages/recipientspage.tsx","./src/pages/smtpsettingspage.tsx","./src/pages/systemspage.tsx"],"version":"5.9.3"}
\ No newline at end of file
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

### `a/scripts/clean-for-transfer.sh b/scripts/clean-for-transfer.sh`

`````diff
diff --git a/scripts/clean-for-transfer.sh b/scripts/clean-for-transfer.sh
index 24930a0..55067a4 100755
--- a/scripts/clean-for-transfer.sh
+++ b/scripts/clean-for-transfer.sh
@@ -47,7 +47,8 @@ FILES_TO_DELETE=$(find "$ROOT" \
         -name "*.trx" -o \
         -name "*.coveragexml" -o \
         -name "*.user" -o \
-        -name "*.suo" \
+        -name "*.suo" -o \
+        -name "*.tsbuildinfo" \
     \) -print 2>/dev/null | sort)
 
 TOTAL_SIZE=0
`````

### `a/src/.dockerignore b/src/.dockerignore`

`````diff
diff --git a/src/.dockerignore b/src/.dockerignore
new file mode 100644
index 0000000..45cb16c
--- /dev/null
+++ b/src/.dockerignore
@@ -0,0 +1,12 @@
+**/bin/
+**/obj/
+**/node_modules/
+**/dist/
+**/publish/
+**/TestResults/
+**/.vs/
+**/.idea/
+**/.DS_Store
+**/*.user
+**/*.suo
+**/*.tsbuildinfo
`````

### `a/src/Dockerfile.local b/src/Dockerfile.local`

`````diff
diff --git a/src/Dockerfile.local b/src/Dockerfile.local
new file mode 100644
index 0000000..d4a1ad0
--- /dev/null
+++ b/src/Dockerfile.local
@@ -0,0 +1,24 @@
+# Local development build; corporate deployment keeps its existing Dockerfiles.
+FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
+WORKDIR /src
+COPY crt/ /usr/local/share/ca-certificates/mailgate/
+RUN update-ca-certificates
+COPY src/ ./
+ARG PROJECT
+RUN --mount=type=secret,id=nuget_config \
+    if [ -f /run/secrets/nuget_config ]; then \
+      dotnet restore "MailGate.${PROJECT}/MailGate.${PROJECT}.csproj" --configfile /run/secrets/nuget_config; \
+    else \
+      dotnet restore "MailGate.${PROJECT}/MailGate.${PROJECT}.csproj"; \
+    fi
+RUN dotnet publish "MailGate.${PROJECT}/MailGate.${PROJECT}.csproj" -c Release -o /app/publish --no-restore
+
+FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
+WORKDIR /app
+COPY crt/ /usr/local/share/ca-certificates/mailgate/
+RUN update-ca-certificates
+ENV ASPNETCORE_HTTP_PORTS=8080
+COPY --from=build /app/publish ./
+ARG PROJECT
+ENV MAILGATE_PROJECT=${PROJECT}
+ENTRYPOINT ["/bin/sh", "-c", "exec dotnet \"MailGate.${MAILGATE_PROJECT}.dll\" \"$@\"", "--"]
`````

### `a/src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs`

`````diff
diff --git a/src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs
new file mode 100644
index 0000000..25ce0a9
--- /dev/null
+++ b/src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs
@@ -0,0 +1,81 @@
+using System.Text.Json;
+using MailGate.Api.Templates;
+using MailGate.Contracts.Common;
+using MailGate.Contracts.Templates;
+using MailGate.Core.Entities;
+using MailGate.Infrastructure.Persistence;
+using Microsoft.EntityFrameworkCore;
+
+namespace MailGate.Api.Admin.Endpoints;
+
+public static class EmailTemplatesEndpoints
+{
+    public static IEndpointRouteBuilder MapEmailTemplatesEndpoints(this IEndpointRouteBuilder app)
+    {
+        var group = app.MapGroup("/api/admin/email-templates").RequireAuthorization("MailGateAdmin").WithTags("Admin / Email templates");
+        group.MapGet("/", ListAsync);
+        group.MapGet("/{id:guid}", GetAsync);
+        group.MapPost("/", CreateAsync);
+        group.MapPut("/{id:guid}", UpdateAsync);
+        group.MapDelete("/{id:guid}", DeleteAsync);
+        group.MapPost("/preview", Preview);
+        return app;
+    }
+
+    private static async Task<IResult> ListAsync(MailGateDbContext db, CancellationToken ct,
+        string? search = null, int page = 1, int pageSize = 50)
+    {
+        page = Math.Max(1, page); pageSize = Math.Clamp(pageSize, 1, 500);
+        var query = db.EmailTemplates.AsNoTracking();
+        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(t => EF.Functions.ILike(t.Name, $"%{search.Trim()}%"));
+        var count = await query.CountAsync(ct);
+        var rows = await query.OrderByDescending(t => t.UpdatedAt).ThenBy(t => t.Id).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
+        return Results.Ok(new PagedResult<EmailTemplateSummaryDto>(rows.Select(t => new EmailTemplateSummaryDto(
+            t.Id, t.Name, Deserialize(t).Subject, t.Revision, t.UpdatedAt)).ToList(), count, page, pageSize));
+    }
+    private static async Task<IResult> GetAsync(Guid id, MailGateDbContext db, CancellationToken ct)
+    {
+        var entity = await db.EmailTemplates.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, ct);
+        return entity is null ? Results.NotFound() : Results.Ok(ToDto(entity));
+    }
+    private static async Task<IResult> CreateAsync(SaveEmailTemplateDto dto, MailGateDbContext db, CancellationToken ct)
+    {
+        var errors = EmailTemplateRenderer.Validate(dto.Definition);
+        if (errors.Count != 0) return Results.ValidationProblem(errors);
+        var entity = new EmailTemplate { Name = dto.Definition.Name.Trim(), DefinitionJson = JsonSerializer.Serialize(dto.Definition) };
+        db.EmailTemplates.Add(entity); await db.SaveChangesAsync(ct);
+        return Results.Created($"/api/admin/email-templates/{entity.Id}", ToDto(entity));
+    }
+    private static async Task<IResult> UpdateAsync(Guid id, SaveEmailTemplateDto dto, MailGateDbContext db, CancellationToken ct)
+    {
+        var errors = EmailTemplateRenderer.Validate(dto.Definition);
+        if (errors.Count != 0) return Results.ValidationProblem(errors);
+        var entity = await db.EmailTemplates.SingleOrDefaultAsync(t => t.Id == id, ct);
+        if (entity is null) return Results.NotFound();
+        if (dto.Revision != entity.Revision) return Results.Problem(statusCode:409, detail:"Шаблон изменён другим пользователем. Обновите страницу.");
+        entity.Name = dto.Definition.Name.Trim(); entity.DefinitionJson = JsonSerializer.Serialize(dto.Definition);
+        entity.Revision = Guid.NewGuid(); entity.UpdatedAt = DateTimeOffset.UtcNow;
+        try { await db.SaveChangesAsync(ct); }
+        catch (DbUpdateConcurrencyException) { return Results.Problem(statusCode:409, detail:"Шаблон изменён или удалён другим пользователем."); }
+        return Results.Ok(ToDto(entity));
+    }
+    private static async Task<IResult> DeleteAsync(Guid id, MailGateDbContext db, CancellationToken ct)
+    {
+        var entity = await db.EmailTemplates.SingleOrDefaultAsync(t => t.Id == id, ct);
+        if (entity is null) return Results.NotFound();
+        db.EmailTemplates.Remove(entity);
+        try { await db.SaveChangesAsync(ct); }
+        catch (DbUpdateConcurrencyException) { return Results.Problem(statusCode:409, detail:"Шаблон изменён или удалён другим пользователем."); }
+        return Results.NoContent();
+    }
+    private static IResult Preview(PreviewEmailTemplateDto dto)
+    {
+        var errors = EmailTemplateRenderer.Validate(dto.Definition);
+        if (dto.Body is null || dto.Body.Length > 1024 * 1024) errors["Body"] = ["Текст: до 1 МБ."];
+        if (dto.Subject?.Length > 500 || dto.Subject?.Contains('\r') == true || dto.Subject?.Contains('\n') == true) errors["Subject"] = ["Некорректная тема."];
+        return errors.Count != 0 ? Results.ValidationProblem(errors) : Results.Ok(EmailTemplateRenderer.Render(dto.Definition, dto.Body!, dto.Subject));
+    }
+    public static EmailTemplateDefinition Deserialize(EmailTemplate entity) => JsonSerializer.Deserialize<EmailTemplateDefinition>(entity.DefinitionJson)
+        ?? throw new InvalidOperationException("Шаблон повреждён.");
+    private static EmailTemplateDto ToDto(EmailTemplate entity) => new(entity.Id, Deserialize(entity), entity.Revision, entity.CreatedAt, entity.UpdatedAt);
+}
`````

### `a/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs`

`````diff
diff --git a/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs
index f33def6..03d80bc 100644
--- a/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs
+++ b/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs
@@ -1,4 +1,6 @@
 using System.Text.Json;
+using MailGate.Api.Templates;
+using MailGate.Contracts.Templates;
 using FluentValidation;
 using MailGate.Contracts.Common;
 using MailGate.Contracts.Notifications;
@@ -28,6 +30,7 @@ public static class NotificationsAdminEndpoints
         group.MapGet("/", ListAsync).WithName("ListAdminNotifications");
         group.MapGet("/stats", StatsAsync).WithName("AdminNotificationStats");
         group.MapGet("/{id:guid}", GetAsync).WithName("GetAdminNotification");
+        group.MapPost("/preview", PreviewAsync);
         group.MapPost("/", SendAsync).WithName("AdminSendNotification");
         group.MapPost("/{id:guid}/retry", RetryAsync).WithName("AdminRetryNotification");
 
@@ -136,6 +139,12 @@ public static class NotificationsAdminEndpoints
             log.RetryOfMessageId));
     }
 
+    private static async Task<IResult> PreviewAsync(PreviewNotificationDto dto, MailGateDbContext db, CancellationToken ct)
+    {
+        try { return Results.Ok(NotificationComposer.Preview(await NotificationComposer.ComposeAsync(db, dto, ct))); }
+        catch (NotificationCompositionException ex) { return Results.ValidationProblem(new Dictionary<string, string[]> { ["Template"] = [ex.Message] }); }
+    }
+
     private static async Task<Results<Accepted<AcceptedNotificationDto>, ValidationProblem>> SendAsync(
         SendNotificationFromAdminDto dto,
         IValidator<SendNotificationFromAdminDto> validator,
@@ -148,6 +157,20 @@ public static class NotificationsAdminEndpoints
         if (!validation.IsValid)
             return TypedResults.ValidationProblem(validation.ToDictionary());
 
+        ComposedNotification composed;
+        try
+        {
+            composed = await NotificationComposer.ComposeAsync(db, new PreviewNotificationDto
+            {
+                TemplateId = dto.TemplateId, TemplateRevision = dto.TemplateRevision, TemplateDefinition = dto.TemplateDefinition,
+                Subject = dto.Subject, Body = dto.Body, BodyFormat = dto.BodyFormat
+            }, cancellationToken);
+        }
+        catch (NotificationCompositionException ex)
+        {
+            return TypedResults.ValidationProblem(new Dictionary<string, string[]> { ["Template"] = [ex.Message] });
+        }
+
         var emailsFromBook = dto.RecipientIds.Count == 0
             ? []
             : await db.Recipients.AsNoTracking()
@@ -181,9 +204,9 @@ public static class NotificationsAdminEndpoints
         {
             Channel = dto.Channel,
             Recipients = allRecipients,
-            Subject = dto.Subject,
-            Body = dto.Body,
-            BodyFormat = dto.BodyFormat,
+            Subject = composed.Subject,
+            Body = composed.Body,
+            BodyFormat = composed.BodyFormat,
             Priority = dto.Priority,
             TraceId = httpContext.TraceIdentifier
         };
`````

### `a/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs`

`````diff
diff --git a/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs
index 2b08b39..eab8ef3 100644
--- a/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs
+++ b/src/MailGate.Api/Admin/Endpoints/SmtpAdminEndpoints.cs
@@ -1,4 +1,5 @@
 using System.Diagnostics;
+using MailGate.Core.Enums;
 using FluentValidation;
 using MailGate.Channels.Smtp.Options;
 using MailGate.Channels.Smtp.Services;
@@ -35,7 +36,7 @@ public static class SmtpAdminEndpoints
         IOptionsMonitor<SmtpOptions> appSettings,
         CancellationToken cancellationToken)
     {
-        var dbConfig = await store.GetAsync(cancellationToken);
+        var dbConfig = await store.GetMetadataAsync(cancellationToken);
 
         if (dbConfig is null)
         {
@@ -46,9 +47,9 @@ public static class SmtpAdminEndpoints
                 s.OAuth2.SmtpUserPrincipalName);
 
             return TypedResults.Ok(new SmtpConfigurationDto(
-                s.Host, s.Port, s.UseSsl, s.UseTls, s.FromAddress, s.FromName,
+                s.Host, s.Port, s.UseSsl, s.UseTls, s.CheckCertificateRevocation, s.FromAddress, s.FromName,
                 s.Username, !string.IsNullOrEmpty(s.Password),
-                oauth, SmtpConfigurationSource.AppSettings, null));
+                oauth, SmtpConfigurationSource.AppSettings, null, s.CredentialSource));
         }
 
         return TypedResults.Ok(ToDto(dbConfig, SmtpConfigurationSource.Database));
@@ -58,7 +59,7 @@ public static class SmtpAdminEndpoints
         SmtpConfigurationStore store,
         CancellationToken cancellationToken)
     {
-        var dbConfig = await store.GetAsync(cancellationToken);
+        var dbConfig = await store.GetMetadataAsync(cancellationToken);
         return dbConfig is null
             ? TypedResults.NotFound()
             : TypedResults.Ok(ToDto(dbConfig, SmtpConfigurationSource.Database));
@@ -77,10 +78,12 @@ public static class SmtpAdminEndpoints
 
         var entity = new SmtpConfiguration
         {
+            CredentialSource = dto.CredentialSource,
             Host = dto.Host.Trim(),
             Port = dto.Port,
             UseSsl = dto.UseSsl,
             UseTls = dto.UseTls,
+            CheckCertificateRevocation = dto.CheckCertificateRevocation,
             FromAddress = dto.FromAddress.Trim(),
             FromName = dto.FromName.Trim(),
             Username = string.IsNullOrWhiteSpace(dto.Username) ? null : dto.Username.Trim(),
@@ -92,7 +95,7 @@ public static class SmtpAdminEndpoints
         await store.UpsertAsync(entity, dto.Password, dto.OAuth2?.ClientSecret, cancellationToken);
         provider?.Invalidate();
 
-        var saved = await store.GetAsync(cancellationToken);
+        var saved = await store.GetMetadataAsync(cancellationToken);
         return TypedResults.Ok(ToDto(saved!, SmtpConfigurationSource.Database));
     }
 
@@ -109,7 +112,7 @@ public static class SmtpAdminEndpoints
     private static async Task<Ok<SmtpTestResultDto>> TestConnectionAsync(
         UpdateSmtpConfigurationDto dto,
         IValidator<UpdateSmtpConfigurationDto> validator,
-        IOptionsMonitor<SmtpOptions> appSettings,
+        SmtpCredentialResolver credentials,
         CancellationToken cancellationToken)
     {
         var validation = await validator.ValidateAsync(dto, cancellationToken);
@@ -121,11 +124,15 @@ public static class SmtpAdminEndpoints
                 DurationMs: 0));
         }
 
-        var effective = BuildEffectiveOptions(dto, appSettings.CurrentValue);
         var sw = Stopwatch.StartNew();
         try
         {
+            var effective = BuildEffectiveOptions(dto);
+            await credentials.ApplyAsync(effective, cancellationToken);
+            if (effective.OAuth2 is null && !string.IsNullOrWhiteSpace(effective.Username) && string.IsNullOrEmpty(effective.Password))
+                throw new InvalidOperationException("Указан SMTP-логин, но пароль отсутствует.");
             using var client = new SmtpClient();
+            client.CheckCertificateRevocation = effective.CheckCertificateRevocation;
             var secureSocketOptions = effective.UseSsl
                 ? SecureSocketOptions.SslOnConnect
                 : effective.UseTls
@@ -134,17 +141,8 @@ public static class SmtpAdminEndpoints
 
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
@@ -157,28 +155,20 @@ public static class SmtpAdminEndpoints
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
+        CheckCertificateRevocation = dto.CheckCertificateRevocation,
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
 
-    private static SmtpConfigurationDto ToDto(SmtpConfigurationStore.DecryptedConfiguration dc, SmtpConfigurationSource source)
+    private static SmtpConfigurationDto ToDto(SmtpConfiguration e, SmtpConfigurationSource source)
     {
-        var e = dc.Entity;
         SmtpOAuth2Dto? oauth = string.IsNullOrWhiteSpace(e.OAuth2TenantId) ? null : new SmtpOAuth2Dto(
             e.OAuth2TenantId!,
             e.OAuth2ClientId ?? string.Empty,
@@ -186,8 +176,8 @@ public static class SmtpAdminEndpoints
             e.OAuth2SmtpUserPrincipalName ?? string.Empty);
 
         return new SmtpConfigurationDto(
-            e.Host, e.Port, e.UseSsl, e.UseTls, e.FromAddress, e.FromName,
+            e.Host, e.Port, e.UseSsl, e.UseTls, e.CheckCertificateRevocation, e.FromAddress, e.FromName,
             e.Username, !string.IsNullOrEmpty(e.EncryptedPassword),
-            oauth, source, e.UpdatedAt);
+            oauth, source, e.UpdatedAt, e.CredentialSource);
     }
 }
`````

### `a/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs b/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs`

`````diff
diff --git a/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs b/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs
index decffe7..ceaba6a 100644
--- a/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs
+++ b/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs
@@ -9,6 +9,10 @@ public sealed class SendNotificationFromAdminValidator : AbstractValidator<SendN
     {
         RuleFor(x => x.Body).NotEmpty().MaximumLength(1024 * 1024);
         RuleFor(x => x.Subject).MaximumLength(500);
+        RuleFor(x => x.Subject).Must(x => x is null || (!x.Contains('\r') && !x.Contains('\n')))
+            .WithMessage("Тема не должна содержать переводы строки.");
+        RuleFor(x => x.TemplateRevision).NotNull().When(x => x.TemplateId.HasValue);
+        RuleFor(x => x.Channel).Equal(MailGate.Core.Enums.ChannelType.Smtp).When(x => x.TemplateId.HasValue);
 
         RuleFor(x => x)
             .Must(x => x.RecipientIds.Count > 0 || x.RecipientGroupIds.Count > 0 || x.AdHocEmails.Count > 0)
`````

### `a/src/MailGate.Api/Admin/Validators/SmtpValidators.cs b/src/MailGate.Api/Admin/Validators/SmtpValidators.cs`

`````diff
diff --git a/src/MailGate.Api/Admin/Validators/SmtpValidators.cs b/src/MailGate.Api/Admin/Validators/SmtpValidators.cs
index 2d1c78d..650d9e8 100644
--- a/src/MailGate.Api/Admin/Validators/SmtpValidators.cs
+++ b/src/MailGate.Api/Admin/Validators/SmtpValidators.cs
@@ -1,4 +1,5 @@
 using FluentValidation;
+using MailGate.Core.Enums;
 using MailGate.Contracts.Smtp;
 
 namespace MailGate.Api.Admin.Validators;
@@ -7,7 +8,16 @@ public sealed class UpdateSmtpConfigurationValidator : AbstractValidator<UpdateS
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
+        RuleFor(x => x.UseSsl).Must((dto, useSsl) => !useSsl || !dto.UseTls)
+            .WithMessage("Выберите один режим: STARTTLS или SSL при подключении.");
         RuleFor(x => x.Port).InclusiveBetween(1, 65535);
         RuleFor(x => x.FromAddress).NotEmpty().EmailAddress().MaximumLength(320);
         RuleFor(x => x.FromName).NotEmpty().MaximumLength(200);
`````

### `a/src/MailGate.Api/Auth/KeycloakGroupAccess.cs b/src/MailGate.Api/Auth/KeycloakGroupAccess.cs`

`````diff
diff --git a/src/MailGate.Api/Auth/KeycloakGroupAccess.cs b/src/MailGate.Api/Auth/KeycloakGroupAccess.cs
new file mode 100644
index 0000000..cfacb47
--- /dev/null
+++ b/src/MailGate.Api/Auth/KeycloakGroupAccess.cs
@@ -0,0 +1,37 @@
+using System.Security.Claims;
+using System.Text.Json;
+
+namespace MailGate.Api.Auth;
+
+public static class KeycloakGroupAccess
+{
+    public static bool HasGroup(ClaimsPrincipal principal, string? requiredGroup)
+    {
+        if (string.IsNullOrWhiteSpace(requiredGroup))
+            return false;
+
+        foreach (var claim in principal.FindAll("groups"))
+        {
+            if (string.Equals(claim.Value, requiredGroup, StringComparison.Ordinal))
+                return true;
+
+            if (!claim.Value.TrimStart().StartsWith('['))
+                continue;
+
+            try
+            {
+                using var document = JsonDocument.Parse(claim.Value);
+                if (document.RootElement.EnumerateArray().Any(group =>
+                    group.ValueKind == JsonValueKind.String &&
+                    string.Equals(group.GetString(), requiredGroup, StringComparison.Ordinal)))
+                    return true;
+            }
+            catch (JsonException)
+            {
+                // Malformed group claims do not grant access.
+            }
+        }
+
+        return false;
+    }
+}
`````

### `a/src/MailGate.Api/Auth/KeycloakRoleClaimsTransformation.cs b/src/MailGate.Api/Auth/KeycloakRoleClaimsTransformation.cs`

`````diff
diff --git a/src/MailGate.Api/Auth/KeycloakRoleClaimsTransformation.cs b/src/MailGate.Api/Auth/KeycloakRoleClaimsTransformation.cs
deleted file mode 100644
index 95bacf7..0000000
--- a/src/MailGate.Api/Auth/KeycloakRoleClaimsTransformation.cs
+++ /dev/null
@@ -1,128 +0,0 @@
-using System.Security.Claims;
-using System.Text.Json;
-using Microsoft.AspNetCore.Authentication;
-
-namespace MailGate.Api.Auth;
-
-public sealed class KeycloakRoleClaimsTransformation(IConfiguration configuration) : IClaimsTransformation
-{
-    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
-    {
-        if (principal.Identity is not ClaimsIdentity source || !source.IsAuthenticated)
-            return Task.FromResult(principal);
-
-        // JWT handler часто задаёт RoleClaimType не как ClaimTypes.Role; IsInRole смотрит только RoleClaimType.
-        var identity = new ClaimsIdentity(
-            source.Claims,
-            source.AuthenticationType,
-            source.NameClaimType,
-            ClaimTypes.Role);
-
-        AddJwtRolesClaim(principal, identity);
-        AddRealmRolesFromJson(principal, identity);
-        AddRealmRolesFromFlatClaims(principal, identity);
-        AddClientRolesFromJson(principal, identity);
-        AddClientRolesFromFlatClaims(principal, identity);
-
-        return Task.FromResult(new ClaimsPrincipal(identity));
-    }
-
-    private static void AddJwtRolesClaim(ClaimsPrincipal principal, ClaimsIdentity identity)
-    {
-        foreach (var claim in principal.FindAll("roles"))
-        {
-            if (string.IsNullOrWhiteSpace(claim.Value))
-                continue;
-
-            if (claim.Value.TrimStart().StartsWith('['))
-            {
-                TryAddRolesFromJsonString(identity, claim.Value);
-                continue;
-            }
-
-            AddRole(identity, claim.Value);
-        }
-    }
-
-    private static void AddRealmRolesFromJson(ClaimsPrincipal principal, ClaimsIdentity identity)
-    {
-        var realmAccess = principal.FindFirst("realm_access")?.Value;
-        if (string.IsNullOrWhiteSpace(realmAccess))
-            return;
-
-        using var document = JsonDocument.Parse(realmAccess);
-        if (!document.RootElement.TryGetProperty("roles", out var roles))
-            return;
-
-        AddRolesFromJsonElement(identity, roles);
-    }
-
-    /// <summary>
-    /// Keycloak при claim.name вида realm_access.roles может отдавать отдельные claim'ы с таким типом.
-    /// </summary>
-    private static void AddRealmRolesFromFlatClaims(ClaimsPrincipal principal, ClaimsIdentity identity)
-    {
-        foreach (var claim in principal.FindAll("realm_access.roles"))
-            AddRole(identity, claim.Value);
-    }
-
-    private void AddClientRolesFromJson(ClaimsPrincipal principal, ClaimsIdentity identity)
-    {
-        var resourceAccess = principal.FindFirst("resource_access")?.Value;
-        if (string.IsNullOrWhiteSpace(resourceAccess))
-            return;
-
-        var clientId = configuration["Authentication:Keycloak:ClientId"] ?? "mailgate-admin";
-
-        using var document = JsonDocument.Parse(resourceAccess);
-        if (!document.RootElement.TryGetProperty(clientId, out var client) ||
-            !client.TryGetProperty("roles", out var roles))
-        {
-            return;
-        }
-
-        AddRolesFromJsonElement(identity, roles);
-    }
-
-    private void AddClientRolesFromFlatClaims(ClaimsPrincipal principal, ClaimsIdentity identity)
-    {
-        var clientId = configuration["Authentication:Keycloak:ClientId"] ?? "mailgate-admin";
-        var claimType = $"resource_access.{clientId}.roles";
-
-        foreach (var claim in principal.FindAll(claimType))
-            AddRole(identity, claim.Value);
-    }
-
-    private static void TryAddRolesFromJsonString(ClaimsIdentity identity, string json)
-    {
-        try
-        {
-            using var document = JsonDocument.Parse(json);
-            AddRolesFromJsonElement(identity, document.RootElement);
-        }
-        catch (JsonException)
-        {
-            // ignore
-        }
-    }
-
-    private static void AddRolesFromJsonElement(ClaimsIdentity identity, JsonElement roles)
-    {
-        if (roles.ValueKind != JsonValueKind.Array)
-            return;
-
-        foreach (var role in roles.EnumerateArray())
-        {
-            var value = role.GetString();
-            AddRole(identity, value);
-        }
-    }
-
-    private static void AddRole(ClaimsIdentity identity, string? value)
-    {
-        if (string.IsNullOrWhiteSpace(value) || identity.HasClaim(ClaimTypes.Role, value))
-            return;
-
-        identity.AddClaim(new Claim(ClaimTypes.Role, value));
-    }
-}
`````

### `a/src/MailGate.Api/Dockerfile b/src/MailGate.Api/Dockerfile`

`````diff
diff --git a/src/MailGate.Api/Dockerfile b/src/MailGate.Api/Dockerfile
index c75c6f9..c50d249 100644
--- a/src/MailGate.Api/Dockerfile
+++ b/src/MailGate.Api/Dockerfile
@@ -3,29 +3,40 @@ WORKDIR /app
 EXPOSE 8080
 
 RUN dnf makecache \
-    && dnf install -y dotnet-runtime-10.0 aspnetcore-runtime-10.0 \
+    && dnf install -y ca-certificates dotnet-runtime-10.0 aspnetcore-runtime-10.0 \
     && dnf clean all
 
 FROM registry.red-soft.ru/ubi8/ubi:8.0.2-260714 AS build
 WORKDIR /src
 
 RUN dnf makecache \
-    && dnf install -y dotnet-sdk-10.0 \
+    && dnf install -y ca-certificates dotnet-sdk-10.0 \
     && dnf clean all
 
-COPY ["src/MailGate.Api/MailGate.Api.csproj", "src/MailGate.Api/"]
-COPY ["src/MailGate.Contracts/MailGate.Contracts.csproj", "src/MailGate.Contracts/"]
-COPY ["src/MailGate.Core/MailGate.Core.csproj", "src/MailGate.Core/"]
-COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj", "src/MailGate.Channels.Smtp/"]
-COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj", "src/MailGate.Infrastructure/"]
+COPY crt/ /etc/pki/ca-trust/source/anchors/
+RUN update-ca-trust extract
 
-RUN dotnet restore "src/MailGate.Api/MailGate.Api.csproj"
+COPY ["src/NuGet.Config", "./"]
+COPY ["src/MailGate.Api/MailGate.Api.csproj", "MailGate.Api/"]
+COPY ["src/MailGate.Contracts/MailGate.Contracts.csproj", "MailGate.Contracts/"]
+COPY ["src/MailGate.Core/MailGate.Core.csproj", "MailGate.Core/"]
+COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj", "MailGate.Channels.Smtp/"]
+COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj", "MailGate.Infrastructure/"]
 
-COPY . .
-RUN dotnet publish "src/MailGate.Api/MailGate.Api.csproj" \
+RUN --mount=type=secret,id=nuget_config \
+    if [ -f /run/secrets/nuget_config ]; then \
+      dotnet restore "MailGate.Api/MailGate.Api.csproj" --configfile /run/secrets/nuget_config; \
+    else \
+      dotnet restore "MailGate.Api/MailGate.Api.csproj"; \
+    fi
+
+COPY src/ .
+RUN dotnet publish "MailGate.Api/MailGate.Api.csproj" \
     -c Release -o /app/publish --no-restore
 
 FROM base AS final
 WORKDIR /app
+COPY crt/ /etc/pki/ca-trust/source/anchors/
+RUN update-ca-trust extract
 COPY --from=build /app/publish .
 ENTRYPOINT ["dotnet", "MailGate.Api.dll"]
`````

### `a/src/MailGate.Api/MailGate.Api.http b/src/MailGate.Api/MailGate.Api.http`

`````diff
diff --git a/src/MailGate.Api/MailGate.Api.http b/src/MailGate.Api/MailGate.Api.http
deleted file mode 100644
index 68412e3..0000000
--- a/src/MailGate.Api/MailGate.Api.http
+++ /dev/null
@@ -1,6 +0,0 @@
-@MailGate.Api_HostAddress = http://localhost:5260
-
-GET {{MailGate.Api_HostAddress}}/weatherforecast/
-Accept: application/json
-
-###
`````

### `a/src/MailGate.Api/Program.cs b/src/MailGate.Api/Program.cs`

`````diff
diff --git a/src/MailGate.Api/Program.cs b/src/MailGate.Api/Program.cs
index b695fa0..9537e78 100644
--- a/src/MailGate.Api/Program.cs
+++ b/src/MailGate.Api/Program.cs
@@ -14,7 +14,6 @@ using MailGate.Contracts.Recipients;
 using MailGate.Contracts.Smtp;
 using MailGate.Contracts.Systems;
 using MailGate.Infrastructure.Extensions;
-using Microsoft.AspNetCore.Authentication;
 using Microsoft.AspNetCore.Authentication.JwtBearer;
 using Microsoft.IdentityModel.Tokens;
 using Microsoft.Extensions.DependencyInjection;
@@ -31,7 +30,7 @@ const string keycloakOAuthSecurityScheme = "Keycloak OAuth2";
 var keycloak = builder.Configuration.GetSection("Authentication:Keycloak");
 var keycloakAuthority = keycloak["Authority"] ?? "http://keycloak.localhost:8180/realms/mailgate";
 var keycloakAudience = keycloak["Audience"] ?? "mailgate-api";
-var keycloakAdminRole = keycloak["AdminRole"] ?? "mailgate-admin";
+var keycloakAdminGroup = keycloak["AdminGroup"] ?? "";
 var keycloakSwaggerClientId = keycloak["SwaggerClientId"] ?? "mailgate-admin";
 
 builder.Services.ConfigureHttpJsonOptions(o =>
@@ -149,24 +148,15 @@ builder.Services
             ValidIssuer = keycloakAuthority
         };
 
-        // IClaimsTransformation is not invoked automatically for JwtBearer without ASP.NET Core Identity.
-        options.Events = new JwtBearerEvents
-        {
-            OnTokenValidated = async context =>
-            {
-                var transform = context.HttpContext.RequestServices.GetRequiredService<IClaimsTransformation>();
-                context.Principal = await transform.TransformAsync(context.Principal!);
-            }
-        };
     });
 
-builder.Services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformation>();
 builder.Services.AddAuthorization(options =>
 {
     options.AddPolicy(adminPolicy, policy =>
     {
         policy.RequireAuthenticatedUser();
-        policy.RequireRole(keycloakAdminRole);
+        policy.RequireAssertion(context =>
+            KeycloakGroupAccess.HasGroup(context.User, keycloakAdminGroup));
     });
 });
 
@@ -237,6 +227,7 @@ app.MapRecipientGroupsAdminEndpoints();
 app.MapSystemsAdminEndpoints();
 app.MapNotificationsAdminEndpoints();
 app.MapSmtpAdminEndpoints();
+app.MapEmailTemplatesEndpoints();
 
 app.MapHealthChecks("/healthz");
 app.MapHealthChecks("/healthz/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
`````

### `a/src/MailGate.Api/Templates/EmailTemplateRenderer.cs b/src/MailGate.Api/Templates/EmailTemplateRenderer.cs`

`````diff
diff --git a/src/MailGate.Api/Templates/EmailTemplateRenderer.cs b/src/MailGate.Api/Templates/EmailTemplateRenderer.cs
new file mode 100644
index 0000000..4a1ff4b
--- /dev/null
+++ b/src/MailGate.Api/Templates/EmailTemplateRenderer.cs
@@ -0,0 +1,92 @@
+using System.Net;
+using MailGate.Core.Models;
+using System.Text;
+using System.Text.RegularExpressions;
+using MailGate.Contracts.Templates;
+
+namespace MailGate.Api.Templates;
+
+public static class EmailTemplateRenderer
+{
+    private static readonly HashSet<string> BlockTypes = ["heading", "text", "content", "button", "divider", "columns", "image"];
+    private static readonly Regex Color = new("^#[0-9a-fA-F]{6}$", RegexOptions.NonBacktracking);
+
+    public static Dictionary<string, string[]> Validate(EmailTemplateDefinition? definition)
+    {
+        var errors = new Dictionary<string, string[]>();
+        void Error(string field, string message) => errors[field] = [message];
+        if (definition is null) { Error("Definition", "Укажите шаблон."); return errors; }
+        if (string.IsNullOrWhiteSpace(definition.Name) || definition.Name.Length > 200)
+            Error("Name", "Название должно содержать от 1 до 200 символов.");
+        if (definition.Subject is null || definition.Subject.Length > 500 || definition.Subject.Contains('\r') || definition.Subject.Contains('\n'))
+            Error("Subject", "Тема: не более 500 символов, без перевода строки.");
+        if (definition.Preheader is null || definition.Preheader.Length > 500 || definition.Footer is null || definition.Footer.Length > 4000)
+            Error("Text", "Прехедер: до 500 символов, подпись: до 4000.");
+        if (definition.AccentColor is null || !Color.IsMatch(definition.AccentColor) ||
+            definition.BackgroundColor is null || !Color.IsMatch(definition.BackgroundColor))
+            Error("Colors", "Цвет должен иметь формат #RRGGBB.");
+        if (definition.Blocks is null || definition.Blocks.Count is < 1 or > 40)
+        { Error("Blocks", "Добавьте от 1 до 40 блоков."); return errors; }
+        if (definition.Blocks.Count(b => b?.Type == "content") != 1)
+            Error("Blocks", "Шаблон должен содержать ровно один блок основного сообщения.");
+        var totalImageBytes = 0;
+        for (var i = 0; i < definition.Blocks.Count; i++)
+        {
+            var block = definition.Blocks[i];
+            if (block is null || !BlockTypes.Contains(block.Type ?? ""))
+            { Error($"Blocks[{i}]", "Неизвестный тип блока."); continue; }
+            if (block.Text is null || block.Text.Length > 10000 || block.SecondaryText is null || block.SecondaryText.Length > 10000 || block.Url is null || (block.Type != "image" && block.Url.Length > 2000))
+            { Error($"Blocks[{i}]", "Слишком длинное или отсутствующее содержимое блока."); continue; }
+            if (block.Type == "button" && (string.IsNullOrWhiteSpace(block.Text) || !IsHttpUrl(block.Url)))
+                Error($"Blocks[{i}]", "Для кнопки нужны текст и абсолютная ссылка http:// или https://.");
+            if (block.Type == "image")
+            {
+                if (EmbeddedEmailImage.TryRead(block.Url, out _, out var bytes))
+                    totalImageBytes += bytes.Length;
+                else if (block.Url.Length > 2000 || !IsHttpUrl(block.Url) || !block.Url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
+                    Error($"Blocks[{i}]", "Загрузите PNG, JPEG или GIF до 128 КБ.");
+            }
+        }
+        if (totalImageBytes > EmbeddedEmailImage.MaxTotalBytes)
+            Error("Blocks", "Общий размер изображений: не более 512 КБ.");
+        return errors;
+    }
+
+    public static bool IsHttpUrl(string value) => Uri.TryCreate(value, UriKind.Absolute, out var uri)
+        && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp) && string.IsNullOrEmpty(uri.UserInfo);
+
+    public static string Encode(string? value) => WebUtility.HtmlEncode(value ?? "");
+    private static string Lines(string? value) => Encode(value).Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", "<br />");
+
+    public static EmailPreviewDto Render(EmailTemplateDefinition definition, string body, string? subject = null)
+    {
+        if (Validate(definition).Count != 0) throw new ArgumentException("Некорректный шаблон.");
+        var title = string.IsNullOrWhiteSpace(subject) ? definition.Subject : subject.Trim();
+        var html = new StringBuilder();
+        html.Append("<!doctype html><html lang=\"ru\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>")
+            .Append(Encode(title)).Append("</title><style>@media only screen and (max-width:620px){.email-column{display:block!important;width:100%!important}.email-padding{padding:20px!important}}</style></head>")
+            .Append($"<body style=\"margin:0;padding:0;background-color:{definition.BackgroundColor}\">")
+            .Append($"<div style=\"display:none;max-height:0;overflow:hidden;opacity:0\">{Encode(definition.Preheader)}</div>")
+            .Append($"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"{definition.BackgroundColor}\"><tr><td align=\"center\" style=\"padding:24px 8px\">")
+            .Append("<!--[if mso]><table role=\"presentation\" width=\"600\"><tr><td><![endif]-->")
+            .Append($"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" bgcolor=\"#ffffff\" style=\"max-width:600px;border-top:4px solid {definition.AccentColor}\"><tr><td class=\"email-padding\" style=\"padding:32px;font-family:Arial,Helvetica,sans-serif;font-size:16px;line-height:1.6;color:#202631;word-break:break-word\">");
+        foreach (var block in definition.Blocks)
+        {
+            html.Append(block.Type switch
+            {
+                "image" => $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"margin-bottom:20px\"><tr><td align=\"center\"><img src=\"{Encode(block.Url)}\" alt=\"{Encode(block.Text)}\" width=\"536\" style=\"display:block;width:100%;max-width:536px;height:auto;border:0\" /></td></tr></table>",
+                "heading" => $"<h1 style=\"margin:0 0 20px;font-size:26px;line-height:1.3;color:{definition.AccentColor}\">{Lines(block.Text)}</h1>",
+                "text" => $"<p style=\"margin:0 0 20px\">{Lines(block.Text)}</p>",
+                "content" => $"<div style=\"margin:0 0 20px\">{Lines(body)}</div>",
+                "divider" => "<table role=\"presentation\" width=\"100%\"><tr><td style=\"border-top:1px solid #dfe3e8;height:20px\"></td></tr></table>",
+                "button" => $"<table role=\"presentation\" cellspacing=\"0\" cellpadding=\"0\" style=\"margin-bottom:20px\"><tr><td bgcolor=\"{definition.AccentColor}\" style=\"padding:12px 22px;border-radius:4px\"><a href=\"{Encode(block.Url)}\" style=\"color:#ffffff;text-decoration:none;font-weight:bold\">{Encode(block.Text)}</a></td></tr></table>",
+                "columns" => $"<table role=\"presentation\" width=\"100%\" cellspacing=\"0\" cellpadding=\"0\" style=\"margin-bottom:20px\"><tr><td class=\"email-column\" width=\"50%\" valign=\"top\" style=\"padding:0 12px 12px 0\">{Lines(block.Text)}</td><td class=\"email-column\" width=\"50%\" valign=\"top\" style=\"padding-bottom:12px\">{Lines(block.SecondaryText)}</td></tr></table>",
+                _ => throw new ArgumentException("Неизвестный блок.")
+            });
+        }
+        if (!string.IsNullOrWhiteSpace(definition.Footer))
+            html.Append($"<p style=\"margin:24px 0 0;font-size:12px;color:#667085;border-top:1px solid #e5e7eb;padding-top:16px\">{Lines(definition.Footer)}</p>");
+        html.Append("</td></tr></table><!--[if mso]></td></tr></table><![endif]--></td></tr></table></body></html>");
+        return new EmailPreviewDto(title, html.ToString());
+    }
+}
`````

### `a/src/MailGate.Api/Templates/NotificationComposer.cs b/src/MailGate.Api/Templates/NotificationComposer.cs`

`````diff
diff --git a/src/MailGate.Api/Templates/NotificationComposer.cs b/src/MailGate.Api/Templates/NotificationComposer.cs
new file mode 100644
index 0000000..5255de2
--- /dev/null
+++ b/src/MailGate.Api/Templates/NotificationComposer.cs
@@ -0,0 +1,39 @@
+using MailGate.Contracts.Templates;
+using MailGate.Core.Enums;
+using MailGate.Infrastructure.Persistence;
+using Microsoft.EntityFrameworkCore;
+
+namespace MailGate.Api.Templates;
+
+public sealed record ComposedNotification(string? Subject, string Body, BodyFormat BodyFormat);
+public sealed class NotificationCompositionException(string message) : Exception(message);
+
+public static class NotificationComposer
+{
+    public static async Task<ComposedNotification> ComposeAsync(MailGateDbContext db, PreviewNotificationDto dto, CancellationToken ct)
+    {
+        if (dto.Body is null || dto.Body.Length > 1024 * 1024) throw new NotificationCompositionException("Текст: до 1 МБ.");
+        if (dto.Subject?.Length > 500 || dto.Subject?.Contains('\r') == true || dto.Subject?.Contains('\n') == true)
+            throw new NotificationCompositionException("Тема: до 500 символов, без перевода строки.");
+        if (!Enum.IsDefined(dto.BodyFormat)) throw new NotificationCompositionException("Неизвестный формат сообщения.");
+        if (dto.TemplateId is not { } id)
+        {
+            if (dto.TemplateDefinition is not null) throw new NotificationCompositionException("Выберите исходный шаблон.");
+            return new(dto.Subject, dto.Body, dto.BodyFormat);
+        }
+        var template = await db.EmailTemplates.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id, ct)
+            ?? throw new NotificationCompositionException("Шаблон удалён или не найден. Выберите другой шаблон.");
+        if (dto.TemplateRevision != template.Revision)
+            throw new NotificationCompositionException("Шаблон изменился. Выберите его заново, чтобы обновить предпросмотр.");
+        var definition = dto.TemplateDefinition ?? Admin.Endpoints.EmailTemplatesEndpoints.Deserialize(template);
+        var errors = EmailTemplateRenderer.Validate(definition);
+        if (errors.Count != 0)
+            throw new NotificationCompositionException(string.Join(" ", errors.Values.SelectMany(messages => messages)));
+        var result = EmailTemplateRenderer.Render(definition, dto.Body, dto.Subject);
+        return new(result.Subject, result.Html, BodyFormat.Html);
+    }
+    public static EmailPreviewDto Preview(ComposedNotification composed) => new(composed.Subject ?? "",
+        composed.BodyFormat == BodyFormat.PlainText
+            ? $"<!doctype html><html><head><meta charset=\"utf-8\"></head><body><pre style=\"white-space:pre-wrap;overflow-wrap:anywhere;font:16px/1.6 Arial\">{EmailTemplateRenderer.Encode(composed.Body)}</pre></body></html>"
+            : composed.Body);
+}
`````

### `a/src/MailGate.Api/appsettings.json b/src/MailGate.Api/appsettings.json`

`````diff
diff --git a/src/MailGate.Api/appsettings.json b/src/MailGate.Api/appsettings.json
index fc67f42..3210087 100644
--- a/src/MailGate.Api/appsettings.json
+++ b/src/MailGate.Api/appsettings.json
@@ -18,7 +18,12 @@
     "Password": "mailgate",
     "NotificationsQueue": "mailgate-notifications"
   },
+  "DataProtection": {
+    "KeysPath": ""
+  },
   "Smtp": {
+    "CheckCertificateRevocation": true,
+    "SettingsCacheSeconds": 30,
     "Host": "localhost",
     "Port": 1025,
     "UseSsl": false,
@@ -37,7 +42,7 @@
       "Authority": "http://keycloak.localhost:8180/realms/mailgate",
       "Audience": "mailgate-api",
       "ClientId": "mailgate-admin",
-      "AdminRole": "mailgate-admin",
+      "AdminGroup": "",
       "RequireHttpsMetadata": false
     }
   }
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
index 0f04cb4..2f2fa41 100644
--- a/src/MailGate.Channels.Smtp/Options/SmtpOptions.cs
+++ b/src/MailGate.Channels.Smtp/Options/SmtpOptions.cs
@@ -1,13 +1,21 @@
+using MailGate.Core.Enums;
+
 namespace MailGate.Channels.Smtp.Options;
 
 public sealed class SmtpOptions
 {
     public const string SectionName = "Smtp";
 
+    public SmtpCredentialSource CredentialSource { get; set; } = SmtpCredentialSource.Legacy;
+    public string CredentialsFilePath { get; set; } = "/var/mailgate/smtp-credentials/account.json";
+
+    public int SettingsCacheSeconds { get; set; } = 30;
+
     public string Host { get; set; } = "localhost";
     public int Port { get; set; } = 587;
     public bool UseSsl { get; set; } = false;
     public bool UseTls { get; set; } = true;
+    public bool CheckCertificateRevocation { get; set; } = true;
     public string FromAddress { get; set; } = "noreply@example.com";
     public string FromName { get; set; } = "MailGate";
 
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

### `a/src/MailGate.Channels.Smtp/SmtpChannel.cs b/src/MailGate.Channels.Smtp/SmtpChannel.cs`

`````diff
diff --git a/src/MailGate.Channels.Smtp/SmtpChannel.cs b/src/MailGate.Channels.Smtp/SmtpChannel.cs
index f2fd1db..7ad4f8c 100644
--- a/src/MailGate.Channels.Smtp/SmtpChannel.cs
+++ b/src/MailGate.Channels.Smtp/SmtpChannel.cs
@@ -8,6 +8,7 @@ using MailKit.Security;
 using Microsoft.Extensions.Logging;
 using MimeKit;
 using MimeKit.Text;
+using System.Text.RegularExpressions;
 
 namespace MailGate.Channels.Smtp;
 
@@ -22,13 +23,18 @@ public sealed class SmtpChannel(
         if (message.Recipients.Count == 0)
             return ChannelResult.Skip("No recipients.");
 
-        var options = await settingsProvider.GetEffectiveAsync(cancellationToken);
-
         try
         {
+            var options = await settingsProvider.GetEffectiveAsync(cancellationToken);
+            logger.LogInformation(
+                "SMTP credentials: UsernameConfigured={UsernameConfigured}, PasswordAvailable={PasswordAvailable}, OAuth2Configured={OAuth2Configured}",
+                !string.IsNullOrWhiteSpace(options.Username), !string.IsNullOrEmpty(options.Password), options.OAuth2 is not null);
+            if (options.OAuth2 is null && !string.IsNullOrWhiteSpace(options.Username) && string.IsNullOrEmpty(options.Password))
+                throw new InvalidOperationException("Указан SMTP-логин, но пароль отсутствует.");
             var mimeMessage = BuildMimeMessage(message, options);
 
             using var client = new SmtpClient();
+            client.CheckCertificateRevocation = options.CheckCertificateRevocation;
 
             var secureSocketOptions = options.UseSsl
                 ? SecureSocketOptions.SslOnConnect
@@ -51,6 +57,8 @@ public sealed class SmtpChannel(
                 await client.AuthenticateAsync(options.Username, options.Password, cancellationToken);
             }
 
+            logger.LogInformation("SMTP connection: IsSecure={IsSecure}, IsAuthenticated={IsAuthenticated}",
+                client.IsSecure, client.IsAuthenticated);
             await client.SendAsync(mimeMessage, cancellationToken);
             await client.DisconnectAsync(quit: true, cancellationToken);
 
@@ -86,7 +94,27 @@ public sealed class SmtpChannel(
             _ => TextFormat.Plain
         };
 
-        mime.Body = new TextPart(textFormat) { Text = message.Body };
+        if (message.BodyFormat == BodyFormat.Html)
+        {
+            var builder = new BodyBuilder();
+            var totalBytes = 0;
+            builder.HtmlBody = Regex.Replace(message.Body,
+                "src=\"(data:image/(?:png|jpeg|gif);base64,[A-Za-z0-9+/=]+)\"",
+                match =>
+                {
+                    if (!EmbeddedEmailImage.TryRead(match.Groups[1].Value, out var mimeType, out var bytes))
+                        throw new InvalidOperationException("Некорректное вложенное изображение.");
+                    totalBytes += bytes.Length;
+                    if (totalBytes > EmbeddedEmailImage.MaxTotalBytes)
+                        throw new InvalidOperationException("Общий размер изображений превышает 512 КБ.");
+                    var resource = builder.LinkedResources.Add("image", bytes, ContentType.Parse(mimeType));
+                    resource.ContentId = MimeKit.Utils.MimeUtils.GenerateMessageId();
+                    return $"src=\"cid:{resource.ContentId}\"";
+                }, RegexOptions.CultureInvariant, TimeSpan.FromSeconds(5));
+            mime.Body = builder.ToMessageBody();
+        }
+        else
+            mime.Body = new TextPart(textFormat) { Text = message.Body };
 
         if (message.Priority == NotificationPriority.Critical || message.Priority == NotificationPriority.High)
             mime.Priority = MessagePriority.Urgent;
`````

### `a/src/MailGate.Contracts/Notifications/NotificationLogDto.cs b/src/MailGate.Contracts/Notifications/NotificationLogDto.cs`

`````diff
diff --git a/src/MailGate.Contracts/Notifications/NotificationLogDto.cs b/src/MailGate.Contracts/Notifications/NotificationLogDto.cs
index d3bc98c..30f405d 100644
--- a/src/MailGate.Contracts/Notifications/NotificationLogDto.cs
+++ b/src/MailGate.Contracts/Notifications/NotificationLogDto.cs
@@ -35,6 +35,9 @@ public sealed record NotificationLogDetailsDto(
 
 public sealed class SendNotificationFromAdminDto
 {
+    public MailGate.Contracts.Templates.EmailTemplateDefinition? TemplateDefinition { get; set; }
+    public Guid? TemplateId { get; set; }
+    public Guid? TemplateRevision { get; set; }
     public ChannelType Channel { get; set; } = ChannelType.Smtp;
     public IReadOnlyList<Guid> RecipientIds { get; set; } = [];
     public IReadOnlyList<Guid> RecipientGroupIds { get; set; } = [];
`````

### `a/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs b/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs`

`````diff
diff --git a/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs b/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs
index a3997aa..afd46a3 100644
--- a/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs
+++ b/src/MailGate.Contracts/Smtp/SmtpConfigurationDto.cs
@@ -1,3 +1,5 @@
+using MailGate.Core.Enums;
+
 namespace MailGate.Contracts.Smtp;
 
 public enum SmtpConfigurationSource
@@ -11,13 +13,15 @@ public sealed record SmtpConfigurationDto(
     int Port,
     bool UseSsl,
     bool UseTls,
+    bool CheckCertificateRevocation,
     string FromAddress,
     string FromName,
     string? Username,
     bool HasPassword,
     SmtpOAuth2Dto? OAuth2,
     SmtpConfigurationSource Source,
-    DateTimeOffset? UpdatedAt);
+    DateTimeOffset? UpdatedAt,
+    SmtpCredentialSource CredentialSource = SmtpCredentialSource.Legacy);
 
 public sealed record SmtpOAuth2Dto(
     string TenantId,
@@ -27,10 +31,12 @@ public sealed record SmtpOAuth2Dto(
 
 public sealed class UpdateSmtpConfigurationDto
 {
+    public SmtpCredentialSource CredentialSource { get; set; } = SmtpCredentialSource.Manual;
     public string Host { get; set; } = string.Empty;
     public int Port { get; set; } = 587;
     public bool UseSsl { get; set; }
     public bool UseTls { get; set; } = true;
+    public bool CheckCertificateRevocation { get; set; } = true;
     public string FromAddress { get; set; } = string.Empty;
     public string FromName { get; set; } = string.Empty;
     public string? Username { get; set; }
`````

### `a/src/MailGate.Contracts/Templates/EmailTemplateDto.cs b/src/MailGate.Contracts/Templates/EmailTemplateDto.cs`

`````diff
diff --git a/src/MailGate.Contracts/Templates/EmailTemplateDto.cs b/src/MailGate.Contracts/Templates/EmailTemplateDto.cs
new file mode 100644
index 0000000..9067d88
--- /dev/null
+++ b/src/MailGate.Contracts/Templates/EmailTemplateDto.cs
@@ -0,0 +1,45 @@
+namespace MailGate.Contracts.Templates;
+
+public sealed class EmailTemplateDefinition
+{
+    public string Name { get; set; } = string.Empty;
+    public string Subject { get; set; } = string.Empty;
+    public string Preheader { get; set; } = string.Empty;
+    public string AccentColor { get; set; } = "#1677ff";
+    public string BackgroundColor { get; set; } = "#f3f5f7";
+    public string Footer { get; set; } = string.Empty;
+    public List<EmailTemplateBlock> Blocks { get; set; } = [];
+}
+
+public sealed class EmailTemplateBlock
+{
+    public string Type { get; set; } = "text";
+    public string Text { get; set; } = string.Empty;
+    public string SecondaryText { get; set; } = string.Empty;
+    public string Url { get; set; } = string.Empty;
+}
+
+public sealed record EmailTemplateDto(Guid Id, EmailTemplateDefinition Definition, Guid Revision,
+    DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
+public sealed record EmailTemplateSummaryDto(Guid Id, string Name, string Subject, Guid Revision, DateTimeOffset UpdatedAt);
+public sealed class SaveEmailTemplateDto
+{
+    public EmailTemplateDefinition Definition { get; set; } = new();
+    public Guid? Revision { get; set; }
+}
+public sealed class PreviewEmailTemplateDto
+{
+    public EmailTemplateDefinition Definition { get; set; } = new();
+    public string Body { get; set; } = string.Empty;
+    public string? Subject { get; set; }
+}
+public sealed class PreviewNotificationDto
+{
+    public EmailTemplateDefinition? TemplateDefinition { get; set; }
+    public Guid? TemplateId { get; set; }
+    public Guid? TemplateRevision { get; set; }
+    public string Body { get; set; } = string.Empty;
+    public string? Subject { get; set; }
+    public MailGate.Core.Enums.BodyFormat BodyFormat { get; set; }
+}
+public sealed record EmailPreviewDto(string Subject, string Html);
`````

### `a/src/MailGate.Core/Entities/EmailTemplate.cs b/src/MailGate.Core/Entities/EmailTemplate.cs`

`````diff
diff --git a/src/MailGate.Core/Entities/EmailTemplate.cs b/src/MailGate.Core/Entities/EmailTemplate.cs
new file mode 100644
index 0000000..b59f335
--- /dev/null
+++ b/src/MailGate.Core/Entities/EmailTemplate.cs
@@ -0,0 +1,11 @@
+namespace MailGate.Core.Entities;
+
+public sealed class EmailTemplate
+{
+    public Guid Id { get; set; } = Guid.NewGuid();
+    public string Name { get; set; } = string.Empty;
+    public string DefinitionJson { get; set; } = string.Empty;
+    public Guid Revision { get; set; } = Guid.NewGuid();
+    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
+    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
+}
`````

### `a/src/MailGate.Core/Entities/SmtpConfiguration.cs b/src/MailGate.Core/Entities/SmtpConfiguration.cs`

`````diff
diff --git a/src/MailGate.Core/Entities/SmtpConfiguration.cs b/src/MailGate.Core/Entities/SmtpConfiguration.cs
index feeca7f..2f158e2 100644
--- a/src/MailGate.Core/Entities/SmtpConfiguration.cs
+++ b/src/MailGate.Core/Entities/SmtpConfiguration.cs
@@ -1,3 +1,5 @@
+using MailGate.Core.Enums;
+
 namespace MailGate.Core.Entities;
 
 /// <summary>
@@ -7,11 +9,13 @@ namespace MailGate.Core.Entities;
 /// </summary>
 public sealed class SmtpConfiguration
 {
+    public SmtpCredentialSource CredentialSource { get; set; } = SmtpCredentialSource.Legacy;
     public Guid Id { get; set; } = Guid.NewGuid();
     public string Host { get; set; } = string.Empty;
     public int Port { get; set; } = 587;
     public bool UseSsl { get; set; }
     public bool UseTls { get; set; } = true;
+    public bool CheckCertificateRevocation { get; set; } = true;
     public string FromAddress { get; set; } = string.Empty;
     public string FromName { get; set; } = "MailGate";
     public string? Username { get; set; }
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

### `a/src/MailGate.Core/Models/EmbeddedEmailImage.cs b/src/MailGate.Core/Models/EmbeddedEmailImage.cs`

`````diff
diff --git a/src/MailGate.Core/Models/EmbeddedEmailImage.cs b/src/MailGate.Core/Models/EmbeddedEmailImage.cs
new file mode 100644
index 0000000..55c7eda
--- /dev/null
+++ b/src/MailGate.Core/Models/EmbeddedEmailImage.cs
@@ -0,0 +1,35 @@
+namespace MailGate.Core.Models;
+
+/// <summary>Bounded raster images stored as data URLs inside template JSON.</summary>
+public static class EmbeddedEmailImage
+{
+    public const int MaxBytes = 128 * 1024;
+    public const int MaxTotalBytes = 512 * 1024;
+
+    public static bool TryRead(string value, out string mimeType, out byte[] bytes)
+    {
+        mimeType = string.Empty;
+        bytes = [];
+        if (value.Length > MaxBytes * 4 / 3 + 100) return false;
+        foreach (var type in new[] { "image/png", "image/jpeg", "image/gif" })
+        {
+            var prefix = $"data:{type};base64,";
+            if (!value.StartsWith(prefix, StringComparison.Ordinal)) continue;
+            try { bytes = Convert.FromBase64String(value[prefix.Length..]); }
+            catch (FormatException) { return false; }
+            if (bytes.Length is < 10 or > MaxBytes) return false;
+            // The SMTP converter expects canonical base64 without whitespace.
+            if (value[prefix.Length..] != Convert.ToBase64String(bytes)) return false;
+            var valid = type switch
+            {
+                "image/png" => bytes.AsSpan().StartsWith(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 }),
+                "image/jpeg" => bytes[0] == 255 && bytes[1] == 216 && bytes[2] == 255,
+                _ => bytes.AsSpan().StartsWith("GIF87a"u8) || bytes.AsSpan().StartsWith("GIF89a"u8)
+            };
+            if (!valid) return false;
+            mimeType = type;
+            return true;
+        }
+        return false;
+    }
+}
`````

### `a/src/MailGate.Frontend/.dockerignore b/src/MailGate.Frontend/.dockerignore`

`````diff
diff --git a/src/MailGate.Frontend/.dockerignore b/src/MailGate.Frontend/.dockerignore
new file mode 100644
index 0000000..bde2488
--- /dev/null
+++ b/src/MailGate.Frontend/.dockerignore
@@ -0,0 +1,9 @@
+.npmrc
+.env
+.env.*
+node_modules/
+dist/
+*.tsbuildinfo
+
+**/account.json
+secrets/
`````

### `a/src/MailGate.Frontend/.npmrc.example b/src/MailGate.Frontend/.npmrc.example`

`````diff
diff --git a/src/MailGate.Frontend/.npmrc.example b/src/MailGate.Frontend/.npmrc.example
new file mode 100644
index 0000000..2258701
--- /dev/null
+++ b/src/MailGate.Frontend/.npmrc.example
@@ -0,0 +1,2 @@
+registry=https://nexus.example.local/repository/npm-group/
+//nexus.example.local/repository/npm-group/:_authToken=${NPM_TOKEN}
`````

### `a/frontend/Dockerfile b/src/MailGate.Frontend/Dockerfile`

`````diff
diff --git a/frontend/Dockerfile b/src/MailGate.Frontend/Dockerfile
similarity index 71%
rename from frontend/Dockerfile
rename to src/MailGate.Frontend/Dockerfile
index f18f688..9f944c0 100644
--- a/frontend/Dockerfile
+++ b/src/MailGate.Frontend/Dockerfile
@@ -2,12 +2,15 @@ FROM node:22-alpine AS build
 WORKDIR /app
 
 COPY package.json package-lock.json ./
-RUN npm ci
+RUN --mount=type=secret,id=npmrc,target=/root/.npmrc \
+    --mount=type=secret,id=npm_token,env=NPM_TOKEN \
+    npm ci
 
 COPY . .
 RUN npm run build
 
 FROM nginx:1.27-alpine AS final
+RUN apk upgrade --no-cache libxml2
 COPY nginx.conf /etc/nginx/conf.d/default.conf
 COPY --from=build /app/dist /usr/share/nginx/html
 COPY docker-entrypoint.sh /docker-entrypoint-custom.sh
`````

### `a/frontend/docker-entrypoint.sh b/src/MailGate.Frontend/docker-entrypoint.sh`

`````diff
diff --git a/frontend/docker-entrypoint.sh b/src/MailGate.Frontend/docker-entrypoint.sh
similarity index 88%
rename from frontend/docker-entrypoint.sh
rename to src/MailGate.Frontend/docker-entrypoint.sh
index b8a3489..4a82e6f 100644
--- a/frontend/docker-entrypoint.sh
+++ b/src/MailGate.Frontend/docker-entrypoint.sh
@@ -8,7 +8,7 @@ cat > /usr/share/nginx/html/config.json <<EOF
   "keycloak": {
     "authority": "${KEYCLOAK_AUTHORITY:-http://keycloak.localhost:8180/realms/mailgate}",
     "clientId": "${KEYCLOAK_CLIENT_ID:-mailgate-admin}",
-    "adminRole": "${KEYCLOAK_ADMIN_ROLE:-mailgate-admin}"
+    "adminGroup": "${KEYCLOAK_ADMIN_GROUP:-}"
   }
 }
 EOF
`````

### `a/frontend/index.html b/src/MailGate.Frontend/index.html`

`````diff
diff --git a/frontend/index.html b/src/MailGate.Frontend/index.html
similarity index 100%
rename from frontend/index.html
rename to src/MailGate.Frontend/index.html
`````

### `a/frontend/nginx.conf b/src/MailGate.Frontend/nginx.conf`

`````diff
diff --git a/frontend/nginx.conf b/src/MailGate.Frontend/nginx.conf
similarity index 100%
rename from frontend/nginx.conf
rename to src/MailGate.Frontend/nginx.conf
`````

### `a/frontend/package-lock.json b/src/MailGate.Frontend/package-lock.json`

`````diff
diff --git a/frontend/package-lock.json b/src/MailGate.Frontend/package-lock.json
similarity index 100%
rename from frontend/package-lock.json
rename to src/MailGate.Frontend/package-lock.json
`````

### `a/frontend/package.json b/src/MailGate.Frontend/package.json`

`````diff
diff --git a/frontend/package.json b/src/MailGate.Frontend/package.json
similarity index 100%
rename from frontend/package.json
rename to src/MailGate.Frontend/package.json
`````

### `a/frontend/public/config.json b/src/MailGate.Frontend/public/config.json`

`````diff
diff --git a/frontend/public/config.json b/src/MailGate.Frontend/public/config.json
similarity index 83%
rename from frontend/public/config.json
rename to src/MailGate.Frontend/public/config.json
index 0fd834b..d3da1df 100644
--- a/frontend/public/config.json
+++ b/src/MailGate.Frontend/public/config.json
@@ -3,6 +3,6 @@
   "keycloak": {
     "authority": "http://keycloak.localhost:8180/realms/mailgate",
     "clientId": "mailgate-admin",
-    "adminRole": "mailgate-admin"
+    "adminGroup": ""
   }
 }
`````

### `a/frontend/src/App.tsx b/src/MailGate.Frontend/src/App.tsx`

`````diff
diff --git a/frontend/src/App.tsx b/src/MailGate.Frontend/src/App.tsx
similarity index 88%
rename from frontend/src/App.tsx
rename to src/MailGate.Frontend/src/App.tsx
index 04a2245..ca81943 100644
--- a/frontend/src/App.tsx
+++ b/src/MailGate.Frontend/src/App.tsx
@@ -16,6 +16,8 @@ import { RecipientGroupsPage } from './pages/RecipientGroupsPage';
 import { RecipientGroupEditPage } from './pages/RecipientGroupEditPage';
 import { SmtpSettingsPage } from './pages/SmtpSettingsPage';
 import { DebugClaimsPage } from './pages/DebugClaimsPage';
+import { EmailTemplatesPage } from './pages/EmailTemplatesPage';
+import { EmailTemplateEditPage } from './pages/EmailTemplateEditPage';
 import { NotFoundPage } from './pages/NotFoundPage';
 
 dayjs.locale('ru');
@@ -52,6 +54,9 @@ function ThemedApp() {
               <Route path="/recipient-groups" element={<RecipientGroupsPage />} />
               <Route path="/recipient-groups/new" element={<RecipientGroupEditPage />} />
               <Route path="/recipient-groups/:id/edit" element={<RecipientGroupEditPage />} />
+              <Route path="/templates" element={<EmailTemplatesPage />} />
+              <Route path="/templates/new" element={<EmailTemplateEditPage />} />
+              <Route path="/templates/:id/edit" element={<EmailTemplateEditPage />} />
               <Route path="/smtp" element={<SmtpSettingsPage />} />
               <Route path="/debug/claims" element={<DebugClaimsPage />} />
               <Route path="/not-found" element={<NotFoundPage />} />
`````

### `a/frontend/src/api/client.ts b/src/MailGate.Frontend/src/api/client.ts`

`````diff
diff --git a/frontend/src/api/client.ts b/src/MailGate.Frontend/src/api/client.ts
similarity index 100%
rename from frontend/src/api/client.ts
rename to src/MailGate.Frontend/src/api/client.ts
`````

### `a/frontend/src/api/endpoints.ts b/src/MailGate.Frontend/src/api/endpoints.ts`

`````diff
diff --git a/frontend/src/api/endpoints.ts b/src/MailGate.Frontend/src/api/endpoints.ts
similarity index 77%
rename from frontend/src/api/endpoints.ts
rename to src/MailGate.Frontend/src/api/endpoints.ts
index 28642cf..bad3de1 100644
--- a/frontend/src/api/endpoints.ts
+++ b/src/MailGate.Frontend/src/api/endpoints.ts
@@ -1,5 +1,6 @@
 import { api, toQuery } from './client';
 import type {
+  EmailTemplateDto, EmailTemplateSummaryDto, SaveEmailTemplateDto, EmailPreviewDto, PreviewNotificationDto, EmailTemplateDefinition,
   AcceptedNotificationDto,
   CreateRecipientDto,
   CreateRecipientGroupDto,
@@ -29,6 +30,7 @@ export interface PageParams {
 }
 
 export const notificationsApi = {
+  preview: (dto: PreviewNotificationDto) => api.post<EmailPreviewDto>('/api/admin/notifications/preview', dto),
   list: (params: PageParams & { status?: DeliveryStatus }) =>
     api.get<PagedResult<NotificationLogDto>>(`/api/admin/notifications${toQuery(params)}`),
   stats: () => api.get<NotificationStatsDto>('/api/admin/notifications/stats'),
@@ -79,3 +81,12 @@ export const smtpApi = {
   testConnection: (dto: UpdateSmtpConfigurationDto) =>
     api.post<SmtpTestResultDto>('/api/admin/smtp/test-connection', dto),
 };
+
+export const emailTemplatesApi = {
+  list: (params: PageParams) => api.get<PagedResult<EmailTemplateSummaryDto>>(`/api/admin/email-templates${toQuery(params)}`),
+  get: (id: string) => api.get<EmailTemplateDto>(`/api/admin/email-templates/${id}`),
+  create: (dto: SaveEmailTemplateDto) => api.post<EmailTemplateDto>('/api/admin/email-templates', dto),
+  update: (id: string, dto: SaveEmailTemplateDto) => api.put<EmailTemplateDto>(`/api/admin/email-templates/${id}`, dto),
+  remove: (id: string) => api.delete(`/api/admin/email-templates/${id}`),
+  preview: (definition: EmailTemplateDefinition, body: string) => api.post<EmailPreviewDto>('/api/admin/email-templates/preview', { definition, body }),
+};
`````

### `a/frontend/src/api/types.ts b/src/MailGate.Frontend/src/api/types.ts`

`````diff
diff --git a/frontend/src/api/types.ts b/src/MailGate.Frontend/src/api/types.ts
similarity index 73%
rename from frontend/src/api/types.ts
rename to src/MailGate.Frontend/src/api/types.ts
index 57d9427..f787ae9 100644
--- a/frontend/src/api/types.ts
+++ b/src/MailGate.Frontend/src/api/types.ts
@@ -36,6 +36,9 @@ export interface NotificationLogDetailsDto extends NotificationLogDto {
 }
 
 export interface SendNotificationFromAdminDto {
+  templateId?: string | null;
+  templateRevision?: string | null;
+  templateDefinition?: EmailTemplateDefinition | null;
   channel: ChannelType;
   recipientIds: string[];
   recipientGroupIds: string[];
@@ -140,11 +143,15 @@ export interface SmtpOAuth2Dto {
   smtpUserPrincipalName: string;
 }
 
+export type SmtpCredentialSource = 'Manual' | 'Sidecar' | 'Legacy';
+
 export interface SmtpConfigurationDto {
+  credentialSource: SmtpCredentialSource;
   host: string;
   port: number;
   useSsl: boolean;
   useTls: boolean;
+  checkCertificateRevocation: boolean;
   fromAddress: string;
   fromName: string;
   username: string | null;
@@ -162,10 +169,12 @@ export interface UpdateSmtpOAuth2Dto {
 }
 
 export interface UpdateSmtpConfigurationDto {
+  credentialSource: SmtpCredentialSource;
   host: string;
   port: number;
   useSsl: boolean;
   useTls: boolean;
+  checkCertificateRevocation: boolean;
   fromAddress: string;
   fromName: string;
   username: string | null;
@@ -178,3 +187,33 @@ export interface SmtpTestResultDto {
   errorMessage: string | null;
   durationMs: number;
 }
+
+export type EmailBlockType = 'heading' | 'text' | 'content' | 'button' | 'divider' | 'columns' | 'image';
+export interface EmailTemplateBlock { type: EmailBlockType; text: string; secondaryText: string; url: string }
+export interface EmailTemplateDefinition {
+  name: string;
+  subject: string;
+  preheader: string;
+  accentColor: string;
+  backgroundColor: string;
+  footer: string;
+  blocks: EmailTemplateBlock[];
+}
+export interface EmailTemplateDto {
+  id: string;
+  definition: EmailTemplateDefinition;
+  revision: string;
+  createdAt: string;
+  updatedAt: string;
+}
+export interface EmailTemplateSummaryDto { id: string; name: string; subject: string; revision: string; updatedAt: string }
+export interface SaveEmailTemplateDto { definition: EmailTemplateDefinition; revision?: string }
+export interface EmailPreviewDto { subject: string; html: string }
+export interface PreviewNotificationDto {
+  templateId?: string | null;
+  templateRevision?: string | null;
+  templateDefinition?: EmailTemplateDefinition | null;
+  subject?: string | null;
+  body: string;
+  bodyFormat: BodyFormat;
+}
`````

### `a/frontend/src/auth/RequireAuth.tsx b/src/MailGate.Frontend/src/auth/RequireAuth.tsx`

`````diff
diff --git a/frontend/src/auth/RequireAuth.tsx b/src/MailGate.Frontend/src/auth/RequireAuth.tsx
similarity index 77%
rename from frontend/src/auth/RequireAuth.tsx
rename to src/MailGate.Frontend/src/auth/RequireAuth.tsx
index 0cc3c0f..0055d02 100644
--- a/frontend/src/auth/RequireAuth.tsx
+++ b/src/MailGate.Frontend/src/auth/RequireAuth.tsx
@@ -1,7 +1,7 @@
 import { useEffect, type ReactNode } from 'react';
 import { useAuth } from 'react-oidc-context';
 import { Button, Result, Spin } from 'antd';
-import { hasAdminRole } from './oidc';
+import { hasAdminGroup } from './oidc';
 
 function CenteredSpin() {
   return (
@@ -16,7 +16,7 @@ export function RequireAuth({ children }: { children: ReactNode }) {
 
   useEffect(() => {
     if (!auth.isLoading && !auth.isAuthenticated && !auth.activeNavigator && !auth.error) {
-      void auth.signinRedirect({ state: window.location.pathname + window.location.search });
+      void auth.signinRedirect({ state: window.location.pathname + window.location.search + window.location.hash });
     }
   }, [auth]);
 
@@ -31,7 +31,7 @@ export function RequireAuth({ children }: { children: ReactNode }) {
         title="Ошибка аутентификации"
         subTitle={auth.error.message}
         extra={
-          <Button type="primary" onClick={() => void auth.signinRedirect()}>
+          <Button type="primary" onClick={() => void auth.signinRedirect({ state: window.location.pathname + window.location.search + window.location.hash })}>
             Войти снова
           </Button>
         }
@@ -43,12 +43,12 @@ export function RequireAuth({ children }: { children: ReactNode }) {
     return <CenteredSpin />;
   }
 
-  if (!hasAdminRole(auth.user?.access_token)) {
+  if (!hasAdminGroup(auth.user?.access_token)) {
     return (
       <Result
         status="403"
         title="Нет доступа"
-        subTitle={`Учётной записи ${auth.user?.profile.preferred_username ?? ''} не назначена роль администратора MailGate. Обратитесь к администратору Keycloak.`}
+        subTitle={`Учётной записи ${auth.user?.profile.preferred_username ?? ''} нет членства в разрешённой группе MailGate. Обратитесь к администратору Keycloak.`}
         extra={
           <Button onClick={() => void auth.signoutRedirect()}>Выйти</Button>
         }
`````

### `a/frontend/src/auth/oidc.ts b/src/MailGate.Frontend/src/auth/oidc.ts`

`````diff
diff --git a/frontend/src/auth/oidc.ts b/src/MailGate.Frontend/src/auth/oidc.ts
similarity index 64%
rename from frontend/src/auth/oidc.ts
rename to src/MailGate.Frontend/src/auth/oidc.ts
index 8b5cec5..a063e09 100644
--- a/frontend/src/auth/oidc.ts
+++ b/src/MailGate.Frontend/src/auth/oidc.ts
@@ -1,6 +1,7 @@
 import { User, WebStorageStateStore } from 'oidc-client-ts';
 import type { AuthProviderProps } from 'react-oidc-context';
 import { getConfig } from '../config';
+import { getSigninReturnPath } from './returnPath';
 
 export function buildOidcConfig(): AuthProviderProps {
   const { keycloak } = getConfig();
@@ -12,9 +13,10 @@ export function buildOidcConfig(): AuthProviderProps {
     scope: 'openid',
     automaticSilentRenew: true,
     userStore: new WebStorageStateStore({ store: window.sessionStorage }),
-    onSigninCallback: () => {
-      // Убираем code/state из адресной строки после редиректа Keycloak
-      window.history.replaceState({}, document.title, window.location.pathname);
+    onSigninCallback: (user) => {
+      const path = getSigninReturnPath(user?.state, window.location.origin);
+      window.history.replaceState({}, document.title, path);
+      window.dispatchEvent(new PopStateEvent('popstate'));
     },
   };
 }
@@ -31,6 +33,7 @@ export function getAccessToken(): string | null {
 }
 
 interface AccessTokenPayload {
+  groups?: unknown;
   realm_access?: { roles?: string[] };
   roles?: string[];
   [claim: string]: unknown;
@@ -51,7 +54,20 @@ export function extractRoles(accessToken: string | undefined): string[] {
   }
 }
 
-export function hasAdminRole(accessToken: string | undefined): boolean {
-  const { keycloak } = getConfig();
-  return extractRoles(accessToken).includes(keycloak.adminRole);
+export function extractGroups(accessToken: string | undefined): string[] {
+  if (!accessToken) return [];
+  try {
+    const groups = parseJwtPayload(accessToken).groups;
+    return Array.isArray(groups)
+      ? groups.filter((group): group is string => typeof group === 'string')
+      : [];
+  } catch {
+    return [];
+  }
+}
+
+export function hasAdminGroup(accessToken: string | undefined): boolean {
+  const { adminGroup } = getConfig().keycloak;
+  return typeof adminGroup === 'string' && adminGroup.trim().length > 0
+    && extractGroups(accessToken).includes(adminGroup);
 }
`````

### `a/src/MailGate.Frontend/src/auth/returnPath.ts b/src/MailGate.Frontend/src/auth/returnPath.ts`

`````diff
diff --git a/src/MailGate.Frontend/src/auth/returnPath.ts b/src/MailGate.Frontend/src/auth/returnPath.ts
new file mode 100644
index 0000000..66f65d7
--- /dev/null
+++ b/src/MailGate.Frontend/src/auth/returnPath.ts
@@ -0,0 +1,9 @@
+export function getSigninReturnPath(state: unknown, origin: string): string {
+  if (typeof state !== 'string' || !state.startsWith('/') || state.startsWith('//')) return '/';
+  try {
+    const target = new URL(state, origin);
+    return target.origin === origin ? target.pathname + target.search + target.hash : '/';
+  } catch {
+    return '/';
+  }
+}
`````

### `a/src/MailGate.Frontend/src/components/EmailPreview.tsx b/src/MailGate.Frontend/src/components/EmailPreview.tsx`

`````diff
diff --git a/src/MailGate.Frontend/src/components/EmailPreview.tsx b/src/MailGate.Frontend/src/components/EmailPreview.tsx
new file mode 100644
index 0000000..b3c7d4c
--- /dev/null
+++ b/src/MailGate.Frontend/src/components/EmailPreview.tsx
@@ -0,0 +1,29 @@
+import { useState } from 'react';
+import { Alert, Card, Empty, Segmented, Spin, Typography } from 'antd';
+import type { EmailPreviewDto } from '../api/types';
+
+interface Props { preview?: EmailPreviewDto; loading?: boolean; error?: Error | null }
+const csp = `<meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src 'unsafe-inline'; img-src data: https:; base-uri 'none'; form-action 'none'">`;
+
+export function EmailPreview({ preview, loading, error }: Props) {
+  const [mobile, setMobile] = useState(false);
+  return (
+    <Card title="Предпросмотр письма" extra={<Segmented size="small" value={mobile ? 'Телефон' : 'Компьютер'} options={['Компьютер', 'Телефон']} onChange={(value) => setMobile(value === 'Телефон')} />}
+      style={{ height: 'calc(100dvh - 40px)', display: 'flex', flexDirection: 'column' }}
+      styles={{ header: { flexShrink: 0 }, body: { flex: 1, minHeight: 0, display: 'flex', flexDirection: 'column', overflow: 'auto' } }}>
+      <div style={{ width: '100%', flex: 1, minHeight: 0, display: 'flex', flexDirection: 'column', gap: 16 }}>
+        <Typography.Text type="secondary">Браузерный предпросмотр. Отображение в Outlook и других почтовых клиентах может отличаться. Загруженные изображения встроены в письмо.</Typography.Text>
+        {error && <Alert type="error" title={error.message} />}
+        {loading && <Spin description="Обновляем письмо…" />}
+        {preview && !error ? <>
+          <Typography.Text><strong>Тема:</strong> {preview.subject || 'Без темы'}</Typography.Text>
+          <div style={{ background: '#eef0f3', padding: 8, overflow: 'auto', borderRadius: 8, flex: 1, minHeight: 120 }}>
+            {/* Keep the desktop viewport wider than the email's mobile breakpoint. */}
+            <iframe title="Содержимое письма" sandbox="" referrerPolicy="no-referrer" srcDoc={csp + preview.html}
+              style={{ display: 'block', width: mobile ? 375 : 680, height: '100%', margin: '0 auto', border: 0, background: 'white' }} />
+          </div>
+        </> : !loading && !error && <Empty description="Заполните сообщение, чтобы увидеть письмо" />}
+      </div>
+    </Card>
+  );
+}
`````

### `a/src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx b/src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx`

`````diff
diff --git a/src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx b/src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx
new file mode 100644
index 0000000..974c930
--- /dev/null
+++ b/src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx
@@ -0,0 +1,75 @@
+import { useState, type Dispatch, type SetStateAction } from 'react';
+import { App, Button, Card, Form, Input, Select, Space, Typography, Upload } from 'antd';
+import { ArrowDownOutlined, ArrowUpOutlined, CopyOutlined, DeleteOutlined, PlusOutlined } from '@ant-design/icons';
+import type { EmailBlockType, EmailTemplateBlock, EmailTemplateDefinition } from '../api/types';
+
+const labels: Record<EmailBlockType, string> = { heading: 'Заголовок', text: 'Текст', content: 'Основное сообщение', button: 'Кнопка', divider: 'Разделитель', columns: 'Две колонки', image: 'Изображение' };
+const block = (type: EmailBlockType): EmailTemplateBlock => ({ type, text: type === 'heading' ? 'Важная информация' : type === 'button' ? 'Открыть' : '', secondaryText: '', url: type === 'button' ? 'https://example.com' : '' });
+
+interface Props { definition: EmailTemplateDefinition; setDefinition: Dispatch<SetStateAction<EmailTemplateDefinition>>; onUploadingChange?: (uploading: boolean) => void; forMessage?: boolean }
+export function EmailTemplateEditor({ definition, setDefinition, onUploadingChange, forMessage = false }: Props) {
+  const { message } = App.useApp();
+  const [uploadingImage, setUploadingImageState] = useState(false);
+  const setUploadingImage = (value: boolean) => { setUploadingImageState(value); onUploadingChange?.(value); };
+  const [newType, setNewType] = useState<EmailBlockType>('text');
+  const set = <K extends keyof EmailTemplateDefinition>(key: K, value: EmailTemplateDefinition[K]) => setDefinition((current) => ({ ...current, [key]: value }));
+  const changeBlock = (index: number, patch: Partial<EmailTemplateBlock>) => setDefinition((current) => ({ ...current, blocks: current.blocks.map((value, i) => i === index ? { ...value, ...patch } : value) }));
+  const move = (index: number, offset: number) => setDefinition((current) => {
+    const blocks = [...current.blocks]; [blocks[index], blocks[index + offset]] = [blocks[index + offset], blocks[index]]; return { ...current, blocks };
+  });
+  return <fieldset disabled={uploadingImage} style={{ border: 0, padding: 0, margin: 0, minWidth: 0 }}><Space orientation="vertical" size="middle" style={{ width: '100%' }}>
+          <Card title="Оформление">
+            <div>
+              {!forMessage && <Form.Item label="Название шаблона" required><Input value={definition.name} maxLength={200} onChange={(e) => set('name', e.target.value)} /></Form.Item>}
+              <Form.Item label="Тема по умолчанию"><Input value={definition.subject} maxLength={500} onChange={(e) => set('subject', e.target.value)} placeholder="Можно изменить перед отправкой" /></Form.Item>
+              <Form.Item label="Прехедер" tooltip="Короткая строка рядом с темой в почтовом ящике"><Input value={definition.preheader} maxLength={500} onChange={(e) => set('preheader', e.target.value)} /></Form.Item>
+              <Space wrap size="large">
+                <Form.Item label="Акцент"><Input type="color" aria-label="Цвет акцента" value={definition.accentColor} onChange={(e) => set('accentColor', e.target.value)} style={{ width: 90 }} /></Form.Item>
+                <Form.Item label="Фон письма"><Input type="color" aria-label="Цвет фона" value={definition.backgroundColor} onChange={(e) => set('backgroundColor', e.target.value)} style={{ width: 90 }} /></Form.Item>
+              </Space>
+              <Form.Item label="Подпись / подвал"><Input.TextArea rows={2} maxLength={4000} value={definition.footer} onChange={(e) => set('footer', e.target.value)} /></Form.Item>
+            </div>
+          </Card>
+          <Card title="Блоки письма">
+            <Space orientation="vertical" style={{ width: '100%' }} size="middle">
+              {definition.blocks.map((item, index) => <Card size="small" key={index} title={`${index + 1}. ${labels[item.type]}`} extra={<Space size={0}>
+                <Button type="text" aria-label={`Блок ${index + 1}: выше`} icon={<ArrowUpOutlined />} disabled={index === 0} onClick={() => move(index, -1)} />
+                <Button type="text" aria-label={`Блок ${index + 1}: ниже`} icon={<ArrowDownOutlined />} disabled={index === definition.blocks.length - 1} onClick={() => move(index, 1)} />
+                {item.type !== 'content' && <Button type="text" aria-label={`Дублировать блок ${index + 1}`} icon={<CopyOutlined />} disabled={definition.blocks.length >= 40} onClick={() => set('blocks', [...definition.blocks.slice(0, index + 1), { ...item }, ...definition.blocks.slice(index + 1)])} />}
+                <Button type="text" danger aria-label={`Удалить блок ${index + 1}`} icon={<DeleteOutlined />} disabled={item.type === 'content'} onClick={() => set('blocks', definition.blocks.filter((_, i) => i !== index))} />
+              </Space>}>
+                {item.type === 'content' ? <Typography.Text type="secondary">В это место автоматически подставится основной текст рассылки. Можно переместить блок выше или ниже.</Typography.Text>
+                  : item.type === 'image' ? <Space orientation="vertical" style={{ width: '100%' }}>
+                    <Upload accept="image/png,image/jpeg,image/gif" showUploadList={false} disabled={uploadingImage} beforeUpload={async (file) => {
+                      if (file.size > 128 * 1024 || !['image/png', 'image/jpeg', 'image/gif'].includes(file.type)) {
+                        message.error('Выберите PNG, JPEG или GIF размером до 128 КБ'); return false;
+                      }
+                      setUploadingImage(true);
+                      try {
+                        const data = await new Promise<string>((resolve, reject) => {
+                          const reader = new FileReader();
+                          reader.onload = () => resolve(String(reader.result));
+                          reader.onerror = () => reject(new Error('Не удалось прочитать файл'));
+                          reader.readAsDataURL(file);
+                        });
+                        changeBlock(index, { url: data });
+                      } catch { message.error('Не удалось прочитать изображение'); }
+                      finally { setUploadingImage(false); }
+                      return false;
+                    }}><Button loading={uploadingImage}>{item.url ? 'Заменить изображение' : 'Загрузить с компьютера'}</Button></Upload>
+                    {item.url && <Typography.Text type="secondary">{item.url.startsWith('data:') ? (forMessage ? 'Изображение добавлено в это письмо.' : 'Изображение загружено. Сохраните шаблон.') : 'Ранее сохранённое изображение по ссылке. Можно заменить файлом.'}</Typography.Text>}
+                    <Input aria-label={`Описание изображения блока ${index + 1}`} maxLength={1000} value={item.text} onChange={(e) => changeBlock(index, { text: e.target.value })} placeholder="Описание, если почтовый клиент не показывает картинку (необязательно)" />
+                    <Typography.Text type="secondary">PNG, JPEG, GIF: до 128 КБ на файл, до 512 КБ на шаблон. Переместите блок вверх для шапки или вниз для подвала.</Typography.Text>
+                  </Space>
+                  : item.type === 'divider' ? <Typography.Text type="secondary">Горизонтальная линия между разделами.</Typography.Text>
+                  : <Space orientation="vertical" style={{ width: '100%' }}>
+                    <Input.TextArea aria-label={`Текст блока ${index + 1}`} rows={item.type === 'heading' || item.type === 'button' ? 2 : 4} maxLength={10000} value={item.text} onChange={(e) => changeBlock(index, { text: e.target.value })} placeholder={item.type === 'button' ? 'Текст кнопки' : item.type === 'columns' ? 'Левая колонка' : 'Текст блока'} />
+                    {item.type === 'columns' && <Input.TextArea aria-label={`Правая колонка блока ${index + 1}`} rows={4} maxLength={10000} value={item.secondaryText} onChange={(e) => changeBlock(index, { secondaryText: e.target.value })} placeholder="Правая колонка" />}
+                    {item.type === 'button' && <Input aria-label={`Ссылка блока ${index + 1}`} maxLength={2000} value={item.url} onChange={(e) => changeBlock(index, { url: e.target.value })} placeholder="https://…" />}
+                  </Space>}
+              </Card>)}
+              <Space wrap><Select aria-label="Тип нового блока" value={newType} onChange={setNewType} style={{ width: 200 }} options={Object.entries(labels).filter(([type]) => type !== 'content').map(([value, label]) => ({ value, label }))} /><Button icon={<PlusOutlined />} disabled={definition.blocks.length >= 40} onClick={() => set('blocks', [...definition.blocks, block(newType)])}>Добавить блок</Button></Space>
+            </Space>
+          </Card>
+    </Space></fieldset>;
+}
`````

### `a/frontend/src/config.ts b/src/MailGate.Frontend/src/config.ts`

`````diff
diff --git a/frontend/src/config.ts b/src/MailGate.Frontend/src/config.ts
similarity index 96%
rename from frontend/src/config.ts
rename to src/MailGate.Frontend/src/config.ts
index 4621d93..6c02f90 100644
--- a/frontend/src/config.ts
+++ b/src/MailGate.Frontend/src/config.ts
@@ -3,7 +3,7 @@ export interface AppConfig {
   keycloak: {
     authority: string;
     clientId: string;
-    adminRole: string;
+    adminGroup: string;
   };
 }
 
`````

### `a/src/MailGate.Frontend/src/hooks/useDebounced.ts b/src/MailGate.Frontend/src/hooks/useDebounced.ts`

`````diff
diff --git a/src/MailGate.Frontend/src/hooks/useDebounced.ts b/src/MailGate.Frontend/src/hooks/useDebounced.ts
new file mode 100644
index 0000000..a9e3779
--- /dev/null
+++ b/src/MailGate.Frontend/src/hooks/useDebounced.ts
@@ -0,0 +1,10 @@
+import { useEffect, useState } from 'react';
+
+export function useDebounced<T>(value: T, delay = 400): T {
+  const [debounced, setDebounced] = useState(value);
+  useEffect(() => {
+    const timer = setTimeout(() => setDebounced(value), delay);
+    return () => clearTimeout(timer);
+  }, [value, delay]);
+  return debounced;
+}
`````

### `a/frontend/src/layout/AppLayout.tsx b/src/MailGate.Frontend/src/layout/AppLayout.tsx`

`````diff
diff --git a/frontend/src/layout/AppLayout.tsx b/src/MailGate.Frontend/src/layout/AppLayout.tsx
similarity index 96%
rename from frontend/src/layout/AppLayout.tsx
rename to src/MailGate.Frontend/src/layout/AppLayout.tsx
index 132bce6..e93f028 100644
--- a/frontend/src/layout/AppLayout.tsx
+++ b/src/MailGate.Frontend/src/layout/AppLayout.tsx
@@ -29,6 +29,7 @@ const menuItems = [
     children: [
       { key: '/notifications', icon: <UnorderedListOutlined />, label: 'Журнал' },
       { key: '/notifications/new', icon: <SendOutlined />, label: 'Новая рассылка' },
+      { key: '/templates', icon: <MailOutlined />, label: 'Шаблоны писем' },
     ],
   },
   { key: '/recipients', icon: <ContactsOutlined />, label: 'Получатели' },
@@ -52,6 +53,7 @@ export function AppLayout() {
 
   const selectedKey = useMemo(() => {
     const path = location.pathname;
+    if (path.startsWith('/templates')) return '/templates';
     if (path.startsWith('/recipient-groups')) return '/recipient-groups';
     if (path === '/notifications/new') return '/notifications/new';
     if (path.startsWith('/notifications')) return '/notifications';
`````

### `a/frontend/src/main.tsx b/src/MailGate.Frontend/src/main.tsx`

`````diff
diff --git a/frontend/src/main.tsx b/src/MailGate.Frontend/src/main.tsx
similarity index 100%
rename from frontend/src/main.tsx
rename to src/MailGate.Frontend/src/main.tsx
`````

### `a/frontend/src/pages/DashboardPage.tsx b/src/MailGate.Frontend/src/pages/DashboardPage.tsx`

`````diff
diff --git a/frontend/src/pages/DashboardPage.tsx b/src/MailGate.Frontend/src/pages/DashboardPage.tsx
similarity index 100%
rename from frontend/src/pages/DashboardPage.tsx
rename to src/MailGate.Frontend/src/pages/DashboardPage.tsx
`````

### `a/frontend/src/pages/DebugClaimsPage.tsx b/src/MailGate.Frontend/src/pages/DebugClaimsPage.tsx`

`````diff
diff --git a/frontend/src/pages/DebugClaimsPage.tsx b/src/MailGate.Frontend/src/pages/DebugClaimsPage.tsx
similarity index 81%
rename from frontend/src/pages/DebugClaimsPage.tsx
rename to src/MailGate.Frontend/src/pages/DebugClaimsPage.tsx
index 64d16cb..6bbe231 100644
--- a/frontend/src/pages/DebugClaimsPage.tsx
+++ b/src/MailGate.Frontend/src/pages/DebugClaimsPage.tsx
@@ -1,11 +1,12 @@
 import { Card, Descriptions, Tag, Typography } from 'antd';
 import { useAuth } from 'react-oidc-context';
-import { extractRoles, parseJwtPayload } from '../auth/oidc';
+import { extractGroups, extractRoles, parseJwtPayload } from '../auth/oidc';
 
 export function DebugClaimsPage() {
   const auth = useAuth();
   const token = auth.user?.access_token;
   const roles = extractRoles(token);
+  const groups = extractGroups(token);
 
   let payload: Record<string, unknown> = {};
   if (token) {
@@ -24,6 +25,9 @@ export function DebugClaimsPage() {
           <Descriptions.Item label="Пользователь">
             {auth.user?.profile.preferred_username ?? '—'}
           </Descriptions.Item>
+          <Descriptions.Item label="Группы">
+            {groups.length === 0 ? '—' : groups.map((group) => <Tag key={group}>{group}</Tag>)}
+          </Descriptions.Item>
           <Descriptions.Item label="Роли">
             {roles.length === 0 ? '—' : roles.map((r) => <Tag key={r}>{r}</Tag>)}
           </Descriptions.Item>
`````

### `a/src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx b/src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx`

`````diff
diff --git a/src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx b/src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx
new file mode 100644
index 0000000..a31dbe3
--- /dev/null
+++ b/src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx
@@ -0,0 +1,65 @@
+import { useEffect, useState } from 'react';
+import { Alert, App, Button, Card, Col, Input, Row, Space, Spin, Typography } from 'antd';
+import { SaveOutlined } from '@ant-design/icons';
+import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
+import { useNavigate, useParams } from 'react-router-dom';
+import { emailTemplatesApi } from '../api/endpoints';
+import type { EmailBlockType, EmailTemplateBlock, EmailTemplateDefinition, EmailTemplateDto } from '../api/types';
+import { EmailTemplateEditor } from '../components/EmailTemplateEditor';
+import { EmailPreview } from '../components/EmailPreview';
+import { useDebounced } from '../hooks/useDebounced';
+
+const block = (type: EmailBlockType): EmailTemplateBlock => ({ type, text: type === 'heading' ? 'Важная информация' : type === 'button' ? 'Открыть' : '', secondaryText: '', url: type === 'button' ? 'https://example.com' : '' });
+function preset(columns = false): EmailTemplateDefinition {
+  return { name: 'Новый шаблон', subject: 'Уведомление', preheader: '', accentColor: '#1677ff', backgroundColor: '#f3f5f7', footer: 'С уважением, команда поддержки',
+    blocks: [block('heading'), block('content'), ...(columns ? [{ ...block('columns'), text: 'Контакты поддержки', secondaryText: 'Время работы: пн–пт' }] : [])] };
+}
+
+export function EmailTemplateEditPage() {
+  const { id } = useParams<{ id: string }>();
+  const navigate = useNavigate();
+  const { message } = App.useApp();
+  const client = useQueryClient();
+  const [definition, setDefinition] = useState<EmailTemplateDefinition>(() => preset());
+  const [revision, setRevision] = useState<string>();
+  const [loadedId, setLoadedId] = useState<string>();
+  const [sample, setSample] = useState('Здравствуйте!\n\nЗдесь будет ваш основной текст сообщения.\nОн подставляется в макет при создании рассылки.');
+  const [uploadingImage, setUploadingImage] = useState(false);
+  const details = useQuery({ queryKey: ['email-templates', 'detail', id], queryFn: () => emailTemplatesApi.get(id!), enabled: !!id, refetchOnWindowFocus: false });
+  useEffect(() => {
+    if (details.data && loadedId !== id) { setDefinition(details.data.definition); setRevision(details.data.revision); setLoadedId(id); }
+  }, [details.data, loadedId, id]);
+  const previewInput = useDebounced(JSON.stringify({ definition, sample }));
+  const preview = useQuery({ queryKey: ['email-template-preview', previewInput], queryFn: () => {
+    const parsed = JSON.parse(previewInput) as { definition: EmailTemplateDefinition; sample: string };
+    return emailTemplatesApi.preview(parsed.definition, parsed.sample);
+  }, enabled: !id || loadedId === id, retry: false, gcTime: 0 });
+  const save = useMutation({ mutationFn: () => id
+    ? emailTemplatesApi.update(id, { definition, revision })
+    : emailTemplatesApi.create({ definition }),
+    onSuccess: (saved: EmailTemplateDto) => {
+      message.success('Шаблон сохранён'); setRevision(saved.revision);
+      client.setQueryData(['email-templates', 'detail', saved.id], saved);
+      void client.invalidateQueries({ queryKey: ['email-templates', 'list'] });
+      if (!id) navigate(`/templates/${saved.id}/edit`, { replace: true });
+    }, onError: (error: Error) => message.error(error.message),
+  });
+  if (id && (details.isLoading || (!details.isError && loadedId !== id))) return <Spin />;
+  if (details.isError) return <Alert type="error" title="Не удалось открыть шаблон" description={details.error.message} />;
+  return <div style={{ maxWidth: 1600 }}>
+    <Space wrap style={{ justifyContent: 'space-between', width: '100%', marginBottom: 20 }}>
+      <div><Typography.Title level={3} style={{ margin: 0 }}>{id ? 'Конструктор шаблона' : 'Новый шаблон письма'}</Typography.Title><Typography.Text type="secondary">Соберите макет из блоков. Основной текст будет введён при отправке.</Typography.Text></div>
+      <Space><Button onClick={() => navigate('/templates')}>К списку</Button><Button type="primary" icon={<SaveOutlined />} disabled={uploadingImage} loading={save.isPending} onClick={() => save.mutate()}>Сохранить шаблон</Button></Space>
+    </Space>
+    <Row gutter={[24, 24]}>
+      <Col xs={24} xl={12}>
+        <Space orientation="vertical" size="middle" style={{ width: '100%' }}>
+          <EmailTemplateEditor definition={definition} setDefinition={setDefinition} onUploadingChange={setUploadingImage} />
+          {!id && <Space wrap><Typography.Text type="secondary">Начать с макета:</Typography.Text><Button disabled={uploadingImage} onClick={() => setDefinition(preset())}>Одна колонка</Button><Button disabled={uploadingImage} onClick={() => setDefinition(preset(true))}>С двумя колонками</Button></Space>}
+          <Card title="Текст для проверки макета"><Input.TextArea aria-label="Пример сообщения" rows={5} maxLength={100000} value={sample} onChange={(e) => setSample(e.target.value)} /><Typography.Text type="secondary">Этот текст не сохраняется в шаблоне.</Typography.Text></Card>
+        </Space>
+      </Col>
+      <Col xs={24} xl={12}><div style={{ position: 'sticky', top: 20 }}><EmailPreview preview={preview.data} loading={preview.isFetching || previewInput !== JSON.stringify({ definition, sample })} error={preview.error} /></div></Col>
+    </Row>
+  </div>;
+}
`````

### `a/src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx b/src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx`

`````diff
diff --git a/src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx b/src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx
new file mode 100644
index 0000000..d225562
--- /dev/null
+++ b/src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx
@@ -0,0 +1,35 @@
+import { useState } from 'react';
+import { App, Button, Card, Input, Popconfirm, Space, Table, Typography } from 'antd';
+import { PlusOutlined } from '@ant-design/icons';
+import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
+import { Link } from 'react-router-dom';
+import { emailTemplatesApi } from '../api/endpoints';
+
+export function EmailTemplatesPage() {
+  const [search, setSearch] = useState('');
+  const [page, setPage] = useState(1);
+  const { message } = App.useApp();
+  const client = useQueryClient();
+  const query = useQuery({ queryKey: ['email-templates', 'list', search, page], queryFn: () => emailTemplatesApi.list({ search, page, pageSize: 20 }) });
+  const remove = useMutation({ mutationFn: emailTemplatesApi.remove, onSuccess: () => { void client.invalidateQueries({ queryKey: ['email-templates'] }); message.success('Шаблон удалён'); }, onError: (error: Error) => message.error(error.message) });
+  return <div style={{ maxWidth: 1200 }}>
+    <Space style={{ width: '100%', justifyContent: 'space-between', marginBottom: 16 }} wrap>
+      <Typography.Title level={3} style={{ margin: 0 }}>Шаблоны писем</Typography.Title>
+      <Link to="/templates/new"><Button type="primary" icon={<PlusOutlined />}>Создать шаблон</Button></Link>
+    </Space>
+    <Typography.Paragraph type="secondary">Сохраните оформление один раз, затем выбирайте его при создании рассылки и вводите только основной текст.</Typography.Paragraph>
+    <Card>
+      <Input.Search placeholder="Найти шаблон" allowClear onSearch={(value) => { setSearch(value); setPage(1); }} style={{ maxWidth: 360, marginBottom: 20 }} />
+      {query.isError && <Typography.Paragraph type="danger">{query.error.message}</Typography.Paragraph>}
+      <Table rowKey="id" loading={query.isLoading} dataSource={query.data?.items ?? []} scroll={{ x: 650 }}
+        locale={{ emptyText: 'Шаблонов пока нет. Создайте первый макет письма.' }}
+        pagination={{ current: page, pageSize: 20, total: query.data?.totalCount ?? 0, showSizeChanger: false, onChange: setPage }}
+        columns={[
+          { title: 'Название', dataIndex: 'name', render: (name, row) => <Link to={`/templates/${row.id}/edit`}>{name}</Link> },
+          { title: 'Тема по умолчанию', dataIndex: 'subject' },
+          { title: 'Изменён', dataIndex: 'updatedAt', render: (value: string) => new Date(value).toLocaleString('ru-RU') },
+          { title: 'Действия', render: (_, row) => <Space><Link to={`/templates/${row.id}/edit`}>Редактировать</Link><Popconfirm title="Удалить шаблон? Уже поставленные в очередь письма не изменятся." onConfirm={() => remove.mutate(row.id)}><Button type="link" danger loading={remove.isPending}>Удалить</Button></Popconfirm></Space> },
+        ]} />
+    </Card>
+  </div>;
+}
`````

### `a/frontend/src/pages/NotFoundPage.tsx b/src/MailGate.Frontend/src/pages/NotFoundPage.tsx`

`````diff
diff --git a/frontend/src/pages/NotFoundPage.tsx b/src/MailGate.Frontend/src/pages/NotFoundPage.tsx
similarity index 100%
rename from frontend/src/pages/NotFoundPage.tsx
rename to src/MailGate.Frontend/src/pages/NotFoundPage.tsx
`````

### `a/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx b/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx`

`````diff
diff --git a/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx b/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx
new file mode 100644
index 0000000..9cb1af6
--- /dev/null
+++ b/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx
@@ -0,0 +1,227 @@
+import { useState, type Dispatch, type SetStateAction } from 'react';
+import { Link } from 'react-router-dom';
+import { EmailTemplateEditor } from '../components/EmailTemplateEditor';
+import { EmailPreview } from '../components/EmailPreview';
+import { useDebounced } from '../hooks/useDebounced';
+import { App, Alert, Col, Row, Button, Card, Form, Input, Radio, Select, Space, Typography } from 'antd';
+import { SendOutlined } from '@ant-design/icons';
+import { useMutation, useQuery } from '@tanstack/react-query';
+import { useNavigate } from 'react-router-dom';
+import { emailTemplatesApi, notificationsApi, recipientGroupsApi, recipientsApi } from '../api/endpoints';
+import type { BodyFormat, EmailTemplateDefinition, NotificationPriority, SendNotificationFromAdminDto } from '../api/types';
+
+interface FormValues {
+  templateId?: string;
+  recipientIds: string[];
+  recipientGroupIds: string[];
+  adHocEmails: string[];
+  subject?: string;
+  body: string;
+  bodyFormat: BodyFormat;
+  priority: NotificationPriority;
+}
+
+const EMAIL_RE = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
+
+export function NotificationCreatePage() {
+  const { message } = App.useApp();
+  const navigate = useNavigate();
+  const [form] = Form.useForm<FormValues>();
+
+  const [draft, setDraft] = useState<{ id: string; revision: string; definition: EmailTemplateDefinition }>();
+  const [uploadingImage, setUploadingImage] = useState(false);
+  const [templateSearch, setTemplateSearch] = useState('');
+  const templateId = Form.useWatch('templateId', form);
+  const body = Form.useWatch('body', form) ?? '';
+  const subject = Form.useWatch('subject', form) ?? '';
+  const bodyFormat = Form.useWatch('bodyFormat', form) ?? 'PlainText';
+  const templates = useQuery({ queryKey: ['email-templates', 'select', templateSearch], queryFn: () => emailTemplatesApi.list({ search: templateSearch, pageSize: 100 }), refetchOnWindowFocus: true });
+  const template = useQuery({ queryKey: ['email-templates', 'detail', templateId], queryFn: () => emailTemplatesApi.get(templateId!), enabled: !!templateId, refetchOnWindowFocus: false });
+  const definition = draft && draft.id === templateId && draft.revision === template.data?.revision ? draft.definition : template.data?.definition;
+  const setDefinition: Dispatch<SetStateAction<EmailTemplateDefinition>> = (update) => {
+    if (!templateId || !template.data) return;
+    const selected = template.data;
+    setDraft((current) => {
+      const previous = current?.id === templateId && current.revision === selected.revision ? current.definition : selected.definition;
+      return { id: templateId, revision: selected.revision, definition: typeof update === 'function' ? update(previous) : update };
+    });
+  };
+  const previewRequest = JSON.stringify({ templateId: templateId ?? null, templateRevision: template.data?.revision ?? null, templateDefinition: templateId ? definition : null, body, subject: subject.trim() || null, bodyFormat });
+  const debouncedPreview = useDebounced(previewRequest);
+  const preview = useQuery({ queryKey: ['notification-preview', debouncedPreview], queryFn: () => notificationsApi.preview(JSON.parse(debouncedPreview)), enabled: !templateId || !!template.data, retry: false, gcTime: 0 });
+
+  const recipientsQuery = useQuery({
+    queryKey: ['recipients', 'all'],
+    queryFn: () => recipientsApi.list({ pageSize: 500 }),
+  });
+
+  const groupsQuery = useQuery({
+    queryKey: ['recipient-groups', 'all'],
+    queryFn: () => recipientGroupsApi.list({ pageSize: 500 }),
+  });
+
+  const sendMutation = useMutation({
+    mutationFn: (dto: SendNotificationFromAdminDto) => notificationsApi.send(dto),
+    onSuccess: () => {
+      message.success('Рассылка поставлена в очередь');
+      navigate('/notifications');
+    },
+    onError: (error: Error) => message.error(error.message),
+  });
+
+  const onFinish = (values: FormValues) => {
+    const invalidEmails = (values.adHocEmails ?? []).filter((e) => !EMAIL_RE.test(e.trim()));
+    if (invalidEmails.length > 0) {
+      message.error(`Некорректные адреса: ${invalidEmails.join(', ')}`);
+      return;
+    }
+
+    if (
+      (values.recipientIds?.length ?? 0) === 0 &&
+      (values.recipientGroupIds?.length ?? 0) === 0 &&
+      (values.adHocEmails?.length ?? 0) === 0
+    ) {
+      message.error('Укажите хотя бы одного получателя, группу или адрес');
+      return;
+    }
+
+    if (values.templateId && !template.data) { message.error('Дождитесь загрузки шаблона'); return; }
+    if (values.templateId && (preview.isError || preview.isFetching || previewRequest !== debouncedPreview)) {
+      message.error('Дождитесь корректного предпросмотра письма'); return;
+    }
+    if (uploadingImage) { message.error("Дождитесь загрузки изображения"); return; }
+    sendMutation.mutate({
+      templateId: values.templateId ?? null,
+      templateRevision: values.templateId ? template.data?.revision : null,
+      templateDefinition: values.templateId ? definition : null,
+      channel: 'Smtp',
+      recipientIds: values.recipientIds ?? [],
+      recipientGroupIds: values.recipientGroupIds ?? [],
+      adHocEmails: (values.adHocEmails ?? []).map((e) => e.trim()),
+      subject: values.subject?.trim() || null,
+      body: values.body,
+      bodyFormat: values.templateId ? 'Html' : values.bodyFormat,
+      priority: values.priority,
+    });
+  };
+
+  return (
+    <div style={{ maxWidth: 1600 }}>
+      <Typography.Title level={3}>Новая рассылка</Typography.Title>
+      <Row gutter={[24, 24]}>
+      <Col xs={24} xl={12}>
+      <Card>
+        <Form<FormValues>
+          form={form}
+          layout="vertical"
+          onFinish={onFinish}
+          initialValues={{ bodyFormat: 'PlainText', priority: 'Normal' }}
+        >
+          <Form.Item name="recipientIds" label="Получатели из адресной книги">
+            <Select
+              mode="multiple"
+              allowClear
+              placeholder="Выберите получателей"
+              loading={recipientsQuery.isLoading}
+              optionFilterProp="label"
+              options={(recipientsQuery.data?.items ?? []).map((r) => ({
+                value: r.id,
+                label: `${r.fullName} <${r.email}>`,
+              }))}
+            />
+          </Form.Item>
+
+          <Form.Item name="recipientGroupIds" label="Группы получателей">
+            <Select
+              mode="multiple"
+              allowClear
+              placeholder="Выберите группы"
+              loading={groupsQuery.isLoading}
+              optionFilterProp="label"
+              options={(groupsQuery.data?.items ?? []).map((g) => ({
+                value: g.id,
+                label: `${g.name} (${g.recipientCount})`,
+              }))}
+            />
+          </Form.Item>
+
+          <Form.Item
+            name="adHocEmails"
+            label="Дополнительные адреса"
+            tooltip="Введите email и нажмите Enter"
+          >
+            <Select mode="tags" allowClear placeholder="user@example.com" tokenSeparators={[',', ';', ' ']} open={false} suffixIcon={null} />
+          </Form.Item>
+
+          <Form.Item name="templateId" label="Шаблон письма" extra={<Link to="/templates" target="_blank" rel="noopener noreferrer">Управление шаблонами ↗</Link>}>
+            <Select allowClear showSearch filterOption={false} onChange={() => setDraft(undefined)} onSearch={setTemplateSearch} placeholder="Без шаблона" loading={templates.isLoading}
+              options={(templates.data?.items ?? []).map((item) => ({ value: item.id, label: item.name }))} />
+          </Form.Item>
+          {templates.isError && <Alert type="error" title={templates.error.message} style={{ marginBottom: 16 }} />}
+          {template.isError && <Alert type="error" title={template.error.message} style={{ marginBottom: 16 }} />}
+          {templateId && preview.isError && <Button style={{ marginBottom: 16 }} onClick={() => { void template.refetch(); }}>Обновить выбранный шаблон</Button>}
+          {templateId && <Alert type="info" title="Основной текст будет помещён в выбранный макет" description="Редактируйте оформление и блоки ниже — изменения применятся только к этому письму. Исходный шаблон не изменится. Основной текст вводится без HTML." style={{ marginBottom: 16 }} />}
+          {templateId && definition && <div style={{ marginBottom: 20 }}>
+            <Button style={{ marginBottom: 12 }} disabled={uploadingImage} onClick={() => setDraft(undefined)}>Сбросить изменения макета</Button>
+            <EmailTemplateEditor key={templateId} forMessage definition={definition} setDefinition={setDefinition} onUploadingChange={setUploadingImage} />
+          </div>}
+          <Form.Item name="subject" label="Тема">
+            <Input placeholder={templateId ? definition?.subject || 'Тема из шаблона' : 'Тема письма'} maxLength={500} />
+          </Form.Item>
+
+          <Form.Item
+            name="body"
+            label="Текст сообщения"
+            rules={[{ required: true, message: 'Введите текст сообщения' }]}
+          >
+            <Input.TextArea rows={10} placeholder="Текст письма" />
+          </Form.Item>
+
+          <Space size="large" wrap>
+            {!templateId && <Form.Item name="bodyFormat" label="Формат">
+              <Radio.Group
+                disabled={!!templateId}
+                options={[
+                  { value: 'PlainText', label: 'Текст' },
+                  { value: 'Html', label: 'HTML' },
+                  { value: 'Markdown', label: 'Markdown' },
+                ]}
+                optionType="button"
+              />
+            </Form.Item>}
+
+            <Form.Item name="priority" label="Приоритет">
+              <Select
+                style={{ width: 160 }}
+                options={[
+                  { value: 'Low', label: 'Низкий' },
+                  { value: 'Normal', label: 'Обычный' },
+                  { value: 'High', label: 'Высокий' },
+                  { value: 'Critical', label: 'Критический' },
+                ]}
+              />
+            </Form.Item>
+          </Space>
+
+          <Form.Item>
+            <Space>
+              <Button
+                type="primary"
+                htmlType="submit"
+                icon={<SendOutlined />}
+                loading={sendMutation.isPending}
+                disabled={uploadingImage || (!!templateId && (!template.data || preview.isError || preview.isFetching || previewRequest !== debouncedPreview))}
+              >
+                Отправить
+              </Button>
+              <Button onClick={() => navigate('/notifications')}>Отмена</Button>
+            </Space>
+          </Form.Item>
+        </Form>
+      </Card>
+      </Col>
+      <Col xs={24} xl={12}><div style={{ position: 'sticky', top: 20 }}><EmailPreview preview={preview.data} loading={preview.isFetching || previewRequest !== debouncedPreview || (!!templateId && template.isLoading)} error={preview.error} /></div></Col>
+      </Row>
+    </div>
+  );
+}
`````

### `a/frontend/src/pages/NotificationsPage.tsx b/src/MailGate.Frontend/src/pages/NotificationsPage.tsx`

`````diff
diff --git a/frontend/src/pages/NotificationsPage.tsx b/src/MailGate.Frontend/src/pages/NotificationsPage.tsx
similarity index 100%
rename from frontend/src/pages/NotificationsPage.tsx
rename to src/MailGate.Frontend/src/pages/NotificationsPage.tsx
`````

### `a/frontend/src/pages/RecipientGroupEditPage.tsx b/src/MailGate.Frontend/src/pages/RecipientGroupEditPage.tsx`

`````diff
diff --git a/frontend/src/pages/RecipientGroupEditPage.tsx b/src/MailGate.Frontend/src/pages/RecipientGroupEditPage.tsx
similarity index 100%
rename from frontend/src/pages/RecipientGroupEditPage.tsx
rename to src/MailGate.Frontend/src/pages/RecipientGroupEditPage.tsx
`````

### `a/frontend/src/pages/RecipientGroupsPage.tsx b/src/MailGate.Frontend/src/pages/RecipientGroupsPage.tsx`

`````diff
diff --git a/frontend/src/pages/RecipientGroupsPage.tsx b/src/MailGate.Frontend/src/pages/RecipientGroupsPage.tsx
similarity index 100%
rename from frontend/src/pages/RecipientGroupsPage.tsx
rename to src/MailGate.Frontend/src/pages/RecipientGroupsPage.tsx
`````

### `a/frontend/src/pages/RecipientsPage.tsx b/src/MailGate.Frontend/src/pages/RecipientsPage.tsx`

`````diff
diff --git a/frontend/src/pages/RecipientsPage.tsx b/src/MailGate.Frontend/src/pages/RecipientsPage.tsx
similarity index 100%
rename from frontend/src/pages/RecipientsPage.tsx
rename to src/MailGate.Frontend/src/pages/RecipientsPage.tsx
`````

### `a/frontend/src/pages/SmtpSettingsPage.tsx b/src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx`

`````diff
diff --git a/frontend/src/pages/SmtpSettingsPage.tsx b/src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx
similarity index 63%
rename from frontend/src/pages/SmtpSettingsPage.tsx
rename to src/MailGate.Frontend/src/pages/SmtpSettingsPage.tsx
index 31fc7d3..11b8b0a 100644
--- a/frontend/src/pages/SmtpSettingsPage.tsx
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
@@ -19,22 +19,17 @@ import {
 import { ApiOutlined, DeleteOutlined, SaveOutlined } from '@ant-design/icons';
 import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
 import { smtpApi } from '../api/endpoints';
-import type { SmtpTestResultDto, UpdateSmtpConfigurationDto } from '../api/types';
+import type { SmtpCredentialSource, SmtpTestResultDto, UpdateSmtpConfigurationDto } from '../api/types';
 
 interface FormValues {
   host: string;
   port: number;
   useSsl: boolean;
   useTls: boolean;
+  checkCertificateRevocation: boolean;
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
@@ -43,18 +38,13 @@ function toDto(values: FormValues): UpdateSmtpConfigurationDto {
     port: values.port,
     useSsl: values.useSsl,
     useTls: values.useTls,
+    checkCertificateRevocation: values.checkCertificateRevocation,
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
 
@@ -78,13 +68,10 @@ export function SmtpSettingsPage() {
         port: effective.port,
         useSsl: effective.useSsl,
         useTls: effective.useTls,
+        checkCertificateRevocation: effective.checkCertificateRevocation ?? true,
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
@@ -119,7 +106,8 @@ export function SmtpSettingsPage() {
     onError: (error: Error) => message.error(error.message),
   });
 
-  const oauthEnabled = Form.useWatch('oauthEnabled', form);
+  const credentialSource = Form.useWatch('credentialSource', form);
+  const pending = saveMutation.isPending || testMutation.isPending || deleteMutation.isPending;
 
   if (effectiveQuery.isLoading) {
     return <Spin size="large" style={{ display: 'block', margin: '80px auto' }} />;
@@ -129,6 +117,7 @@ export function SmtpSettingsPage() {
     <div style={{ maxWidth: 760 }}>
       <Typography.Title level={3}>Настройки SMTP</Typography.Title>
 
+      {effectiveQuery.isError && <Alert type="error" title={effectiveQuery.error.message} style={{ marginBottom: 16 }} />}
       {effective && (
         <Card size="small" style={{ marginBottom: 16 }}>
           <Descriptions column={2} size="small" title="Действующая конфигурация">
@@ -149,8 +138,25 @@ export function SmtpSettingsPage() {
           form={form}
           layout="vertical"
           onFinish={(values) => saveMutation.mutate(values)}
-          initialValues={{ port: 587, useTls: true, useSsl: false, oauthEnabled: false }}
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
@@ -170,12 +176,16 @@ export function SmtpSettingsPage() {
             </Form.Item>
           </Space.Compact>
 
-          <Space size="large">
+          <Space size="large" wrap>
             <Form.Item name="useTls" valuePropName="checked" style={{ marginBottom: 8 }}>
-              <Checkbox>STARTTLS</Checkbox>
+              <Checkbox onChange={(event) => { if (event.target.checked) form.setFieldValue('useSsl', false); }}>STARTTLS</Checkbox>
             </Form.Item>
             <Form.Item name="useSsl" valuePropName="checked" style={{ marginBottom: 8 }}>
-              <Checkbox>SSL при подключении</Checkbox>
+              <Checkbox onChange={(event) => { if (event.target.checked) form.setFieldValue('useTls', false); }}>SSL при подключении</Checkbox>
+            </Form.Item>
+            <Form.Item name="checkCertificateRevocation" valuePropName="checked" style={{ marginBottom: 8 }}
+              tooltip="При отключении не проверяется отзыв сертификатов SMTP. Доверие УЦ и имя сервера продолжают проверяться.">
+              <Checkbox>Проверять отзыв сертификатов</Checkbox>
             </Form.Item>
           </Space>
 
@@ -198,71 +208,6 @@ export function SmtpSettingsPage() {
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
@@ -278,10 +223,10 @@ export function SmtpSettingsPage() {
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

### `a/frontend/src/pages/SystemsPage.tsx b/src/MailGate.Frontend/src/pages/SystemsPage.tsx`

`````diff
diff --git a/frontend/src/pages/SystemsPage.tsx b/src/MailGate.Frontend/src/pages/SystemsPage.tsx
similarity index 100%
rename from frontend/src/pages/SystemsPage.tsx
rename to src/MailGate.Frontend/src/pages/SystemsPage.tsx
`````

### `a/frontend/src/theme.tsx b/src/MailGate.Frontend/src/theme.tsx`

`````diff
diff --git a/frontend/src/theme.tsx b/src/MailGate.Frontend/src/theme.tsx
similarity index 100%
rename from frontend/src/theme.tsx
rename to src/MailGate.Frontend/src/theme.tsx
`````

### `a/frontend/tsconfig.json b/src/MailGate.Frontend/tsconfig.json`

`````diff
diff --git a/frontend/tsconfig.json b/src/MailGate.Frontend/tsconfig.json
similarity index 100%
rename from frontend/tsconfig.json
rename to src/MailGate.Frontend/tsconfig.json
`````

### `a/frontend/vite.config.ts b/src/MailGate.Frontend/vite.config.ts`

`````diff
diff --git a/frontend/vite.config.ts b/src/MailGate.Frontend/vite.config.ts
similarity index 100%
rename from frontend/vite.config.ts
rename to src/MailGate.Frontend/vite.config.ts
`````

### `a/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs b/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs b/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs
index 479dd68..e383299 100644
--- a/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs
+++ b/src/MailGate.Infrastructure/Extensions/ServiceCollectionExtensions.cs
@@ -29,7 +29,18 @@ public static class ServiceCollectionExtensions
                 npgsql.MigrationsAssembly(typeof(MailGateDbContext).Assembly.FullName)));
 
         services.AddMemoryCache();
-        services.AddDataProtection();
+        var keysPath = configuration["DataProtection:KeysPath"];
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

### `a/src/MailGate.Infrastructure/MailGate.Infrastructure.csproj b/src/MailGate.Infrastructure/MailGate.Infrastructure.csproj`

`````diff
diff --git a/src/MailGate.Infrastructure/MailGate.Infrastructure.csproj b/src/MailGate.Infrastructure/MailGate.Infrastructure.csproj
index 770b367..2be24ce 100644
--- a/src/MailGate.Infrastructure/MailGate.Infrastructure.csproj
+++ b/src/MailGate.Infrastructure/MailGate.Infrastructure.csproj
@@ -1,4 +1,4 @@
-﻿<Project Sdk="Microsoft.NET.Sdk">
+<Project Sdk="Microsoft.NET.Sdk">
 
   <ItemGroup>
     <ProjectReference Include="..\MailGate.Core\MailGate.Core.csproj" />
@@ -8,7 +8,8 @@
   <ItemGroup>
     <PackageReference Include="MassTransit.EntityFrameworkCore" Version="8.3.6" />
     <PackageReference Include="MassTransit.RabbitMQ" Version="8.3.6" />
-    <PackageReference Include="Microsoft.AspNetCore.DataProtection.Extensions" Version="10.0.6" />
+    <PackageReference Include="Microsoft.AspNetCore.DataProtection" Version="10.0.7" />
+    <PackageReference Include="Microsoft.AspNetCore.DataProtection.Extensions" Version="10.0.7" />
     <PackageReference Include="Microsoft.EntityFrameworkCore" Version="10.0.6" />
     <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="10.0.6">
       <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
`````

### `a/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs b/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs b/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs
index 3d4b767..a67ad16 100644
--- a/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs
+++ b/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs
@@ -10,6 +10,7 @@ public sealed class MailGateDbContext(DbContextOptions<MailGateDbContext> option
     public DbSet<Recipient> Recipients => Set<Recipient>();
     public DbSet<RecipientGroup> RecipientGroups => Set<RecipientGroup>();
     public DbSet<NotificationSystem> Systems => Set<NotificationSystem>();
+    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
     public DbSet<SmtpConfiguration> SmtpConfigurations => Set<SmtpConfiguration>();
 
     protected override void OnModelCreating(ModelBuilder modelBuilder)
@@ -104,9 +105,21 @@ public sealed class MailGateDbContext(DbContextOptions<MailGateDbContext> option
                     });
         });
 
+        modelBuilder.Entity<EmailTemplate>(e =>
+        {
+            e.ToTable("email_templates");
+            e.HasKey(x => x.Id);
+            e.Property(x => x.Id).ValueGeneratedNever();
+            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
+            e.Property(x => x.DefinitionJson).IsRequired().HasColumnType("jsonb");
+            e.Property(x => x.Revision).IsConcurrencyToken();
+            e.HasIndex(x => x.UpdatedAt);
+        });
+
         modelBuilder.Entity<SmtpConfiguration>(e =>
         {
             e.ToTable("smtp_configurations");
+            e.Property(x => x.CheckCertificateRevocation).HasDefaultValue(true);
             e.HasKey(x => x.Id);
             e.Property(x => x.Id).ValueGeneratedNever();
             e.Property(x => x.Host).IsRequired().HasMaxLength(256);
`````

### `a/src/MailGate.Infrastructure/Persistence/MailGateDbContextFactory.cs b/src/MailGate.Infrastructure/Persistence/MailGateDbContextFactory.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/MailGateDbContextFactory.cs b/src/MailGate.Infrastructure/Persistence/MailGateDbContextFactory.cs
index 0f0616d..a1c72ee 100644
--- a/src/MailGate.Infrastructure/Persistence/MailGateDbContextFactory.cs
+++ b/src/MailGate.Infrastructure/Persistence/MailGateDbContextFactory.cs
@@ -8,7 +8,8 @@ public sealed class MailGateDbContextFactory : IDesignTimeDbContextFactory<MailG
     public MailGateDbContext CreateDbContext(string[] args)
     {
         var options = new DbContextOptionsBuilder<MailGateDbContext>()
-            .UseNpgsql("Host=localhost;Database=mailgate;Username=mailgate;Password=mailgate")
+            .UseNpgsql(Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
+                ?? "Host=localhost;Database=mailgate;Username=mailgate;Password=mailgate")
             .Options;
 
         return new MailGateDbContext(options);
`````

### `a/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.Designer.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.Designer.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.Designer.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.Designer.cs
new file mode 100644
index 0000000..5bd7963
--- /dev/null
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.Designer.cs
@@ -0,0 +1,490 @@
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
+    [Migration("20261026090000_AddSmtpCertificateRevocationCheck")]
+    partial class AddSmtpCertificateRevocationCheck
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

### `a/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.cs
new file mode 100644
index 0000000..d00448e
--- /dev/null
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/20261026090000_AddSmtpCertificateRevocationCheck.cs
@@ -0,0 +1,25 @@
+using Microsoft.EntityFrameworkCore.Migrations;
+
+#nullable disable
+
+namespace MailGate.Infrastructure.Persistence.Migrations;
+
+public partial class AddSmtpCertificateRevocationCheck : Migration
+{
+    protected override void Up(MigrationBuilder migrationBuilder)
+    {
+        migrationBuilder.AddColumn<bool>(
+            name: "CheckCertificateRevocation",
+            table: "smtp_configurations",
+            type: "boolean",
+            nullable: false,
+            defaultValue: true);
+    }
+
+    protected override void Down(MigrationBuilder migrationBuilder)
+    {
+        migrationBuilder.DropColumn(
+            name: "CheckCertificateRevocation",
+            table: "smtp_configurations");
+    }
+}
`````

### `a/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs
new file mode 100644
index 0000000..13d3e34
--- /dev/null
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs
@@ -0,0 +1,521 @@
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
+    [Migration("20261028090000_AddEmailTemplates")]
+    partial class AddEmailTemplates
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

### `a/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs
new file mode 100644
index 0000000..de11db8
--- /dev/null
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs
@@ -0,0 +1,28 @@
+using System;
+using Microsoft.EntityFrameworkCore.Migrations;
+
+#nullable disable
+
+namespace MailGate.Infrastructure.Persistence.Migrations;
+
+public partial class AddEmailTemplates : Migration
+{
+    protected override void Up(MigrationBuilder migrationBuilder)
+    {
+        migrationBuilder.CreateTable(
+            name: "email_templates",
+            columns: table => new
+            {
+                Id = table.Column<Guid>(type: "uuid", nullable: false),
+                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
+                DefinitionJson = table.Column<string>(type: "jsonb", nullable: false),
+                Revision = table.Column<Guid>(type: "uuid", nullable: false),
+                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
+                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
+            },
+            constraints: table => table.PrimaryKey("PK_email_templates", x => x.Id));
+        migrationBuilder.CreateIndex(name: "IX_email_templates_UpdatedAt", table: "email_templates", column: "UpdatedAt");
+    }
+
+    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "email_templates");
+}
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
index cf9c0bd..51945b6 100644
--- a/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
@@ -1,4 +1,4 @@
-﻿// <auto-generated />
+// <auto-generated />
 using System;
 using MailGate.Infrastructure.Persistence;
 using Microsoft.EntityFrameworkCore;
@@ -22,6 +22,37 @@ namespace MailGate.Infrastructure.Persistence.Migrations
 
             NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);
 
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
             modelBuilder.Entity("MailGate.Core.Entities.NotificationLog", b =>
                 {
                     b.Property<Guid>("Id")
@@ -179,6 +210,14 @@ namespace MailGate.Infrastructure.Persistence.Migrations
 
             modelBuilder.Entity("MailGate.Core.Entities.SmtpConfiguration", b =>
                 {
+                    b.Property<bool>("CheckCertificateRevocation")
+                        .ValueGeneratedOnAdd()
+                        .HasColumnType("boolean")
+                        .HasDefaultValue(true);
+
+                    b.Property<int>("CredentialSource")
+                        .HasColumnType("integer");
+
                     b.Property<Guid>("Id")
                         .HasColumnType("uuid");
 
`````

### `a/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs b/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs b/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs
index e3f075e..25fddf1 100644
--- a/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs
+++ b/src/MailGate.Infrastructure/Services/DbSmtpSettingsProvider.cs
@@ -8,24 +8,30 @@ namespace MailGate.Infrastructure.Services;
 
 /// <summary>
 /// SMTP settings provider that merges appsettings defaults with the optional database override.
-/// Database values take precedence when present. Results are cached in memory for 30 seconds.
+/// Database values take precedence when present. Results are cached in memory for Smtp:SettingsCacheSeconds (default 30).
 /// </summary>
 public sealed class DbSmtpSettingsProvider(
     IServiceScopeFactory scopeFactory,
     IOptionsMonitor<SmtpOptions> appSettings,
-    IMemoryCache cache) : ISmtpSettingsProvider
+    IMemoryCache cache,
+    SmtpCredentialResolver credentials) : ISmtpSettingsProvider
 {
     private const string CacheKey = "MailGate.Smtp.EffectiveOptions";
-    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(30);
 
     public async Task<SmtpOptions> GetEffectiveAsync(CancellationToken cancellationToken = default)
     {
-        if (cache.TryGetValue(CacheKey, out SmtpOptions? cached) && cached is not null)
-            return cached;
-
-        var merged = await LoadAsync(cancellationToken);
-        cache.Set(CacheKey, merged, CacheTtl);
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
@@ -45,10 +51,12 @@ public sealed class DbSmtpSettingsProvider(
 
         var merged = new SmtpOptions
         {
+            CredentialSource = entity.CredentialSource,
             Host = entity.Host,
             Port = entity.Port,
             UseSsl = entity.UseSsl,
             UseTls = entity.UseTls,
+            CheckCertificateRevocation = entity.CheckCertificateRevocation,
             FromAddress = entity.FromAddress,
             FromName = entity.FromName,
             Username = entity.Username ?? fromSettings.Username,
@@ -71,10 +79,13 @@ public sealed class DbSmtpSettingsProvider(
 
     private static SmtpOptions Clone(SmtpOptions src) => new()
     {
+        CredentialSource = src.CredentialSource,
+        CredentialsFilePath = src.CredentialsFilePath,
         Host = src.Host,
         Port = src.Port,
         UseSsl = src.UseSsl,
         UseTls = src.UseTls,
+        CheckCertificateRevocation = src.CheckCertificateRevocation,
         FromAddress = src.FromAddress,
         FromName = src.FromName,
         Username = src.Username,
`````

### `a/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs b/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs`

`````diff
diff --git a/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs b/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs
index eb7e51c..9d8ad92 100644
--- a/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs
+++ b/src/MailGate.Infrastructure/Services/SmtpConfigurationStore.cs
@@ -1,3 +1,5 @@
+using System.Security.Cryptography;
+using MailGate.Core.Enums;
 using MailGate.Core.Entities;
 using MailGate.Infrastructure.Persistence;
 using Microsoft.AspNetCore.DataProtection;
@@ -24,19 +26,29 @@ public sealed class SmtpConfigurationStore(
 
     public async Task<DecryptedConfiguration?> GetAsync(CancellationToken cancellationToken = default)
     {
-        var entity = await dbContext.SmtpConfigurations.AsNoTracking()
-            .OrderByDescending(x => x.UpdatedAt)
-            .FirstOrDefaultAsync(cancellationToken);
+        var entity = await GetMetadataAsync(cancellationToken);
 
         if (entity is null)
             return null;
 
-        var password = TryUnprotect(entity.EncryptedPassword);
-        var clientSecret = TryUnprotect(entity.OAuth2EncryptedClientSecret);
+        if (entity.CredentialSource != SmtpCredentialSource.Legacy)
+            return new DecryptedConfiguration(entity, null, null);
+
+        var oauthEnabled = !string.IsNullOrWhiteSpace(entity.OAuth2TenantId);
+        var password = oauthEnabled ? null : DecryptPassword(entity);
+        var clientSecret = oauthEnabled ? DecryptOAuth2ClientSecret(entity) : null;
 
         return new DecryptedConfiguration(entity, password, clientSecret);
     }
 
+    public Task<SmtpConfiguration?> GetMetadataAsync(CancellationToken cancellationToken = default) =>
+        dbContext.SmtpConfigurations.AsNoTracking()
+            .OrderByDescending(x => x.UpdatedAt)
+            .FirstOrDefaultAsync(cancellationToken);
+
+    public string? DecryptPassword(SmtpConfiguration entity) => TryUnprotect(entity.EncryptedPassword);
+    public string? DecryptOAuth2ClientSecret(SmtpConfiguration entity) => TryUnprotect(entity.OAuth2EncryptedClientSecret);
+
     public async Task UpsertAsync(
         SmtpConfiguration incoming,
         string? password,
@@ -46,8 +58,20 @@ public sealed class SmtpConfigurationStore(
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
@@ -56,10 +80,12 @@ public sealed class SmtpConfigurationStore(
         }
         else
         {
+            existing.CredentialSource = incoming.CredentialSource;
             existing.Host = incoming.Host;
             existing.Port = incoming.Port;
             existing.UseSsl = incoming.UseSsl;
             existing.UseTls = incoming.UseTls;
+            existing.CheckCertificateRevocation = incoming.CheckCertificateRevocation;
             existing.FromAddress = incoming.FromAddress;
             existing.FromName = incoming.FromName;
             existing.Username = incoming.Username;
@@ -92,9 +118,10 @@ public sealed class SmtpConfigurationStore(
         {
             return _protector.Unprotect(encrypted);
         }
-        catch
+        catch (CryptographicException ex)
         {
-            return null;
+            throw new InvalidOperationException(
+                "Не удалось расшифровать сохранённый SMTP-секрет. Проверьте общий key ring API и Worker или сохраните секрет заново.", ex);
         }
     }
 }
`````

### `a/src/MailGate.Worker/Dockerfile b/src/MailGate.Worker/Dockerfile`

`````diff
diff --git a/src/MailGate.Worker/Dockerfile b/src/MailGate.Worker/Dockerfile
index e375d65..0d34147 100644
--- a/src/MailGate.Worker/Dockerfile
+++ b/src/MailGate.Worker/Dockerfile
@@ -2,27 +2,38 @@ FROM registry.red-soft.ru/ubi8/ubi-minimal:8.0.2-260714 AS base
 WORKDIR /app
 
 RUN dnf makecache \
-    && dnf install -y dotnet-runtime-10.0 \
+    && dnf install -y ca-certificates dotnet-runtime-10.0 \
     && dnf clean all
 
 FROM registry.red-soft.ru/ubi8/ubi:8.0.2-260714 AS build
 WORKDIR /src
 
 RUN dnf makecache \
-    && dnf install -y dotnet-sdk-10.0 \
+    && dnf install -y ca-certificates dotnet-sdk-10.0 \
     && dnf clean all
 
-COPY ["src/MailGate.Worker/MailGate.Worker.csproj",                   "src/MailGate.Worker/"]
-COPY ["src/MailGate.Core/MailGate.Core.csproj",                       "src/MailGate.Core/"]
-COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj",   "src/MailGate.Infrastructure/"]
-COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj",     "src/MailGate.Channels.Smtp/"]
-RUN dotnet restore "src/MailGate.Worker/MailGate.Worker.csproj"
+COPY crt/ /etc/pki/ca-trust/source/anchors/
+RUN update-ca-trust extract
 
-COPY . .
-RUN dotnet publish "src/MailGate.Worker/MailGate.Worker.csproj" \
+COPY ["src/NuGet.Config", "./"]
+COPY ["src/MailGate.Worker/MailGate.Worker.csproj",                   "MailGate.Worker/"]
+COPY ["src/MailGate.Core/MailGate.Core.csproj",                       "MailGate.Core/"]
+COPY ["src/MailGate.Infrastructure/MailGate.Infrastructure.csproj",   "MailGate.Infrastructure/"]
+COPY ["src/MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj",     "MailGate.Channels.Smtp/"]
+RUN --mount=type=secret,id=nuget_config \
+    if [ -f /run/secrets/nuget_config ]; then \
+      dotnet restore "MailGate.Worker/MailGate.Worker.csproj" --configfile /run/secrets/nuget_config; \
+    else \
+      dotnet restore "MailGate.Worker/MailGate.Worker.csproj"; \
+    fi
+
+COPY src/ .
+RUN dotnet publish "MailGate.Worker/MailGate.Worker.csproj" \
       -c Release -o /app/publish --no-restore
 
 FROM base AS final
 WORKDIR /app
+COPY crt/ /etc/pki/ca-trust/source/anchors/
+RUN update-ca-trust extract
 COPY --from=build /app/publish .
 ENTRYPOINT ["dotnet", "MailGate.Worker.dll"]
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

### `a/src/MailGate.Worker/appsettings.json b/src/MailGate.Worker/appsettings.json`

`````diff
diff --git a/src/MailGate.Worker/appsettings.json b/src/MailGate.Worker/appsettings.json
index b4f0fae..b6c707a 100644
--- a/src/MailGate.Worker/appsettings.json
+++ b/src/MailGate.Worker/appsettings.json
@@ -17,7 +17,12 @@
     "Password": "mailgate",
     "NotificationsQueue": "mailgate-notifications"
   },
+  "DataProtection": {
+    "KeysPath": ""
+  },
   "Smtp": {
+    "CheckCertificateRevocation": true,
+    "SettingsCacheSeconds": 30,
     "Host": "localhost",
     "Port": 1025,
     "UseTls": false,
`````

### `a/MailGate.sln b/src/MailGate.sln`

`````diff
diff --git a/MailGate.sln b/src/MailGate.sln
similarity index 66%
rename from MailGate.sln
rename to src/MailGate.sln
index 045099d..0a809fc 100644
--- a/MailGate.sln
+++ b/src/MailGate.sln
@@ -5,25 +5,21 @@ VisualStudioVersion = 17.5.2.0
 MinimumVisualStudioVersion = 10.0.40219.1
 Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{827E0CD3-B72D-47B6-A68D-7590B98EB39B}"
 EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Core", "src\MailGate.Core\MailGate.Core.csproj", "{B49D5F08-7A97-D882-D7EF-C701D8F37F32}"
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Core", "MailGate.Core\MailGate.Core.csproj", "{B49D5F08-7A97-D882-D7EF-C701D8F37F32}"
 EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Channels.Smtp", "src\MailGate.Channels.Smtp\MailGate.Channels.Smtp.csproj", "{792C58B9-DF72-E4FD-A59A-23D84EED316F}"
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Channels.Smtp", "MailGate.Channels.Smtp\MailGate.Channels.Smtp.csproj", "{792C58B9-DF72-E4FD-A59A-23D84EED316F}"
 EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Infrastructure", "src\MailGate.Infrastructure\MailGate.Infrastructure.csproj", "{C6B891F5-BD9E-BFA3-9ACC-AEFDE5385A55}"
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Infrastructure", "MailGate.Infrastructure\MailGate.Infrastructure.csproj", "{C6B891F5-BD9E-BFA3-9ACC-AEFDE5385A55}"
 EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Worker", "src\MailGate.Worker\MailGate.Worker.csproj", "{41903A7D-10AE-1F94-80A4-2ED4E99E54EC}"
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Worker", "MailGate.Worker\MailGate.Worker.csproj", "{41903A7D-10AE-1F94-80A4-2ED4E99E54EC}"
 EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Api", "src\MailGate.Api\MailGate.Api.csproj", "{47D1AD8B-8B18-EB24-0AE3-64046B9DD9E0}"
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Api", "MailGate.Api\MailGate.Api.csproj", "{47D1AD8B-8B18-EB24-0AE3-64046B9DD9E0}"
 EndProject
-Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "tests", "tests", "{0AB3BF05-4346-4AA6-1389-037BE0695223}"
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Contracts", "MailGate.Contracts\MailGate.Contracts.csproj", "{51C9A635-404B-4952-B6E8-7D1330DD2343}"
 EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Api.Tests", "tests\MailGate.Api.Tests\MailGate.Api.Tests.csproj", "{D44C9B03-989C-4599-8746-36B763801988}"
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.TemplateChecks", "..\tests\MailGate.TemplateChecks\MailGate.TemplateChecks.csproj", "{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}"
 EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Worker.Tests", "tests\MailGate.Worker.Tests\MailGate.Worker.Tests.csproj", "{41AE1395-5E31-4350-9E21-506DC9889C06}"
-EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Channels.Smtp.Tests", "tests\MailGate.Channels.Smtp.Tests\MailGate.Channels.Smtp.Tests.csproj", "{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}"
-EndProject
-Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Contracts", "src\MailGate.Contracts\MailGate.Contracts.csproj", "{51C9A635-404B-4952-B6E8-7D1330DD2343}"
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.SmtpCredentialChecks", "..\tests\MailGate.SmtpCredentialChecks\MailGate.SmtpCredentialChecks.csproj", "{66B75348-CA4D-4F6C-B415-E31C9A46305A}"
 EndProject
 Global
 	GlobalSection(SolutionConfigurationPlatforms) = preSolution
@@ -95,42 +91,6 @@ Global
 		{47D1AD8B-8B18-EB24-0AE3-64046B9DD9E0}.Release|x64.Build.0 = Release|Any CPU
 		{47D1AD8B-8B18-EB24-0AE3-64046B9DD9E0}.Release|x86.ActiveCfg = Release|Any CPU
 		{47D1AD8B-8B18-EB24-0AE3-64046B9DD9E0}.Release|x86.Build.0 = Release|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Debug|Any CPU.Build.0 = Debug|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Debug|x64.ActiveCfg = Debug|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Debug|x64.Build.0 = Debug|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Debug|x86.ActiveCfg = Debug|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Debug|x86.Build.0 = Debug|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Release|Any CPU.ActiveCfg = Release|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Release|Any CPU.Build.0 = Release|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Release|x64.ActiveCfg = Release|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Release|x64.Build.0 = Release|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Release|x86.ActiveCfg = Release|Any CPU
-		{D44C9B03-989C-4599-8746-36B763801988}.Release|x86.Build.0 = Release|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Debug|Any CPU.Build.0 = Debug|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Debug|x64.ActiveCfg = Debug|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Debug|x64.Build.0 = Debug|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Debug|x86.ActiveCfg = Debug|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Debug|x86.Build.0 = Debug|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Release|Any CPU.ActiveCfg = Release|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Release|Any CPU.Build.0 = Release|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Release|x64.ActiveCfg = Release|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Release|x64.Build.0 = Release|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Release|x86.ActiveCfg = Release|Any CPU
-		{41AE1395-5E31-4350-9E21-506DC9889C06}.Release|x86.Build.0 = Release|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Debug|Any CPU.Build.0 = Debug|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Debug|x64.ActiveCfg = Debug|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Debug|x64.Build.0 = Debug|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Debug|x86.ActiveCfg = Debug|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Debug|x86.Build.0 = Debug|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Release|Any CPU.ActiveCfg = Release|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Release|Any CPU.Build.0 = Release|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Release|x64.ActiveCfg = Release|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Release|x64.Build.0 = Release|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Release|x86.ActiveCfg = Release|Any CPU
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D}.Release|x86.Build.0 = Release|Any CPU
 		{51C9A635-404B-4952-B6E8-7D1330DD2343}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
 		{51C9A635-404B-4952-B6E8-7D1330DD2343}.Debug|Any CPU.Build.0 = Debug|Any CPU
 		{51C9A635-404B-4952-B6E8-7D1330DD2343}.Debug|x64.ActiveCfg = Debug|Any CPU
@@ -143,6 +103,30 @@ Global
 		{51C9A635-404B-4952-B6E8-7D1330DD2343}.Release|x64.Build.0 = Release|Any CPU
 		{51C9A635-404B-4952-B6E8-7D1330DD2343}.Release|x86.ActiveCfg = Release|Any CPU
 		{51C9A635-404B-4952-B6E8-7D1330DD2343}.Release|x86.Build.0 = Release|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Debug|Any CPU.Build.0 = Debug|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Debug|x64.ActiveCfg = Debug|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Debug|x64.Build.0 = Debug|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Debug|x86.ActiveCfg = Debug|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Debug|x86.Build.0 = Debug|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|Any CPU.ActiveCfg = Release|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|Any CPU.Build.0 = Release|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|x64.ActiveCfg = Release|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|x64.Build.0 = Release|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|x86.ActiveCfg = Release|Any CPU
+		{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}.Release|x86.Build.0 = Release|Any CPU
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
@@ -153,9 +137,6 @@ Global
 		{C6B891F5-BD9E-BFA3-9ACC-AEFDE5385A55} = {827E0CD3-B72D-47B6-A68D-7590B98EB39B}
 		{41903A7D-10AE-1F94-80A4-2ED4E99E54EC} = {827E0CD3-B72D-47B6-A68D-7590B98EB39B}
 		{47D1AD8B-8B18-EB24-0AE3-64046B9DD9E0} = {827E0CD3-B72D-47B6-A68D-7590B98EB39B}
-		{D44C9B03-989C-4599-8746-36B763801988} = {0AB3BF05-4346-4AA6-1389-037BE0695223}
-		{41AE1395-5E31-4350-9E21-506DC9889C06} = {0AB3BF05-4346-4AA6-1389-037BE0695223}
-		{F0785BC6-D25C-476C-BA0C-2CF6E4C4AF0D} = {0AB3BF05-4346-4AA6-1389-037BE0695223}
 		{51C9A635-404B-4952-B6E8-7D1330DD2343} = {827E0CD3-B72D-47B6-A68D-7590B98EB39B}
 	EndGlobalSection
 	GlobalSection(ExtensibilityGlobals) = postSolution
`````

### `a/src/MailGate.slnx b/src/MailGate.slnx`

`````diff
diff --git a/src/MailGate.slnx b/src/MailGate.slnx
new file mode 100644
index 0000000..ec21ab6
--- /dev/null
+++ b/src/MailGate.slnx
@@ -0,0 +1,17 @@
+<Solution>
+  <Configurations>
+    <Platform Name="Any CPU" />
+    <Platform Name="x64" />
+    <Platform Name="x86" />
+  </Configurations>
+  <Folder Name="/src/">
+    <Project Path="MailGate.Core/MailGate.Core.csproj" />
+    <Project Path="MailGate.Channels.Smtp/MailGate.Channels.Smtp.csproj" />
+    <Project Path="MailGate.Infrastructure/MailGate.Infrastructure.csproj" />
+    <Project Path="MailGate.Worker/MailGate.Worker.csproj" />
+    <Project Path="MailGate.Api/MailGate.Api.csproj" />
+    <Project Path="MailGate.Contracts/MailGate.Contracts.csproj" />
+  </Folder>
+  <Project Path="../tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj" />
+  <Project Path="../tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj" />
+</Solution>
`````

### `a/src/NuGet.Config b/src/NuGet.Config`

`````diff
diff --git a/src/NuGet.Config b/src/NuGet.Config
new file mode 100644
index 0000000..4d736c1
--- /dev/null
+++ b/src/NuGet.Config
@@ -0,0 +1,7 @@
+<?xml version="1.0" encoding="utf-8"?>
+<configuration>
+  <packageSources>
+    <clear />
+    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
+  </packageSources>
+</configuration>
`````

### `a/tests/MailGate.Api.Tests/MailGate.Api.Tests.csproj b/tests/MailGate.Api.Tests/MailGate.Api.Tests.csproj`

`````diff
diff --git a/tests/MailGate.Api.Tests/MailGate.Api.Tests.csproj b/tests/MailGate.Api.Tests/MailGate.Api.Tests.csproj
deleted file mode 100644
index 62514d0..0000000
--- a/tests/MailGate.Api.Tests/MailGate.Api.Tests.csproj
+++ /dev/null
@@ -1,28 +0,0 @@
-﻿<Project Sdk="Microsoft.NET.Sdk">
-
-  <PropertyGroup>
-    <TargetFramework>net10.0</TargetFramework>
-    <ImplicitUsings>enable</ImplicitUsings>
-    <Nullable>enable</Nullable>
-    <IsPackable>false</IsPackable>
-  </PropertyGroup>
-
-  <ItemGroup>
-    <PackageReference Include="coverlet.collector" Version="6.0.4" />
-    <PackageReference Include="Microsoft.AspNetCore.Mvc.Testing" Version="10.0.6" />
-    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
-    <PackageReference Include="NSubstitute" Version="5.3.0" />
-    <PackageReference Include="xunit" Version="2.9.3" />
-    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
-  </ItemGroup>
-
-  <ItemGroup>
-    <Using Include="Xunit" />
-  </ItemGroup>
-
-  <ItemGroup>
-    <ProjectReference Include="..\..\src\MailGate.Api\MailGate.Api.csproj" />
-    <ProjectReference Include="..\..\src\MailGate.Core\MailGate.Core.csproj" />
-  </ItemGroup>
-
-</Project>
\ No newline at end of file
`````

### `a/tests/MailGate.Channels.Smtp.Tests/MailGate.Channels.Smtp.Tests.csproj b/tests/MailGate.Channels.Smtp.Tests/MailGate.Channels.Smtp.Tests.csproj`

`````diff
diff --git a/tests/MailGate.Channels.Smtp.Tests/MailGate.Channels.Smtp.Tests.csproj b/tests/MailGate.Channels.Smtp.Tests/MailGate.Channels.Smtp.Tests.csproj
deleted file mode 100644
index 4a0d761..0000000
--- a/tests/MailGate.Channels.Smtp.Tests/MailGate.Channels.Smtp.Tests.csproj
+++ /dev/null
@@ -1,28 +0,0 @@
-﻿<Project Sdk="Microsoft.NET.Sdk">
-
-  <PropertyGroup>
-    <TargetFramework>net10.0</TargetFramework>
-    <ImplicitUsings>enable</ImplicitUsings>
-    <Nullable>enable</Nullable>
-    <IsPackable>false</IsPackable>
-  </PropertyGroup>
-
-  <ItemGroup>
-    <PackageReference Include="coverlet.collector" Version="6.0.4" />
-    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
-    <PackageReference Include="NSubstitute" Version="5.3.0" />
-    <PackageReference Include="Testcontainers.RabbitMq" Version="4.11.0" />
-    <PackageReference Include="xunit" Version="2.9.3" />
-    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
-  </ItemGroup>
-
-  <ItemGroup>
-    <Using Include="Xunit" />
-  </ItemGroup>
-
-  <ItemGroup>
-    <ProjectReference Include="..\..\src\MailGate.Channels.Smtp\MailGate.Channels.Smtp.csproj" />
-    <ProjectReference Include="..\..\src\MailGate.Core\MailGate.Core.csproj" />
-  </ItemGroup>
-
-</Project>
\ No newline at end of file
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

### `a/tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj b/tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj`

`````diff
diff --git a/tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj b/tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj
new file mode 100644
index 0000000..9052c77
--- /dev/null
+++ b/tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj
@@ -0,0 +1,14 @@
+<Project Sdk="Microsoft.NET.Sdk">
+  <PropertyGroup>
+    <OutputType>Exe</OutputType>
+    <TargetFramework>net10.0</TargetFramework>
+    <ImplicitUsings>enable</ImplicitUsings>
+    <Nullable>enable</Nullable>
+  </PropertyGroup>
+  <ItemGroup>
+    <Compile Include="../../src/MailGate.Api/Templates/EmailTemplateRenderer.cs" Link="EmailTemplateRenderer.cs" />
+    <Compile Include="../../src/MailGate.Contracts/Templates/EmailTemplateDto.cs" Link="EmailTemplateDto.cs" />
+    <Compile Include="../../src/MailGate.Core/Enums/BodyFormat.cs" Link="BodyFormat.cs" />
+    <Compile Include="../../src/MailGate.Core/Models/EmbeddedEmailImage.cs" Link="EmbeddedEmailImage.cs" />
+  </ItemGroup>
+</Project>
`````

### `a/tests/MailGate.TemplateChecks/Program.cs b/tests/MailGate.TemplateChecks/Program.cs`

`````diff
diff --git a/tests/MailGate.TemplateChecks/Program.cs b/tests/MailGate.TemplateChecks/Program.cs
new file mode 100644
index 0000000..77d70a4
--- /dev/null
+++ b/tests/MailGate.TemplateChecks/Program.cs
@@ -0,0 +1,80 @@
+using System.Text.Json;
+using MailGate.Api.Templates;
+using MailGate.Contracts.Templates;
+
+var checks = 0;
+void Check(bool condition, string name)
+{
+    if (!condition) throw new Exception(name);
+    checks++;
+}
+EmailTemplateDefinition Example() => new()
+{
+    Name = "Тестовый макет", Subject = "Тема по умолчанию", Preheader = "Краткое описание",
+    Footer = "Команда поддержки", Blocks = [new() { Type = "heading", Text = "Здравствуйте" }, new() { Type = "content" }]
+};
+var definition = Example();
+Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Valid definition");
+var body = "Первая строка\r\n<script>alert('x')</script> & {{content}}";
+var rendered = EmailTemplateRenderer.Render(definition, body);
+Check(rendered.Subject == definition.Subject, "Subject fallback");
+Check(rendered.Html.Contains("Первая строка<br />&lt;script&gt;"), "Body encoding and line breaks");
+Check(!rendered.Html.Contains("<script>"), "No executable markup from text");
+Check(rendered.Html.Contains("{{content}}"), "User text is not recursively interpolated");
+Check(EmailTemplateRenderer.Render(definition, body, "  Другая тема  ").Subject == "Другая тема", "Subject override");
+Check(rendered.Html.Contains("<!--[if mso]>") && rendered.Html.Contains("role=\"presentation\""), "Email table layout");
+var snapshot = JsonSerializer.Deserialize<EmailTemplateDefinition>(JsonSerializer.Serialize(definition))!;
+Check(EmailTemplateRenderer.Render(snapshot, body).Html == rendered.Html, "JSON persistence preserves rendering");
+definition.Blocks.Add(new() { Type = "content" });
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks"), "Reject duplicate content slots");
+definition.Blocks.RemoveAll(b => b.Type == "content");
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks"), "Require content slot");
+definition = Example(); definition.AccentColor = "red; background:url(x)";
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Colors"), "Reject CSS injection");
+definition = Example(); definition.Blocks.Add(new() { Type = "button", Text = "Click", Url = "javascript:alert(1)" });
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject executable URLs");
+definition.Blocks[2].Url = "https://example.com/path?a=1&b=2";
+Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Accept HTTPS button");
+Check(EmailTemplateRenderer.Render(definition, body).Html.Contains("a=1&amp;b=2"), "Encode URL attributes");
+definition.Blocks.Add(new() { Type = "columns", Text = "<left>", SecondaryText = "<right>" });
+Check(EmailTemplateRenderer.Render(definition, body).Html.Contains("&lt;right&gt;"), "Encode column text");
+definition.Subject = "Hello\r\nBcc:someone@example.com";
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Subject"), "Reject multiline subject");
+definition = Example(); definition.Blocks.Add(null!);
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject null blocks without crashing");
+definition = Example(); definition.Blocks = Enumerable.Repeat(new EmailTemplateBlock { Type = "content" }, 41).ToList();
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks"), "Limit block count");
+Check(!EmailTemplateRenderer.IsHttpUrl("https://user:secret@example.com"), "Reject credentials in URLs");
+definition = Example();
+definition.Blocks.Insert(0, new() { Type = "image", Url = "https://example.com/banner.png?a=1&b=2", Text = "<Banner>" });
+Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Accept HTTPS image");
+var imageHtml = EmailTemplateRenderer.Render(definition, body).Html;
+Check(imageHtml.Contains("alt=\"&lt;Banner&gt;\"") && imageHtml.Contains("banner.png?a=1&amp;b=2"), "Encode image attributes");
+Check(imageHtml.IndexOf("<img ") < imageHtml.IndexOf("<h1 "), "Preserve image block order");
+foreach (var url in new[] { "http://example.com/a.png", "javascript:alert(1)", "data:image/svg+xml,test", "https://user:secret@example.com/a.png", "/image.png" })
+{
+    definition.Blocks[0].Url = url;
+    Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[0]"), "Reject unsafe image URL: " + url);
+}
+var png = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aNecAAAAASUVORK5CYII=";
+definition = Example(); definition.Blocks.Add(new() { Type = "image", Url = png });
+Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Accept uploaded PNG");
+definition.Blocks[2].Url = png.Insert(30, "\n");
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject whitespace in base64 before SMTP conversion");
+definition.Blocks[2].Url = png;
+Check(EmailTemplateRenderer.Render(definition, body).Html.Contains(png), "Preserve embedded image in HTML snapshot");
+var storedImage = JsonSerializer.Deserialize<EmailTemplateDefinition>(JsonSerializer.Serialize(definition))!;
+Check(storedImage.Blocks[2].Url == png, "Preserve uploaded bytes in template JSON");
+foreach (var invalid in new[] { "data:image/svg+xml;base64,PHN2Zz4=", "data:image/png;base64,aGVsbG8gd29ybGQ=", "data:image/png;base64,!!" })
+{
+    definition.Blocks[2].Url = invalid;
+    Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject invalid uploaded image");
+}
+var big = new byte[128 * 1024]; Convert.FromBase64String(png.Split(',')[1])[..8].CopyTo(big, 0);
+definition.Blocks[2].Url = "data:image/png;base64," + Convert.ToBase64String(big);
+Check(EmailTemplateRenderer.Validate(definition).Count == 0, "Accept image at size limit");
+for (var i = 0; i < 4; i++) definition.Blocks.Add(new() { Type = "image", Url = definition.Blocks[2].Url });
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks"), "Limit total uploaded image bytes");
+definition = Example(); definition.Blocks.Add(new() { Type = "image", Url = "data:image/png;base64," + Convert.ToBase64String(new byte[128 * 1024 + 1]) });
+Check(EmailTemplateRenderer.Validate(definition).ContainsKey("Blocks[2]"), "Reject oversized upload");
+Console.WriteLine($"Passed {checks} email template checks.");
`````

### `a/tests/MailGate.Worker.Tests/MailGate.Worker.Tests.csproj b/tests/MailGate.Worker.Tests/MailGate.Worker.Tests.csproj`

`````diff
diff --git a/tests/MailGate.Worker.Tests/MailGate.Worker.Tests.csproj b/tests/MailGate.Worker.Tests/MailGate.Worker.Tests.csproj
deleted file mode 100644
index 1060395..0000000
--- a/tests/MailGate.Worker.Tests/MailGate.Worker.Tests.csproj
+++ /dev/null
@@ -1,27 +0,0 @@
-﻿<Project Sdk="Microsoft.NET.Sdk">
-
-  <PropertyGroup>
-    <TargetFramework>net10.0</TargetFramework>
-    <ImplicitUsings>enable</ImplicitUsings>
-    <Nullable>enable</Nullable>
-    <IsPackable>false</IsPackable>
-  </PropertyGroup>
-
-  <ItemGroup>
-    <PackageReference Include="coverlet.collector" Version="6.0.4" />
-    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1" />
-    <PackageReference Include="NSubstitute" Version="5.3.0" />
-    <PackageReference Include="xunit" Version="2.9.3" />
-    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4" />
-  </ItemGroup>
-
-  <ItemGroup>
-    <Using Include="Xunit" />
-  </ItemGroup>
-
-  <ItemGroup>
-    <ProjectReference Include="..\..\src\MailGate.Worker\MailGate.Worker.csproj" />
-    <ProjectReference Include="..\..\src\MailGate.Core\MailGate.Core.csproj" />
-  </ItemGroup>
-
-</Project>
\ No newline at end of file
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


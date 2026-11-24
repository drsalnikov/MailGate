# MailGate: пошаговый перенос доработок в закрытый контур

**Если все изменения до 23.11.2026 уже перенесены, используйте отдельную [инструкцию только за 23 ноября с диффами](transfer-2026-11-23-only.md).** Шаблоны и прежние миграции повторно переносить не нужно.

Дата комплекта: **23.11.2026**. Исходная ветка — **develop**. Инструкция предназначена для переноса кода, миграций и необходимых настроек без копирования локальных паролей или подмены корпоративного Kubernetes стендом Docker Desktop.

## 1. Что переносится

| Функциональность | Основные файлы/каталоги |
| --- | --- |
| Шаблоны писем и оба конструктора | `Core/Entities/EmailTemplate.cs`, `Contracts/Templates`, `Api/Templates`, `EmailTemplatesEndpoints.cs`, страницы/компоненты frontend |
| Предпросмотр, картинки, HTML-снимок письма | `EmailTemplateRenderer`, `NotificationComposer`, `EmbeddedEmailImage`, MIME-логика `SmtpChannel` |
| Выбор SMTP Manual/Sidecar | `SmtpCredentialSource`, `SmtpCredentialResolver`, `SmtpOptions`, SMTP DTO/validator/endpoints/store/provider, `SmtpSettingsPage` |
| Ротация файловой учётки без повторного сохранения пароля | `SmtpCredentialResolver`, `DbSmtpSettingsProvider`, `OptionsSmtpSettingsProvider` |
| Ранее добавленная проверка отзыва сертификатов | SMTP options/entity/contracts/UI и миграция `AddSmtpCertificateRevocationCheck` |
| Применение миграций отдельно от отправки | `Worker/Program.cs`, аргумент `--migrate-only` |
| Авторизация по группе и возврат на исходный URL | `KeycloakGroupAccess`, API Program, frontend auth/config |
| Проверки | `tests/MailGate.TemplateChecks`, `tests/MailGate.SmtpCredentialChecks`, `tests/ci` |
| Локальный запуск | Один `docker-compose.yml`, параметры `.env`, Dockerfile и BuildKit secrets |
| Черновик GitLab CI | `.gitlab-ci.yml`, `scripts/ci`, `config/gitlab-ci.md`; deploy Kubernetes отдельно |

Здесь не все части нужно включать заново: если шаблоны, группа Keycloak или проверка отзыва уже работают в закрытой ветке, сохраните их и перенесите только отсутствующий код. Дополнительные инструкции: [SMTP sidecar](smtp-sidecar.md), [единый Compose](compose.md), [GitLab CI](gitlab-ci.md), [операционные настройки](operations.md).

## 2. Где полные диффы

В GitHub в `artifacts/transfer-2026-11-23/` лежат:

- [incremental.patch](../artifacts/transfer-2026-11-23/incremental.patch) и [incremental-diffs.md](../artifacts/transfer-2026-11-23/incremental-diffs.md): изменения после коммита `e775c18` — новые SMTP-режимы, миграция источника, единый Compose, build secrets, режим миграций и CI/документация.
- [full.patch](../artifacts/transfer-2026-11-23/full.patch) и [full-diffs.md](../artifacts/transfer-2026-11-23/full-diffs.md): суммарные изменения относительно `3536209` — включая шаблоны, картинки, перенос frontend в `src` и прежние SMTP/Keycloak-доработки.
- [manifest.json](../artifacts/transfer-2026-11-23/manifest.json): точные базы сравнения и количество изменённых файлов. Сам перечень есть в каждом Markdown с диффами.

Новые файлы в диффе показаны целиком с `+`. Существующие строки с `-` удаляются, с `+` добавляются, без знака — контекст. Полные `.patch` получены из Git, не реконструированы вручную. Markdown-версии удобны для чтения; при автоматическом применении используйте `.patch`.

**Выберите один набор, не применяйте оба подряд.** Полный набор для ветки со старой структурой не подходит автоматически к копии, где frontend и solution уже перенесены. Генерируемая инструкция и её собственные диффы исключены из patch, чтобы не создавать рекурсивный документ; на работу приложения это не влияет.

## 3. Подготовить закрытый репозиторий

Откройте **PowerShell** в корне закрытого репозитория. Команды для Linux выполняйте в соответствующем shell, не смешивайте синтаксис `$env:` с Bash.

```powershell
Get-Location
git status --short
git remote -v
git switch -c feature/mailgate-smtp-sidecar
dotnet --info
node --version
npm --version
```

Если ветка уже создана, используйте её. Сохраните свои текущие незакоммиченные изменения перед переносом. Нужны .NET SDK 10 и зависимости вашей ветки; не меняйте адреса Nexus на адреса из примеров. Исходные файлы из GitHub перенесите согласованным способом; новые документы не содержат настоящих корпоративных credentials.

Сделайте резервную копию PostgreSQL вашим стандартным способом. При наличии старых зашифрованных SMTP-паролей сохраните также существующий key ring. В Kubernetes не выполняйте удаление PVC, namespace или базы для установки этой доработки.

### Если база Git действительно совпадает

Можно сначала проверить patch. Укажите реальный путь к перенесённому файлу:

```powershell
$patchPath = Read-Host 'Полный путь к выбранному incremental.patch ИЛИ full.patch'
git apply --check "$patchPath"
```

Только если проверка успешна и набор соответствует вашей базе:

```powershell
git apply "$patchPath"
```

Если есть ошибки контекста, отличия истории миграций или свои изменения, **не форсируйте применение**. Объединяйте файлы руками по `*-diffs.md`, пользуясь следующими шагами. Не заменяйте целиком вашу рабочую авторизацию, Helm values или package registry ради совпадения с примером.

## 4. Сверить структуру файлов

Актуальная структура:

```text
src/
  MailGate.sln
  MailGate.slnx
  NuGet.Config
  MailGate.Api/
  MailGate.Worker/
  MailGate.Core/
  MailGate.Contracts/
  MailGate.Channels.Smtp/
  MailGate.Infrastructure/
  MailGate.Frontend/
tests/
  MailGate.TemplateChecks/
  MailGate.SmtpCredentialChecks/
```

Если в закрытой ветке frontend по-прежнему в `frontend/`, либо переносите его вместе с путями сборки, либо применяйте frontend-изменения к старому каталогу и корректируйте команды. Не оставляйте две разные активные копии frontend. Solution не обязателен для сборки отдельного `.csproj`; оба проекта проверок могут запускаться напрямую.

## 5. Перенести SMTP backend

Создайте новые файлы из выбранного диффа:

1. `src/MailGate.Core/Enums/SmtpCredentialSource.cs`.
2. `src/MailGate.Channels.Smtp/Services/SmtpCredentialResolver.cs`.
3. `tests/MailGate.SmtpCredentialChecks/MailGate.SmtpCredentialChecks.csproj` и `Program.cs`.

Объедините по диффам:

1. **SmtpOptions** — `CredentialSource` и `CredentialsFilePath`, существующие параметры соединения/кэша.
2. **SmtpConfiguration entity** — `CredentialSource`. Числа: Manual=0, Sidecar=1, Legacy=2.
3. **SmtpConfigurationDto** — поле источника в ответе и запросе.
4. **AddSmtpChannel** — регистрация общего `SmtpCredentialResolver`.
5. **SmtpConfigurationStore** — новые режимы не расшифровывают старые секреты; при сохранении Manual/Sidecar очищаются старые username/password/OAuth2. Metadata читается без расшифровки.
6. **DbSmtpSettingsProvider** — кэшируется конфигурация, но файловые credentials читаются при каждом получении настроек, а не помещаются в кэш. Перед обработкой копируется объект настроек.
7. **OptionsSmtpSettingsProvider** — тот же resolver для конфигурации без DB override.
8. **SmtpValidators** — только Manual/Sidecar для новых сохранений; запрет ввода credentials/OAuth2 в запросе; TLS обязательно для Sidecar; SSL и STARTTLS не включаются одновременно.
9. **SmtpAdminEndpoints** — источник сохраняется в БД; test-connection использует тот же resolver, не подставляет старый пароль при ошибке файла и не возвращает секреты в GET.
10. **ServiceCollectionExtensions** инфраструктуры — KeysPath больше не обязателен. Когда путь не задан, генерация ключей отключена; новые SMTP-режимы криптографию не вызывают. Не удаляйте прежние ключи автоматически.

Файл credentials читается только по серверной настройке. Не добавляйте поле пути файла в браузерный DTO и не разрешайте API читать произвольный путь из запроса.

Проверьте **SmtpChannel**: получение effective-настроек должно находиться внутри `try`, чтобы ошибка файла превратилась в `ChannelResult.Failure` и попала в журнал. Legacy OAuth2-отправка оставлена для существующих записей, но её ввод через новую форму скрыт/запрещён.

## 6. Перенести SMTP frontend

Объедините изменения в `src/MailGate.Frontend/src/api/types.ts` и `src/pages/SmtpSettingsPage.tsx`:

- экспорт типа `SmtpCredentialSource`;
- поле `credentialSource` в SMTP DTO;
- два варианта в форме: ручной тестовый SMTP без аутентификации и существующая учётная запись из файла;
- логин/пароль/OAuth2 не вводятся, отправляются как null;
- предупреждение для старых Legacy-записей и очистки прежних credentials при сохранении нового режима;
- после изменения формы прежний результат проверки соединения сбрасывается;
- параллельные сохранение/проверка блокируются.

Не заменяйте этот `types.ts` маленьким файлом только для SMTP: в нём также остаются типы шаблонов, адресной книги и уведомлений. При ошибках определения типов не маскируйте всё через `any`: сверяйте типы DTO и методы API.

## 7. Если шаблоны ещё не перенесены

Используйте разделы полного диффа для следующих частей:

1. Entity `EmailTemplate`, contracts `Templates`, DbSet и конфигурация `email_templates` в DbContext.
2. `EmailTemplateRenderer`, `NotificationComposer`, `EmailTemplatesEndpoints`, регистрация `MapEmailTemplatesEndpoints()`.
3. Preview endpoint уведомлений; `TemplateId`, `TemplateRevision`, `TemplateDefinition` в запросах отправки/preview.
4. В очередь должны уходить **composed.Subject, composed.Body, composed.BodyFormat**, а не исходные поля DTO.
5. MIME inline-вложения в `SmtpChannel` для загруженных изображений, helper `EmbeddedEmailImage`.
6. Frontend pages/components/hooks, маршруты и меню.
7. Общий редактор на странице шаблона и `/notifications/new`, локальная копия макета только для письма, сброс изменений и ожидание preview перед отправкой.
8. Возврат после Keycloak через `getSigninReturnPath` и сохранение исходного pathname/search/hash.

Если всё это уже перенесено, не заменяйте собственные исправления повторно полным набором. Используйте дополнительный дифф и существующую инструкцию `artifacts/mailgate-templates-full-instructions.md` для сверки старых частей.

## 8. Миграции — выбрать один способ

В исходной ветке есть три доработочные миграции:

| Миграция | Что делает |
| --- | --- |
| `20261026090000_AddSmtpCertificateRevocationCheck` | Поле проверки отзыва сертификата |
| `20261028090000_AddEmailTemplates` | Таблица шаблонов |
| `20261123090000_AddSmtpCredentialSource` | Источник credentials SMTP; существующие записи получают Legacy |

**При совпадающей истории** добавьте отсутствующие `.cs` и `.Designer.cs`, объедините snapshot. Не удаляйте исходную `InitialCreate` и применённые миграции.

**Если в закрытой ветке те же поля/таблицы уже созданы вашими миграциями с другими именами**, не добавляйте ещё одну миграцию для той же операции. Перенесите модель и создайте только недостающую миграцию по вашей истории. Тогда готовые Designer/snapshot целиком не копируются.

Проверьте инструмент:

```powershell
dotnet ef --version
```

Если не установлен, используйте вашу версию EF и настоящий корпоративный NuGet.Config. Для текущей модели EF 10.0.6:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.6 --configfile .\src\NuGet.Config
```

Для **второго способа**, после переноса модели, если миграция источника ещё не существует:

```powershell
dotnet ef migrations add AddSmtpCredentialSource --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker
```

Проверьте Up(): новая колонка CredentialSource, старым строкам назначается 2 (Legacy). Новые Manual=0 должны реально сохраняться как 0, а не подменяться DB default=2: готовая миграция после заполнения старых строк убирает default. Не допускайте непредусмотренного удаления/пересоздания таблиц. При выборе сгенерированной миграции согласуйте ту же семантику.

## 9. Проверить сборку исходников

```powershell
dotnet restore .\src\MailGate.Api\MailGate.Api.csproj --configfile .\src\NuGet.Config
if ($LASTEXITCODE -ne 0) { throw 'API restore failed' }
dotnet restore .\src\MailGate.Worker\MailGate.Worker.csproj --configfile .\src\NuGet.Config
if ($LASTEXITCODE -ne 0) { throw 'Worker restore failed' }
dotnet build .\src\MailGate.Api\MailGate.Api.csproj --no-restore
if ($LASTEXITCODE -ne 0) { throw 'API build failed' }
dotnet build .\src\MailGate.Worker\MailGate.Worker.csproj --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Worker build failed' }
dotnet run --project .\tests\MailGate.TemplateChecks
if ($LASTEXITCODE -ne 0) { throw 'Template checks failed' }
dotnet run --project .\tests\MailGate.SmtpCredentialChecks
if ($LASTEXITCODE -ne 0) { throw 'SMTP credential checks failed' }
```

Ожидаются 37 проверок шаблонов и 25 проверок credentials. Проверки не используют SMTP/Vault/БД. Для frontend перейдите в настоящий каталог, чтобы npm увидел проектный `.npmrc`:

```powershell
Push-Location .\src\MailGate.Frontend
try {
    npm ci
    if ($LASTEXITCODE -ne 0) { throw 'npm ci failed' }
    npm run build
    if ($LASTEXITCODE -ne 0) { throw 'Frontend build failed' }
}
finally { Pop-Location }
```

Корневой `.env` автоматически не задаёт NPM_TOKEN для любой локальной команды. Если npmrc использует `${NPM_TOKEN}`, предоставьте токен процессу PowerShell вашим механизмом секретов либо используйте авторизацию npm, разрешённую организацией. Ошибки 401/ENOTFOUND — это доступ к настоящему Nexus/DNS, а не повод заменить hostname адресом из примера или положить пароль в Git.

## 10. Применить миграции к нужной БД

В Kubernetes используйте ваш принятый migration Job или согласованный SQL-процесс. Новый Worker поддерживает аргумент `--migrate-only`: применяет схему и завершается до старта MassTransit. Контейнер/Job должен получить действительную DB-конфигурацию; init/sidecar такого Job не должен мешать его завершению. Сам режим миграций SMTP-файл не читает.

Если выполняете миграцию с Windows, укажите БД, доступную **с Windows**, а не внутреннее имя Kubernetes Service. В текущей design-time factory используется `ConnectionStrings__DefaultConnection`:

```powershell
$connection = Read-Host 'Введите настоящую строку PostgreSQL: Host=...;Port=...;Database=...;Username=...;Password=...' -AsSecureString
$env:ConnectionStrings__DefaultConnection = [System.Net.NetworkCredential]::new('', $connection).Password
try {
    dotnet ef migrations list --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker
    if ($LASTEXITCODE -ne 0) { throw 'Cannot read migration history' }
    dotnet ef database update --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker
    if ($LASTEXITCODE -ne 0) { throw 'Database migration failed' }
}
finally {
    Remove-Item Env:ConnectionStrings__DefaultConnection -ErrorAction SilentlyContinue
    Remove-Variable connection
}
```

Многоточия в приглашении — пояснение структуры, не значения для копирования. Настоящие Host/Port/Database/Username/Password возьмите из действующей конфигурации приложения или у администратора. Пароли не присылайте в чат и не печатайте в pipeline. Backup обязателен до изменения схемы. Не применяйте SQL и dotnet ef одновременно к одной операции.

## 11. Kubernetes и Vault sidecar — необходимые изменения инфраструктуры

Для вашего закрытого контура это основной способ запуска. **Не переносите локальный Compose, тестовый realm или локальные адреса автоматически в Helm values.** Сохраните ваш корпоративный Keycloak, ingress, Service, сети и правила доступа.

В Pod **API и Worker** sidecar должен отрисовывать:

```json
{
  "username": "НАСТОЯЩИЙ_SMTP_ЛОГИН",
  "password": "НАСТОЯЩИЙ_SMTP_ПАРОЛЬ"
}
```

Итоговый путь: `/var/mailgate/smtp-credentials/account.json`. В env обоих приложений:

```yaml
- name: Smtp__CredentialsFilePath
  value: /var/mailgate/smtp-credentials/account.json
```

Настройте чтение каталога приложением read-only и запись sidecar; обычно подходит memory `emptyDir` внутри каждого Pod. Общий меж-Pod PVC не нужен: оба sidecar читают одну логическую учётку из Vault и отрисовывают её в свои Pod. Не используйте subPath для одного файла, если отрисовка заменяет inode.

Имя volume может быть вашим прежним, но его содержимое теперь **не XML key ring**. Не смешивайте JSON credentials и прежние ключи. Sidecar нужны права **read** SMTP-секрета Vault; приложение Vault-токен не получает. Укажите корректные ServiceAccount, role, secret path, UID/GID/fsGroup и права в вашем chart. Новый JSON не должен содержать оболочку `data/metadata`; кавычки пароля экранируются JSON-сериализатором шаблона.

Файл обновляется атомарно. API читает его при test-connection, Worker при получении настроек перед каждой отправкой. Изменения файла видны без перезапуска приложения, но реальная ротация пароля включает время обновления Vault/sidecar и самого SMTP. При ошибке файла нет fallback к старому паролю или анонимной отправке. Последний оставшийся валидный файл сам по себе не истекает — мониторинг sidecar нужен отдельно.

В новых Manual/Sidecar режимах Data Protection не требуется, поэтому `DataProtection__KeysPath`/PVC ключей можно не задавать на новом развёртывании. Существующие Legacy-записи сохраняются и требуют прежних ключей до переключения. При явном сохранении нового режима прежние credentials удаляются из соответствующей строки БД; **не удаляйте ключи прежде проверки перехода всех нужных записей**.

Публичные CA должны быть установлены в **API и Worker**. STARTTLS либо SSL обязательны для файловой учётки. Не отключайте TLS-доверие ради запуска. Изменение политики отзыва сертификатов — отдельное согласованное решение, а не часть монтирования файла.

## 12. Образы и pipeline в закрытом контуре

Пересоберите и опубликуйте все три образа: API, Worker, frontend. Новые DTO/колонка должны быть согласованы между API/frontend; Worker обязательно обновить для нового источника credentials и MIME шаблонов.

В API/Worker Dockerfile есть необязательный BuildKit secret `nuget_config`. Во frontend — `npmrc` и `npm_token`. Точные параметры вашего builder зависят от Runner; не передавайте секреты через обычные ARG/COPY.

**GitLab YAML в комплекте рассчитан на Linux Shell Runner с Docker.** Job deploy по SSH относится только к Compose, отключена по умолчанию и включается лишь `MAILGATE_DEPLOY_TARGET=compose-ssh`. Для вашего Kubernetes её не включайте. Подключите принятый Helm/Argo CD/GitLab Agent deploy и проверку rollout после migration Job. В этом репозитории нет готового проверенного chart вашего кластера или kubeconfig, поэтому команда `kubectl apply` с выдуманным namespace здесь не приводится.

Сохраняйте теги трёх образов конкретного pipeline. Не используйте изменяемый latest как доказательство, что запущен новый код. Проверьте image tag/digest и rollout средствами вашей платформы.

## 13. Локальная проверка (при необходимости)

Локально теперь один файл:

```powershell
docker compose up -d --build
docker compose ps
```

По умолчанию это официальные .NET-образы, Keycloak 8181, frontend 8091, API 8080, MailHog 8025. Для корпоративных base images задайте в `.env` API_DOCKERFILE/WORKER_DOCKERFILE. Для учётки задайте SMTP_CREDENTIALS_DIRECTORY на каталог с account.json. Для приватного npm укажите NPMRC_FILE/NPM_TOKEN_FILE; для NuGet — NUGET_CONFIG_FILE. Настоящие секреты и файлы account.json не коммитьте.

Дополнительные Compose-файлы удалены. При переносе существующего локального Keycloak, который раньше держал H2 внутри контейнера, сначала перенесите БД в новый keycloak_data volume — подробности в `config/compose.md`. `down -v` не является обычным обновлением.

## 14. Приёмка после развёртывания

1. Приложение открывается по вашему корпоративному адресу, Keycloak остаётся корпоративным. Вход пользователя нужной группы работает; пользователь без доступа получает отказ.
2. GET SMTP-настроек не возвращает пароль из файла. Страница показывает только два новых режима и предупреждение о Legacy, когда оно действительно нужно.
3. Ручной режим проверяется на разрешённом тестовом SMTP без аутентификации, не на корпоративной учётке с парольным входом.
4. Файловый режим: правильный JSON, доступный SMTP, STARTTLS/SSL, разрешённый FromAddress. «Проверить подключение» проходит.
5. Сохраните режим, подождите TTL кэша параметров Worker (обычно до 30 секунд), отправьте на разрешённый тестовый адрес. Проверьте журнал и фактическую доставку.
6. Пустой/некорректный файл должен дать понятную ошибку, не анонимную отправку. Пароль/JSON/путь не должны попасть в логи ответа.
7. При ротации файла следующий запрос использует новую пару. Изменение локального JSON не меняет пароль на SMTP-сервере.
8. Создание/редактирование шаблона, загрузка картинки, две колонки, mobile/desktop preview и внутренняя прокрутка работают.
9. На `/notifications/new` правки макета применяются только к письму; исходный шаблон после отправки не меняется. Картинка приходит inline CID.
10. Перезапуск API/Worker/Pod не требует повторного ввода файлового пароля и не зависит от прежнего key ring.

Успешный test-connection не гарантирует MAIL FROM/Send As/relay и конечную доставку. Отображение в Outlook проверяйте реальным письмом. Если health endpoint зелёный, это ещё не доказательство правильности файловой учётки: её проверяют SMTP test и отправка.

## 15. Типовые ошибки

| Ошибка | Действие |
| --- | --- |
| Колонка CredentialSource отсутствует | Применить миграцию к той же БД, которую использует API/Worker |
| migration already exists / duplicate column | Сверить закрытую историю и не копировать вторую миграцию с той же операцией |
| Ошибка чтения SMTP-файла | JSON без BOM, правильный путь, права, отрисовка в обоих Pod, без subPath на заменяемый файл |
| Legacy key not found / payload invalid | Сохранить старые ключи для старых данных либо явно перейти на новый режим; не подставлять ciphertext как пароль |
| 535 | Проверить пару, SMTP AUTH, блокировки и момент ротации на почтовом сервере |
| TLS issuer / CRL | Проверить CA и доступ к CRL внутри обоих контейнеров, это не ошибка пароля |
| npm/NuGet 401 | Настоящие credentials Nexus и механизм передачи секрета в выбранный builder |
| ENOTFOUND | Настоящие DNS/VPN/proxy с Runner и из контейнера; не копировать демонстрационные hostname |
| Frontend старый | Проверить реально запущенный image/digest и обновление статических файлов |

## Проверенный локальный результат и ограничения

Для итогового состояния проверяются YAML/Bash, сборки API/Worker/frontend, 37 template checks, 25 SMTP credential checks и 8 offline CI-script tests. Локально проверены миграция CredentialSource, сохранение обоих режимов, отсутствие credentials в ответах, чтение read-only каталога, атомарная замена файла, работа без KeysPath и реальная отправка только в локальный MailHog.

Это не тест настоящего корпоративного Vault/SMTP/Kubernetes и не проверка chart, которого нет в репозитории. Восстановление/перезапуск локального Keycloak выполнены с резервными копиями, но не переносят корпоративных пользователей. В исходных зависимостях остаются ранее отмеченные NuGet advisory warnings; этот комплект не объявляется исправлением всех уязвимостей пакетов.

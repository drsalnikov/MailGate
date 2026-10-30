# MailGate: полная инструкция по переносу шаблонов и конструкторов

Дата реализации: 28.10.2026. Команды ниже предназначены для **PowerShell в Windows**, не для CMD. Откройте терминал PowerShell в корне закрытого репозитория MailGate. Саму инструкцию можно читать без дополнительных документов: в конце находятся полный перечень файлов и все диффы, включая полное содержимое новых файлов.

Результат: список шаблонов, конструктор блоков и оформления, загрузка изображений с компьютера, прокручиваемый предпросмотр, редактирование макета только текущего письма на `/notifications/new`, отправка картинок как встроенных MIME-вложений и корректный возврат из Keycloak по прямым ссылкам.

## Как проходить эту инструкцию

Выполняйте шаги по порядку. Код переносите из приложения «Полные диффы» в конце этого файла: строки с `+` добавляются, строки с `-` удаляются, строки без этих префиксов — контекст. Для новых файлов показано всё содержимое с `+`; сами префиксы в исходник не копируйте.

Для ускорения рядом подготовлен архив `mailgate-templates-2026-10-28.zip`: новые файлы в нём лежат в `new-files/`. Если архив недоступен, создайте эти файлы руками из диффов — результат будет тем же.

**База диффов тематически восстановлена.** В проекте смешаны сегодняшняя система шаблонов и более ранние исправления. Комплект показывает только изменения для шаблонов относительно версии без этой функциональности, с приведением путей к текущей структуре. Это не история всех изменений Git и не гарантия автоматического применения к вашей закрытой ветке. Существующие файлы объединяйте по диффам, сохраняя свои настройки и исправления.

Не переносите локальные `docker-compose.local.yml`, `src/Dockerfile.local`, `config/keycloak/mailgate-local-realm.json`. Корпоративные Keycloak, Nexus, SMTP, сертификаты, Data Protection/key ring, RabbitMQ и пароли остаются вашими. Для шаблонов новый Keycloak-клиент или новая роль не нужны: используется существующая политика `MailGateAdmin`.

## Шаг 1. Проверить терминал, пути и инструменты

```powershell
Get-Location
git status --short
dotnet --info
node --version
npm --version
docker version
docker compose version

Test-Path .\src\MailGate.Api\MailGate.Api.csproj
Test-Path .\src\MailGate.Worker\MailGate.Worker.csproj
Test-Path .\src\MailGate.Infrastructure\MailGate.Infrastructure.csproj
Test-Path .\src\MailGate.Frontend\package.json
```

Четыре `Test-Path` должны вернуть `True`. Иначе скорректируйте пути во всей инструкции: например, если frontend у вас всё ещё в `frontend`, используйте этот путь вместо `src/MailGate.Frontend`. Переносить папки ради шаблонов не нужно.

Текущие C#-проекты используют `net10.0`. Нужен .NET SDK 10. Для frontend используйте существующее окружение проекта; локальная Docker-сборка выполнялась на Node 22. Новых npm/NuGet-зависимостей эта функциональность не требует.

Создайте рабочую ветку, если ещё не создали:

```powershell
git switch -c feature/email-templates
```

Если ветка с этим именем уже существует, переключитесь на нужную существующую ветку. Не применяйте `git reset --hard` и не удаляйте текущие изменения ради переноса.

## Шаг 2. Подготовить dotnet ef

```powershell
dotnet ef --version
```

Если команда работает, используйте её. Версия инструмента должна соответствовать EF Core вашей ветки. В текущем проекте EF Core и Design имеют версию `10.0.6`. Версия Data Protection здесь не является версией EF.

Если инструмента нет, установите его через ваш существующий NuGet.Config/Nexus. Команда для текущего расположения файла и версии EF:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.6 --configfile .\src\NuGet.Config
dotnet ef --version
```

Если `dotnet-ef` установлен, но требуется другая согласованная версия, используйте `dotnet tool update --global dotnet-ef --version <ваша-версия-EF> --configfile <путь-к-вашему-NuGet.Config>`. Угловые скобки — обозначение значений, а не часть команды. При 401 настройте существующий доступ к корпоративному Nexus; шаблоны не устраняют ошибки авторизации package registry.

## Шаг 3. Сделать резервную копию БД

Если PostgreSQL работает в Compose как сервис `postgres`, выполните:

```powershell
docker compose config --services
docker compose ps postgres

$backupDir = Join-Path $env:USERPROFILE 'MailGateBackups'
New-Item -ItemType Directory -Force -Path $backupDir | Out-Null
$backupFile = Join-Path $backupDir ('mailgate-before-templates-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '.dump')

docker compose exec -T postgres sh -c 'pg_dump -U "$POSTGRES_USER" -d "$POSTGRES_DB" -Fc -f /tmp/mailgate-before-templates.dump'
if ($LASTEXITCODE -ne 0) { throw 'pg_dump failed' }
docker compose cp postgres:/tmp/mailgate-before-templates.dump "$backupFile"
if ($LASTEXITCODE -ne 0) { throw 'Copying backup failed' }
Get-Item "$backupFile" | Select-Object FullName,Length
```

Файл должен существовать и иметь ненулевой размер. Бэкап находится вне Git-репозитория. Если PostgreSQL внешний или у сервиса другое имя, используйте согласованный для вашего контура backup-процесс вместо этих команд. Указанные команды не останавливают и не очищают базу.

## Шаг 4. Добавить новые исходники

Создайте новые файлы, перечисленные в приложении. Их всего 17, включая два файла готовой миграции.

**При рекомендуемом пути для закрытой ветки пока не копируйте эти два файла:**

- `src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs`;
- `src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs`.

В шаге 10 вы сгенерируете миграцию по собственной истории модели. Готовая миграция нужна только в альтернативном варианте для совпадающей истории.

Если используете ZIP, положите его в доступную вам папку, укажите путь через диалог и распакуйте вне репозитория:

```powershell
$packageZip = Read-Host 'Полный путь к mailgate-templates-2026-10-28.zip'
$unpackDir = Join-Path $env:TEMP ('mailgate-templates-' + (Get-Date -Format 'yyyyMMdd-HHmmss'))
Expand-Archive -LiteralPath $packageZip -DestinationPath $unpackDir
$newFilesRoot = Join-Path $unpackDir 'new-files'
$repoRoot = (Get-Location).Path

Get-ChildItem -LiteralPath $newFilesRoot -File -Recurse | ForEach-Object {
    $relativePath = $_.FullName.Substring($newFilesRoot.Length).TrimStart([char[]]'\/')
    if ($_.Name -like '20261028090000_AddEmailTemplates*') { return }
    $destination = Join-Path $repoRoot $relativePath
    if (Test-Path -LiteralPath $destination) {
        Write-Warning "Уже существует, объедините вручную: $relativePath"
    } else {
        New-Item -ItemType Directory -Force -Path (Split-Path -Parent $destination) | Out-Null
        Copy-Item -LiteralPath $_.FullName -Destination $destination
    }
}
```

Этот скрипт не заменяет существующие файлы. Все предупреждения «Уже существует» нужно разобрать вручную. Можно обойтись без ZIP и создать те же файлы из полных диффов ниже.

## Шаг 5. Изменить Core, Contracts и DbContext

Новые `EmailTemplate.cs`, `EmbeddedEmailImage.cs` и `EmailTemplateDto.cs` должны уже находиться в своих проектах. SDK автоматически включает `.cs` в сборку — дополнительных Include в основных `.csproj` не требуется.

Откройте и измените по соответствующим диффам:

1. `src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs`: добавьте `DbSet<EmailTemplate>` и конфигурацию таблицы `email_templates`.
2. `src/MailGate.Contracts/Notifications/NotificationLogDto.cs`: добавьте в `SendNotificationFromAdminDto` поля `TemplateId`, `TemplateRevision`, `TemplateDefinition`.
3. Snapshot пока оставьте без изменений, если будете генерировать миграцию в шаге 10.

Не добавляйте повторно `CheckCertificateRevocation` и другие ранее перенесённые SMTP-поля — они не являются частью этой доработки.

Картинки хранятся внутри `DefinitionJson` в PostgreSQL, а не в отдельном файловом каталоге. Поэтому Docker volume для картинок не нужен. Лимиты задаются в `EmbeddedEmailImage`: 128 КБ на файл и 512 КБ суммарно. Поддерживаются PNG/JPEG/GIF; SVG не поддерживается. Проверяются размер, формат base64 и сигнатура файла, а не полное декодирование всех пикселей.

## Шаг 6. Подключить API

Новые `EmailTemplateRenderer.cs`, `NotificationComposer.cs`, `EmailTemplatesEndpoints.cs` уже добавлены. Дополнительной регистрации composer/renderer в DI не нужно: это статические классы.

Измените по полным диффам:

1. `src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs`.
2. `src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs`.
3. `src/MailGate.Api/Program.cs`: добавьте `app.MapEmailTemplatesEndpoints();` рядом с существующими admin endpoints.

Особенно проверьте `SendAsync`: в `NotificationMessage` должны попасть **composed.Subject, composed.Body, composed.BodyFormat**. Если оставить `dto.Body`, письмо уйдёт без макета, даже при рабочем предпросмотре.

API использует существующий `MailGateDbContext` и существующую политику `MailGateAdmin`. Настройки authority/audience/группы/ролей и другие административные маршруты сохраните своими.

Новые маршруты:

| Метод | Путь | Назначение |
| --- | --- | --- |
| GET/POST | `/api/admin/email-templates` | Список / создание |
| GET/PUT/DELETE | `/api/admin/email-templates/{id}` | Просмотр / изменение / удаление |
| POST | `/api/admin/email-templates/preview` | Предпросмотр макета конструктора |
| POST | `/api/admin/notifications/preview` | Предпросмотр конкретного письма |

## Шаг 7. Обновить SMTP-формирование письма

В `src/MailGate.Channels.Smtp/SmtpChannel.cs` примените тематический дифф:

1. Добавьте `using System.Text.RegularExpressions;`.
2. В `BuildMimeMessage` замените одиночное присваивание `mime.Body = new TextPart(...)` на блок с `BodyBuilder`, `LinkedResources` и заменой data URL на `cid:`.

В текущем файле уже подключён `MailGate.Core.Models`, где находится `EmbeddedEmailImage`. Если в вашей версии using отсутствует, добавьте `using MailGate.Core.Models;`.

Сохраните текущую SMTP-аутентификацию, STARTTLS/SSL, проверку отзыва сертификатов и Data Protection. Они не заменяются ради картинок. Обновление только API недостаточно: именно Worker выполняет преобразование изображения во встроенное вложение.

`NotificationMessage` менять не нужно. Готовый HTML-снимок содержит встроенные данные; при доставке Worker создаёт MIME. Старые HTTPS-картинки поддерживаются для совместимости, но новый интерфейс предлагает загрузку файла.

## Шаг 8. Подключить frontend

Новые общие компоненты, страницы, hook и `returnPath.ts` уже добавлены в шаге 4. Существующие файлы измените по полным диффам:

1. `src/MailGate.Frontend/src/api/types.ts` — типы шаблонов и новые поля запросов.
2. `src/MailGate.Frontend/src/api/endpoints.ts` — методы API.
3. `src/MailGate.Frontend/src/App.tsx` — импорты и маршруты `/templates`, `/templates/new`, `/templates/:id/edit`.
4. `src/MailGate.Frontend/src/layout/AppLayout.tsx` — пункт меню и его выделение.
5. `src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx` — выбор шаблона, локальная копия, конструктор, preview и отправка.

Страница редактирования исходного шаблона и страница нового письма используют один `EmailTemplateEditor`. Не создавайте два независимых редактора: иначе исправления начнут расходиться.

Проверьте, что preview и send отправляют **одинаковый `templateDefinition`**. Правки конкретного письма не отправляются в PUT исходного шаблона. `templateRevision` нужна для проверки версии исходного шаблона, даже при локальном редактировании.

`EmailPreview` уже содержит ограничение высоты `calc(100dvh - 40px)`, flex-разметку и iframe с внутренней прокруткой. Desktop-ширина iframe — 680 px, телефон — 375 px. Не возвращайте `maxWidth: 100%` для desktop iframe: узкая панель снова включит мобильную раскладку двух колонок.

## Шаг 9. Исправить возврат после Keycloak

Измените только соответствующие участки двух файлов:

- `src/MailGate.Frontend/src/auth/oidc.ts`: импорт `getSigninReturnPath`, обработчик `onSigninCallback` с восстановлением `user.state` и уведомлением router.
- `src/MailGate.Frontend/src/auth/RequireAuth.tsx`: передавать pathname/search/hash при обычном и повторном входе.

Ваши authority, clientId, scope, userStore и правила доступа не меняйте. Ссылка «Управление шаблонами ↗» остаётся в новой вкладке, чтобы не потерять заполненную рассылку.

## Шаг 10. Создать миграцию по своей истории

Это рекомендуемый вариант, если закрытая ветка отличается от локальной. Не добавляйте готовые два файла миграции из ZIP и не заменяйте snapshot вручную перед генерацией.

```powershell
dotnet restore .\src\MailGate.Api\MailGate.Api.csproj --configfile .\src\NuGet.Config
if ($LASTEXITCODE -ne 0) { throw 'API restore failed' }
dotnet restore .\src\MailGate.Worker\MailGate.Worker.csproj --configfile .\src\NuGet.Config
if ($LASTEXITCODE -ne 0) { throw 'Worker restore failed' }

dotnet build .\src\MailGate.Api\MailGate.Api.csproj --no-restore
if ($LASTEXITCODE -ne 0) { throw 'API build failed' }
dotnet build .\src\MailGate.Worker\MailGate.Worker.csproj --no-restore
if ($LASTEXITCODE -ne 0) { throw 'Worker build failed' }

dotnet ef migrations list --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker --no-connect
```

Если миграция `AddEmailTemplates` уже существует, не создавайте дубликат: проверьте её и переходите к сборке/применению. Иначе:

```powershell
dotnet ef migrations add AddEmailTemplates --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker
if ($LASTEXITCODE -ne 0) { throw 'Generating migration failed' }
```

EF создаст `..._AddEmailTemplates.cs`, `.Designer.cs` и обновит `MailGateDbContextModelSnapshot.cs`. Их timestamp будет отличаться от `20261028090000` — это нормально.

Откройте созданный `Up()`. Ожидается **только создание таблицы `email_templates` и индекса `IX_email_templates_UpdatedAt`**, как в готовом образце в приложении. Если появились изменения других таблиц, разберите несоответствие вашей модели предыдущему snapshot до применения. В частности, не протаскивайте случайно незавершённые SMTP-миграции вместе с шаблонами.

### Альтернатива: история миграций полностью совпадает

Вместо генерации можно добавить готовые `20261028090000_AddEmailTemplates.cs`, `.Designer.cs` и блок `EmailTemplate` из диффа snapshot. Делайте это только при совпадении остальных моделей. Готовый Designer содержит ранее добавленное SMTP-поле `CheckCertificateRevocation`.

Выбирайте **один** вариант: готовая миграция или сгенерированная в вашей ветке. Не добавляйте обе миграции, создающие одну таблицу.

## Шаг 11. Выполнить проверки исходников

```powershell
dotnet build .\src\MailGate.Api\MailGate.Api.csproj
if ($LASTEXITCODE -ne 0) { throw 'API build failed' }
dotnet build .\src\MailGate.Worker\MailGate.Worker.csproj
if ($LASTEXITCODE -ne 0) { throw 'Worker build failed' }
dotnet run --project .\tests\MailGate.TemplateChecks
if ($LASTEXITCODE -ne 0) { throw 'Template checks failed' }

npm ci --prefix .\src\MailGate.Frontend
if ($LASTEXITCODE -ne 0) { throw 'npm ci failed' }
npm run build --prefix .\src\MailGate.Frontend
if ($LASTEXITCODE -ne 0) { throw 'Frontend build failed' }
```

Ожидается `Passed 37 email template checks.` и успешные сборки. npm использует ваш существующий `.npmrc`/Nexus и переменную NPM_TOKEN, если они нужны. Не добавляйте значение токена в исходники. Если версии зависимостей не менялись и они уже установлены, шаг `npm ci` можно пропустить.

Добавление проекта проверок в solution необязательно для работы приложения. Для удобства вместо ручного переноса UUID из `.sln` можно выполнить:

```powershell
dotnet sln .\src\MailGate.sln add .\tests\MailGate.TemplateChecks\MailGate.TemplateChecks.csproj
```

Для `.slnx` или solution в корне укажите свой фактический путь. Если запускаете команду добавления, диффы solution вручную повторно не применяйте.

## Шаг 12. Применить миграцию к нужной PostgreSQL

В текущем проекте design-time factory берёт строку из **`ConnectionStrings__DefaultConnection`**, а не автоматически из `.env`. При выполнении с Windows используйте адрес PostgreSQL, доступный с Windows. Имя сервиса `postgres` обычно доступно внутри Docker-сети, но не с хоста.

Введите действительную строку подключения к своей БД без отображения пароля в терминале:

```powershell
$databaseConnection = Read-Host 'Строка подключения PostgreSQL: Host=...;Port=...;Database=...;Username=...;Password=...' -AsSecureString
$env:ConnectionStrings__DefaultConnection = [System.Net.NetworkCredential]::new('', $databaseConnection).Password

dotnet ef migrations list --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker
if ($LASTEXITCODE -ne 0) { throw 'Cannot read migration history' }

dotnet ef migrations script --idempotent --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker --output "$backupDir\mailgate-template-migrations.sql"
if ($LASTEXITCODE -ne 0) { throw 'Generating SQL failed' }
```

Откройте SQL и проверьте историю/таблицу. Idempotent-скрипт включает историю миграций, а не только последний файл. После проверки и наличия бэкапа:

```powershell
dotnet ef database update --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker
if ($LASTEXITCODE -ne 0) { throw 'Database migration failed' }

dotnet ef migrations list --project .\src\MailGate.Infrastructure --startup-project .\src\MailGate.Worker
Remove-Item Env:ConnectionStrings__DefaultConnection
Remove-Variable databaseConnection
```

Новая миграция должна числиться применённой. Если процесс выполняется через корпоративный pipeline/DBA, передайте проверенный SQL в ваш процесс вместо прямого `database update`. Не применяйте его к другой среде по умолчанию.

Worker в текущем коде также вызывает `Database.MigrateAsync()` при старте. Это не отменяет контроль порядка развёртывания: таблица должна появиться до использования нового API. `docker compose down -v`, удаление volume или пересоздание БД для этой доработки не нужны.

## Шаг 13. Пересобрать и обновить API, Worker и frontend

Если ваш Compose содержит build для этих трёх сервисов:

```powershell
docker compose build api worker frontend
if ($LASTEXITCODE -ne 0) { throw 'Docker build failed' }
docker compose up -d --no-deps api worker frontend
if ($LASTEXITCODE -ne 0) { throw 'Starting containers failed' }
docker compose ps
docker compose logs --tail 100 api worker frontend
```

По умолчанию cache оставляем включённым. `--no-cache` не требуется для обычного переноса исходников. `--force-recreate` сам по себе не собирает новые образы.

Если frontend использует **уже настроенный** `docker-compose.npm.yml` для корпоративных build secrets, во всех командах сборки/запуска используйте те же Compose-файлы:

```powershell
docker compose -f docker-compose.yml -f docker-compose.npm.yml build api worker frontend
if ($LASTEXITCODE -ne 0) { throw 'Docker build failed' }
docker compose -f docker-compose.yml -f docker-compose.npm.yml up -d --no-deps api worker frontend
```

NPM_TOKEN должен быть передан существующим механизмом секретов. Добавлять локальный Compose override или менять корпоративные Docker base images ради шаблонов не нужно.

Если pipeline публикует готовые образы, вместо локальных команд выполните стандартные стадии сборки/публикации/развёртывания **всех трёх** компонентов в вашем pipeline. Убедитесь, что Dockerfile копирует добавленные папки исходников, а образ Worker тоже обновился.

## Шаг 14. Проверить работу в интерфейсе

Откройте **свой существующий адрес MailGate** и обновите страницу. Локальные адреса из моего окружения в корпоративные настройки не переносите.

1. В меню «Рассылка» появился пункт «Шаблоны писем». Откройте его.
2. Создайте шаблон «Проверка переноса», задайте тему и подпись.
3. Добавьте заголовок, две колонки, кнопку, изображение. Для кнопки укажите корректный HTTP(S)-адрес.
4. Загрузите PNG/JPEG/GIF до 128 КБ с компьютера и поднимите блок в начало.
5. Вставьте пример основного текста. Основной content-блок должен остаться ровно один.
6. В preview «Компьютер» две колонки стоят рядом; в «Телефон» — друг под другом. Длинное письмо прокручивается внутри preview.
7. Сохраните шаблон, перезагрузите страницу и откройте его заново. Текст, картинка и порядок блоков должны сохраниться.
8. Откройте `/notifications/new`, выберите этот шаблон. Должен появиться тот же конструктор без поля переименования исходного шаблона.
9. Введите получателя из разрешённых тестовых адресов и основной текст. Измените заголовок/подпись/цвета/изображение для этого письма.
10. Дождитесь завершения preview. Он должен содержать внесённые правки.
11. Отправьте. Проверьте статус в журнале и полученное письмо. Картинка должна быть внутри письма, без необходимости скачивания с внешнего HTTPS-сервера.
12. Откройте исходный шаблон: он остался прежним. Проверьте кнопку «Сбросить изменения макета» на странице нового письма.
13. Проверьте отправку без шаблона: обычный текст по-прежнему работает. Режим Markdown не получил отдельный Markdown-to-HTML конвертер в этой доработке.
14. Пустая тема конкретного письма берётся из макета, непустая переопределяет тему макета.
15. Нажмите «Управление шаблонами ↗»: новая вкладка открывает `/templates`, а после входа Keycloak не сбрасывает её на главную.
16. Создайте шаблон в соседней вкладке и вернитесь к письму: список должен обновиться без перезагрузки страницы.
17. Отредактируйте один исходный шаблон в двух вкладках. Попытка сохранить устаревшую версию получает 409. При отправке с устаревшей версией API просит обновить выбор.

Локальные правки макета конкретного письма не являются сохранённым черновиком: закрытие страницы или смена шаблона их сбрасывает. Загрузка файла временно блокирует изменение блоков, чтобы картинка не попала в другой блок. Почтовые клиенты могут отображать HTML по-разному — отдельно проверьте используемый вами Outlook.

## Шаг 15. Если что-то не работает

| Признак | Что проверить |
| --- | --- |
| Не появился пункт меню | Собран и запущен новый frontend; применены App.tsx и AppLayout.tsx; обновлена вкладка браузера |
| 404 на шаблонах API | В Program.cs добавлен `MapEmailTemplatesEndpoints`, работает новый API |
| Ошибка `email_templates does not exist` | Миграция применена к той же БД, к которой подключён API |
| Preview работает, письмо без макета | В очередь передаются значения `composed`, а не `dto` |
| В preview картинка есть, в письме нет | Обновлён Worker и SMTP MIME-блок с LinkedResources/cid, а не только API/frontend |
| Не удаётся сохранить картинку | PNG/JPEG/GIF, 128 КБ на файл, 512 КБ суммарно; проверьте конкретный текст ошибки API |
| HTTP 413 при картинках | Лимит размера запроса на reverse proxy/ingress; base64 увеличивает размер примерно на треть, плюс остальной JSON |
| Ошибка размера сообщения брокера | Лимит брокера должен вмещать HTML-снимок с base64; не увеличивайте лимиты картинок без проверки очереди и журнала |
| `dotnet ef` неизвестна | PowerShell, SDK, установка инструмента; при необходимости переоткрыть терминал после установки |
| NuGet/npm 401 | Существующий корпоративный доступ к Nexus; секреты не попадают автоматически из .env в любую команду сборки |
| Страница шаблонов открывает главную | Применены изменения oidc/RequireAuth/returnPath и запущен новый frontend |
| Кнопка отправки неактивна | Ошибка preview, незавершённая загрузка картинки или устаревшая версия шаблона |

Не отключайте авторизацию, TLS или проверку сертификатов ради устранения этих ошибок: новые шаблоны используют существующие соединения приложения.

## Что уже проверено в локальной реализации

Успешны сборки API/Worker/frontend, 37 проверок рендеринга, 9 проверок безопасного возврата после входа, CRUD/preview, проверка устаревшей версии и интеграционная отправка в MailHog. В полученном MIME проверены исходные байты картинки и соответствующий CID; исходный шаблон не меняется после отправки локальной копии.

Эта проверка не означает автоматическую совместимость с неизвестными изменениями закрытой ветки. В существующих зависимостях также остаются ранее отмеченные NuGet-предупреждения об уязвимостях — этот перенос не является их обновлением.

---

## Приложение: перечень файлов и полные диффы

Ниже зафиксированы все 33 файла итоговой реализации. Миграцию/Designer/snapshot и solution применяйте с учётом выбранного выше варианта, а не одновременно с командами, генерирующими те же изменения.

## Перечень файлов
| Файл | Действие |
| --- | --- |
| `src/MailGate.Core/Entities/EmailTemplate.cs` | Создать |
| `src/MailGate.Core/Models/EmbeddedEmailImage.cs` | Создать |
| `src/MailGate.Contracts/Templates/EmailTemplateDto.cs` | Создать |
| `src/MailGate.Api/Templates/EmailTemplateRenderer.cs` | Создать |
| `src/MailGate.Api/Templates/NotificationComposer.cs` | Создать |
| `src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs` | Создать |
| `src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs` | Создать |
| `src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs` | Создать |
| `src/MailGate.Frontend/src/hooks/useDebounced.ts` | Создать |
| `src/MailGate.Frontend/src/components/EmailPreview.tsx` | Создать |
| `src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx` | Создать |
| `src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx` | Создать |
| `src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx` | Создать |
| `src/MailGate.Frontend/src/auth/returnPath.ts` | Создать |
| `config/email-templates.md` | Создать |
| `tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj` | Создать |
| `tests/MailGate.TemplateChecks/Program.cs` | Создать |
| `src/MailGate.Contracts/Notifications/NotificationLogDto.cs` | Объединить по диффу |
| `src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs` | Объединить по диффу |
| `src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs` | Объединить по диффу |
| `src/MailGate.Api/Program.cs` | Объединить по диффу |
| `src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs` | Объединить по диффу |
| `src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs` | Объединить по диффу |
| `src/MailGate.Channels.Smtp/SmtpChannel.cs` | Объединить по диффу |
| `src/MailGate.Frontend/src/App.tsx` | Объединить по диффу |
| `src/MailGate.Frontend/src/layout/AppLayout.tsx` | Объединить по диффу |
| `src/MailGate.Frontend/src/api/endpoints.ts` | Объединить по диффу |
| `src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx` | Объединить по диффу |
| `src/MailGate.Frontend/src/api/types.ts` | Объединить по диффу |
| `src/MailGate.Frontend/src/auth/oidc.ts` | Объединить по диффу |
| `src/MailGate.Frontend/src/auth/RequireAuth.tsx` | Объединить по диффу |
| `src/MailGate.slnx` | Объединить по диффу |
| `src/MailGate.sln` | Объединить по диффу |

## `src/MailGate.Core/Entities/EmailTemplate.cs`

```diff
diff --git a/src/MailGate.Core/Entities/EmailTemplate.cs b/src/MailGate.Core/Entities/EmailTemplate.cs
new file mode 100644
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
```

## `src/MailGate.Core/Models/EmbeddedEmailImage.cs`

```diff
diff --git a/src/MailGate.Core/Models/EmbeddedEmailImage.cs b/src/MailGate.Core/Models/EmbeddedEmailImage.cs
new file mode 100644
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
```

## `src/MailGate.Contracts/Templates/EmailTemplateDto.cs`

```diff
diff --git a/src/MailGate.Contracts/Templates/EmailTemplateDto.cs b/src/MailGate.Contracts/Templates/EmailTemplateDto.cs
new file mode 100644
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
```

## `src/MailGate.Api/Templates/EmailTemplateRenderer.cs`

```diff
diff --git a/src/MailGate.Api/Templates/EmailTemplateRenderer.cs b/src/MailGate.Api/Templates/EmailTemplateRenderer.cs
new file mode 100644
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
```

## `src/MailGate.Api/Templates/NotificationComposer.cs`

```diff
diff --git a/src/MailGate.Api/Templates/NotificationComposer.cs b/src/MailGate.Api/Templates/NotificationComposer.cs
new file mode 100644
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
```

## `src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs`

```diff
diff --git a/src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/EmailTemplatesEndpoints.cs
new file mode 100644
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
```

## `src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs`

```diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.cs
new file mode 100644
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
```

## `src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs`

```diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs b/src/MailGate.Infrastructure/Persistence/Migrations/20261028090000_AddEmailTemplates.Designer.cs
new file mode 100644
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
```

## `src/MailGate.Frontend/src/hooks/useDebounced.ts`

```diff
diff --git a/src/MailGate.Frontend/src/hooks/useDebounced.ts b/src/MailGate.Frontend/src/hooks/useDebounced.ts
new file mode 100644
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
```

## `src/MailGate.Frontend/src/components/EmailPreview.tsx`

```diff
diff --git a/src/MailGate.Frontend/src/components/EmailPreview.tsx b/src/MailGate.Frontend/src/components/EmailPreview.tsx
new file mode 100644
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
```

## `src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx`

```diff
diff --git a/src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx b/src/MailGate.Frontend/src/components/EmailTemplateEditor.tsx
new file mode 100644
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
```

## `src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx`

```diff
diff --git a/src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx b/src/MailGate.Frontend/src/pages/EmailTemplatesPage.tsx
new file mode 100644
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
```

## `src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx`

```diff
diff --git a/src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx b/src/MailGate.Frontend/src/pages/EmailTemplateEditPage.tsx
new file mode 100644
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
```

## `src/MailGate.Frontend/src/auth/returnPath.ts`

```diff
diff --git a/src/MailGate.Frontend/src/auth/returnPath.ts b/src/MailGate.Frontend/src/auth/returnPath.ts
new file mode 100644
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
```

## `config/email-templates.md`

```diff
diff --git a/config/email-templates.md b/config/email-templates.md
new file mode 100644
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
```

## `tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj`

```diff
diff --git a/tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj b/tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj
new file mode 100644
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
```

## `tests/MailGate.TemplateChecks/Program.cs`

```diff
diff --git a/tests/MailGate.TemplateChecks/Program.cs b/tests/MailGate.TemplateChecks/Program.cs
new file mode 100644
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
```

## `src/MailGate.Contracts/Notifications/NotificationLogDto.cs`

```diff
diff --git a/src/MailGate.Contracts/Notifications/NotificationLogDto.cs b/src/MailGate.Contracts/Notifications/NotificationLogDto.cs
--- a/src/MailGate.Contracts/Notifications/NotificationLogDto.cs
+++ b/src/MailGate.Contracts/Notifications/NotificationLogDto.cs
@@ -35,6 +35,9 @@
 
 public sealed class SendNotificationFromAdminDto
 {
+    public MailGate.Contracts.Templates.EmailTemplateDefinition? TemplateDefinition { get; set; }
+    public Guid? TemplateId { get; set; }
+    public Guid? TemplateRevision { get; set; }
     public ChannelType Channel { get; set; } = ChannelType.Smtp;
     public IReadOnlyList<Guid> RecipientIds { get; set; } = [];
     public IReadOnlyList<Guid> RecipientGroupIds { get; set; } = [];
```

## `src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs`

```diff
diff --git a/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs b/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs
--- a/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs
+++ b/src/MailGate.Api/Admin/Endpoints/NotificationsAdminEndpoints.cs
@@ -1,4 +1,6 @@
 using System.Text.Json;
+using MailGate.Api.Templates;
+using MailGate.Contracts.Templates;
 using FluentValidation;
 using MailGate.Contracts.Common;
 using MailGate.Contracts.Notifications;
@@ -28,6 +30,7 @@
         group.MapGet("/", ListAsync).WithName("ListAdminNotifications");
         group.MapGet("/stats", StatsAsync).WithName("AdminNotificationStats");
         group.MapGet("/{id:guid}", GetAsync).WithName("GetAdminNotification");
+        group.MapPost("/preview", PreviewAsync);
         group.MapPost("/", SendAsync).WithName("AdminSendNotification");
         group.MapPost("/{id:guid}/retry", RetryAsync).WithName("AdminRetryNotification");
 
@@ -136,6 +139,12 @@
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
@@ -147,6 +156,20 @@
         var validation = await validator.ValidateAsync(dto, cancellationToken);
         if (!validation.IsValid)
             return TypedResults.ValidationProblem(validation.ToDictionary());
+
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
 
         var emailsFromBook = dto.RecipientIds.Count == 0
             ? []
@@ -181,9 +204,9 @@
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
```

## `src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs`

```diff
diff --git a/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs b/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs
--- a/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs
+++ b/src/MailGate.Api/Admin/Validators/SendNotificationFromAdminValidator.cs
@@ -9,6 +9,10 @@
     {
         RuleFor(x => x.Body).NotEmpty().MaximumLength(1024 * 1024);
         RuleFor(x => x.Subject).MaximumLength(500);
+        RuleFor(x => x.Subject).Must(x => x is null || (!x.Contains('\r') && !x.Contains('\n')))
+            .WithMessage("Тема не должна содержать переводы строки.");
+        RuleFor(x => x.TemplateRevision).NotNull().When(x => x.TemplateId.HasValue);
+        RuleFor(x => x.Channel).Equal(MailGate.Core.Enums.ChannelType.Smtp).When(x => x.TemplateId.HasValue);
 
         RuleFor(x => x)
             .Must(x => x.RecipientIds.Count > 0 || x.RecipientGroupIds.Count > 0 || x.AdHocEmails.Count > 0)
```

## `src/MailGate.Api/Program.cs`

```diff
diff --git a/src/MailGate.Api/Program.cs b/src/MailGate.Api/Program.cs
--- a/src/MailGate.Api/Program.cs
+++ b/src/MailGate.Api/Program.cs
@@ -227,6 +227,7 @@
 app.MapSystemsAdminEndpoints();
 app.MapNotificationsAdminEndpoints();
 app.MapSmtpAdminEndpoints();
+app.MapEmailTemplatesEndpoints();
 
 app.MapHealthChecks("/healthz");
 app.MapHealthChecks("/healthz/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
```

## `src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs`

```diff
diff --git a/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs b/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs
--- a/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs
+++ b/src/MailGate.Infrastructure/Persistence/MailGateDbContext.cs
@@ -10,6 +10,7 @@
     public DbSet<Recipient> Recipients => Set<Recipient>();
     public DbSet<RecipientGroup> RecipientGroups => Set<RecipientGroup>();
     public DbSet<NotificationSystem> Systems => Set<NotificationSystem>();
+    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
     public DbSet<SmtpConfiguration> SmtpConfigurations => Set<SmtpConfiguration>();
 
     protected override void OnModelCreating(ModelBuilder modelBuilder)
@@ -104,6 +105,17 @@
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
```

## `src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs`

```diff
diff --git a/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs b/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
--- a/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
+++ b/src/MailGate.Infrastructure/Persistence/Migrations/MailGateDbContextModelSnapshot.cs
@@ -22,6 +22,37 @@
 
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
```

## `src/MailGate.Channels.Smtp/SmtpChannel.cs`

```diff
diff --git a/src/MailGate.Channels.Smtp/SmtpChannel.cs b/src/MailGate.Channels.Smtp/SmtpChannel.cs
--- a/src/MailGate.Channels.Smtp/SmtpChannel.cs
+++ b/src/MailGate.Channels.Smtp/SmtpChannel.cs
@@ -8,6 +8,7 @@
 using Microsoft.Extensions.Logging;
 using MimeKit;
 using MimeKit.Text;
+using System.Text.RegularExpressions;
 
 namespace MailGate.Channels.Smtp;
 
@@ -93,7 +94,27 @@
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
```

## `src/MailGate.Frontend/src/App.tsx`

```diff
diff --git a/src/MailGate.Frontend/src/App.tsx b/src/MailGate.Frontend/src/App.tsx
--- a/src/MailGate.Frontend/src/App.tsx
+++ b/src/MailGate.Frontend/src/App.tsx
@@ -16,6 +16,8 @@
 import { RecipientGroupEditPage } from './pages/RecipientGroupEditPage';
 import { SmtpSettingsPage } from './pages/SmtpSettingsPage';
 import { DebugClaimsPage } from './pages/DebugClaimsPage';
+import { EmailTemplatesPage } from './pages/EmailTemplatesPage';
+import { EmailTemplateEditPage } from './pages/EmailTemplateEditPage';
 import { NotFoundPage } from './pages/NotFoundPage';
 
 dayjs.locale('ru');
@@ -52,6 +54,9 @@
               <Route path="/recipient-groups" element={<RecipientGroupsPage />} />
               <Route path="/recipient-groups/new" element={<RecipientGroupEditPage />} />
               <Route path="/recipient-groups/:id/edit" element={<RecipientGroupEditPage />} />
+              <Route path="/templates" element={<EmailTemplatesPage />} />
+              <Route path="/templates/new" element={<EmailTemplateEditPage />} />
+              <Route path="/templates/:id/edit" element={<EmailTemplateEditPage />} />
               <Route path="/smtp" element={<SmtpSettingsPage />} />
               <Route path="/debug/claims" element={<DebugClaimsPage />} />
               <Route path="/not-found" element={<NotFoundPage />} />
```

## `src/MailGate.Frontend/src/layout/AppLayout.tsx`

```diff
diff --git a/src/MailGate.Frontend/src/layout/AppLayout.tsx b/src/MailGate.Frontend/src/layout/AppLayout.tsx
--- a/src/MailGate.Frontend/src/layout/AppLayout.tsx
+++ b/src/MailGate.Frontend/src/layout/AppLayout.tsx
@@ -29,6 +29,7 @@
     children: [
       { key: '/notifications', icon: <UnorderedListOutlined />, label: 'Журнал' },
       { key: '/notifications/new', icon: <SendOutlined />, label: 'Новая рассылка' },
+      { key: '/templates', icon: <MailOutlined />, label: 'Шаблоны писем' },
     ],
   },
   { key: '/recipients', icon: <ContactsOutlined />, label: 'Получатели' },
@@ -52,6 +53,7 @@
 
   const selectedKey = useMemo(() => {
     const path = location.pathname;
+    if (path.startsWith('/templates')) return '/templates';
     if (path.startsWith('/recipient-groups')) return '/recipient-groups';
     if (path === '/notifications/new') return '/notifications/new';
     if (path.startsWith('/notifications')) return '/notifications';
```

## `src/MailGate.Frontend/src/api/endpoints.ts`

```diff
diff --git a/src/MailGate.Frontend/src/api/endpoints.ts b/src/MailGate.Frontend/src/api/endpoints.ts
--- a/src/MailGate.Frontend/src/api/endpoints.ts
+++ b/src/MailGate.Frontend/src/api/endpoints.ts
@@ -1,5 +1,6 @@
 import { api, toQuery } from './client';
 import type {
+  EmailTemplateDto, EmailTemplateSummaryDto, SaveEmailTemplateDto, EmailPreviewDto, PreviewNotificationDto, EmailTemplateDefinition,
   AcceptedNotificationDto,
   CreateRecipientDto,
   CreateRecipientGroupDto,
@@ -29,6 +30,7 @@
 }
 
 export const notificationsApi = {
+  preview: (dto: PreviewNotificationDto) => api.post<EmailPreviewDto>('/api/admin/notifications/preview', dto),
   list: (params: PageParams & { status?: DeliveryStatus }) =>
     api.get<PagedResult<NotificationLogDto>>(`/api/admin/notifications${toQuery(params)}`),
   stats: () => api.get<NotificationStatsDto>('/api/admin/notifications/stats'),
@@ -79,3 +81,12 @@
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
```

## `src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx`

```diff
diff --git a/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx b/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx
--- a/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx
+++ b/src/MailGate.Frontend/src/pages/NotificationCreatePage.tsx
@@ -1,11 +1,17 @@
-import { App, Button, Card, Form, Input, Radio, Select, Space, Typography } from 'antd';
+import { useState, type Dispatch, type SetStateAction } from 'react';
+import { Link } from 'react-router-dom';
+import { EmailTemplateEditor } from '../components/EmailTemplateEditor';
+import { EmailPreview } from '../components/EmailPreview';
+import { useDebounced } from '../hooks/useDebounced';
+import { App, Alert, Col, Row, Button, Card, Form, Input, Radio, Select, Space, Typography } from 'antd';
 import { SendOutlined } from '@ant-design/icons';
 import { useMutation, useQuery } from '@tanstack/react-query';
 import { useNavigate } from 'react-router-dom';
-import { notificationsApi, recipientGroupsApi, recipientsApi } from '../api/endpoints';
-import type { BodyFormat, NotificationPriority, SendNotificationFromAdminDto } from '../api/types';
+import { emailTemplatesApi, notificationsApi, recipientGroupsApi, recipientsApi } from '../api/endpoints';
+import type { BodyFormat, EmailTemplateDefinition, NotificationPriority, SendNotificationFromAdminDto } from '../api/types';
 
 interface FormValues {
+  templateId?: string;
   recipientIds: string[];
   recipientGroupIds: string[];
   adHocEmails: string[];
@@ -22,6 +28,28 @@
   const navigate = useNavigate();
   const [form] = Form.useForm<FormValues>();
 
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
   const recipientsQuery = useQuery({
     queryKey: ['recipients', 'all'],
     queryFn: () => recipientsApi.list({ pageSize: 500 }),
@@ -57,21 +85,31 @@
       return;
     }
 
+    if (values.templateId && !template.data) { message.error('Дождитесь загрузки шаблона'); return; }
+    if (values.templateId && (preview.isError || preview.isFetching || previewRequest !== debouncedPreview)) {
+      message.error('Дождитесь корректного предпросмотра письма'); return;
+    }
+    if (uploadingImage) { message.error("Дождитесь загрузки изображения"); return; }
     sendMutation.mutate({
+      templateId: values.templateId ?? null,
+      templateRevision: values.templateId ? template.data?.revision : null,
+      templateDefinition: values.templateId ? definition : null,
       channel: 'Smtp',
       recipientIds: values.recipientIds ?? [],
       recipientGroupIds: values.recipientGroupIds ?? [],
       adHocEmails: (values.adHocEmails ?? []).map((e) => e.trim()),
       subject: values.subject?.trim() || null,
       body: values.body,
-      bodyFormat: values.bodyFormat,
+      bodyFormat: values.templateId ? 'Html' : values.bodyFormat,
       priority: values.priority,
     });
   };
 
   return (
-    <div style={{ maxWidth: 800 }}>
+    <div style={{ maxWidth: 1600 }}>
       <Typography.Title level={3}>Новая рассылка</Typography.Title>
+      <Row gutter={[24, 24]}>
+      <Col xs={24} xl={12}>
       <Card>
         <Form<FormValues>
           form={form}
@@ -115,8 +153,20 @@
             <Select mode="tags" allowClear placeholder="user@example.com" tokenSeparators={[',', ';', ' ']} open={false} suffixIcon={null} />
           </Form.Item>
 
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
           <Form.Item name="subject" label="Тема">
-            <Input placeholder="Тема письма" maxLength={500} />
+            <Input placeholder={templateId ? definition?.subject || 'Тема из шаблона' : 'Тема письма'} maxLength={500} />
           </Form.Item>
 
           <Form.Item
@@ -128,8 +178,9 @@
           </Form.Item>
 
           <Space size="large" wrap>
-            <Form.Item name="bodyFormat" label="Формат">
+            {!templateId && <Form.Item name="bodyFormat" label="Формат">
               <Radio.Group
+                disabled={!!templateId}
                 options={[
                   { value: 'PlainText', label: 'Текст' },
                   { value: 'Html', label: 'HTML' },
@@ -137,7 +188,7 @@
                 ]}
                 optionType="button"
               />
-            </Form.Item>
+            </Form.Item>}
 
             <Form.Item name="priority" label="Приоритет">
               <Select
@@ -159,6 +210,7 @@
                 htmlType="submit"
                 icon={<SendOutlined />}
                 loading={sendMutation.isPending}
+                disabled={uploadingImage || (!!templateId && (!template.data || preview.isError || preview.isFetching || previewRequest !== debouncedPreview))}
               >
                 Отправить
               </Button>
@@ -167,6 +219,9 @@
           </Form.Item>
         </Form>
       </Card>
+      </Col>
+      <Col xs={24} xl={12}><div style={{ position: 'sticky', top: 20 }}><EmailPreview preview={preview.data} loading={preview.isFetching || previewRequest !== debouncedPreview || (!!templateId && template.isLoading)} error={preview.error} /></div></Col>
+      </Row>
     </div>
   );
 }
```

## `src/MailGate.Frontend/src/api/types.ts`

```diff
diff --git a/src/MailGate.Frontend/src/api/types.ts b/src/MailGate.Frontend/src/api/types.ts
--- a/src/MailGate.Frontend/src/api/types.ts
+++ b/src/MailGate.Frontend/src/api/types.ts
@@ -36,6 +36,9 @@
 }
 
 export interface SendNotificationFromAdminDto {
+  templateId?: string | null;
+  templateRevision?: string | null;
+  templateDefinition?: EmailTemplateDefinition | null;
   channel: ChannelType;
   recipientIds: string[];
   recipientGroupIds: string[];
@@ -180,3 +183,33 @@
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
```

## `src/MailGate.Frontend/src/auth/oidc.ts`

```diff
diff --git a/src/MailGate.Frontend/src/auth/oidc.ts b/src/MailGate.Frontend/src/auth/oidc.ts
--- a/src/MailGate.Frontend/src/auth/oidc.ts
+++ b/src/MailGate.Frontend/src/auth/oidc.ts
@@ -1,6 +1,7 @@
 import { User, WebStorageStateStore } from 'oidc-client-ts';
 import type { AuthProviderProps } from 'react-oidc-context';
 import { getConfig } from '../config';
+import { getSigninReturnPath } from './returnPath';
 
 export function buildOidcConfig(): AuthProviderProps {
   const { keycloak } = getConfig();
@@ -12,9 +13,10 @@
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
```

## `src/MailGate.Frontend/src/auth/RequireAuth.tsx`

```diff
diff --git a/src/MailGate.Frontend/src/auth/RequireAuth.tsx b/src/MailGate.Frontend/src/auth/RequireAuth.tsx
--- a/src/MailGate.Frontend/src/auth/RequireAuth.tsx
+++ b/src/MailGate.Frontend/src/auth/RequireAuth.tsx
@@ -16,7 +16,7 @@
 
   useEffect(() => {
     if (!auth.isLoading && !auth.isAuthenticated && !auth.activeNavigator && !auth.error) {
-      void auth.signinRedirect({ state: window.location.pathname + window.location.search });
+      void auth.signinRedirect({ state: window.location.pathname + window.location.search + window.location.hash });
     }
   }, [auth]);
 
@@ -31,7 +31,7 @@
         title="Ошибка аутентификации"
         subTitle={auth.error.message}
         extra={
-          <Button type="primary" onClick={() => void auth.signinRedirect()}>
+          <Button type="primary" onClick={() => void auth.signinRedirect({ state: window.location.pathname + window.location.search + window.location.hash })}>
             Войти снова
           </Button>
         }
```

## `src/MailGate.slnx`

```diff
diff --git a/src/MailGate.slnx b/src/MailGate.slnx
--- a/src/MailGate.slnx
+++ b/src/MailGate.slnx
@@ -12,4 +12,5 @@
     <Project Path="MailGate.Api/MailGate.Api.csproj" />
     <Project Path="MailGate.Contracts/MailGate.Contracts.csproj" />
   </Folder>
+  <Project Path="../tests/MailGate.TemplateChecks/MailGate.TemplateChecks.csproj" />
 </Solution>
```

## `src/MailGate.sln`

```diff
diff --git a/src/MailGate.sln b/src/MailGate.sln
--- a/src/MailGate.sln
+++ b/src/MailGate.sln
@@ -16,6 +16,8 @@
 Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Api", "MailGate.Api\MailGate.Api.csproj", "{47D1AD8B-8B18-EB24-0AE3-64046B9DD9E0}"
 EndProject
 Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.Contracts", "MailGate.Contracts\MailGate.Contracts.csproj", "{51C9A635-404B-4952-B6E8-7D1330DD2343}"
+EndProject
+Project("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}") = "MailGate.TemplateChecks", "..\tests\MailGate.TemplateChecks\MailGate.TemplateChecks.csproj", "{A6BE0529-49EF-400C-88A7-B6FDCB2B54AF}"
 EndProject
 Global
 	GlobalSection(SolutionConfigurationPlatforms) = preSolution
@@ -99,6 +101,18 @@
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
 	EndGlobalSection
 	GlobalSection(SolutionProperties) = preSolution
 		HideSolutionNode = FALSE
```

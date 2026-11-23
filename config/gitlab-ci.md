# GitLab CI/CD: автоматический dev для MailGate

**Для вашего Kubernetes:** готовый deploy через SSH/Compose не применяется. По умолчанию job `deploy_dev` отключена; включается только при `MAILGATE_DEPLOY_TARGET=compose-ssh`. Для Kubernetes оставьте её выключенной и добавьте развёртывание через используемые у вас Helm/Argo CD/GitLab Agent. До согласования типа Runner даже test/build jobs могут требовать адаптации: они рассчитаны на Linux Shell Runner с Docker.

## Принятая схема и её границы

Готовый `.gitlab-ci.yml` рассчитан на **Linux Shell Runner с Docker** и **Linux dev-сервер с Docker Compose**, доступный Runner по SSH. Runner помечен тегом `mailgate`. Если у вас Docker executor, Kubernetes или Windows Runner, этот YAML нельзя просто запустить без адаптации. Фактические адреса, тип Runner и переменные инфраструктуры в этой среде неизвестны.

Pipeline запускается для ветки `develop`: проверяет шаблоны и SMTP credentials, собирает API/Worker/frontend и публикует три образа в GitLab Container Registry. Автоматическое обновление Compose-dev включается только отдельной переменной выше. Для `main` deploy не определён. Последний **успешно проверенный и собранный** pipeline может попасть на dev; запуск нового pipeline сам по себе ничего на сервере не меняет.

**Тег образа:** `<полный-SHA>-pipeline<CI_PIPELINE_ID>`. Это фиксированный набор образов конкретной сборки, без изменяемого `latest`. Нельзя вручную перезаписывать эти теги другими образами; при наличии настройки registry включите их immutability.

`resource_group` не допускает параллельные deploy jobs. На сервере дополнительно используются `flock` и номер pipeline: более старый pipeline не заменяет новый, который уже начал обновление. Задача deploy не прерывается автоматически новым коммитом. Если новая версия сломалась, pipeline становится красным; автоматического отката БД или контейнеров нет. [Сериализация deploy](https://docs.gitlab.com/ci/resource_groups/), [настройки безопасности развёртывания](https://docs.gitlab.com/ci/environments/deployment_safety/).

## Какие файлы добавлены

| Файл | Зачем |
| --- | --- |
| `.gitlab-ci.yml` | Ветка, стадии, jobs, артефакты и окружение development |
| `scripts/ci/check-templates.sh` | 37 проверок в SDK-контейнере |
| `scripts/ci/build-images.sh` | BuildKit-секреты, три build/push, формирование `images.env` |
| `scripts/ci/deploy-dev.sh` | Проверка переменных, SSH и передача только списка образов |
| `scripts/ci/deploy-dev-server.sh` | Блокировка, миграция, обновление и проверка доступности |

Также Worker получил режим `--migrate-only`: применяет миграции и выходит, не запуская обработчик RabbitMQ. API/Worker Dockerfile принимают необязательный секрет `nuget_config` для restore; без него используют прежний `src/NuGet.Config`. Существующий локальный Compose не заменён.

Локальный `src/Dockerfile.local` теперь передаёт аргументы приложению, чтобы тот же режим миграций работал и с локальным образом.

## Шаг 1. Уточнить у администраторов инфраструктуру

Попросите подтвердить:

1. Адрес GitLab, наличие проекта и включённого Container Registry.
2. Linux **Shell** Runner с тегом `mailgate`. На нём доступны Bash, Python 3, Docker Engine с BuildKit, `ssh`, `scp`. Пользователь Runner имеет право выполнять Docker.
3. CPU-архитектуры Runner и dev совпадают. Эта реализация не делает multiarch-сборку.
4. Runner имеет DNS/сетевой доступ к GitLab Registry, RED SOFT registry, используемым NuGet/Nexus, npm/Nexus и dev SSH.
5. Dev-сервер имеет Docker Engine, Compose v2 с поддержкой нескольких `--env-file`, Bash, `curl`, `flock` (обычно пакет `util-linux`). Он видит GitLab Registry и свою PostgreSQL/RabbitMQ/Keycloak.

Это отдельный доверенный Runner проекта/команды: доступ к Docker daemon даёт широкие права на хост. Не выдавайте deploy-секреты недоверенным fork/MR. [Варианты GitLab Runner для Docker-команд](https://docs.gitlab.com/ci/docker/using_docker_build/).

Если Runner ещё нет: Settings → CI/CD → Runners → New project runner. Задайте тег `mailgate`, при необходимости ограничьте protected-ветками. На подготовленном Linux-хосте администратор выполняет `gitlab-runner register`, указывает настоящий URL GitLab, выданный runner authentication token и executor **shell**. Установку GitLab Runner/Docker выполняют по правилам вашей ОС и контура.

Проверьте от имени пользователя Runner:

```bash
bash --version
docker version
docker buildx version
ssh -V
```

## Шаг 2. Подготовить директорию приложения на dev

Pipeline **не создаёт инфраструктуру с нуля**, не выполняет `down`, не удаляет volumes и не отправляет `.env` из GitLab. Он обновляет только `api`, `worker`, `frontend` поверх **существующего** серверного `docker-compose.yml`.

На dev должны быть:

- постоянная директория проекта с подходящим `docker-compose.yml`;
- `.env` с настоящими dev-настройками, читаемый deploy-пользователем, права желательно `600`;
- уже работающая инфраструктура: PostgreSQL и RabbitMQ, а также нужные серверные сети;
- доступные API и Worker общие ключи `/var/mailgate/keys`;
- корректные сертификаты, CORS и настройки корпоративного Keycloak;
- пользователь SSH с доступом к этой директории и Docker, без интерактивного sudo.

Для нового стенда сначала вручную разверните инфраструктуру по основной инструкции проекта и настройте `.env`. Внешние PostgreSQL/RabbitMQ подключайте через ваш серверный Compose. Не запускайте вторую БД с дефолтными паролями вместо существующей. Внешний Keycloak не заменяйте тестовым.

**Важно сохранить имя Compose-проекта:** оно определяет имена сети и volumes. На сервере с уже работающим MailGate получите его так:

```bash
docker inspect mailgate-api --format '{{ index .Config.Labels "com.docker.compose.project" }}'
```

Результат укажите в `DEV_COMPOSE_PROJECT`. `DEV_DEPLOY_PATH` — абсолютная директория **этого же** проекта на dev, не директория Runner и не путь на Windows. В этой версии скриптов путь должен быть без пробелов и содержать только буквы ASCII, цифры, `/`, `.`, `_`, `-`, без `..`.

Перед автоматизацией проверьте, что в серверном Compose уже есть все требуемые настройки новой версии. Pipeline сохраняет этот файл, поэтому старый Compose сам по себе не получит новых параметров key ring или группы Keycloak.

Скрипт использует один серверный docker-compose.yml. У сервисов должны быть поля image с переменными MAILGATE_API_IMAGE, MAILGATE_WORKER_IMAGE, MAILGATE_FRONTEND_IMAGE, как в актуальном общем Compose. Только эти значения подставляются из images.env. Свои корпоративные сети и runtime-настройки сохраните в серверном файле.

## Шаг 3. Дать dev-серверу право скачивать образы

В GitLab создайте Deploy Token с **read_registry** для проекта/группы. От имени **того же Linux-пользователя, под которым будет SSH deploy**, один раз выполните на dev:

```bash
read -rp 'Настоящий адрес GitLab Registry без https://: ' REGISTRY_HOST
read -rp 'Имя Deploy Token: ' REGISTRY_READER
read -rsp 'Значение Deploy Token: ' REGISTRY_SECRET
printf '\n'
printf '%s' "$REGISTRY_SECRET" | docker login "$REGISTRY_HOST" --username "$REGISTRY_READER" --password-stdin
unset REGISTRY_SECRET
```

Адрес registry возьмите из своего GitLab, не копируйте выдуманное имя сервера. Dev хранит доступ средствами Docker credential configuration/credential helper; применяйте корпоративный способ хранения. Токен должен оставаться действующим. Использовать пароль job из CI для постоянного доступа сервера не нужно: он краткоживущий.

В build job GitLab автоматически предоставляет `CI_REGISTRY`, `CI_REGISTRY_IMAGE`, `CI_REGISTRY_USER`, `CI_REGISTRY_PASSWORD`. Вводить их вручную в YAML не надо.

## Шаг 4. Подготовить SSH-ключ и known_hosts

Создайте отдельный ключ только для dev CI. Например, на Windows PowerShell:

```powershell
ssh-keygen -t ed25519 -f "$env:USERPROFILE\.ssh\mailgate-dev-ci" -C 'mailgate-dev-ci'
```

В этой реализации используется неинтерактивный ключ без passphrase. Администратор добавляет содержимое `mailgate-dev-ci.pub` в `~/.ssh/authorized_keys` deploy-пользователя на dev. Приватный файл не добавляйте в Git.

Получите SSH host key dev из доверенной сети и сверьте fingerprint с администратором. Содержимое known_hosts загрузите в GitLab как File-переменную. Для нестандартного порта должна быть запись `[имя-сервера]:порт`. Не отключайте `StrictHostKeyChecking`; pipeline не запускает `ssh-keyscan` для слепого доверия новому ключу. [Настройка SSH в GitLab CI](https://docs.gitlab.com/ci/jobs/ssh_keys/).

## Шаг 5. Заполнить GitLab CI/CD Variables

Settings → CI/CD → Variables → Add variable. Не вставляйте пароли в `.gitlab-ci.yml`. Переменные типа **File** содержат сам файл, а GitLab передаёт job путь к временному файлу.

| Имя | Тип | Значение / откуда взять |
| --- | --- | --- |
| `DEV_SSH_HOST` | Variable | Настоящее DNS-имя или IPv4 dev-сервера, без протокола и пути |
| `DEV_SSH_USER` | Variable | Linux-пользователь с доступом к Docker и директории проекта |
| `DEV_SSH_PORT` | Variable | SSH-порт; по умолчанию 22 |
| `DEV_DEPLOY_PATH` | Variable | Существующий абсолютный путь проекта на dev |
| `DEV_COMPOSE_PROJECT` | Variable | Имя существующего Compose-проекта из inspect |
| `SSH_PRIVATE_KEY` | **File** | Приватный SSH-ключ с переводом строки в конце |
| `SSH_KNOWN_HOSTS` | **File** | Проверенные SSH host keys dev |
| `DEV_API_HEALTH_URL` | Variable | URL проверки API, доступный **с dev-сервера**; по умолчанию `http://127.0.0.1:8080/healthz/ready` |
| `DEV_FRONTEND_HEALTH_URL` | Variable | URL frontend **с dev-сервера**; по умолчанию `http://127.0.0.1:8091/` |
| `NUGET_CONFIG_FILE` | **File**, необязательно | Полный действующий NuGet.Config с корпоративными sources/credentials для сборки |
| `NPMRC_FILE` | **File**, необязательно | Действующий `.npmrc` с настоящим registry; можно использовать `${NPM_TOKEN}` |
| `NPM_TOKEN` | Variable, masked, необязательно | Настоящий npm/Nexus token, если выбран этот способ авторизации |
| `DOTNET_SDK_IMAGE` | Variable, необязательно | Доступный Runner .NET 10 SDK image для проверок; default — официальный SDK из YAML |

Если Nexus требует Basic Authentication, используйте согласованный корпоративный npmrc, а не подставляйте обычный пароль в `_authToken`. Docker BuildKit получает файл как секрет, а не как COPY/ARG. Для NuGet секрет передаётся аналогично. Эти файлы не надо коммитить. Значения runtime-паролей PostgreSQL/SMTP не нужны Runner: они остаются в конфигурации dev.

Для deploy-переменных можно установить Environment scope `development`; для npm/NuGet-переменных build job оставьте scope `*`, так как build не является environment job. Установите Protected там, где это требуется; тогда **develop тоже должна быть protected**, иначе GitLab не выдаст эти переменные. SSH-ключ с переносами строк обычно нельзя сделать Masked: используйте File + Protected, не печатайте содержимое. Для однострочных токенов включите Masked/Hidden, если версия GitLab поддерживает.

На корпоративных УЦ trust должен быть настроен отдельно на Runner/Docker daemon, в build-контейнерах и dev. Сертификаты из `crt/` устанавливаются API/Worker Dockerfile, но не настраивают доверие Runner к registry и не исправляют DNS/VPN. Если frontend должен доверять корпоративному Nexus, его Node build-stage также должен иметь подходящий CA по вашей существующей конфигурации. Не отключайте TLS-проверку.

## Шаг 6. Проверить YAML и запустить первый pipeline

1. В GitLab откройте Build → Pipeline editor (в старых версиях CI/CD → Editor).
2. Выберите ветку `develop`, откройте `.gitlab-ci.yml`, выполните Validate/CI Lint. Локальная проверка YAML не заменяет проверку вашей версией GitLab.
3. Settings → CI/CD → General pipelines: включите **Prevent outdated deployment jobs**, отключите **Allow job retries for rollback deployments**, если не планируете такой путь отката.
4. Убедитесь, что Runner online и его tag совпадает с `mailgate` в `default.tags`.
5. После проверки переменных и dev-конфигурации отправьте изменения в GitLab `develop`:

```bash
git add .gitlab-ci.yml scripts/ci tests/ci config/gitlab-ci.md
git add src/MailGate.Worker/Program.cs src/MailGate.Api/Dockerfile src/MailGate.Worker/Dockerfile .gitignore .dockerignore README.md
git add src/Dockerfile.local
git commit -m "Add GitLab CI builds and automatic dev deployment"
git push origin develop
```

Проверьте, что **origin указывает именно на ваш GitLab**, а не на GitHub из локального репозитория: `git remote -v`. Если GitLab имеет другой remote, используйте его имя в push. Вносить изменения remote автоматически эта настройка не пытается.

Если файлы уже попали в develop, используйте Build → Pipelines → New pipeline → develop. Pipeline запускается автоматически; отдельной кнопки подтверждения deploy здесь нет. Не делайте первый push до заполнения секретов и проверки, что dev — нужный сервер.

## Как читать jobs

1. `template_checks` запускает реальный renderer и валидацию шаблонов в SDK-контейнере. БД/SMTP он не трогает. Параллельно `pipeline_script_checks` проверяет Bash и сценарии deploy/build с заглушками, без обращения к реальному серверу.
2. `build_images` компилирует API и Worker внутри корпоративных Dockerfile, выполняет npm ci + TypeScript/Vite build во frontend Dockerfile. Если любая сборка падает, deploy не запускается. `images.env` содержит **только три адреса образов**, не пароли.
3. `deploy_dev` скачивает artifact, по SSH передаёт images.env в `.mailgate-ci/releases/<pipeline-id>` на dev. Он не копирует серверную `.env` и не делает git pull на сервере.
4. Dev скачивает точные образы, запускает одноразовый Worker с `--migrate-only`. При ошибке миграции контейнеры приложения не обновляются. Неудачная миграция всё же может изменить БД до своей ошибки — перед изменениями схемы нужны backup и review SQL.
5. После миграции обновляются только три сервиса через `up -d --no-deps --no-build`. PostgreSQL, RabbitMQ и volumes не удаляются.
6. Проверяются HTTP API/frontend и состояние трёх контейнеров. На сервере сохраняются `.mailgate-ci/latest-successful` и `deployed-commit`. В GitLab появляется environment `development` с номером deployment.

Это последовательное обновление Compose, **не zero-downtime и не транзакционный deploy**. Краткий перерыв возможен. Изменения схемы должны быть совместимы со старым приложением, которое ещё работает во время миграции. Любые изменения списка сервисов, сетей и runtime-настроек в серверном Compose выполняются отдельно, осознанно.

## Как самостоятельно писать такой YAML

Минимальный каркас уже виден в файле:

```yaml
stages:
  - test
  - build
  - deploy

some_job:
  stage: test
  script:
    - echo "Команда внутри job"
```

- Отступы — пробелы, не табуляция. `-` обозначает элемент списка.
- `workflow.rules` решает, создавать ли pipeline для ветки. Здесь это только develop.
- `stages` задаёт порядок: следующая стадия не начинается после ошибки предыдущей.
- Имя `template_checks`/`build_images`/`deploy_dev` — это job, который виден в интерфейсе.
- `script` — команды на Runner. Большие программы вынесены в Bash-файлы: их проще тестировать и читать, чем экранировать в YAML.
- `variables` — несекретные значения по умолчанию; настоящие секреты вводятся в Settings.
- `artifacts` передаёт images.env из build в deploy; `reports.dotenv` также делает его строки переменными job.
- `needs` объявляет, какой build artifact нужен deploy.
- `environment` связывает deploy с dev в GitLab, `resource_group` сериализует такие обновления.
- `interruptible: true` позволяет отменить устаревшую проверку/сборку новым коммитом. Для deploy стоит false, чтобы не оборвать миграцию или смену контейнеров.

Меняйте одну часть за раз и сначала используйте CI Lint. Для изменения ветки отредактируйте `workflow.rules`, для типа Runner — архитектуру jobs, а не только tag. [Справочник GitLab YAML](https://docs.gitlab.com/ci/yaml/).

## Проверка и ошибки

| Симптом | Причина / действие |
| --- | --- |
| Pipeline не создаётся | Ветка не develop, файл не попал в GitLab, pipeline creation запрещён настройками |
| Pending/Stuck | Нет online Shell Runner с тегом mailgate или protected-настройки не совпадают |
| Нет обязательной переменной | Не заполнена Variables; Protected переменная на unprotected develop; неправильный environment scope |
| Cannot connect to Docker | Docker не запущен или пользователь Runner не имеет прав; это не ошибка YAML |
| Registry 401 | Не включён Registry или недостаточно прав job/deploy token; dev залогинен не тем Linux-пользователем |
| npm/NuGet 401 | NPMRC_FILE/NUGET_CONFIG_FILE не настроены или содержат неверные корпоративные credentials |
| ENOTFOUND | Настоящий DNS/сеть/прокси недоступны с Runner или build-контейнера |
| SSH host key verification failed | Несовпадение known_hosts; проверьте сервер у администраторов, не отключайте проверку |
| Пустая новая БД/ключи | Неверное DEV_COMPOSE_PROJECT/DEV_DEPLOY_PATH: используются другие volumes. Исправьте настройки, не удаляйте старые volumes |
| Health check failed | Проверьте порты, логи сервисов, Keycloak/DB/RabbitMQ. GitLab не выполняет автоматический откат |
| Старый pipeline пропущен | Защита от перезаписи нового развёртывания; это ожидаемо |

Диагностика на dev из постоянной директории проекта, под правильным пользователем:

```bash
cat .mailgate-ci/latest-successful
cat .mailgate-ci/deployed-commit
docker ps --filter name=mailgate
docker logs --tail 100 mailgate-api
docker logs --tail 100 mailgate-worker
```

Не удаляйте `.mailgate-ci/latest-attempt` ради retry старого pipeline: это обходит защиту. Для отката приложения запустите **новый pipeline на нужном состоянии исходников** (например, новым revert-коммитом в develop); совместимость с текущей схемой БД проверьте отдельно. Образы и release manifests должны сохраняться в registry/server на требуемый срок. Артефакт GitLab хранится 7 дней; для более позднего развёртывания создайте новый pipeline.

## Что можно проверить без доступа к вашему GitLab

Локально проверяются синтаксис Bash/YAML, единого Compose, проверки renderer, сборка кода Worker и сценарии deploy с заглушками Docker/SSH. Здесь нет доступа к вашей GitLab-инсталляции, Runner и dev-секретам, поэтому фактический CI Lint, push образов в корпоративный Registry и реальный deploy проверяются первым pipeline после настройки. Никакие команды развёртывания на ваш dev при подготовке файлов не выполнялись.

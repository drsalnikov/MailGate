# Keycloak: доступ к MailGate по группе

MailGate использует существующий realm и существующую группу пользователей. Менять роли или состав группы не требуется. Можно настроить только новый клиент.

## Клиент фронтенда

Создайте OpenID Connect клиент: Client authentication — Off, Standard flow — On, PKCE — S256. Для локального Docker укажите redirect URI `http://localhost:8091/*`, post logout redirect URI `http://localhost:8091/*` и Web origins `http://localhost:8091`. Для сервера используйте фактический origin фронтенда.

В Client scopes → dedicated scope → Mappers добавьте:

- **Group Membership**: Name — `groups`, Token Claim Name — `groups`, Full group path — On, Add to access token — On. Остальные переключатели можно оставить Off при использовании обычных access tokens.
- **Audience**: Name — `mailgate-api-audience`, Included Custom Audience — `mailgate-api`, Add to access token — On, Add to ID token — Off. Если клиент API уже существует, вместо Custom Audience можно выбрать его в Included Client Audience.

Group Membership передаёт группы вошедшего пользователя. Название разрешённой группы задаётся в MailGate, а не в mapper.

## Настройка MailGate

В корневом `.env` задайте:

```dotenv
KEYCLOAK_AUTHORITY=https://keycloak.example.ru/realms/your-realm
KEYCLOAK_CLIENT_ID=mailgate-ui
KEYCLOAK_AUDIENCE=mailgate-api
KEYCLOAK_ADMIN_GROUP=/OtherApp/Administrators
KEYCLOAK_REQUIRE_HTTPS_METADATA=true
```

Замените значения на свои. `KEYCLOAK_ADMIN_GROUP` должен точно совпадать с элементом массива `groups` в access token, включая регистр и полный путь. Доступ закрыт при пустом значении. Подгруппа с другим полным путём автоматически не разрешается.

API проверяет подпись, issuer, audience, срок действия JWT и разрешённую группу. Фронтенд проверяет группу для отображения интерфейса; защита API действует независимо от фронтенда.

Пересоберите контейнеры:

```bash
docker compose up -d --build api frontend
```

Выйдите и войдите заново. Проверьте участника разрешённой группы и пользователя без неё: второй должен получить отказ во фронтенде и 403 от защищённых API-эндпоинтов. Диагностика токена в MailGate показывает группы и claims.

Для запуска без Docker настройка API называется `Authentication:Keycloak:AdminGroup` (переменная окружения `Authentication__Keycloak__AdminGroup`). В `src/MailGate.Frontend/public/config.json` укажите `keycloak.adminGroup`, `keycloak.clientId` и `keycloak.authority`.

`mailgate-realm.json` — прежний пример realm для локального стенда с ролями. Он не настраивает новый доступ по группе автоматически. Для существующего realm импортировать его не нужно.

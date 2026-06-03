# Keycloak для MailGate

Документ описывает настройки Keycloak для защищённой среды. Локальный файл `mailgate-realm.json` можно использовать как dev-пример, но в production/stage лучше создать realm и clients вручную или через IaC, без dev-пользователей и тестовых паролей.

## Целевая схема

MailGate использует Keycloak как OpenID Connect provider:

- **Admin UI**: Blazor WebAssembly, Authorization Code Flow + PKCE.
- **MailGate API**: JWT Bearer validation.
- **Внешние системы**: client credentials flow через отдельный confidential client.
- **Решение о доступе**: роль `mailgate-admin`.

MailGate не проверяет пароль пользователя и не работает напрямую с Kerberos/LDAP. Он доверяет только OIDC access token от Keycloak.

## Realm

Создайте realm:

- **Realm name**: `mailgate`
- **Enabled**: `On`
- **SSL required**:
  - `External requests` или `All requests` для защищённой среды с HTTPS.
  - `None` допустимо только для локального dev-стека.

Публичный issuer должен быть стабильным и совпадать с настройкой API:

```text
https://<keycloak-host>/realms/mailgate
```

Например:

```text
https://sso.company.local/realms/mailgate
```

## Realm Role

Создайте realm role:

- **Role name**: `mailgate-admin`
- **Description**: `Allows access to MailGate Admin UI and admin API.`

Эта роль нужна:

- пользователям, которые заходят в Admin UI;
- service account внешней системы, если ей нужен доступ к `/api/admin/*`;
- группам AD/LDAP, если роли выдаются через federation/group mapping.

## Client: MailGate API

Клиент API нужен как audience для access token.

Создайте client:

- **Client ID**: `mailgate-api`
- **Client type**: `OpenID Connect`
- **Access type / Capability**: bearer-only/resource server, если доступно в вашей версии Keycloak.
- **Standard flow**: `Off`
- **Direct access grants**: `Off`
- **Service accounts**: `Off`

Если в вашей версии Keycloak нет режима `bearer-only`, создайте обычный confidential client без интерактивных flows и используйте его только как audience.

## Client: Admin UI

Клиент для Blazor WebAssembly.

Создайте client:

- **Client ID**: `mailgate-admin`
- **Client type**: `OpenID Connect`
- **Client authentication**: `Off`  
  Это public client, secret в браузере хранить нельзя.
- **Standard flow**: `On`
- **Implicit flow**: `Off`
- **Direct access grants**: `Off` для защищённой среды  
  В dev можно включать только для ручной проверки.
- **Service accounts**: `Off`
- **PKCE**: `S256`

Настройте URLs под реальный адрес MailGate Admin UI:

```text
Root URL:              https://<mailgate-host>
Home URL:              https://<mailgate-host>/
Valid redirect URIs:   https://<mailgate-host>/authentication/login-callback
                       https://<mailgate-host>/swagger/oauth2-redirect.html
Valid post logout URIs:https://<mailgate-host>/authentication/logout-callback
Web origins:           https://<mailgate-host>
```

Для локального Docker-стека это:

```text
http://localhost:8080/authentication/login-callback
http://localhost:8080/swagger/oauth2-redirect.html
http://localhost:8080/authentication/logout-callback
http://localhost:8080
```

## Mappers для Admin UI

Для client `mailgate-admin` добавьте protocol mappers.

### Audience mapper

Нужен, чтобы access token для UI принимался MailGate API.

- **Mapper type**: Audience
- **Name**: `mailgate-api-audience`
- **Included Client Audience**: `mailgate-api`
- **Add to access token**: `On`
- **Add to ID token**: `Off`

После этого access token должен содержать:

```json
{
  "aud": ["mailgate-api"]
}
```

Допустим и строковый вариант `"aud": "mailgate-api"` в зависимости от версии Keycloak/настроек.

### Realm roles mapper

Нужен, чтобы роли попадали в токены.

Рекомендуемый mapper:

- **Mapper type**: User Realm Role
- **Name**: `realm roles`
- **Multivalued**: `On`
- **Token Claim Name**: `realm_access.roles`
- **Claim JSON Type**: `String`
- **Add to access token**: `On`
- **Add to ID token**: `On` желательно для UI

Дополнительно можно добавить top-level mapper для UI:

- **Mapper type**: User Realm Role
- **Name**: `realm roles (top-level)`
- **Multivalued**: `On`
- **Token Claim Name**: `roles`
- **Claim JSON Type**: `String`
- **Add to access token**: `On`
- **Add to ID token**: `On`

MailGate API умеет читать роли из:

- `realm_access.roles`
- `resource_access.<client>.roles`
- `roles`

Admin UI также умеет подмешивать роли из access token, но корректнее всё равно отдавать роли в ID token или top-level `roles`.

## Пользователи и группы

Назначайте `mailgate-admin` только администраторам MailGate.

Варианты:

- вручную на пользователя: `Users` → пользователь → `Role mapping` → `Assign role` → `mailgate-admin`;
- через группу Keycloak: создайте группу, назначьте ей role `mailgate-admin`, добавляйте пользователей в группу;
- через LDAP/AD federation: маппинг AD-группы в Keycloak group/realm role.

Для защищённой среды не создавайте dev-пользователей:

- `mailgate-admin` / `mailgate`
- `mailgate-user` / `mailgate`

Они нужны только для локального Docker-стека.

## Client: внешняя интеграция

Для сторонних систем создайте отдельный confidential client, например `mailgate-integration`.

Настройки:

- **Client ID**: `mailgate-integration`
- **Client authentication**: `On`
- **Standard flow**: `Off`
- **Direct access grants**: `Off`
- **Service accounts roles**: `On`
- **Web origins**: URL MailGate/Swagger UI, например `https://<mailgate-host>`  
  Это нужно, если Swagger UI сам получает токен через browser request к Keycloak.

После создания:

1. Откройте вкладку **Credentials** и сохраните `Client secret` в защищённом хранилище внешней системы.
2. Откройте **Service accounts roles**.
3. Нажмите **Assign role**.
4. Выберите фильтр **Filter by realm roles**.
5. Назначьте роль `mailgate-admin`, если интеграции нужен доступ к `/api/admin/*`.

Добавьте mapper audience для `mailgate-integration`:

- **Mapper type**: Audience
- **Included Client Audience**: `mailgate-api`
- **Add to access token**: `On`

Получение токена:

```bash
curl -X POST "https://<keycloak-host>/realms/mailgate/protocol/openid-connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials" \
  -d "client_id=mailgate-integration" \
  -d "client_secret=<client-secret>"
```

Вызов API:

```bash
curl -H "Authorization: Bearer <access_token>" \
  https://<mailgate-host>/api/admin/notifications/stats
```

## Настройки MailGate API

В `MailGate.Api` должны совпадать issuer, audience и роль:

```json
{
  "Authentication": {
    "Keycloak": {
      "Authority": "https://<keycloak-host>/realms/mailgate",
      "Audience": "mailgate-api",
      "ClientId": "mailgate-admin",
      "AdminRole": "mailgate-admin",
      "RequireHttpsMetadata": true
    }
  }
}
```

Для Docker/env:

```bash
Authentication__Keycloak__Authority=https://<keycloak-host>/realms/mailgate
Authentication__Keycloak__Audience=mailgate-api
Authentication__Keycloak__ClientId=mailgate-admin
Authentication__Keycloak__AdminRole=mailgate-admin
Authentication__Keycloak__RequireHttpsMetadata=true
```

`Authority` должен совпадать с `iss` в токене. Если Keycloak стоит за reverse proxy, настройте hostname/proxy headers так, чтобы discovery document и `iss` возвращали внешний HTTPS URL.

## Настройки Admin UI

Blazor WASM читает публичные OIDC-настройки из `wwwroot/appsettings.json`:

```json
{
  "Oidc": {
    "Authority": "https://<keycloak-host>/realms/mailgate",
    "ClientId": "mailgate-admin",
    "ResponseType": "code",
    "DefaultScopes": [
      "openid"
    ]
  }
}
```

Не храните client secret в Admin UI. Клиент `mailgate-admin` должен быть public.

## Reverse Proxy / HTTPS

Для защищённой среды рекомендуется:

- Keycloak доступен только по HTTPS.
- MailGate Admin/API доступны только по HTTPS.
- В Keycloak настроен корректный hostname, например:

```bash
KC_HOSTNAME=https://sso.company.local
KC_PROXY_HEADERS=xforwarded
KC_HTTP_ENABLED=true
```

Конкретные параметры зависят от способа деплоя Keycloak и reverse proxy. Важно, чтобы:

- `/.well-known/openid-configuration` возвращал внешний HTTPS issuer;
- `issuer` совпадал с `Authentication:Keycloak:Authority`;
- redirect URI у `mailgate-admin` совпадал с внешним URL MailGate.

## Kerberos / AD / LDAP

Kerberos/AD подключается на стороне Keycloak, не в MailGate.

Типовой вариант:

1. Настроить User Federation к LDAP/AD.
2. Настроить Kerberos/SPNEGO flow в Keycloak, если нужен бесшовный вход.
3. Смаппить AD-группу администраторов MailGate в realm role `mailgate-admin`.
4. Проверить, что после входа пользователь получает `mailgate-admin` в токене.

MailGate продолжает проверять только OIDC/JWT.

## Проверка токена

Проверьте payload `access_token`. Для admin API нужны:

```json
{
  "iss": "https://<keycloak-host>/realms/mailgate",
  "aud": "mailgate-api",
  "realm_access": {
    "roles": ["mailgate-admin"]
  }
}
```

Возможен вариант:

```json
{
  "aud": ["account", "mailgate-api"],
  "roles": ["mailgate-admin"]
}
```

Проверка API:

```bash
curl -i -H "Authorization: Bearer <access_token>" \
  https://<mailgate-host>/api/admin/notifications/stats
```

Ожидаемые статусы:

- `200` — токен валиден и роль есть.
- `401` — токена нет, токен невалиден, неверный issuer/audience, API не может получить metadata/JWKS.
- `403` — токен валиден, но нет роли `mailgate-admin`.

## Swagger

Swagger UI:

```text
https://<mailgate-host>/swagger
```

1. Нажмите **Authorize**.
2. Выберите **Keycloak OAuth2**.
3. Swagger перенаправит в Keycloak.
4. Войдите пользователем с ролью `mailgate-admin`.
5. После возврата Swagger получит `access_token` через Authorization Code Flow + PKCE.
6. Выполняйте защищённые `/api/admin/*` endpoints.

Схема **Bearer** оставлена как ручной режим: туда вставляется уже полученный `access_token`, а не `client_secret`.

## Troubleshooting

### 401 от API

Проверьте:

- `Authorization: Bearer <token>` действительно отправляется.
- `iss` в токене совпадает с `Authentication:Keycloak:Authority`.
- `aud` содержит `mailgate-api`.
- `RequireHttpsMetadata=true` используется только с HTTPS Keycloak.
- API имеет сетевой доступ к discovery endpoint:

```text
https://<keycloak-host>/realms/mailgate/.well-known/openid-configuration
```

### 403 от API

Токен принят, но не хватает роли. Проверьте:

- пользователь или service account имеет realm role `mailgate-admin`;
- role mapper добавляет роли в access token;
- роль называется точно `mailgate-admin`.

### Admin UI вошёл, но пишет «нет доступа»

Проверьте страницу диагностики:

```text
https://<mailgate-host>/debug/claims
```

Должно быть:

```text
RoleClaimType: roles
IsInRole("mailgate-admin"): True
```

Если роли нет, проверьте mappers `realm_access.roles` / `roles` и назначение роли пользователю.

### Keycloak требует HTTPS в dev

В защищённой среде это правильно. Для локального dev можно временно:

```bash
kcadm.sh update realms/mailgate -s sslRequired=NONE
```

Не используйте это в production.

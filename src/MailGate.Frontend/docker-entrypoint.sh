#!/bin/sh
set -e

# Генерируем runtime-конфигурацию из переменных окружения
cat > /usr/share/nginx/html/config.json <<EOF
{
  "apiBaseUrl": "${API_BASE_URL:-http://localhost:8080}",
  "keycloak": {
    "authority": "${KEYCLOAK_AUTHORITY:-http://keycloak.localhost:8180/realms/mailgate}",
    "clientId": "${KEYCLOAK_CLIENT_ID:-mailgate-admin}",
    "adminGroup": "${KEYCLOAK_ADMIN_GROUP:-}"
  }
}
EOF

exec nginx -g 'daemon off;'

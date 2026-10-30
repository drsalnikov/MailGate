FROM node:22-alpine AS build
WORKDIR /app

COPY package.json package-lock.json ./
RUN --mount=type=secret,id=npmrc,target=/root/.npmrc \
    --mount=type=secret,id=npm_token,env=NPM_TOKEN \
    npm ci

COPY . .
RUN npm run build

FROM nginx:1.27-alpine AS final
RUN apk upgrade --no-cache libxml2
COPY nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/dist /usr/share/nginx/html
COPY docker-entrypoint.sh /docker-entrypoint-custom.sh
RUN chmod +x /docker-entrypoint-custom.sh

EXPOSE 80
ENTRYPOINT ["/docker-entrypoint-custom.sh"]

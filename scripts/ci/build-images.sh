#!/usr/bin/env bash
set -euo pipefail
: "${CI_REGISTRY:?Enable GitLab Container Registry}"
: "${CI_REGISTRY_IMAGE:?Missing GitLab image namespace}"
: "${CI_REGISTRY_USER:?Missing GitLab registry user}"
: "${CI_REGISTRY_PASSWORD:?Missing GitLab registry password}"
: "${CI_COMMIT_SHA:?Missing commit SHA}"
: "${CI_PIPELINE_ID:?Missing pipeline ID}"

export DOCKER_CONFIG
DOCKER_CONFIG=$(mktemp -d)
trap 'cleanup_status=$?; rm -rf -- "$DOCKER_CONFIG" || true; exit "$cleanup_status"' EXIT
printf '%s' "$CI_REGISTRY_PASSWORD" | docker login "$CI_REGISTRY" --username "$CI_REGISTRY_USER" --password-stdin

tag="${CI_COMMIT_SHA}-pipeline${CI_PIPELINE_ID}"
backend_args=(--label "org.opencontainers.image.revision=$CI_COMMIT_SHA")
if [[ -n ${NUGET_CONFIG_FILE:-} ]]; then
  test -f "$NUGET_CONFIG_FILE"
  backend_args+=(--secret "id=nuget_config,src=$NUGET_CONFIG_FILE")
fi
npm_args=(--label "org.opencontainers.image.revision=$CI_COMMIT_SHA")
if [[ -n ${NPMRC_FILE:-} ]]; then
  test -f "$NPMRC_FILE"
  npm_args+=(--secret "id=npmrc,src=$NPMRC_FILE")
fi
if [[ -n ${NPM_TOKEN:-} ]]; then
  npm_args+=(--secret id=npm_token,env=NPM_TOKEN)
fi

docker build "${backend_args[@]}" \
  --file src/MailGate.Api/Dockerfile --tag "$CI_REGISTRY_IMAGE/api:$tag" .
docker build "${backend_args[@]}" \
  --file src/MailGate.Worker/Dockerfile --tag "$CI_REGISTRY_IMAGE/worker:$tag" .
# npm ci and TypeScript/Vite compilation happen inside this Dockerfile.
docker build "${npm_args[@]}" \
  --file src/MailGate.Frontend/Dockerfile --tag "$CI_REGISTRY_IMAGE/frontend:$tag" src/MailGate.Frontend

# Publish only after all three builds succeed. Deployment is a separate job.
for component in api worker frontend; do
  docker push "$CI_REGISTRY_IMAGE/$component:$tag"
done
printf 'MAILGATE_API_IMAGE=%s/api:%s\nMAILGATE_WORKER_IMAGE=%s/worker:%s\nMAILGATE_FRONTEND_IMAGE=%s/frontend:%s\n' \
  "$CI_REGISTRY_IMAGE" "$tag" "$CI_REGISTRY_IMAGE" "$tag" "$CI_REGISTRY_IMAGE" "$tag" > images.env

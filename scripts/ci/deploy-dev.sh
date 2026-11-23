#!/usr/bin/env bash
set -euo pipefail
: "${DEV_SSH_HOST:?Set real dev server hostname or IPv4 address}"
: "${DEV_SSH_USER:?Set dev SSH user}"
: "${DEV_DEPLOY_PATH:?Set existing absolute server project directory}"
: "${DEV_COMPOSE_PROJECT:?Set existing Compose project name}"
: "${SSH_PRIVATE_KEY:?Set a GitLab File variable}"
: "${SSH_KNOWN_HOSTS:?Set a GitLab File variable}"
: "${CI_PIPELINE_ID:?Missing pipeline ID}"
: "${CI_PROJECT_ID:?Missing project ID}"
: "${CI_COMMIT_SHA:?Missing commit SHA}"
: "${DEV_API_HEALTH_URL:?Set server-side API health URL}"
: "${DEV_FRONTEND_HEALTH_URL:?Set server-side frontend URL}"
port=${DEV_SSH_PORT:-22}

# Values are also used in remote shell commands. Accept simple, explicit paths.
[[ $DEV_SSH_HOST =~ ^[a-zA-Z0-9][a-zA-Z0-9.-]*$ ]]
[[ $DEV_SSH_USER =~ ^[a-zA-Z_][a-zA-Z0-9_-]*$ ]]
[[ $port =~ ^[0-9]+$ ]]
[[ $DEV_DEPLOY_PATH =~ ^/[a-zA-Z0-9_./-]+$ && $DEV_DEPLOY_PATH != / && $DEV_DEPLOY_PATH != *..* ]]
[[ $DEV_COMPOSE_PROJECT =~ ^[a-z0-9][a-z0-9_-]*$ ]]
[[ $CI_PIPELINE_ID =~ ^[0-9]+$ && $CI_PROJECT_ID =~ ^[0-9]+$ && $CI_COMMIT_SHA =~ ^[0-9a-f]{40}$ ]]
test -f images.env
test -f "$SSH_PRIVATE_KEY"
test -f "$SSH_KNOWN_HOSTS"

ssh_dir=$(mktemp -d)
trap 'cleanup_status=$?; rm -rf -- "$ssh_dir" || true; exit "$cleanup_status"' EXIT
cp -- "$SSH_PRIVATE_KEY" "$ssh_dir/key"
cp -- "$SSH_KNOWN_HOSTS" "$ssh_dir/known_hosts"
chmod 600 "$ssh_dir/key" "$ssh_dir/known_hosts"
ssh_options=(-i "$ssh_dir/key" -o BatchMode=yes -o IdentitiesOnly=yes \
  -o StrictHostKeyChecking=yes -o "UserKnownHostsFile=$ssh_dir/known_hosts" -o ConnectTimeout=15)
destination="$DEV_SSH_USER@$DEV_SSH_HOST"
release="$DEV_DEPLOY_PATH/.mailgate-ci/releases/$CI_PIPELINE_ID"
ssh "${ssh_options[@]}" -p "$port" "$destination" "mkdir -p -- '$release'"
scp "${ssh_options[@]}" -P "$port" images.env "$destination:$release/images.env"

# Quote URLs as shell arguments rather than interpolating them as shell code.
printf -v remote_command 'bash -s -- %q %q %q %q %q %q %q' \
  "$DEV_DEPLOY_PATH" "$CI_PIPELINE_ID" "$DEV_COMPOSE_PROJECT" "$CI_COMMIT_SHA" "$CI_PROJECT_ID" \
  "$DEV_API_HEALTH_URL" "$DEV_FRONTEND_HEALTH_URL"
ssh "${ssh_options[@]}" -p "$port" "$destination" "$remote_command" < scripts/ci/deploy-dev-server.sh

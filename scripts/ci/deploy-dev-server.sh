#!/usr/bin/env bash
set -euo pipefail
umask 077
root=${1:?}; pipeline=${2:?}; project=${3:?}; commit=${4:?}; gitlab_project=${5:?}
api_health=${6:?}; frontend_health=${7:?}
[[ $pipeline =~ ^[0-9]+$ && $gitlab_project =~ ^[0-9]+$ ]]
cd "$root"
test -f docker-compose.yml
test -f .env
command -v flock >/dev/null
command -v curl >/dev/null
docker compose version

state="$root/.mailgate-ci"
release="$state/releases/$pipeline"
mkdir -p "$state"
exec 9>"$state/deploy.lock"
flock -x 9

if [[ -f $state/gitlab-project && $(cat "$state/gitlab-project") != "$gitlab_project" ]]; then
  echo 'This deployment directory belongs to another GitLab project.' >&2
  exit 1
fi
printf '%s\n' "$gitlab_project" > "$state/gitlab-project"
previous=0
if [[ -f $state/latest-attempt ]]; then previous=$(cat "$state/latest-attempt"); fi
[[ $previous =~ ^[0-9]+$ ]]
if (( pipeline < previous )); then
  echo "Skip outdated pipeline $pipeline; pipeline $previous has already started deployment."
  exit 0
fi

compose=(docker compose --project-name "$project" --project-directory "$root" \
  --env-file "$root/.env" --env-file "$release/images.env" \
  -f "$root/docker-compose.yml")
# Validate without printing resolved environment values/passwords.
"${compose[@]}" config --quiet
"${compose[@]}" pull api worker frontend
printf '%s\n' "$pipeline" > "$state/latest-attempt"

# Applies migrations and exits before starting MassTransit or consuming mail.
# Fail before replacing the running app if the migration fails.
"${compose[@]}" run --rm --no-deps worker --migrate-only
"${compose[@]}" up -d --no-deps --no-build api worker frontend

wait_http() {
  local url=$1
  for (( attempt=1; attempt<=30; attempt++ )); do
    if curl --fail --silent --show-error --connect-timeout 3 --max-time 8 --output /dev/null "$url" 2>/dev/null; then
      return 0
    fi
    sleep 3
  done
  echo "Health check failed: $url" >&2
  return 1
}
wait_http "$api_health"
wait_http "$frontend_health"
for component in api worker frontend; do
  container=$("${compose[@]}" ps -q "$component")
  test -n "$container"
  test "$(docker inspect --format '{{.State.Running}}' "$container")" = true
done
printf '%s\n' "$pipeline" > "$state/latest-successful"
printf '%s\n' "$commit" > "$state/deployed-commit"
echo "Deployed pipeline $pipeline, commit $commit."

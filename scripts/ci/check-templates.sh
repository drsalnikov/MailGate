#!/usr/bin/env bash
set -euo pipefail
: "${CI_PROJECT_DIR:?Run from GitLab CI}"
: "${DOTNET_SDK_IMAGE:?Set DOTNET_SDK_IMAGE}"

# A shell executor is intentional: the Docker daemon sees CI_PROJECT_DIR.
docker run --rm \
  --env DOTNET_CLI_HOME=/tmp/dotnet-home \
  --env DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 \
  --env DOTNET_CLI_TELEMETRY_OPTOUT=1 \
  --volume "$CI_PROJECT_DIR:/source:ro" \
  "$DOTNET_SDK_IMAGE" \
  sh -ec '
    mkdir -p /tmp/mailgate-checks
    tar -C /source --exclude=bin --exclude=obj -cf - \
      tests/MailGate.TemplateChecks src/MailGate.Api/Templates \
      src/MailGate.Contracts/Templates src/MailGate.Core/Enums src/MailGate.Core/Models \
      | tar -C /tmp/mailgate-checks -xf -
    cd /tmp/mailgate-checks
    dotnet run --project tests/MailGate.TemplateChecks -p:NuGetAudit=false
  '

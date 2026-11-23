#!/usr/bin/env bash
set -euo pipefail
: "${CI_PROJECT_DIR:?Run from GitLab CI}"
: "${DOTNET_SDK_IMAGE:?Set DOTNET_SDK_IMAGE}"
docker run --rm --env DOTNET_CLI_TELEMETRY_OPTOUT=1 \
  --volume "$CI_PROJECT_DIR:/source:ro" "$DOTNET_SDK_IMAGE" sh -ec '
    mkdir -p /tmp/mailgate-checks
    tar -C /source --exclude=bin --exclude=obj -cf - \
      tests/MailGate.SmtpCredentialChecks src/MailGate.Channels.Smtp/Options \
      src/MailGate.Channels.Smtp/Services src/MailGate.Core/Enums \
      | tar -C /tmp/mailgate-checks -xf -
    cd /tmp/mailgate-checks
    dotnet run --project tests/MailGate.SmtpCredentialChecks -p:NuGetAudit=false
  '

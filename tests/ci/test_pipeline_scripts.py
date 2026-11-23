"""Offline regression checks: no registry, SSH server or Docker daemon is used."""
import os
from pathlib import Path
import subprocess
import tempfile
import unittest

ROOT = Path(__file__).resolve().parents[2]


class PipelineScripts(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.base = Path(self.temp.name)
        self.bin = self.base / 'bin'
        self.bin.mkdir()
        self.log = self.base / 'commands.log'
        self.dev = self.base / 'dev'
        self.dev.mkdir()
        (self.dev / '.env').write_text('RUNTIME_SECRET=must-not-appear-in-logs\n')
        (self.dev / 'docker-compose.yml').write_text('services: {}\n')
        self.command('docker', '''printf '%s\\n' "$*" >> "$MOCK_LOG"
if [[ $1 == login ]]; then cat >/dev/null; fi
if [[ " $* " == *" run --rm --no-deps worker --migrate-only "* && ${FAIL_MIGRATION:-0} == 1 ]]; then exit 1; fi
if [[ $1 == build && ${FAIL_BUILD:-0} == 1 ]]; then exit 1; fi
if [[ " $* " == *" ps -q "* ]]; then printf 'container-%s\\n' "${!#}"; fi
if [[ $1 == inspect ]]; then echo true; fi
''')
        self.command('curl', 'exit "${FAIL_HEALTH:-0}"\n')
        self.command('sleep', 'exit 0\n')
        # The test verifies deployment decisions, not OS locking implementation.
        self.command('flock', 'exit 0\n')
        self.env = dict(os.environ, PATH=str(self.bin) + os.pathsep + os.environ['PATH'], MOCK_LOG=str(self.log))

    def command(self, name, body):
        p = self.bin / name
        p.write_text('#!/usr/bin/env bash\nset -eu\n' + body)
        p.chmod(0o755)

    def deploy(self, pipeline=20, **extra):
        release = self.dev / '.mailgate-ci' / 'releases' / str(pipeline)
        release.mkdir(parents=True, exist_ok=True)
        (release / 'images.env').write_text('MAILGATE_API_IMAGE=registry/api:test\n')
        return subprocess.run(['bash', str(ROOT / 'scripts/ci/deploy-dev-server.sh'), str(self.dev),
                               str(pipeline), 'mailgate', 'a' * 40, '123',
                               'http://127.0.0.1:8080/healthz/ready', 'http://127.0.0.1:8091/'],
                              env=dict(self.env, **extra), text=True, capture_output=True)

    def commands(self):
        return self.log.read_text() if self.log.exists() else ''

    def test_success_migrates_before_up_and_preserves_runtime_files(self):
        original = (self.dev / '.env').read_text()
        result = self.deploy()
        self.assertEqual(result.returncode, 0, result.stderr)
        commands = self.commands()
        self.assertLess(commands.index('run --rm --no-deps worker --migrate-only'), commands.index('up -d --no-deps --no-build api worker frontend'))
        self.assertNotIn(' down', commands)
        self.assertEqual((self.dev / '.env').read_text(), original)
        self.assertNotIn('must-not-appear', result.stdout + result.stderr)
        self.assertEqual((self.dev / '.mailgate-ci/latest-successful').read_text().strip(), '20')

    def test_older_pipeline_cannot_replace_newer_deployment(self):
        self.assertEqual(self.deploy(30).returncode, 0)
        self.log.write_text('')
        result = self.deploy(20)
        self.assertEqual(result.returncode, 0, result.stderr)
        self.assertIn('Skip outdated', result.stdout)
        self.assertNotIn(' pull ', self.commands())
        self.assertNotIn(' up ', self.commands())

    def test_failed_migration_does_not_replace_containers(self):
        result = self.deploy(20, FAIL_MIGRATION='1')
        self.assertNotEqual(result.returncode, 0)
        self.assertNotIn(' up ', self.commands())
        self.assertFalse((self.dev / '.mailgate-ci/latest-successful').exists())
        self.log.write_text('')
        self.assertEqual(self.deploy(19).returncode, 0)
        self.assertNotIn(' up ', self.commands())

    def test_retry_same_pipeline_is_allowed(self):
        self.assertNotEqual(self.deploy(20, FAIL_MIGRATION='1').returncode, 0)
        self.assertEqual(self.deploy(20).returncode, 0)

    def test_failed_health_does_not_mark_deployment_successful(self):
        result = self.deploy(20, FAIL_HEALTH='1')
        self.assertNotEqual(result.returncode, 0)
        self.assertIn('Health check failed', result.stderr)
        self.assertFalse((self.dev / '.mailgate-ci/latest-successful').exists())

    def test_rejects_shared_directory_from_another_gitlab_project(self):
        state = self.dev / '.mailgate-ci'
        state.mkdir()
        (state / 'gitlab-project').write_text('456\n')
        result = self.deploy()
        self.assertNotEqual(result.returncode, 0)
        self.assertNotIn(' pull ', self.commands())

    def build(self, **extra):
        return subprocess.run(['bash', str(ROOT / 'scripts/ci/build-images.sh')], cwd=self.base,
                              env=dict(self.env, CI_REGISTRY='registry', CI_REGISTRY_IMAGE='registry/project',
                                       CI_REGISTRY_USER='ci-user', CI_REGISTRY_PASSWORD='secret-token',
                                       CI_COMMIT_SHA='b' * 40, CI_PIPELINE_ID='42', **extra), text=True, capture_output=True)

    def test_build_uses_exact_pipeline_tag_and_no_secret_in_manifest(self):
        result = self.build()
        self.assertEqual(result.returncode, 0, result.stderr)
        manifest = (self.base / 'images.env').read_text()
        self.assertEqual(len(manifest.splitlines()), 3)
        self.assertEqual(manifest.count('b' * 40 + '-pipeline42'), 3)
        self.assertNotIn('secret-token', manifest + self.commands() + result.stdout + result.stderr)
        self.assertEqual(sum(line.startswith('push ') for line in self.commands().splitlines()), 3)

    def test_failed_build_does_not_push_or_create_manifest(self):
        result = self.build(FAIL_BUILD='1')
        self.assertNotEqual(result.returncode, 0)
        self.assertNotIn('push ', self.commands())
        self.assertFalse((self.base / 'images.env').exists())


if __name__ == '__main__':
    unittest.main()

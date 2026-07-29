import { Card, Descriptions, Tag, Typography } from 'antd';
import { useAuth } from 'react-oidc-context';
import { extractRoles, parseJwtPayload } from '../auth/oidc';

export function DebugClaimsPage() {
  const auth = useAuth();
  const token = auth.user?.access_token;
  const roles = extractRoles(token);

  let payload: Record<string, unknown> = {};
  if (token) {
    try {
      payload = parseJwtPayload(token);
    } catch {
      // Токен не разобрался — покажем пустой payload
    }
  }

  return (
    <div style={{ maxWidth: 900 }}>
      <Typography.Title level={3}>Диагностика токена</Typography.Title>
      <Card size="small" style={{ marginBottom: 16 }}>
        <Descriptions column={1} size="small">
          <Descriptions.Item label="Пользователь">
            {auth.user?.profile.preferred_username ?? '—'}
          </Descriptions.Item>
          <Descriptions.Item label="Роли">
            {roles.length === 0 ? '—' : roles.map((r) => <Tag key={r}>{r}</Tag>)}
          </Descriptions.Item>
        </Descriptions>
      </Card>
      <Card size="small" title="Claims access token">
        <pre style={{ whiteSpace: 'pre-wrap', wordBreak: 'break-word', margin: 0 }}>
          {JSON.stringify(payload, null, 2)}
        </pre>
      </Card>
    </div>
  );
}

import { useEffect, type ReactNode } from 'react';
import { useAuth } from 'react-oidc-context';
import { Button, Result, Spin } from 'antd';
import { hasAdminRole } from './oidc';

function CenteredSpin() {
  return (
    <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>
      <Spin size="large" />
    </div>
  );
}

export function RequireAuth({ children }: { children: ReactNode }) {
  const auth = useAuth();

  useEffect(() => {
    if (!auth.isLoading && !auth.isAuthenticated && !auth.activeNavigator && !auth.error) {
      void auth.signinRedirect({ state: window.location.pathname + window.location.search });
    }
  }, [auth]);

  if (auth.isLoading || auth.activeNavigator) {
    return <CenteredSpin />;
  }

  if (auth.error) {
    return (
      <Result
        status="error"
        title="Ошибка аутентификации"
        subTitle={auth.error.message}
        extra={
          <Button type="primary" onClick={() => void auth.signinRedirect()}>
            Войти снова
          </Button>
        }
      />
    );
  }

  if (!auth.isAuthenticated) {
    return <CenteredSpin />;
  }

  if (!hasAdminRole(auth.user?.access_token)) {
    return (
      <Result
        status="403"
        title="Нет доступа"
        subTitle={`Учётной записи ${auth.user?.profile.preferred_username ?? ''} не назначена роль администратора MailGate. Обратитесь к администратору Keycloak.`}
        extra={
          <Button onClick={() => void auth.signoutRedirect()}>Выйти</Button>
        }
      />
    );
  }

  return <>{children}</>;
}

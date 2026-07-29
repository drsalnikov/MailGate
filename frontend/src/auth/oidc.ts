import { User, WebStorageStateStore } from 'oidc-client-ts';
import type { AuthProviderProps } from 'react-oidc-context';
import { getConfig } from '../config';

export function buildOidcConfig(): AuthProviderProps {
  const { keycloak } = getConfig();
  return {
    authority: keycloak.authority,
    client_id: keycloak.clientId,
    redirect_uri: `${window.location.origin}/`,
    post_logout_redirect_uri: `${window.location.origin}/`,
    scope: 'openid',
    automaticSilentRenew: true,
    userStore: new WebStorageStateStore({ store: window.sessionStorage }),
    onSigninCallback: () => {
      // Убираем code/state из адресной строки после редиректа Keycloak
      window.history.replaceState({}, document.title, window.location.pathname);
    },
  };
}

export function getStoredUser(): User | null {
  const { keycloak } = getConfig();
  const key = `oidc.user:${keycloak.authority}:${keycloak.clientId}`;
  const raw = window.sessionStorage.getItem(key);
  return raw ? User.fromStorageString(raw) : null;
}

export function getAccessToken(): string | null {
  return getStoredUser()?.access_token ?? null;
}

interface AccessTokenPayload {
  realm_access?: { roles?: string[] };
  roles?: string[];
  [claim: string]: unknown;
}

export function parseJwtPayload(token: string): AccessTokenPayload {
  const base64 = token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/');
  return JSON.parse(decodeURIComponent(escape(atob(base64)))) as AccessTokenPayload;
}

export function extractRoles(accessToken: string | undefined): string[] {
  if (!accessToken) return [];
  try {
    const payload = parseJwtPayload(accessToken);
    return [...(payload.realm_access?.roles ?? []), ...(payload.roles ?? [])];
  } catch {
    return [];
  }
}

export function hasAdminRole(accessToken: string | undefined): boolean {
  const { keycloak } = getConfig();
  return extractRoles(accessToken).includes(keycloak.adminRole);
}

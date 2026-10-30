import { User, WebStorageStateStore } from 'oidc-client-ts';
import type { AuthProviderProps } from 'react-oidc-context';
import { getConfig } from '../config';
import { getSigninReturnPath } from './returnPath';

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
    onSigninCallback: (user) => {
      const path = getSigninReturnPath(user?.state, window.location.origin);
      window.history.replaceState({}, document.title, path);
      window.dispatchEvent(new PopStateEvent('popstate'));
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
  groups?: unknown;
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

export function extractGroups(accessToken: string | undefined): string[] {
  if (!accessToken) return [];
  try {
    const groups = parseJwtPayload(accessToken).groups;
    return Array.isArray(groups)
      ? groups.filter((group): group is string => typeof group === 'string')
      : [];
  } catch {
    return [];
  }
}

export function hasAdminGroup(accessToken: string | undefined): boolean {
  const { adminGroup } = getConfig().keycloak;
  return typeof adminGroup === 'string' && adminGroup.trim().length > 0
    && extractGroups(accessToken).includes(adminGroup);
}

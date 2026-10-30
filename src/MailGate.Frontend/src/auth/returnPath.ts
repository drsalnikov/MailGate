export function getSigninReturnPath(state: unknown, origin: string): string {
  if (typeof state !== 'string' || !state.startsWith('/') || state.startsWith('//')) return '/';
  try {
    const target = new URL(state, origin);
    return target.origin === origin ? target.pathname + target.search + target.hash : '/';
  } catch {
    return '/';
  }
}

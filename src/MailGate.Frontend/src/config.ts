export interface AppConfig {
  apiBaseUrl: string;
  keycloak: {
    authority: string;
    clientId: string;
    adminGroup: string;
  };
}

let config: AppConfig | null = null;

export async function loadConfig(): Promise<AppConfig> {
  const response = await fetch('/config.json', { cache: 'no-store' });
  if (!response.ok) {
    throw new Error(`Не удалось загрузить config.json: ${response.status}`);
  }
  config = (await response.json()) as AppConfig;
  return config;
}

export function getConfig(): AppConfig {
  if (!config) {
    throw new Error('Конфигурация ещё не загружена');
  }
  return config;
}

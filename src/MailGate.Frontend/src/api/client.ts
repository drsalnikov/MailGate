import { getConfig } from '../config';
import { getAccessToken } from '../auth/oidc';

export class ApiError extends Error {
  readonly status: number;
  readonly problem: ProblemDetails | null;

  constructor(status: number, message: string, problem: ProblemDetails | null = null) {
    super(message);
    this.status = status;
    this.problem = problem;
  }
}

export interface ProblemDetails {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
}

async function request<T>(method: string, path: string, body?: unknown): Promise<T> {
  const headers: Record<string, string> = {};
  const token = getAccessToken();
  if (token) headers['Authorization'] = `Bearer ${token}`;
  if (body !== undefined) headers['Content-Type'] = 'application/json';

  const response = await fetch(`${getConfig().apiBaseUrl}${path}`, {
    method,
    headers,
    body: body === undefined ? undefined : JSON.stringify(body),
  });

  if (!response.ok) {
    let problem: ProblemDetails | null = null;
    let message = `Ошибка запроса: HTTP ${response.status}`;
    try {
      const contentType = response.headers.get('content-type') ?? '';
      if (contentType.includes('json')) {
        const data = await response.json();
        if (typeof data === 'string') {
          message = data;
        } else {
          problem = data as ProblemDetails;
          if (problem.errors) {
            message = Object.values(problem.errors).flat().join('; ');
          } else if (problem.detail || problem.title) {
            message = problem.detail ?? problem.title ?? message;
          }
        }
      } else {
        const text = await response.text();
        if (text) message = text;
      }
    } catch {
      // Тело ответа не удалось разобрать — оставляем стандартное сообщение
    }
    throw new ApiError(response.status, message, problem);
  }

  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

export const api = {
  get: <T>(path: string) => request<T>('GET', path),
  post: <T>(path: string, body?: unknown) => request<T>('POST', path, body),
  put: <T>(path: string, body?: unknown) => request<T>('PUT', path, body),
  delete: <T = void>(path: string) => request<T>('DELETE', path),
};

export function toQuery(params: object): string {
  const search = new URLSearchParams();
  for (const [key, value] of Object.entries(params)) {
    if (value !== undefined && value !== null && value !== '') {
      search.set(key, String(value));
    }
  }
  const qs = search.toString();
  return qs ? `?${qs}` : '';
}

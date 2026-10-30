import React from 'react';
import ReactDOM from 'react-dom/client';
import { AuthProvider } from 'react-oidc-context';
import { loadConfig } from './config';
import { buildOidcConfig } from './auth/oidc';
import App from './App';

async function bootstrap() {
  await loadConfig();

  ReactDOM.createRoot(document.getElementById('root')!).render(
    <React.StrictMode>
      <AuthProvider {...buildOidcConfig()}>
        <App />
      </AuthProvider>
    </React.StrictMode>,
  );
}

void bootstrap();

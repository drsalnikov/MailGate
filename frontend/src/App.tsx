import { App as AntApp, ConfigProvider, theme as antdTheme } from 'antd';
import ruRU from 'antd/locale/ru_RU';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import dayjs from 'dayjs';
import 'dayjs/locale/ru';
import { ThemeModeProvider, useThemeMode } from './theme';
import { RequireAuth } from './auth/RequireAuth';
import { AppLayout } from './layout/AppLayout';
import { DashboardPage } from './pages/DashboardPage';
import { NotificationsPage } from './pages/NotificationsPage';
import { NotificationCreatePage } from './pages/NotificationCreatePage';
import { SystemsPage } from './pages/SystemsPage';
import { RecipientsPage } from './pages/RecipientsPage';
import { RecipientGroupsPage } from './pages/RecipientGroupsPage';
import { RecipientGroupEditPage } from './pages/RecipientGroupEditPage';
import { SmtpSettingsPage } from './pages/SmtpSettingsPage';
import { DebugClaimsPage } from './pages/DebugClaimsPage';
import { NotFoundPage } from './pages/NotFoundPage';

dayjs.locale('ru');

const queryClient = new QueryClient({
  defaultOptions: {
    queries: { retry: 1, refetchOnWindowFocus: false },
  },
});

function ThemedApp() {
  const { isDark } = useThemeMode();

  return (
    <ConfigProvider
      locale={ruRU}
      theme={{ algorithm: isDark ? antdTheme.darkAlgorithm : antdTheme.defaultAlgorithm }}
    >
      <AntApp>
        <BrowserRouter>
          <Routes>
            <Route
              element={
                <RequireAuth>
                  <AppLayout />
                </RequireAuth>
              }
            >
              <Route path="/" element={<DashboardPage />} />
              <Route path="/notifications" element={<NotificationsPage />} />
              <Route path="/notifications/new" element={<NotificationCreatePage />} />
              <Route path="/systems" element={<SystemsPage />} />
              <Route path="/recipients" element={<RecipientsPage />} />
              <Route path="/recipient-groups" element={<RecipientGroupsPage />} />
              <Route path="/recipient-groups/new" element={<RecipientGroupEditPage />} />
              <Route path="/recipient-groups/:id/edit" element={<RecipientGroupEditPage />} />
              <Route path="/smtp" element={<SmtpSettingsPage />} />
              <Route path="/debug/claims" element={<DebugClaimsPage />} />
              <Route path="/not-found" element={<NotFoundPage />} />
              <Route path="*" element={<Navigate to="/not-found" replace />} />
            </Route>
          </Routes>
        </BrowserRouter>
      </AntApp>
    </ConfigProvider>
  );
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <ThemeModeProvider>
        <ThemedApp />
      </ThemeModeProvider>
    </QueryClientProvider>
  );
}

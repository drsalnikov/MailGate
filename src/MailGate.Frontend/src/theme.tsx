import { createContext, useCallback, useContext, useMemo, useState, type ReactNode } from 'react';

const STORAGE_KEY = 'mailgate-theme';

interface ThemeContextValue {
  isDark: boolean;
  toggle: () => void;
}

const ThemeContext = createContext<ThemeContextValue>({ isDark: false, toggle: () => {} });

export function ThemeModeProvider({ children }: { children: ReactNode }) {
  const [isDark, setIsDark] = useState(() => localStorage.getItem(STORAGE_KEY) === 'dark');

  const toggle = useCallback(() => {
    setIsDark((prev) => {
      localStorage.setItem(STORAGE_KEY, prev ? 'light' : 'dark');
      return !prev;
    });
  }, []);

  const value = useMemo(() => ({ isDark, toggle }), [isDark, toggle]);
  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
}

export function useThemeMode() {
  return useContext(ThemeContext);
}

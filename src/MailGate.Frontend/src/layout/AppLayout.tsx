import { useMemo, useState } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import { Button, Dropdown, Layout, Menu, Space, Typography, theme } from 'antd';
import {
  ApartmentOutlined,
  ContactsOutlined,
  DashboardOutlined,
  LogoutOutlined,
  MailOutlined,
  MoonOutlined,
  SendOutlined,
  SettingOutlined,
  SunOutlined,
  TeamOutlined,
  UnorderedListOutlined,
  UserOutlined,
} from '@ant-design/icons';
import { useAuth } from 'react-oidc-context';
import { useThemeMode } from '../theme';

const { Header, Sider, Content } = Layout;

const menuItems = [
  { key: '/', icon: <DashboardOutlined />, label: 'Главная' },
  {
    key: 'mailing',
    icon: <MailOutlined />,
    label: 'Рассылка',
    children: [
      { key: '/notifications', icon: <UnorderedListOutlined />, label: 'Журнал' },
      { key: '/notifications/new', icon: <SendOutlined />, label: 'Новая рассылка' },
      { key: '/templates', icon: <MailOutlined />, label: 'Шаблоны писем' },
    ],
  },
  { key: '/recipients', icon: <ContactsOutlined />, label: 'Получатели' },
  { key: '/recipient-groups', icon: <TeamOutlined />, label: 'Группы' },
  { key: '/systems', icon: <ApartmentOutlined />, label: 'Системы' },
  {
    key: 'settings',
    icon: <SettingOutlined />,
    label: 'Настройки',
    children: [{ key: '/smtp', icon: <MailOutlined />, label: 'SMTP' }],
  },
];

export function AppLayout() {
  const [collapsed, setCollapsed] = useState(false);
  const navigate = useNavigate();
  const location = useLocation();
  const auth = useAuth();
  const { isDark, toggle } = useThemeMode();
  const { token } = theme.useToken();

  const selectedKey = useMemo(() => {
    const path = location.pathname;
    if (path.startsWith('/templates')) return '/templates';
    if (path.startsWith('/recipient-groups')) return '/recipient-groups';
    if (path === '/notifications/new') return '/notifications/new';
    if (path.startsWith('/notifications')) return '/notifications';
    return path;
  }, [location.pathname]);

  const username = auth.user?.profile.preferred_username ?? 'пользователь';

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider collapsible collapsed={collapsed} onCollapse={setCollapsed} theme={isDark ? 'dark' : 'light'}>
        <div
          style={{
            height: 48,
            margin: 8,
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'center',
            gap: 8,
            color: token.colorPrimary,
            fontWeight: 600,
            fontSize: collapsed ? 18 : 20,
          }}
        >
          <MailOutlined />
          {!collapsed && 'MailGate'}
        </div>
        <Menu
          theme={isDark ? 'dark' : 'light'}
          mode="inline"
          selectedKeys={[selectedKey]}
          defaultOpenKeys={['mailing', 'settings']}
          items={menuItems}
          onClick={({ key }) => navigate(key)}
        />
      </Sider>
      <Layout>
        <Header
          style={{
            background: token.colorBgContainer,
            padding: '0 24px',
            display: 'flex',
            justifyContent: 'space-between',
            alignItems: 'center',
          }}
        >
          <Typography.Title level={4} style={{ margin: 0 }}>
            Панель администратора
          </Typography.Title>
          <Space>
            <Button
              type="text"
              icon={isDark ? <SunOutlined /> : <MoonOutlined />}
              onClick={toggle}
              title={isDark ? 'Светлая тема' : 'Тёмная тема'}
            />
            <Dropdown
              menu={{
                items: [
                  {
                    key: 'claims',
                    icon: <UserOutlined />,
                    label: 'Диагностика токена',
                    onClick: () => navigate('/debug/claims'),
                  },
                  {
                    key: 'logout',
                    icon: <LogoutOutlined />,
                    label: 'Выйти',
                    onClick: () => void auth.signoutRedirect(),
                  },
                ],
              }}
            >
              <Button type="text" icon={<UserOutlined />}>
                {username}
              </Button>
            </Dropdown>
          </Space>
        </Header>
        <Content style={{ margin: 24 }}>
          <Outlet />
        </Content>
      </Layout>
    </Layout>
  );
}

import { Alert, Card, Col, Row, Skeleton, Statistic, Typography } from 'antd';
import {
  CheckCircleOutlined,
  CloseCircleOutlined,
  MinusCircleOutlined,
  TeamOutlined,
} from '@ant-design/icons';
import { useQuery } from '@tanstack/react-query';
import { notificationsApi } from '../api/endpoints';

export function DashboardPage() {
  const { data, isLoading, error } = useQuery({
    queryKey: ['notifications', 'stats'],
    queryFn: notificationsApi.stats,
    refetchInterval: 60_000,
  });

  if (error) {
    return <Alert type="error" message="Не удалось загрузить статистику" description={(error as Error).message} />;
  }

  const cards = [
    {
      title: 'Отправлено за 24 ч',
      value: data?.sentLast24h,
      icon: <CheckCircleOutlined />,
      color: '#52c41a',
    },
    {
      title: 'Ошибок за 24 ч',
      value: data?.failedLast24h,
      icon: <CloseCircleOutlined />,
      color: '#ff4d4f',
    },
    {
      title: 'Пропущено за 24 ч',
      value: data?.skippedLast24h,
      icon: <MinusCircleOutlined />,
      color: '#faad14',
    },
    {
      title: 'Получателей в базе',
      value: data?.totalRecipients,
      icon: <TeamOutlined />,
      color: '#1677ff',
    },
  ];

  return (
    <div>
      <Typography.Title level={3}>Обзор</Typography.Title>
      <Row gutter={[16, 16]}>
        {cards.map((card) => (
          <Col xs={24} sm={12} lg={6} key={card.title}>
            <Card>
              {isLoading ? (
                <Skeleton active paragraph={{ rows: 1 }} />
              ) : (
                <Statistic
                  title={card.title}
                  value={card.value ?? 0}
                  prefix={<span style={{ color: card.color }}>{card.icon}</span>}
                />
              )}
            </Card>
          </Col>
        ))}
      </Row>
    </div>
  );
}

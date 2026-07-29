import { useState } from 'react';
import {
  App,
  Button,
  Descriptions,
  Drawer,
  Flex,
  Input,
  Select,
  Space,
  Table,
  Tag,
  Tooltip,
  Typography,
} from 'antd';
import { PlusOutlined, RedoOutlined, ReloadOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import dayjs from 'dayjs';
import { notificationsApi } from '../api/endpoints';
import type { DeliveryStatus, NotificationLogDto } from '../api/types';

const statusColors: Record<DeliveryStatus, string> = {
  Sent: 'success',
  Failed: 'error',
  Skipped: 'warning',
};

const statusLabels: Record<DeliveryStatus, string> = {
  Sent: 'Отправлено',
  Failed: 'Ошибка',
  Skipped: 'Пропущено',
};

export function StatusTag({ status }: { status: DeliveryStatus }) {
  return <Tag color={statusColors[status]}>{statusLabels[status]}</Tag>;
}

function formatDate(value: string | null) {
  return value ? dayjs(value).format('DD.MM.YYYY HH:mm:ss') : '—';
}

export function NotificationsPage() {
  const { message } = App.useApp();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [status, setStatus] = useState<DeliveryStatus | undefined>();
  const [search, setSearch] = useState('');
  const [appliedSearch, setAppliedSearch] = useState('');
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(50);
  const [detailsId, setDetailsId] = useState<string | null>(null);

  const listQuery = useQuery({
    queryKey: ['notifications', 'list', { status, search: appliedSearch, page, pageSize }],
    queryFn: () => notificationsApi.list({ status, search: appliedSearch, page, pageSize }),
  });

  const detailsQuery = useQuery({
    queryKey: ['notifications', 'details', detailsId],
    queryFn: () => notificationsApi.get(detailsId!),
    enabled: detailsId !== null,
  });

  const retryMutation = useMutation({
    mutationFn: notificationsApi.retry,
    onSuccess: () => {
      message.success('Повторная отправка поставлена в очередь');
      void queryClient.invalidateQueries({ queryKey: ['notifications'] });
    },
    onError: (error: Error) => message.error(error.message),
  });

  const columns: ColumnsType<NotificationLogDto> = [
    {
      title: 'Создано',
      dataIndex: 'createdAt',
      width: 170,
      render: (value: string) => formatDate(value),
    },
    {
      title: 'Статус',
      dataIndex: 'status',
      width: 130,
      render: (value: DeliveryStatus) => <StatusTag status={value} />,
    },
    {
      title: 'Тема',
      dataIndex: 'subject',
      ellipsis: true,
      render: (value: string | null) => value ?? '—',
    },
    {
      title: 'Получатели',
      dataIndex: 'recipients',
      ellipsis: true,
      render: (recipients: string[]) => (
        <Tooltip title={recipients.join(', ')}>
          {recipients.slice(0, 2).join(', ')}
          {recipients.length > 2 ? ` +${recipients.length - 2}` : ''}
        </Tooltip>
      ),
    },
    {
      title: 'Канал',
      dataIndex: 'channel',
      width: 100,
    },
    {
      title: 'Попытки',
      dataIndex: 'attemptCount',
      width: 90,
      align: 'center',
    },
    {
      title: 'Действия',
      key: 'actions',
      width: 120,
      render: (_, record) => (
        <Space>
          <Button size="small" onClick={() => setDetailsId(record.id)}>
            Детали
          </Button>
          {record.status === 'Failed' && (
            <Tooltip title="Повторить отправку">
              <Button
                size="small"
                icon={<RedoOutlined />}
                loading={retryMutation.isPending && retryMutation.variables === record.id}
                onClick={() => retryMutation.mutate(record.id)}
              />
            </Tooltip>
          )}
        </Space>
      ),
    },
  ];

  const details = detailsQuery.data;

  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 16 }}>
        <Typography.Title level={3} style={{ margin: 0 }}>
          Журнал рассылок
        </Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => navigate('/notifications/new')}>
          Новая рассылка
        </Button>
      </Flex>

      <Space style={{ marginBottom: 16 }} wrap>
        <Select
          allowClear
          placeholder="Статус"
          style={{ width: 160 }}
          value={status}
          onChange={(value) => {
            setStatus(value);
            setPage(1);
          }}
          options={(Object.keys(statusLabels) as DeliveryStatus[]).map((s) => ({
            value: s,
            label: statusLabels[s],
          }))}
        />
        <Input.Search
          placeholder="Тема, получатель или ключ идемпотентности"
          style={{ width: 340 }}
          allowClear
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          onSearch={(value) => {
            setAppliedSearch(value.trim());
            setPage(1);
          }}
        />
        <Button icon={<ReloadOutlined />} onClick={() => void listQuery.refetch()}>
          Обновить
        </Button>
      </Space>

      <Table<NotificationLogDto>
        rowKey="id"
        size="middle"
        loading={listQuery.isLoading}
        columns={columns}
        dataSource={listQuery.data?.items ?? []}
        pagination={{
          current: page,
          pageSize,
          total: listQuery.data?.totalCount ?? 0,
          showSizeChanger: true,
          showTotal: (total) => `Всего: ${total}`,
          onChange: (newPage, newPageSize) => {
            setPage(newPage);
            setPageSize(newPageSize);
          },
        }}
      />

      <Drawer
        title="Детали сообщения"
        width={640}
        open={detailsId !== null}
        onClose={() => setDetailsId(null)}
        loading={detailsQuery.isLoading}
      >
        {details && (
          <>
            <Descriptions column={1} bordered size="small">
              <Descriptions.Item label="Статус">
                <StatusTag status={details.status} />
              </Descriptions.Item>
              <Descriptions.Item label="Тема">{details.subject ?? '—'}</Descriptions.Item>
              <Descriptions.Item label="Получатели">{details.recipients.join(', ')}</Descriptions.Item>
              <Descriptions.Item label="Канал">{details.channel}</Descriptions.Item>
              <Descriptions.Item label="Формат">{details.bodyFormat}</Descriptions.Item>
              <Descriptions.Item label="Приоритет">{details.priority}</Descriptions.Item>
              <Descriptions.Item label="Попытки">{details.attemptCount}</Descriptions.Item>
              <Descriptions.Item label="Создано">{formatDate(details.createdAt)}</Descriptions.Item>
              <Descriptions.Item label="Обработано">{formatDate(details.processedAt)}</Descriptions.Item>
              <Descriptions.Item label="Message ID">{details.messageId}</Descriptions.Item>
              {details.idempotencyKey && (
                <Descriptions.Item label="Ключ идемпотентности">{details.idempotencyKey}</Descriptions.Item>
              )}
              {details.retryOfMessageId && (
                <Descriptions.Item label="Повтор сообщения">{details.retryOfMessageId}</Descriptions.Item>
              )}
              {details.errorMessage && (
                <Descriptions.Item label="Ошибка">
                  <Typography.Text type="danger">{details.errorMessage}</Typography.Text>
                </Descriptions.Item>
              )}
            </Descriptions>
            {details.body && (
              <>
                <Typography.Title level={5} style={{ marginTop: 16 }}>
                  Текст сообщения
                </Typography.Title>
                <pre
                  style={{
                    whiteSpace: 'pre-wrap',
                    wordBreak: 'break-word',
                    maxHeight: 320,
                    overflow: 'auto',
                    padding: 12,
                    borderRadius: 8,
                    background: 'rgba(128, 128, 128, 0.1)',
                  }}
                >
                  {details.body}
                </pre>
              </>
            )}
            {details.status === 'Failed' && (
              <Button
                type="primary"
                icon={<RedoOutlined />}
                style={{ marginTop: 16 }}
                loading={retryMutation.isPending}
                onClick={() => retryMutation.mutate(details.id)}
              >
                Повторить отправку
              </Button>
            )}
          </>
        )}
      </Drawer>
    </div>
  );
}

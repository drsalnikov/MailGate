import { useState } from 'react';
import { App, Button, Flex, Input, Popconfirm, Space, Table, Typography } from 'antd';
import { DeleteOutlined, EditOutlined, PlusOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { recipientGroupsApi } from '../api/endpoints';
import type { RecipientGroupDto } from '../api/types';

export function RecipientGroupsPage() {
  const { message } = App.useApp();
  const navigate = useNavigate();
  const queryClient = useQueryClient();

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(50);

  const listQuery = useQuery({
    queryKey: ['recipient-groups', { search, page, pageSize }],
    queryFn: () => recipientGroupsApi.list({ search, page, pageSize }),
  });

  const deleteMutation = useMutation({
    mutationFn: recipientGroupsApi.remove,
    onSuccess: () => {
      message.success('Группа удалена');
      void queryClient.invalidateQueries({ queryKey: ['recipient-groups'] });
    },
    onError: (error: Error) => message.error(error.message),
  });

  const columns: ColumnsType<RecipientGroupDto> = [
    { title: 'Название', dataIndex: 'name' },
    {
      title: 'Описание',
      dataIndex: 'description',
      ellipsis: true,
      render: (value: string | null) => value ?? '—',
    },
    { title: 'Получателей', dataIndex: 'recipientCount', width: 130, align: 'center' },
    {
      title: 'Действия',
      key: 'actions',
      width: 110,
      render: (_, record) => (
        <Space>
          <Button
            size="small"
            icon={<EditOutlined />}
            onClick={() => navigate(`/recipient-groups/${record.id}/edit`)}
          />
          <Popconfirm
            title="Удалить группу?"
            okText="Удалить"
            cancelText="Отмена"
            onConfirm={() => deleteMutation.mutate(record.id)}
          >
            <Button size="small" danger icon={<DeleteOutlined />} />
          </Popconfirm>
        </Space>
      ),
    },
  ];

  return (
    <div>
      <Flex justify="space-between" align="center" style={{ marginBottom: 16 }}>
        <Typography.Title level={3} style={{ margin: 0 }}>
          Группы получателей
        </Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={() => navigate('/recipient-groups/new')}>
          Создать группу
        </Button>
      </Flex>

      <Input.Search
        placeholder="Поиск по названию"
        style={{ width: 320, marginBottom: 16 }}
        allowClear
        onSearch={(value) => {
          setSearch(value.trim());
          setPage(1);
        }}
      />

      <Table<RecipientGroupDto>
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
    </div>
  );
}

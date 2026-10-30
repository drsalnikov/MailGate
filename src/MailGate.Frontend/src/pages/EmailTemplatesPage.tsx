import { useState } from 'react';
import { App, Button, Card, Input, Popconfirm, Space, Table, Typography } from 'antd';
import { PlusOutlined } from '@ant-design/icons';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { Link } from 'react-router-dom';
import { emailTemplatesApi } from '../api/endpoints';

export function EmailTemplatesPage() {
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const { message } = App.useApp();
  const client = useQueryClient();
  const query = useQuery({ queryKey: ['email-templates', 'list', search, page], queryFn: () => emailTemplatesApi.list({ search, page, pageSize: 20 }) });
  const remove = useMutation({ mutationFn: emailTemplatesApi.remove, onSuccess: () => { void client.invalidateQueries({ queryKey: ['email-templates'] }); message.success('Шаблон удалён'); }, onError: (error: Error) => message.error(error.message) });
  return <div style={{ maxWidth: 1200 }}>
    <Space style={{ width: '100%', justifyContent: 'space-between', marginBottom: 16 }} wrap>
      <Typography.Title level={3} style={{ margin: 0 }}>Шаблоны писем</Typography.Title>
      <Link to="/templates/new"><Button type="primary" icon={<PlusOutlined />}>Создать шаблон</Button></Link>
    </Space>
    <Typography.Paragraph type="secondary">Сохраните оформление один раз, затем выбирайте его при создании рассылки и вводите только основной текст.</Typography.Paragraph>
    <Card>
      <Input.Search placeholder="Найти шаблон" allowClear onSearch={(value) => { setSearch(value); setPage(1); }} style={{ maxWidth: 360, marginBottom: 20 }} />
      {query.isError && <Typography.Paragraph type="danger">{query.error.message}</Typography.Paragraph>}
      <Table rowKey="id" loading={query.isLoading} dataSource={query.data?.items ?? []} scroll={{ x: 650 }}
        locale={{ emptyText: 'Шаблонов пока нет. Создайте первый макет письма.' }}
        pagination={{ current: page, pageSize: 20, total: query.data?.totalCount ?? 0, showSizeChanger: false, onChange: setPage }}
        columns={[
          { title: 'Название', dataIndex: 'name', render: (name, row) => <Link to={`/templates/${row.id}/edit`}>{name}</Link> },
          { title: 'Тема по умолчанию', dataIndex: 'subject' },
          { title: 'Изменён', dataIndex: 'updatedAt', render: (value: string) => new Date(value).toLocaleString('ru-RU') },
          { title: 'Действия', render: (_, row) => <Space><Link to={`/templates/${row.id}/edit`}>Редактировать</Link><Popconfirm title="Удалить шаблон? Уже поставленные в очередь письма не изменятся." onConfirm={() => remove.mutate(row.id)}><Button type="link" danger loading={remove.isPending}>Удалить</Button></Popconfirm></Space> },
        ]} />
    </Card>
  </div>;
}

import { useState } from 'react';
import { App, Button, Flex, Form, Input, Modal, Popconfirm, Select, Space, Table, Tag, Typography } from 'antd';
import { DeleteOutlined, EditOutlined, PlusOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { recipientsApi, systemsApi } from '../api/endpoints';
import type { CreateRecipientDto, RecipientDto } from '../api/types';

interface FormValues {
  fullName: string;
  email: string;
  systemIds: string[];
}

export function RecipientsPage() {
  const { message } = App.useApp();
  const queryClient = useQueryClient();
  const [form] = Form.useForm<FormValues>();

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(50);
  const [editing, setEditing] = useState<RecipientDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);

  const listQuery = useQuery({
    queryKey: ['recipients', { search, page, pageSize }],
    queryFn: () => recipientsApi.list({ search, page, pageSize }),
  });

  const systemsQuery = useQuery({
    queryKey: ['systems', 'all'],
    queryFn: () => systemsApi.list({ pageSize: 500 }),
  });

  const invalidate = () => void queryClient.invalidateQueries({ queryKey: ['recipients'] });

  const saveMutation = useMutation({
    mutationFn: (values: FormValues) => {
      const dto: CreateRecipientDto = {
        fullName: values.fullName.trim(),
        email: values.email.trim(),
        systemIds: values.systemIds ?? [],
      };
      return editing ? recipientsApi.update(editing.id, dto) : recipientsApi.create(dto);
    },
    onSuccess: () => {
      message.success(editing ? 'Получатель обновлён' : 'Получатель создан');
      setModalOpen(false);
      invalidate();
    },
    onError: (error: Error) => message.error(error.message),
  });

  const deleteMutation = useMutation({
    mutationFn: recipientsApi.remove,
    onSuccess: () => {
      message.success('Получатель удалён');
      invalidate();
    },
    onError: (error: Error) => message.error(error.message),
  });

  const openCreate = () => {
    setEditing(null);
    form.resetFields();
    setModalOpen(true);
  };

  const openEdit = (record: RecipientDto) => {
    setEditing(record);
    form.setFieldsValue({
      fullName: record.fullName,
      email: record.email,
      systemIds: record.systems.map((s) => s.id),
    });
    setModalOpen(true);
  };

  const columns: ColumnsType<RecipientDto> = [
    { title: 'ФИО', dataIndex: 'fullName' },
    { title: 'Email', dataIndex: 'email' },
    {
      title: 'Системы',
      dataIndex: 'systems',
      render: (systems: RecipientDto['systems']) =>
        systems.length === 0 ? '—' : systems.map((s) => <Tag key={s.id}>{s.name}</Tag>),
    },
    {
      title: 'Группы',
      dataIndex: 'groups',
      render: (groups: RecipientDto['groups']) =>
        groups.length === 0 ? '—' : groups.map((g) => <Tag key={g.id} color="blue">{g.name}</Tag>),
    },
    {
      title: 'Действия',
      key: 'actions',
      width: 110,
      render: (_, record) => (
        <Space>
          <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm
            title="Удалить получателя?"
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
          Получатели
        </Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
          Добавить получателя
        </Button>
      </Flex>

      <Input.Search
        placeholder="Поиск по ФИО или email"
        style={{ width: 320, marginBottom: 16 }}
        allowClear
        onSearch={(value) => {
          setSearch(value.trim());
          setPage(1);
        }}
      />

      <Table<RecipientDto>
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

      <Modal
        title={editing ? 'Редактирование получателя' : 'Новый получатель'}
        open={modalOpen}
        onCancel={() => setModalOpen(false)}
        onOk={() => form.submit()}
        okText="Сохранить"
        cancelText="Отмена"
        confirmLoading={saveMutation.isPending}
        destroyOnHidden
      >
        <Form<FormValues> form={form} layout="vertical" onFinish={(values) => saveMutation.mutate(values)}>
          <Form.Item
            name="fullName"
            label="ФИО"
            rules={[{ required: true, message: 'Введите ФИО' }]}
          >
            <Input placeholder="Иванов Иван Иванович" maxLength={300} />
          </Form.Item>
          <Form.Item
            name="email"
            label="Email"
            rules={[
              { required: true, message: 'Введите email' },
              { type: 'email', message: 'Некорректный email' },
            ]}
          >
            <Input placeholder="user@example.com" maxLength={320} />
          </Form.Item>
          <Form.Item name="systemIds" label="Подписки на системы">
            <Select
              mode="multiple"
              allowClear
              placeholder="Выберите системы"
              loading={systemsQuery.isLoading}
              optionFilterProp="label"
              options={(systemsQuery.data?.items ?? []).map((s) => ({
                value: s.id,
                label: `${s.name} (${s.code})`,
              }))}
            />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

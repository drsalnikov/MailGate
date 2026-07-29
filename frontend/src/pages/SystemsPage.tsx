import { useState } from 'react';
import { App, Button, Flex, Form, Input, Modal, Popconfirm, Space, Table, Typography } from 'antd';
import { DeleteOutlined, EditOutlined, PlusOutlined } from '@ant-design/icons';
import type { ColumnsType } from 'antd/es/table';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { systemsApi } from '../api/endpoints';
import type { CreateSystemDto, SystemDto } from '../api/types';

interface FormValues {
  code: string;
  name: string;
  description?: string;
}

export function SystemsPage() {
  const { message } = App.useApp();
  const queryClient = useQueryClient();
  const [form] = Form.useForm<FormValues>();

  const [search, setSearch] = useState('');
  const [page, setPage] = useState(1);
  const [pageSize, setPageSize] = useState(50);
  const [editing, setEditing] = useState<SystemDto | null>(null);
  const [modalOpen, setModalOpen] = useState(false);

  const listQuery = useQuery({
    queryKey: ['systems', { search, page, pageSize }],
    queryFn: () => systemsApi.list({ search, page, pageSize }),
  });

  const invalidate = () => void queryClient.invalidateQueries({ queryKey: ['systems'] });

  const saveMutation = useMutation({
    mutationFn: (values: FormValues) => {
      const dto: CreateSystemDto = {
        code: values.code.trim(),
        name: values.name.trim(),
        description: values.description?.trim() || null,
      };
      return editing ? systemsApi.update(editing.id, dto) : systemsApi.create(dto);
    },
    onSuccess: () => {
      message.success(editing ? 'Система обновлена' : 'Система создана');
      setModalOpen(false);
      invalidate();
    },
    onError: (error: Error) => message.error(error.message),
  });

  const deleteMutation = useMutation({
    mutationFn: systemsApi.remove,
    onSuccess: () => {
      message.success('Система удалена');
      invalidate();
    },
    onError: (error: Error) => message.error(error.message),
  });

  const openCreate = () => {
    setEditing(null);
    form.resetFields();
    setModalOpen(true);
  };

  const openEdit = (record: SystemDto) => {
    setEditing(record);
    form.setFieldsValue({
      code: record.code,
      name: record.name,
      description: record.description ?? undefined,
    });
    setModalOpen(true);
  };

  const columns: ColumnsType<SystemDto> = [
    { title: 'Код', dataIndex: 'code', width: 180 },
    { title: 'Название', dataIndex: 'name' },
    {
      title: 'Описание',
      dataIndex: 'description',
      ellipsis: true,
      render: (value: string | null) => value ?? '—',
    },
    { title: 'Подписчиков', dataIndex: 'subscriberCount', width: 130, align: 'center' },
    {
      title: 'Действия',
      key: 'actions',
      width: 110,
      render: (_, record) => (
        <Space>
          <Button size="small" icon={<EditOutlined />} onClick={() => openEdit(record)} />
          <Popconfirm
            title="Удалить систему?"
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
          Системы
        </Typography.Title>
        <Button type="primary" icon={<PlusOutlined />} onClick={openCreate}>
          Добавить систему
        </Button>
      </Flex>

      <Input.Search
        placeholder="Поиск по коду или названию"
        style={{ width: 320, marginBottom: 16 }}
        allowClear
        onSearch={(value) => {
          setSearch(value.trim());
          setPage(1);
        }}
      />

      <Table<SystemDto>
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
        title={editing ? 'Редактирование системы' : 'Новая система'}
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
            name="code"
            label="Код"
            rules={[{ required: true, message: 'Введите код системы' }]}
          >
            <Input placeholder="crm" maxLength={100} />
          </Form.Item>
          <Form.Item
            name="name"
            label="Название"
            rules={[{ required: true, message: 'Введите название' }]}
          >
            <Input placeholder="CRM-система" maxLength={200} />
          </Form.Item>
          <Form.Item name="description" label="Описание">
            <Input.TextArea rows={3} maxLength={1000} />
          </Form.Item>
        </Form>
      </Modal>
    </div>
  );
}

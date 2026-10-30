import { useEffect } from 'react';
import { App, Button, Card, Form, Input, Select, Space, Spin, Typography } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useParams } from 'react-router-dom';
import { recipientGroupsApi, recipientsApi } from '../api/endpoints';
import type { CreateRecipientGroupDto } from '../api/types';

interface FormValues {
  name: string;
  description?: string;
  recipientIds: string[];
}

export function RecipientGroupEditPage() {
  const { id } = useParams<{ id: string }>();
  const isEdit = Boolean(id);
  const { message } = App.useApp();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [form] = Form.useForm<FormValues>();

  const groupQuery = useQuery({
    queryKey: ['recipient-groups', 'details', id],
    queryFn: () => recipientGroupsApi.get(id!),
    enabled: isEdit,
  });

  const recipientsQuery = useQuery({
    queryKey: ['recipients', 'all'],
    queryFn: () => recipientsApi.list({ pageSize: 500 }),
  });

  useEffect(() => {
    if (groupQuery.data) {
      form.setFieldsValue({
        name: groupQuery.data.name,
        description: groupQuery.data.description ?? undefined,
        recipientIds: groupQuery.data.recipients.map((r) => r.id),
      });
    }
  }, [groupQuery.data, form]);

  const saveMutation = useMutation({
    mutationFn: (values: FormValues) => {
      const dto: CreateRecipientGroupDto = {
        name: values.name.trim(),
        description: values.description?.trim() || null,
        recipientIds: values.recipientIds ?? [],
      };
      return isEdit ? recipientGroupsApi.update(id!, dto) : recipientGroupsApi.create(dto);
    },
    onSuccess: () => {
      message.success(isEdit ? 'Группа обновлена' : 'Группа создана');
      void queryClient.invalidateQueries({ queryKey: ['recipient-groups'] });
      navigate('/recipient-groups');
    },
    onError: (error: Error) => message.error(error.message),
  });

  if (isEdit && groupQuery.isLoading) {
    return <Spin size="large" style={{ display: 'block', margin: '80px auto' }} />;
  }

  return (
    <div style={{ maxWidth: 720 }}>
      <Typography.Title level={3}>
        {isEdit ? 'Редактирование группы' : 'Новая группа получателей'}
      </Typography.Title>
      <Card>
        <Form<FormValues> form={form} layout="vertical" onFinish={(values) => saveMutation.mutate(values)}>
          <Form.Item
            name="name"
            label="Название"
            rules={[{ required: true, message: 'Введите название группы' }]}
          >
            <Input placeholder="Отдел разработки" maxLength={200} />
          </Form.Item>
          <Form.Item name="description" label="Описание">
            <Input.TextArea rows={3} maxLength={1000} />
          </Form.Item>
          <Form.Item name="recipientIds" label="Получатели">
            <Select
              mode="multiple"
              allowClear
              placeholder="Выберите получателей"
              loading={recipientsQuery.isLoading}
              optionFilterProp="label"
              options={(recipientsQuery.data?.items ?? []).map((r) => ({
                value: r.id,
                label: `${r.fullName} <${r.email}>`,
              }))}
            />
          </Form.Item>
          <Form.Item>
            <Space>
              <Button type="primary" htmlType="submit" icon={<SaveOutlined />} loading={saveMutation.isPending}>
                Сохранить
              </Button>
              <Button onClick={() => navigate('/recipient-groups')}>Отмена</Button>
            </Space>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
}

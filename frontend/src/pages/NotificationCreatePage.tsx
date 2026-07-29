import { App, Button, Card, Form, Input, Radio, Select, Space, Typography } from 'antd';
import { SendOutlined } from '@ant-design/icons';
import { useMutation, useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { notificationsApi, recipientGroupsApi, recipientsApi } from '../api/endpoints';
import type { BodyFormat, NotificationPriority, SendNotificationFromAdminDto } from '../api/types';

interface FormValues {
  recipientIds: string[];
  recipientGroupIds: string[];
  adHocEmails: string[];
  subject?: string;
  body: string;
  bodyFormat: BodyFormat;
  priority: NotificationPriority;
}

const EMAIL_RE = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export function NotificationCreatePage() {
  const { message } = App.useApp();
  const navigate = useNavigate();
  const [form] = Form.useForm<FormValues>();

  const recipientsQuery = useQuery({
    queryKey: ['recipients', 'all'],
    queryFn: () => recipientsApi.list({ pageSize: 500 }),
  });

  const groupsQuery = useQuery({
    queryKey: ['recipient-groups', 'all'],
    queryFn: () => recipientGroupsApi.list({ pageSize: 500 }),
  });

  const sendMutation = useMutation({
    mutationFn: (dto: SendNotificationFromAdminDto) => notificationsApi.send(dto),
    onSuccess: () => {
      message.success('Рассылка поставлена в очередь');
      navigate('/notifications');
    },
    onError: (error: Error) => message.error(error.message),
  });

  const onFinish = (values: FormValues) => {
    const invalidEmails = (values.adHocEmails ?? []).filter((e) => !EMAIL_RE.test(e.trim()));
    if (invalidEmails.length > 0) {
      message.error(`Некорректные адреса: ${invalidEmails.join(', ')}`);
      return;
    }

    if (
      (values.recipientIds?.length ?? 0) === 0 &&
      (values.recipientGroupIds?.length ?? 0) === 0 &&
      (values.adHocEmails?.length ?? 0) === 0
    ) {
      message.error('Укажите хотя бы одного получателя, группу или адрес');
      return;
    }

    sendMutation.mutate({
      channel: 'Smtp',
      recipientIds: values.recipientIds ?? [],
      recipientGroupIds: values.recipientGroupIds ?? [],
      adHocEmails: (values.adHocEmails ?? []).map((e) => e.trim()),
      subject: values.subject?.trim() || null,
      body: values.body,
      bodyFormat: values.bodyFormat,
      priority: values.priority,
    });
  };

  return (
    <div style={{ maxWidth: 800 }}>
      <Typography.Title level={3}>Новая рассылка</Typography.Title>
      <Card>
        <Form<FormValues>
          form={form}
          layout="vertical"
          onFinish={onFinish}
          initialValues={{ bodyFormat: 'PlainText', priority: 'Normal' }}
        >
          <Form.Item name="recipientIds" label="Получатели из адресной книги">
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

          <Form.Item name="recipientGroupIds" label="Группы получателей">
            <Select
              mode="multiple"
              allowClear
              placeholder="Выберите группы"
              loading={groupsQuery.isLoading}
              optionFilterProp="label"
              options={(groupsQuery.data?.items ?? []).map((g) => ({
                value: g.id,
                label: `${g.name} (${g.recipientCount})`,
              }))}
            />
          </Form.Item>

          <Form.Item
            name="adHocEmails"
            label="Дополнительные адреса"
            tooltip="Введите email и нажмите Enter"
          >
            <Select mode="tags" allowClear placeholder="user@example.com" tokenSeparators={[',', ';', ' ']} open={false} suffixIcon={null} />
          </Form.Item>

          <Form.Item name="subject" label="Тема">
            <Input placeholder="Тема письма" maxLength={500} />
          </Form.Item>

          <Form.Item
            name="body"
            label="Текст сообщения"
            rules={[{ required: true, message: 'Введите текст сообщения' }]}
          >
            <Input.TextArea rows={10} placeholder="Текст письма" />
          </Form.Item>

          <Space size="large" wrap>
            <Form.Item name="bodyFormat" label="Формат">
              <Radio.Group
                options={[
                  { value: 'PlainText', label: 'Текст' },
                  { value: 'Html', label: 'HTML' },
                  { value: 'Markdown', label: 'Markdown' },
                ]}
                optionType="button"
              />
            </Form.Item>

            <Form.Item name="priority" label="Приоритет">
              <Select
                style={{ width: 160 }}
                options={[
                  { value: 'Low', label: 'Низкий' },
                  { value: 'Normal', label: 'Обычный' },
                  { value: 'High', label: 'Высокий' },
                  { value: 'Critical', label: 'Критический' },
                ]}
              />
            </Form.Item>
          </Space>

          <Form.Item>
            <Space>
              <Button
                type="primary"
                htmlType="submit"
                icon={<SendOutlined />}
                loading={sendMutation.isPending}
              >
                Отправить
              </Button>
              <Button onClick={() => navigate('/notifications')}>Отмена</Button>
            </Space>
          </Form.Item>
        </Form>
      </Card>
    </div>
  );
}

import { useState, type Dispatch, type SetStateAction } from 'react';
import { Link } from 'react-router-dom';
import { EmailTemplateEditor } from '../components/EmailTemplateEditor';
import { EmailPreview } from '../components/EmailPreview';
import { useDebounced } from '../hooks/useDebounced';
import { App, Alert, Col, Row, Button, Card, Form, Input, Radio, Select, Space, Typography } from 'antd';
import { SendOutlined } from '@ant-design/icons';
import { useMutation, useQuery } from '@tanstack/react-query';
import { useNavigate } from 'react-router-dom';
import { emailTemplatesApi, notificationsApi, recipientGroupsApi, recipientsApi } from '../api/endpoints';
import type { BodyFormat, EmailTemplateDefinition, NotificationPriority, SendNotificationFromAdminDto } from '../api/types';

interface FormValues {
  templateId?: string;
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

  const [draft, setDraft] = useState<{ id: string; revision: string; definition: EmailTemplateDefinition }>();
  const [uploadingImage, setUploadingImage] = useState(false);
  const [templateSearch, setTemplateSearch] = useState('');
  const templateId = Form.useWatch('templateId', form);
  const body = Form.useWatch('body', form) ?? '';
  const subject = Form.useWatch('subject', form) ?? '';
  const bodyFormat = Form.useWatch('bodyFormat', form) ?? 'PlainText';
  const templates = useQuery({ queryKey: ['email-templates', 'select', templateSearch], queryFn: () => emailTemplatesApi.list({ search: templateSearch, pageSize: 100 }), refetchOnWindowFocus: true });
  const template = useQuery({ queryKey: ['email-templates', 'detail', templateId], queryFn: () => emailTemplatesApi.get(templateId!), enabled: !!templateId, refetchOnWindowFocus: false });
  const definition = draft && draft.id === templateId && draft.revision === template.data?.revision ? draft.definition : template.data?.definition;
  const setDefinition: Dispatch<SetStateAction<EmailTemplateDefinition>> = (update) => {
    if (!templateId || !template.data) return;
    const selected = template.data;
    setDraft((current) => {
      const previous = current?.id === templateId && current.revision === selected.revision ? current.definition : selected.definition;
      return { id: templateId, revision: selected.revision, definition: typeof update === 'function' ? update(previous) : update };
    });
  };
  const previewRequest = JSON.stringify({ templateId: templateId ?? null, templateRevision: template.data?.revision ?? null, templateDefinition: templateId ? definition : null, body, subject: subject.trim() || null, bodyFormat });
  const debouncedPreview = useDebounced(previewRequest);
  const preview = useQuery({ queryKey: ['notification-preview', debouncedPreview], queryFn: () => notificationsApi.preview(JSON.parse(debouncedPreview)), enabled: !templateId || !!template.data, retry: false, gcTime: 0 });

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

    if (values.templateId && !template.data) { message.error('Дождитесь загрузки шаблона'); return; }
    if (values.templateId && (preview.isError || preview.isFetching || previewRequest !== debouncedPreview)) {
      message.error('Дождитесь корректного предпросмотра письма'); return;
    }
    if (uploadingImage) { message.error("Дождитесь загрузки изображения"); return; }
    sendMutation.mutate({
      templateId: values.templateId ?? null,
      templateRevision: values.templateId ? template.data?.revision : null,
      templateDefinition: values.templateId ? definition : null,
      channel: 'Smtp',
      recipientIds: values.recipientIds ?? [],
      recipientGroupIds: values.recipientGroupIds ?? [],
      adHocEmails: (values.adHocEmails ?? []).map((e) => e.trim()),
      subject: values.subject?.trim() || null,
      body: values.body,
      bodyFormat: values.templateId ? 'Html' : values.bodyFormat,
      priority: values.priority,
    });
  };

  return (
    <div style={{ maxWidth: 1600 }}>
      <Typography.Title level={3}>Новая рассылка</Typography.Title>
      <Row gutter={[24, 24]}>
      <Col xs={24} xl={12}>
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

          <Form.Item name="templateId" label="Шаблон письма" extra={<Link to="/templates" target="_blank" rel="noopener noreferrer">Управление шаблонами ↗</Link>}>
            <Select allowClear showSearch filterOption={false} onChange={() => setDraft(undefined)} onSearch={setTemplateSearch} placeholder="Без шаблона" loading={templates.isLoading}
              options={(templates.data?.items ?? []).map((item) => ({ value: item.id, label: item.name }))} />
          </Form.Item>
          {templates.isError && <Alert type="error" title={templates.error.message} style={{ marginBottom: 16 }} />}
          {template.isError && <Alert type="error" title={template.error.message} style={{ marginBottom: 16 }} />}
          {templateId && preview.isError && <Button style={{ marginBottom: 16 }} onClick={() => { void template.refetch(); }}>Обновить выбранный шаблон</Button>}
          {templateId && <Alert type="info" title="Основной текст будет помещён в выбранный макет" description="Редактируйте оформление и блоки ниже — изменения применятся только к этому письму. Исходный шаблон не изменится. Основной текст вводится без HTML." style={{ marginBottom: 16 }} />}
          {templateId && definition && <div style={{ marginBottom: 20 }}>
            <Button style={{ marginBottom: 12 }} disabled={uploadingImage} onClick={() => setDraft(undefined)}>Сбросить изменения макета</Button>
            <EmailTemplateEditor key={templateId} forMessage definition={definition} setDefinition={setDefinition} onUploadingChange={setUploadingImage} />
          </div>}
          <Form.Item name="subject" label="Тема">
            <Input placeholder={templateId ? definition?.subject || 'Тема из шаблона' : 'Тема письма'} maxLength={500} />
          </Form.Item>

          <Form.Item
            name="body"
            label="Текст сообщения"
            rules={[{ required: true, message: 'Введите текст сообщения' }]}
          >
            <Input.TextArea rows={10} placeholder="Текст письма" />
          </Form.Item>

          <Space size="large" wrap>
            {!templateId && <Form.Item name="bodyFormat" label="Формат">
              <Radio.Group
                disabled={!!templateId}
                options={[
                  { value: 'PlainText', label: 'Текст' },
                  { value: 'Html', label: 'HTML' },
                  { value: 'Markdown', label: 'Markdown' },
                ]}
                optionType="button"
              />
            </Form.Item>}

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
                disabled={uploadingImage || (!!templateId && (!template.data || preview.isError || preview.isFetching || previewRequest !== debouncedPreview))}
              >
                Отправить
              </Button>
              <Button onClick={() => navigate('/notifications')}>Отмена</Button>
            </Space>
          </Form.Item>
        </Form>
      </Card>
      </Col>
      <Col xs={24} xl={12}><div style={{ position: 'sticky', top: 20 }}><EmailPreview preview={preview.data} loading={preview.isFetching || previewRequest !== debouncedPreview || (!!templateId && template.isLoading)} error={preview.error} /></div></Col>
      </Row>
    </div>
  );
}

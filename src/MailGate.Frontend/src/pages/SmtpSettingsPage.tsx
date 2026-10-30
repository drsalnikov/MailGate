import { useEffect, useState } from 'react';
import {
  Alert,
  App,
  Button,
  Card,
  Checkbox,
  Collapse,
  Descriptions,
  Form,
  Input,
  InputNumber,
  Popconfirm,
  Space,
  Spin,
  Tag,
  Typography,
} from 'antd';
import { ApiOutlined, DeleteOutlined, SaveOutlined } from '@ant-design/icons';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { smtpApi } from '../api/endpoints';
import type { SmtpTestResultDto, UpdateSmtpConfigurationDto } from '../api/types';

interface FormValues {
  host: string;
  port: number;
  useSsl: boolean;
  useTls: boolean;
  checkCertificateRevocation: boolean;
  fromAddress: string;
  fromName: string;
  username?: string;
  password?: string;
  oauthEnabled: boolean;
  oauthTenantId?: string;
  oauthClientId?: string;
  oauthClientSecret?: string;
  oauthUpn?: string;
}

function toDto(values: FormValues): UpdateSmtpConfigurationDto {
  return {
    host: values.host.trim(),
    port: values.port,
    useSsl: values.useSsl,
    useTls: values.useTls,
    checkCertificateRevocation: values.checkCertificateRevocation,
    fromAddress: values.fromAddress.trim(),
    fromName: values.fromName.trim(),
    username: values.username?.trim() || null,
    password: values.password || null,
    oAuth2: values.oauthEnabled
      ? {
          tenantId: values.oauthTenantId?.trim() ?? '',
          clientId: values.oauthClientId?.trim() ?? '',
          clientSecret: values.oauthClientSecret || null,
          smtpUserPrincipalName: values.oauthUpn?.trim() ?? '',
        }
      : null,
  };
}

export function SmtpSettingsPage() {
  const { message } = App.useApp();
  const queryClient = useQueryClient();
  const [form] = Form.useForm<FormValues>();
  const [testResult, setTestResult] = useState<SmtpTestResultDto | null>(null);

  const effectiveQuery = useQuery({
    queryKey: ['smtp', 'effective'],
    queryFn: smtpApi.effective,
  });

  const effective = effectiveQuery.data;

  useEffect(() => {
    if (effective) {
      form.setFieldsValue({
        host: effective.host,
        port: effective.port,
        useSsl: effective.useSsl,
        useTls: effective.useTls,
        checkCertificateRevocation: effective.checkCertificateRevocation ?? true,
        fromAddress: effective.fromAddress,
        fromName: effective.fromName,
        username: effective.username ?? undefined,
        oauthEnabled: effective.oAuth2 !== null,
        oauthTenantId: effective.oAuth2?.tenantId,
        oauthClientId: effective.oAuth2?.clientId,
        oauthUpn: effective.oAuth2?.smtpUserPrincipalName,
      });
    }
  }, [effective, form]);

  const invalidate = () => void queryClient.invalidateQueries({ queryKey: ['smtp'] });

  const saveMutation = useMutation({
    mutationFn: (values: FormValues) => smtpApi.putOverride(toDto(values)),
    onSuccess: () => {
      message.success('Настройки SMTP сохранены');
      invalidate();
    },
    onError: (error: Error) => message.error(error.message),
  });

  const deleteMutation = useMutation({
    mutationFn: smtpApi.deleteOverride,
    onSuccess: () => {
      message.success('Переопределение удалено, используются настройки из appsettings');
      setTestResult(null);
      invalidate();
    },
    onError: (error: Error) => message.error(error.message),
  });

  const testMutation = useMutation({
    mutationFn: async () => {
      const values = await form.validateFields();
      return smtpApi.testConnection(toDto(values));
    },
    onSuccess: (result) => setTestResult(result),
    onError: (error: Error) => message.error(error.message),
  });

  const oauthEnabled = Form.useWatch('oauthEnabled', form);

  if (effectiveQuery.isLoading) {
    return <Spin size="large" style={{ display: 'block', margin: '80px auto' }} />;
  }

  return (
    <div style={{ maxWidth: 760 }}>
      <Typography.Title level={3}>Настройки SMTP</Typography.Title>

      {effective && (
        <Card size="small" style={{ marginBottom: 16 }}>
          <Descriptions column={2} size="small" title="Действующая конфигурация">
            <Descriptions.Item label="Источник">
              <Tag color={effective.source === 'Database' ? 'blue' : 'default'}>
                {effective.source === 'Database' ? 'База данных (override)' : 'appsettings'}
              </Tag>
            </Descriptions.Item>
            <Descriptions.Item label="Сервер">
              {effective.host}:{effective.port}
            </Descriptions.Item>
          </Descriptions>
        </Card>
      )}

      <Card>
        <Form<FormValues>
          form={form}
          layout="vertical"
          onFinish={(values) => saveMutation.mutate(values)}
          initialValues={{ port: 587, useTls: true, useSsl: false, checkCertificateRevocation: true, oauthEnabled: false }}
        >
          <Space.Compact block>
            <Form.Item
              name="host"
              label="Хост"
              style={{ flex: 1 }}
              rules={[{ required: true, message: 'Введите хост' }]}
            >
              <Input placeholder="smtp.example.com" />
            </Form.Item>
            <Form.Item
              name="port"
              label="Порт"
              style={{ width: 120 }}
              rules={[{ required: true, message: 'Порт' }]}
            >
              <InputNumber min={1} max={65535} style={{ width: '100%' }} />
            </Form.Item>
          </Space.Compact>

          <Space size="large" wrap>
            <Form.Item name="useTls" valuePropName="checked" style={{ marginBottom: 8 }}>
              <Checkbox onChange={(event) => { if (event.target.checked) form.setFieldValue('useSsl', false); }}>STARTTLS</Checkbox>
            </Form.Item>
            <Form.Item name="useSsl" valuePropName="checked" style={{ marginBottom: 8 }}>
              <Checkbox onChange={(event) => { if (event.target.checked) form.setFieldValue('useTls', false); }}>SSL при подключении</Checkbox>
            </Form.Item>
            <Form.Item name="checkCertificateRevocation" valuePropName="checked" style={{ marginBottom: 8 }}
              tooltip="При отключении не проверяется отзыв сертификатов SMTP. Доверие УЦ и имя сервера продолжают проверяться.">
              <Checkbox>Проверять отзыв сертификатов</Checkbox>
            </Form.Item>
          </Space>

          <Form.Item
            name="fromAddress"
            label="Адрес отправителя"
            rules={[
              { required: true, message: 'Введите адрес отправителя' },
              { type: 'email', message: 'Некорректный email' },
            ]}
          >
            <Input placeholder="noreply@example.com" />
          </Form.Item>

          <Form.Item
            name="fromName"
            label="Имя отправителя"
            rules={[{ required: true, message: 'Введите имя отправителя' }]}
          >
            <Input placeholder="MailGate" />
          </Form.Item>

          <Form.Item name="username" label="Логин">
            <Input autoComplete="off" />
          </Form.Item>

          <Form.Item
            name="password"
            label="Пароль"
            tooltip={effective?.hasPassword ? 'Пароль уже сохранён. Оставьте пустым, чтобы не менять.' : undefined}
          >
            <Input.Password autoComplete="new-password" placeholder={effective?.hasPassword ? '••••••••' : ''} />
          </Form.Item>

          <Form.Item name="oauthEnabled" valuePropName="checked">
            <Checkbox>OAuth2 (Microsoft Exchange Online)</Checkbox>
          </Form.Item>

          {oauthEnabled && (
            <Collapse
              defaultActiveKey={['oauth']}
              items={[
                {
                  key: 'oauth',
                  label: 'Параметры OAuth2',
                  children: (
                    <>
                      <Form.Item
                        name="oauthTenantId"
                        label="Tenant ID"
                        rules={[{ required: true, message: 'Введите Tenant ID' }]}
                      >
                        <Input />
                      </Form.Item>
                      <Form.Item
                        name="oauthClientId"
                        label="Client ID"
                        rules={[{ required: true, message: 'Введите Client ID' }]}
                      >
                        <Input />
                      </Form.Item>
                      <Form.Item
                        name="oauthClientSecret"
                        label="Client Secret"
                        tooltip={
                          effective?.oAuth2?.hasClientSecret
                            ? 'Секрет уже сохранён. Оставьте пустым, чтобы не менять.'
                            : undefined
                        }
                      >
                        <Input.Password autoComplete="new-password" />
                      </Form.Item>
                      <Form.Item
                        name="oauthUpn"
                        label="SMTP User Principal Name"
                        rules={[{ required: true, message: 'Введите UPN' }]}
                      >
                        <Input placeholder="mailer@tenant.onmicrosoft.com" />
                      </Form.Item>
                    </>
                  ),
                },
              ]}
              style={{ marginBottom: 16 }}
            />
          )}

          {testResult && (
            <Alert
              style={{ marginBottom: 16 }}
              type={testResult.success ? 'success' : 'error'}
              message={
                testResult.success
                  ? `Подключение успешно (${testResult.durationMs} мс)`
                  : `Ошибка подключения: ${testResult.errorMessage}`
              }
              closable
              onClose={() => setTestResult(null)}
            />
          )}

          <Space wrap>
            <Button type="primary" htmlType="submit" icon={<SaveOutlined />} loading={saveMutation.isPending}>
              Сохранить override
            </Button>
            <Button icon={<ApiOutlined />} loading={testMutation.isPending} onClick={() => testMutation.mutate()}>
              Проверить подключение
            </Button>
            {effective?.source === 'Database' && (
              <Popconfirm
                title="Удалить переопределение и вернуться к настройкам из appsettings?"
                okText="Удалить"
                cancelText="Отмена"
                onConfirm={() => deleteMutation.mutate()}
              >
                <Button danger icon={<DeleteOutlined />} loading={deleteMutation.isPending}>
                  Удалить override
                </Button>
              </Popconfirm>
            )}
          </Space>
        </Form>
      </Card>
    </div>
  );
}

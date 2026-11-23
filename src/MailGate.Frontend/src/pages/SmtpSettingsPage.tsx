import { useEffect, useState } from 'react';
import {
  Alert,
  App,
  Button,
  Card,
  Checkbox,
  Radio,
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
import type { SmtpCredentialSource, SmtpTestResultDto, UpdateSmtpConfigurationDto } from '../api/types';

interface FormValues {
  host: string;
  port: number;
  useSsl: boolean;
  useTls: boolean;
  checkCertificateRevocation: boolean;
  fromAddress: string;
  fromName: string;
  credentialSource: SmtpCredentialSource;
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
    credentialSource: values.credentialSource,
    username: null,
    password: null,
    oAuth2: null,
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
        credentialSource: effective.credentialSource === 'Legacy' && !effective.username && !effective.hasPassword && !effective.oAuth2 ? 'Manual' : effective.credentialSource,
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

  const credentialSource = Form.useWatch('credentialSource', form);
  const pending = saveMutation.isPending || testMutation.isPending || deleteMutation.isPending;

  if (effectiveQuery.isLoading) {
    return <Spin size="large" style={{ display: 'block', margin: '80px auto' }} />;
  }

  return (
    <div style={{ maxWidth: 760 }}>
      <Typography.Title level={3}>Настройки SMTP</Typography.Title>

      {effectiveQuery.isError && <Alert type="error" title={effectiveQuery.error.message} style={{ marginBottom: 16 }} />}
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
          initialValues={{ port: 587, useTls: true, useSsl: false, checkCertificateRevocation: true, credentialSource: 'Manual' }}
          disabled={pending}
          onValuesChange={() => setTestResult(null)}
        >
          <Form.Item name="credentialSource" label="Источник SMTP-настроек" rules={[{ required: true }]}>
            <Radio.Group options={[
              { value: 'Manual', label: 'Вручную (тестовый SMTP без авторизации)' },
              { value: 'Sidecar', label: 'Существующая почтовая учётная запись' },
            ]} />
          </Form.Item>
          <Alert style={{ marginBottom: 16 }} type={credentialSource === 'Legacy' ? 'warning' : 'info'} title={
            credentialSource === 'Sidecar'
              ? 'Логин и пароль берутся из файла, предоставленного sidecar. Они не сохраняются в БД. Укажите параметры сервера ниже и включите STARTTLS или SSL.'
              : credentialSource === 'Legacy'
                ? 'Прежняя конфигурация продолжает работать. Выберите новый режим, чтобы изменить настройки. При сохранении прежние логин, пароль и OAuth2 будут удалены из этой записи.'
                : 'Режим для MailHog и других тестовых серверов без аутентификации. Ввод логина, пароля и OAuth2 отключён.'
          } />
          {effective?.source === 'Database' && effective.credentialSource === 'Legacy' && credentialSource !== 'Legacy' &&
            <Alert type="warning" style={{ marginBottom: 16 }} title="Сохранение нового режима очистит старые учётные данные из БД. Проверьте новый режим перед сохранением." />}
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
            <Button type="primary" htmlType="submit" icon={<SaveOutlined />} loading={saveMutation.isPending} disabled={pending || credentialSource === 'Legacy'}>
              Сохранить override
            </Button>
            <Button icon={<ApiOutlined />} loading={testMutation.isPending} disabled={pending || credentialSource === 'Legacy'} onClick={() => testMutation.mutate()}>
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

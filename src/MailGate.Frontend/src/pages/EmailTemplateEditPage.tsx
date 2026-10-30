import { useEffect, useState } from 'react';
import { Alert, App, Button, Card, Col, Input, Row, Space, Spin, Typography } from 'antd';
import { SaveOutlined } from '@ant-design/icons';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { useNavigate, useParams } from 'react-router-dom';
import { emailTemplatesApi } from '../api/endpoints';
import type { EmailBlockType, EmailTemplateBlock, EmailTemplateDefinition, EmailTemplateDto } from '../api/types';
import { EmailTemplateEditor } from '../components/EmailTemplateEditor';
import { EmailPreview } from '../components/EmailPreview';
import { useDebounced } from '../hooks/useDebounced';

const block = (type: EmailBlockType): EmailTemplateBlock => ({ type, text: type === 'heading' ? 'Важная информация' : type === 'button' ? 'Открыть' : '', secondaryText: '', url: type === 'button' ? 'https://example.com' : '' });
function preset(columns = false): EmailTemplateDefinition {
  return { name: 'Новый шаблон', subject: 'Уведомление', preheader: '', accentColor: '#1677ff', backgroundColor: '#f3f5f7', footer: 'С уважением, команда поддержки',
    blocks: [block('heading'), block('content'), ...(columns ? [{ ...block('columns'), text: 'Контакты поддержки', secondaryText: 'Время работы: пн–пт' }] : [])] };
}

export function EmailTemplateEditPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { message } = App.useApp();
  const client = useQueryClient();
  const [definition, setDefinition] = useState<EmailTemplateDefinition>(() => preset());
  const [revision, setRevision] = useState<string>();
  const [loadedId, setLoadedId] = useState<string>();
  const [sample, setSample] = useState('Здравствуйте!\n\nЗдесь будет ваш основной текст сообщения.\nОн подставляется в макет при создании рассылки.');
  const [uploadingImage, setUploadingImage] = useState(false);
  const details = useQuery({ queryKey: ['email-templates', 'detail', id], queryFn: () => emailTemplatesApi.get(id!), enabled: !!id, refetchOnWindowFocus: false });
  useEffect(() => {
    if (details.data && loadedId !== id) { setDefinition(details.data.definition); setRevision(details.data.revision); setLoadedId(id); }
  }, [details.data, loadedId, id]);
  const previewInput = useDebounced(JSON.stringify({ definition, sample }));
  const preview = useQuery({ queryKey: ['email-template-preview', previewInput], queryFn: () => {
    const parsed = JSON.parse(previewInput) as { definition: EmailTemplateDefinition; sample: string };
    return emailTemplatesApi.preview(parsed.definition, parsed.sample);
  }, enabled: !id || loadedId === id, retry: false, gcTime: 0 });
  const save = useMutation({ mutationFn: () => id
    ? emailTemplatesApi.update(id, { definition, revision })
    : emailTemplatesApi.create({ definition }),
    onSuccess: (saved: EmailTemplateDto) => {
      message.success('Шаблон сохранён'); setRevision(saved.revision);
      client.setQueryData(['email-templates', 'detail', saved.id], saved);
      void client.invalidateQueries({ queryKey: ['email-templates', 'list'] });
      if (!id) navigate(`/templates/${saved.id}/edit`, { replace: true });
    }, onError: (error: Error) => message.error(error.message),
  });
  if (id && (details.isLoading || (!details.isError && loadedId !== id))) return <Spin />;
  if (details.isError) return <Alert type="error" title="Не удалось открыть шаблон" description={details.error.message} />;
  return <div style={{ maxWidth: 1600 }}>
    <Space wrap style={{ justifyContent: 'space-between', width: '100%', marginBottom: 20 }}>
      <div><Typography.Title level={3} style={{ margin: 0 }}>{id ? 'Конструктор шаблона' : 'Новый шаблон письма'}</Typography.Title><Typography.Text type="secondary">Соберите макет из блоков. Основной текст будет введён при отправке.</Typography.Text></div>
      <Space><Button onClick={() => navigate('/templates')}>К списку</Button><Button type="primary" icon={<SaveOutlined />} disabled={uploadingImage} loading={save.isPending} onClick={() => save.mutate()}>Сохранить шаблон</Button></Space>
    </Space>
    <Row gutter={[24, 24]}>
      <Col xs={24} xl={12}>
        <Space orientation="vertical" size="middle" style={{ width: '100%' }}>
          <EmailTemplateEditor definition={definition} setDefinition={setDefinition} onUploadingChange={setUploadingImage} />
          {!id && <Space wrap><Typography.Text type="secondary">Начать с макета:</Typography.Text><Button disabled={uploadingImage} onClick={() => setDefinition(preset())}>Одна колонка</Button><Button disabled={uploadingImage} onClick={() => setDefinition(preset(true))}>С двумя колонками</Button></Space>}
          <Card title="Текст для проверки макета"><Input.TextArea aria-label="Пример сообщения" rows={5} maxLength={100000} value={sample} onChange={(e) => setSample(e.target.value)} /><Typography.Text type="secondary">Этот текст не сохраняется в шаблоне.</Typography.Text></Card>
        </Space>
      </Col>
      <Col xs={24} xl={12}><div style={{ position: 'sticky', top: 20 }}><EmailPreview preview={preview.data} loading={preview.isFetching || previewInput !== JSON.stringify({ definition, sample })} error={preview.error} /></div></Col>
    </Row>
  </div>;
}

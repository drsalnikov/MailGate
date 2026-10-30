import { useState } from 'react';
import { Alert, Card, Empty, Segmented, Spin, Typography } from 'antd';
import type { EmailPreviewDto } from '../api/types';

interface Props { preview?: EmailPreviewDto; loading?: boolean; error?: Error | null }
const csp = `<meta http-equiv="Content-Security-Policy" content="default-src 'none'; style-src 'unsafe-inline'; img-src data: https:; base-uri 'none'; form-action 'none'">`;

export function EmailPreview({ preview, loading, error }: Props) {
  const [mobile, setMobile] = useState(false);
  return (
    <Card title="Предпросмотр письма" extra={<Segmented size="small" value={mobile ? 'Телефон' : 'Компьютер'} options={['Компьютер', 'Телефон']} onChange={(value) => setMobile(value === 'Телефон')} />}
      style={{ height: 'calc(100dvh - 40px)', display: 'flex', flexDirection: 'column' }}
      styles={{ header: { flexShrink: 0 }, body: { flex: 1, minHeight: 0, display: 'flex', flexDirection: 'column', overflow: 'auto' } }}>
      <div style={{ width: '100%', flex: 1, minHeight: 0, display: 'flex', flexDirection: 'column', gap: 16 }}>
        <Typography.Text type="secondary">Браузерный предпросмотр. Отображение в Outlook и других почтовых клиентах может отличаться. Загруженные изображения встроены в письмо.</Typography.Text>
        {error && <Alert type="error" title={error.message} />}
        {loading && <Spin description="Обновляем письмо…" />}
        {preview && !error ? <>
          <Typography.Text><strong>Тема:</strong> {preview.subject || 'Без темы'}</Typography.Text>
          <div style={{ background: '#eef0f3', padding: 8, overflow: 'auto', borderRadius: 8, flex: 1, minHeight: 120 }}>
            {/* Keep the desktop viewport wider than the email's mobile breakpoint. */}
            <iframe title="Содержимое письма" sandbox="" referrerPolicy="no-referrer" srcDoc={csp + preview.html}
              style={{ display: 'block', width: mobile ? 375 : 680, height: '100%', margin: '0 auto', border: 0, background: 'white' }} />
          </div>
        </> : !loading && !error && <Empty description="Заполните сообщение, чтобы увидеть письмо" />}
      </div>
    </Card>
  );
}

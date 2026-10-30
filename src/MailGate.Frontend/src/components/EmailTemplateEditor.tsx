import { useState, type Dispatch, type SetStateAction } from 'react';
import { App, Button, Card, Form, Input, Select, Space, Typography, Upload } from 'antd';
import { ArrowDownOutlined, ArrowUpOutlined, CopyOutlined, DeleteOutlined, PlusOutlined } from '@ant-design/icons';
import type { EmailBlockType, EmailTemplateBlock, EmailTemplateDefinition } from '../api/types';

const labels: Record<EmailBlockType, string> = { heading: 'Заголовок', text: 'Текст', content: 'Основное сообщение', button: 'Кнопка', divider: 'Разделитель', columns: 'Две колонки', image: 'Изображение' };
const block = (type: EmailBlockType): EmailTemplateBlock => ({ type, text: type === 'heading' ? 'Важная информация' : type === 'button' ? 'Открыть' : '', secondaryText: '', url: type === 'button' ? 'https://example.com' : '' });

interface Props { definition: EmailTemplateDefinition; setDefinition: Dispatch<SetStateAction<EmailTemplateDefinition>>; onUploadingChange?: (uploading: boolean) => void; forMessage?: boolean }
export function EmailTemplateEditor({ definition, setDefinition, onUploadingChange, forMessage = false }: Props) {
  const { message } = App.useApp();
  const [uploadingImage, setUploadingImageState] = useState(false);
  const setUploadingImage = (value: boolean) => { setUploadingImageState(value); onUploadingChange?.(value); };
  const [newType, setNewType] = useState<EmailBlockType>('text');
  const set = <K extends keyof EmailTemplateDefinition>(key: K, value: EmailTemplateDefinition[K]) => setDefinition((current) => ({ ...current, [key]: value }));
  const changeBlock = (index: number, patch: Partial<EmailTemplateBlock>) => setDefinition((current) => ({ ...current, blocks: current.blocks.map((value, i) => i === index ? { ...value, ...patch } : value) }));
  const move = (index: number, offset: number) => setDefinition((current) => {
    const blocks = [...current.blocks]; [blocks[index], blocks[index + offset]] = [blocks[index + offset], blocks[index]]; return { ...current, blocks };
  });
  return <fieldset disabled={uploadingImage} style={{ border: 0, padding: 0, margin: 0, minWidth: 0 }}><Space orientation="vertical" size="middle" style={{ width: '100%' }}>
          <Card title="Оформление">
            <div>
              {!forMessage && <Form.Item label="Название шаблона" required><Input value={definition.name} maxLength={200} onChange={(e) => set('name', e.target.value)} /></Form.Item>}
              <Form.Item label="Тема по умолчанию"><Input value={definition.subject} maxLength={500} onChange={(e) => set('subject', e.target.value)} placeholder="Можно изменить перед отправкой" /></Form.Item>
              <Form.Item label="Прехедер" tooltip="Короткая строка рядом с темой в почтовом ящике"><Input value={definition.preheader} maxLength={500} onChange={(e) => set('preheader', e.target.value)} /></Form.Item>
              <Space wrap size="large">
                <Form.Item label="Акцент"><Input type="color" aria-label="Цвет акцента" value={definition.accentColor} onChange={(e) => set('accentColor', e.target.value)} style={{ width: 90 }} /></Form.Item>
                <Form.Item label="Фон письма"><Input type="color" aria-label="Цвет фона" value={definition.backgroundColor} onChange={(e) => set('backgroundColor', e.target.value)} style={{ width: 90 }} /></Form.Item>
              </Space>
              <Form.Item label="Подпись / подвал"><Input.TextArea rows={2} maxLength={4000} value={definition.footer} onChange={(e) => set('footer', e.target.value)} /></Form.Item>
            </div>
          </Card>
          <Card title="Блоки письма">
            <Space orientation="vertical" style={{ width: '100%' }} size="middle">
              {definition.blocks.map((item, index) => <Card size="small" key={index} title={`${index + 1}. ${labels[item.type]}`} extra={<Space size={0}>
                <Button type="text" aria-label={`Блок ${index + 1}: выше`} icon={<ArrowUpOutlined />} disabled={index === 0} onClick={() => move(index, -1)} />
                <Button type="text" aria-label={`Блок ${index + 1}: ниже`} icon={<ArrowDownOutlined />} disabled={index === definition.blocks.length - 1} onClick={() => move(index, 1)} />
                {item.type !== 'content' && <Button type="text" aria-label={`Дублировать блок ${index + 1}`} icon={<CopyOutlined />} disabled={definition.blocks.length >= 40} onClick={() => set('blocks', [...definition.blocks.slice(0, index + 1), { ...item }, ...definition.blocks.slice(index + 1)])} />}
                <Button type="text" danger aria-label={`Удалить блок ${index + 1}`} icon={<DeleteOutlined />} disabled={item.type === 'content'} onClick={() => set('blocks', definition.blocks.filter((_, i) => i !== index))} />
              </Space>}>
                {item.type === 'content' ? <Typography.Text type="secondary">В это место автоматически подставится основной текст рассылки. Можно переместить блок выше или ниже.</Typography.Text>
                  : item.type === 'image' ? <Space orientation="vertical" style={{ width: '100%' }}>
                    <Upload accept="image/png,image/jpeg,image/gif" showUploadList={false} disabled={uploadingImage} beforeUpload={async (file) => {
                      if (file.size > 128 * 1024 || !['image/png', 'image/jpeg', 'image/gif'].includes(file.type)) {
                        message.error('Выберите PNG, JPEG или GIF размером до 128 КБ'); return false;
                      }
                      setUploadingImage(true);
                      try {
                        const data = await new Promise<string>((resolve, reject) => {
                          const reader = new FileReader();
                          reader.onload = () => resolve(String(reader.result));
                          reader.onerror = () => reject(new Error('Не удалось прочитать файл'));
                          reader.readAsDataURL(file);
                        });
                        changeBlock(index, { url: data });
                      } catch { message.error('Не удалось прочитать изображение'); }
                      finally { setUploadingImage(false); }
                      return false;
                    }}><Button loading={uploadingImage}>{item.url ? 'Заменить изображение' : 'Загрузить с компьютера'}</Button></Upload>
                    {item.url && <Typography.Text type="secondary">{item.url.startsWith('data:') ? (forMessage ? 'Изображение добавлено в это письмо.' : 'Изображение загружено. Сохраните шаблон.') : 'Ранее сохранённое изображение по ссылке. Можно заменить файлом.'}</Typography.Text>}
                    <Input aria-label={`Описание изображения блока ${index + 1}`} maxLength={1000} value={item.text} onChange={(e) => changeBlock(index, { text: e.target.value })} placeholder="Описание, если почтовый клиент не показывает картинку (необязательно)" />
                    <Typography.Text type="secondary">PNG, JPEG, GIF: до 128 КБ на файл, до 512 КБ на шаблон. Переместите блок вверх для шапки или вниз для подвала.</Typography.Text>
                  </Space>
                  : item.type === 'divider' ? <Typography.Text type="secondary">Горизонтальная линия между разделами.</Typography.Text>
                  : <Space orientation="vertical" style={{ width: '100%' }}>
                    <Input.TextArea aria-label={`Текст блока ${index + 1}`} rows={item.type === 'heading' || item.type === 'button' ? 2 : 4} maxLength={10000} value={item.text} onChange={(e) => changeBlock(index, { text: e.target.value })} placeholder={item.type === 'button' ? 'Текст кнопки' : item.type === 'columns' ? 'Левая колонка' : 'Текст блока'} />
                    {item.type === 'columns' && <Input.TextArea aria-label={`Правая колонка блока ${index + 1}`} rows={4} maxLength={10000} value={item.secondaryText} onChange={(e) => changeBlock(index, { secondaryText: e.target.value })} placeholder="Правая колонка" />}
                    {item.type === 'button' && <Input aria-label={`Ссылка блока ${index + 1}`} maxLength={2000} value={item.url} onChange={(e) => changeBlock(index, { url: e.target.value })} placeholder="https://…" />}
                  </Space>}
              </Card>)}
              <Space wrap><Select aria-label="Тип нового блока" value={newType} onChange={setNewType} style={{ width: 200 }} options={Object.entries(labels).filter(([type]) => type !== 'content').map(([value, label]) => ({ value, label }))} /><Button icon={<PlusOutlined />} disabled={definition.blocks.length >= 40} onClick={() => set('blocks', [...definition.blocks, block(newType)])}>Добавить блок</Button></Space>
            </Space>
          </Card>
    </Space></fieldset>;
}

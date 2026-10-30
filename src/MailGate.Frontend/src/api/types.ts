// Типы соответствуют DTO из MailGate.Contracts (enum'ы сериализуются строками).

export type ChannelType = 'Smtp' | 'Telegram' | 'Slack' | 'Teams' | 'Sms' | 'Webhook';
export type DeliveryStatus = 'Sent' | 'Failed' | 'Skipped';
export type BodyFormat = 'PlainText' | 'Html' | 'Markdown';
export type NotificationPriority = 'Low' | 'Normal' | 'High' | 'Critical';
export type SmtpConfigurationSource = 'AppSettings' | 'Database';

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface NotificationLogDto {
  id: string;
  messageId: string;
  idempotencyKey: string | null;
  channel: ChannelType;
  recipients: string[];
  subject: string | null;
  status: DeliveryStatus;
  errorMessage: string | null;
  attemptCount: number;
  createdAt: string;
  processedAt: string | null;
  retryOfMessageId: string | null;
}

export interface NotificationLogDetailsDto extends NotificationLogDto {
  body: string | null;
  bodyFormat: BodyFormat;
  priority: NotificationPriority;
}

export interface SendNotificationFromAdminDto {
  templateId?: string | null;
  templateRevision?: string | null;
  templateDefinition?: EmailTemplateDefinition | null;
  channel: ChannelType;
  recipientIds: string[];
  recipientGroupIds: string[];
  adHocEmails: string[];
  subject: string | null;
  body: string;
  bodyFormat: BodyFormat;
  priority: NotificationPriority;
}

export interface AcceptedNotificationDto {
  messageId: string;
}

export interface RetryNotificationResponseDto {
  newMessageId: string;
  originalMessageId: string;
}

export interface NotificationStatsDto {
  sentLast24h: number;
  failedLast24h: number;
  skippedLast24h: number;
  totalRecipients: number;
}

export interface RecipientSystemDto {
  id: string;
  code: string;
  name: string;
}

export interface RecipientGroupSummaryDto {
  id: string;
  name: string;
}

export interface RecipientDto {
  id: string;
  fullName: string;
  email: string;
  systems: RecipientSystemDto[];
  groups: RecipientGroupSummaryDto[];
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateRecipientDto {
  fullName: string;
  email: string;
  systemIds: string[];
}

export type UpdateRecipientDto = CreateRecipientDto;

export interface GroupRecipientDto {
  id: string;
  fullName: string;
  email: string;
}

export interface RecipientGroupDto {
  id: string;
  name: string;
  description: string | null;
  recipients: GroupRecipientDto[];
  recipientCount: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateRecipientGroupDto {
  name: string;
  description: string | null;
  recipientIds: string[];
}

export type UpdateRecipientGroupDto = CreateRecipientGroupDto;

export interface SystemDto {
  id: string;
  code: string;
  name: string;
  description: string | null;
  subscriberCount: number;
  createdAt: string;
  updatedAt: string | null;
}

export interface CreateSystemDto {
  code: string;
  name: string;
  description: string | null;
}

export type UpdateSystemDto = CreateSystemDto;

export interface SmtpOAuth2Dto {
  tenantId: string;
  clientId: string;
  hasClientSecret: boolean;
  smtpUserPrincipalName: string;
}

export interface SmtpConfigurationDto {
  host: string;
  port: number;
  useSsl: boolean;
  useTls: boolean;
  checkCertificateRevocation: boolean;
  fromAddress: string;
  fromName: string;
  username: string | null;
  hasPassword: boolean;
  oAuth2: SmtpOAuth2Dto | null;
  source: SmtpConfigurationSource;
  updatedAt: string | null;
}

export interface UpdateSmtpOAuth2Dto {
  tenantId: string;
  clientId: string;
  clientSecret: string | null;
  smtpUserPrincipalName: string;
}

export interface UpdateSmtpConfigurationDto {
  host: string;
  port: number;
  useSsl: boolean;
  useTls: boolean;
  checkCertificateRevocation: boolean;
  fromAddress: string;
  fromName: string;
  username: string | null;
  password: string | null;
  oAuth2: UpdateSmtpOAuth2Dto | null;
}

export interface SmtpTestResultDto {
  success: boolean;
  errorMessage: string | null;
  durationMs: number;
}

export type EmailBlockType = 'heading' | 'text' | 'content' | 'button' | 'divider' | 'columns' | 'image';
export interface EmailTemplateBlock { type: EmailBlockType; text: string; secondaryText: string; url: string }
export interface EmailTemplateDefinition {
  name: string;
  subject: string;
  preheader: string;
  accentColor: string;
  backgroundColor: string;
  footer: string;
  blocks: EmailTemplateBlock[];
}
export interface EmailTemplateDto {
  id: string;
  definition: EmailTemplateDefinition;
  revision: string;
  createdAt: string;
  updatedAt: string;
}
export interface EmailTemplateSummaryDto { id: string; name: string; subject: string; revision: string; updatedAt: string }
export interface SaveEmailTemplateDto { definition: EmailTemplateDefinition; revision?: string }
export interface EmailPreviewDto { subject: string; html: string }
export interface PreviewNotificationDto {
  templateId?: string | null;
  templateRevision?: string | null;
  templateDefinition?: EmailTemplateDefinition | null;
  subject?: string | null;
  body: string;
  bodyFormat: BodyFormat;
}

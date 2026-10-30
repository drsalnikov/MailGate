import { api, toQuery } from './client';
import type {
  EmailTemplateDto, EmailTemplateSummaryDto, SaveEmailTemplateDto, EmailPreviewDto, PreviewNotificationDto, EmailTemplateDefinition,
  AcceptedNotificationDto,
  CreateRecipientDto,
  CreateRecipientGroupDto,
  CreateSystemDto,
  DeliveryStatus,
  NotificationLogDetailsDto,
  NotificationLogDto,
  NotificationStatsDto,
  PagedResult,
  RecipientDto,
  RecipientGroupDto,
  RetryNotificationResponseDto,
  SendNotificationFromAdminDto,
  SmtpConfigurationDto,
  SmtpTestResultDto,
  SystemDto,
  UpdateRecipientDto,
  UpdateRecipientGroupDto,
  UpdateSmtpConfigurationDto,
  UpdateSystemDto,
} from './types';

export interface PageParams {
  search?: string;
  page?: number;
  pageSize?: number;
}

export const notificationsApi = {
  preview: (dto: PreviewNotificationDto) => api.post<EmailPreviewDto>('/api/admin/notifications/preview', dto),
  list: (params: PageParams & { status?: DeliveryStatus }) =>
    api.get<PagedResult<NotificationLogDto>>(`/api/admin/notifications${toQuery(params)}`),
  stats: () => api.get<NotificationStatsDto>('/api/admin/notifications/stats'),
  get: (id: string) => api.get<NotificationLogDetailsDto>(`/api/admin/notifications/${id}`),
  send: (dto: SendNotificationFromAdminDto) =>
    api.post<AcceptedNotificationDto>('/api/admin/notifications', dto),
  retry: (id: string) =>
    api.post<RetryNotificationResponseDto>(`/api/admin/notifications/${id}/retry`),
};

export const recipientsApi = {
  list: (params: PageParams) =>
    api.get<PagedResult<RecipientDto>>(`/api/admin/recipients${toQuery(params)}`),
  get: (id: string) => api.get<RecipientDto>(`/api/admin/recipients/${id}`),
  create: (dto: CreateRecipientDto) => api.post<RecipientDto>('/api/admin/recipients', dto),
  update: (id: string, dto: UpdateRecipientDto) =>
    api.put<RecipientDto>(`/api/admin/recipients/${id}`, dto),
  remove: (id: string) => api.delete(`/api/admin/recipients/${id}`),
};

export const recipientGroupsApi = {
  list: (params: PageParams) =>
    api.get<PagedResult<RecipientGroupDto>>(`/api/admin/recipient-groups${toQuery(params)}`),
  get: (id: string) => api.get<RecipientGroupDto>(`/api/admin/recipient-groups/${id}`),
  create: (dto: CreateRecipientGroupDto) =>
    api.post<RecipientGroupDto>('/api/admin/recipient-groups', dto),
  update: (id: string, dto: UpdateRecipientGroupDto) =>
    api.put<RecipientGroupDto>(`/api/admin/recipient-groups/${id}`, dto),
  remove: (id: string) => api.delete(`/api/admin/recipient-groups/${id}`),
};

export const systemsApi = {
  list: (params: PageParams) =>
    api.get<PagedResult<SystemDto>>(`/api/admin/systems${toQuery(params)}`),
  get: (id: string) => api.get<SystemDto>(`/api/admin/systems/${id}`),
  create: (dto: CreateSystemDto) => api.post<SystemDto>('/api/admin/systems', dto),
  update: (id: string, dto: UpdateSystemDto) =>
    api.put<SystemDto>(`/api/admin/systems/${id}`, dto),
  remove: (id: string) => api.delete(`/api/admin/systems/${id}`),
};

export const smtpApi = {
  effective: () => api.get<SmtpConfigurationDto>('/api/admin/smtp/effective'),
  getOverride: () => api.get<SmtpConfigurationDto>('/api/admin/smtp/override'),
  putOverride: (dto: UpdateSmtpConfigurationDto) =>
    api.put<SmtpConfigurationDto>('/api/admin/smtp/override', dto),
  deleteOverride: () => api.delete('/api/admin/smtp/override'),
  testConnection: (dto: UpdateSmtpConfigurationDto) =>
    api.post<SmtpTestResultDto>('/api/admin/smtp/test-connection', dto),
};

export const emailTemplatesApi = {
  list: (params: PageParams) => api.get<PagedResult<EmailTemplateSummaryDto>>(`/api/admin/email-templates${toQuery(params)}`),
  get: (id: string) => api.get<EmailTemplateDto>(`/api/admin/email-templates/${id}`),
  create: (dto: SaveEmailTemplateDto) => api.post<EmailTemplateDto>('/api/admin/email-templates', dto),
  update: (id: string, dto: SaveEmailTemplateDto) => api.put<EmailTemplateDto>(`/api/admin/email-templates/${id}`, dto),
  remove: (id: string) => api.delete(`/api/admin/email-templates/${id}`),
  preview: (definition: EmailTemplateDefinition, body: string) => api.post<EmailPreviewDto>('/api/admin/email-templates/preview', { definition, body }),
};

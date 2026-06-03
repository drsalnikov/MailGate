using System.Net;
using System.Net.Http.Json;
using MailGate.Contracts.Common;
using MailGate.Contracts.Notifications;
using MailGate.Core.Enums;

namespace MailGate.Admin.Services;

public sealed class NotificationsClient(HttpClient http)
{
    private const string BasePath = "api/admin/notifications";

    public async Task<PagedResult<NotificationLogDto>> ListAsync(
        DeliveryStatus? status, string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var url = $"{BasePath}?page={page}&pageSize={pageSize}";
        if (status.HasValue) url += $"&status={status.Value}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        return await http.GetFromJsonAsync<PagedResult<NotificationLogDto>>(url, AdminJsonOptions.Default, ct)
            ?? new PagedResult<NotificationLogDto>([], 0, page, pageSize);
    }

    public async Task<NotificationStatsDto> GetStatsAsync(CancellationToken ct = default)
        => (await http.GetFromJsonAsync<NotificationStatsDto>($"{BasePath}/stats", AdminJsonOptions.Default, ct))!;

    public async Task<NotificationLogDetailsDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{BasePath}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<NotificationLogDetailsDto>(AdminJsonOptions.Default, ct);
    }

    public async Task<Guid> SendAsync(SendNotificationFromAdminDto dto, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(BasePath, dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        var accepted = await response.Content.ReadFromJsonAsync<AcceptedNotificationResponse>(AdminJsonOptions.Default, ct);
        return accepted?.MessageId ?? Guid.Empty;
    }

    public async Task<RetryNotificationResponseDto?> RetryAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.PostAsync($"{BasePath}/{id}/retry", null, ct);
        if (!response.IsSuccessStatusCode) return null;
        return await response.Content.ReadFromJsonAsync<RetryNotificationResponseDto>(AdminJsonOptions.Default, ct);
    }

    private sealed record AcceptedNotificationResponse(Guid MessageId);
}

using System.Net;
using System.Net.Http.Json;
using MailGate.Contracts.Common;
using MailGate.Contracts.Recipients;

namespace MailGate.Admin.Services;

public sealed class RecipientsClient(HttpClient http)
{
    private const string BasePath = "api/admin/recipients";

    public async Task<PagedResult<RecipientDto>> ListAsync(
        string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var url = $"{BasePath}?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        return await http.GetFromJsonAsync<PagedResult<RecipientDto>>(url, AdminJsonOptions.Default, ct)
            ?? new PagedResult<RecipientDto>([], 0, page, pageSize);
    }

    public async Task<RecipientDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{BasePath}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RecipientDto>(AdminJsonOptions.Default, ct);
    }

    public async Task<RecipientDto> CreateAsync(CreateRecipientDto dto, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(BasePath, dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RecipientDto>(AdminJsonOptions.Default, ct))!;
    }

    public async Task<RecipientDto> UpdateAsync(Guid id, UpdateRecipientDto dto, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{BasePath}/{id}", dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RecipientDto>(AdminJsonOptions.Default, ct))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"{BasePath}/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}

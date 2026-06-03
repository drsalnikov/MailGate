using System.Net;
using System.Net.Http.Json;
using MailGate.Contracts.Common;
using MailGate.Contracts.RecipientGroups;

namespace MailGate.Admin.Services;

public sealed class RecipientGroupsClient(HttpClient http)
{
    private const string BasePath = "api/admin/recipient-groups";

    public async Task<PagedResult<RecipientGroupDto>> ListAsync(
        string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var url = $"{BasePath}?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        return await http.GetFromJsonAsync<PagedResult<RecipientGroupDto>>(url, AdminJsonOptions.Default, ct)
            ?? new PagedResult<RecipientGroupDto>([], 0, page, pageSize);
    }

    public async Task<RecipientGroupDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{BasePath}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RecipientGroupDto>(AdminJsonOptions.Default, ct);
    }

    public async Task<RecipientGroupDto> CreateAsync(CreateRecipientGroupDto dto, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(BasePath, dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RecipientGroupDto>(AdminJsonOptions.Default, ct))!;
    }

    public async Task<RecipientGroupDto> UpdateAsync(Guid id, UpdateRecipientGroupDto dto, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{BasePath}/{id}", dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<RecipientGroupDto>(AdminJsonOptions.Default, ct))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"{BasePath}/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}

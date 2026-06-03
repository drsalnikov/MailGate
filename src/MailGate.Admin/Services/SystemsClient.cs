using System.Net;
using System.Net.Http.Json;
using MailGate.Contracts.Common;
using MailGate.Contracts.Systems;

namespace MailGate.Admin.Services;

public sealed class SystemsClient(HttpClient http)
{
    private const string BasePath = "api/admin/systems";

    public async Task<PagedResult<SystemDto>> ListAsync(
        string? search, int page, int pageSize, CancellationToken ct = default)
    {
        var url = $"{BasePath}?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrWhiteSpace(search))
            url += $"&search={Uri.EscapeDataString(search)}";

        return await http.GetFromJsonAsync<PagedResult<SystemDto>>(url, AdminJsonOptions.Default, ct)
            ?? new PagedResult<SystemDto>([], 0, page, pageSize);
    }

    public async Task<SystemDto?> GetAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{BasePath}/{id}", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SystemDto>(AdminJsonOptions.Default, ct);
    }

    public async Task<SystemDto> CreateAsync(CreateSystemDto dto, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync(BasePath, dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SystemDto>(AdminJsonOptions.Default, ct))!;
    }

    public async Task<SystemDto> UpdateAsync(Guid id, UpdateSystemDto dto, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{BasePath}/{id}", dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SystemDto>(AdminJsonOptions.Default, ct))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"{BasePath}/{id}", ct);
        response.EnsureSuccessStatusCode();
    }
}

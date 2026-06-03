using System.Net;
using System.Net.Http.Json;
using MailGate.Contracts.Smtp;

namespace MailGate.Admin.Services;

public sealed class SmtpAdminClient(HttpClient http)
{
    private const string BasePath = "api/admin/smtp";

    public async Task<SmtpConfigurationDto> GetEffectiveAsync(CancellationToken ct = default)
        => (await http.GetFromJsonAsync<SmtpConfigurationDto>($"{BasePath}/effective", AdminJsonOptions.Default, ct))!;

    public async Task<SmtpConfigurationDto?> GetOverrideAsync(CancellationToken ct = default)
    {
        var response = await http.GetAsync($"{BasePath}/override", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return null;
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<SmtpConfigurationDto>(AdminJsonOptions.Default, ct);
    }

    public async Task<SmtpConfigurationDto> UpdateOverrideAsync(UpdateSmtpConfigurationDto dto, CancellationToken ct = default)
    {
        var response = await http.PutAsJsonAsync($"{BasePath}/override", dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SmtpConfigurationDto>(AdminJsonOptions.Default, ct))!;
    }

    public async Task DeleteOverrideAsync(CancellationToken ct = default)
    {
        var response = await http.DeleteAsync($"{BasePath}/override", ct);
        if (response.StatusCode == HttpStatusCode.NotFound) return;
        response.EnsureSuccessStatusCode();
    }

    public async Task<SmtpTestResultDto> TestAsync(UpdateSmtpConfigurationDto dto, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync($"{BasePath}/test-connection", dto, AdminJsonOptions.Default, ct);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<SmtpTestResultDto>(AdminJsonOptions.Default, ct))!;
    }
}

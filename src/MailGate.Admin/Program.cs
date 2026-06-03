using MailGate.Admin;
using MailGate.Admin.Auth;
using MailGate.Admin.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("MailGate.Api", client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    })
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("MailGate.Api"));

builder.Services.AddMudServices();

builder.Services.AddScoped<RecipientsClient>();
builder.Services.AddScoped<RecipientGroupsClient>();
builder.Services.AddScoped<SystemsClient>();
builder.Services.AddScoped<NotificationsClient>();
builder.Services.AddScoped<SmtpAdminClient>();

builder.Services.AddOidcAuthentication(options =>
{
    var oidc = builder.Configuration.GetSection("Oidc");
    options.ProviderOptions.Authority = oidc["Authority"] ?? "http://keycloak.localhost:8180/realms/mailgate";
    options.ProviderOptions.ClientId = oidc["ClientId"] ?? "mailgate-admin";
    options.ProviderOptions.ResponseType = oidc["ResponseType"] ?? "code";
    options.ProviderOptions.DefaultScopes.Clear();
    options.UserOptions.RoleClaim = "roles";

    foreach (var scope in oidc.GetSection("DefaultScopes").Get<string[]>() ?? [])
        options.ProviderOptions.DefaultScopes.Add(scope);
})
.AddAccountClaimsPrincipalFactory<KeycloakAccountClaimsPrincipalFactory>();

await builder.Build().RunAsync();

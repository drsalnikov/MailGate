using System.Security.Claims;
using System.Text.Json.Serialization;
using FluentValidation;
using MailGate.Api.Admin.Endpoints;
using MailGate.Api.Admin.Validators;
using MailGate.Api.Auth;
using MailGate.Api.Dtos;
using MailGate.Api.Endpoints;
using MailGate.Api.Validators;
using MailGate.Channels.Smtp.Extensions;
using MailGate.Contracts.Notifications;
using MailGate.Contracts.RecipientGroups;
using MailGate.Contracts.Recipients;
using MailGate.Contracts.Smtp;
using MailGate.Contracts.Systems;
using MailGate.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using RabbitMQ.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
const string adminPolicy = "MailGateAdmin";
const string bearerSecurityScheme = "Bearer";
const string keycloakOAuthSecurityScheme = "Keycloak OAuth2";

var keycloak = builder.Configuration.GetSection("Authentication:Keycloak");
var keycloakAuthority = keycloak["Authority"] ?? "http://keycloak.localhost:8180/realms/mailgate";
var keycloakAudience = keycloak["Audience"] ?? "mailgate-api";
var keycloakAdminRole = keycloak["AdminRole"] ?? "mailgate-admin";
var keycloakSwaggerClientId = keycloak["SwaggerClientId"] ?? "mailgate-admin";

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new()
        {
            Title = "MailGate API",
            Version = "v1",
            Description = "REST API для отправки уведомлений через различные каналы связи (SMTP, мессенджеры и др.)"
        };
        return Task.CompletedTask;
    });

    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[bearerSecurityScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Ручной режим: вставьте access_token из Keycloak. Формат: Bearer <token> или только <token>."
        };
        document.Components.SecuritySchemes[keycloakOAuthSecurityScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.OAuth2,
            Description = "Swagger получает access_token через Keycloak Authorization Code Flow + PKCE.",
            Flows = new OpenApiOAuthFlows
            {
                AuthorizationCode = new OpenApiOAuthFlow
                {
                    AuthorizationUrl = new Uri($"{keycloakAuthority.TrimEnd('/')}/protocol/openid-connect/auth"),
                    TokenUrl = new Uri($"{keycloakAuthority.TrimEnd('/')}/protocol/openid-connect/token"),
                    Scopes = new Dictionary<string, string>
                    {
                        ["openid"] = "OpenID Connect"
                    }
                }
            }
        };

        return Task.CompletedTask;
    });

    options.AddOperationTransformer((operation, context, _) =>
    {
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;
        var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any();
        var allowsAnonymous = metadata.OfType<IAllowAnonymous>().Any();

        if (!requiresAuthorization || allowsAnonymous)
            return Task.CompletedTask;

        operation.Security ??= [];
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(bearerSecurityScheme, context.Document)] = []
        });
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(keycloakOAuthSecurityScheme, context.Document)] = []
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddScoped<IValidator<NotificationRequestDto>, NotificationRequestValidator>();
builder.Services.AddScoped<IValidator<CreateRecipientDto>, CreateRecipientValidator>();
builder.Services.AddScoped<IValidator<UpdateRecipientDto>, UpdateRecipientValidator>();
builder.Services.AddScoped<IValidator<CreateRecipientGroupDto>, CreateRecipientGroupValidator>();
builder.Services.AddScoped<IValidator<UpdateRecipientGroupDto>, UpdateRecipientGroupValidator>();
builder.Services.AddScoped<IValidator<CreateSystemDto>, CreateSystemValidator>();
builder.Services.AddScoped<IValidator<UpdateSystemDto>, UpdateSystemValidator>();
builder.Services.AddScoped<IValidator<SendNotificationFromAdminDto>, SendNotificationFromAdminValidator>();
builder.Services.AddScoped<IValidator<UpdateSmtpConfigurationDto>, UpdateSmtpConfigurationValidator>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakAuthority;
        options.Audience = keycloakAudience;
        options.RequireHttpsMetadata = keycloak.GetValue("RequireHttpsMetadata", false);
        options.MapInboundClaims = false;

        var metadataAddress = keycloak["MetadataAddress"];
        if (!string.IsNullOrWhiteSpace(metadataAddress))
            options.MetadataAddress = metadataAddress;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = "preferred_username",
            RoleClaimType = ClaimTypes.Role,
            ValidateAudience = !string.IsNullOrWhiteSpace(keycloakAudience),
            ValidAudience = keycloakAudience,
            ValidIssuer = keycloakAuthority
        };

        // IClaimsTransformation is not invoked automatically for JwtBearer without ASP.NET Core Identity.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var transform = context.HttpContext.RequestServices.GetRequiredService<IClaimsTransformation>();
                context.Principal = await transform.TransformAsync(context.Principal!);
            }
        };
    });

builder.Services.AddTransient<IClaimsTransformation, KeycloakRoleClaimsTransformation>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(adminPolicy, policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole(keycloakAdminRole);
    });
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSmtpChannel(builder.Configuration);
builder.Services.AddDbSmtpSettingsProvider();

var rmqHost = builder.Configuration["MassTransit:Host"] ?? "localhost";
var rmqPort = int.TryParse(builder.Configuration["MassTransit:Port"], out var p) ? p : 5672;
var rmqVhost = builder.Configuration["MassTransit:VirtualHost"] ?? "/";
var rmqUser = builder.Configuration["MassTransit:Username"] ?? "mailgate";
var rmqPass = builder.Configuration["MassTransit:Password"] ?? "mailgate";

builder.Services.AddHealthChecks()
    .AddRabbitMQ(
        factory: sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = rmqHost,
                Port = rmqPort,
                VirtualHost = rmqVhost,
                UserName = rmqUser,
                Password = rmqPass
            };
            return factory.CreateConnectionAsync();
        },
        name: "rabbitmq",
        tags: ["ready"]);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference(options =>
    {
        options.Title = "MailGate API";
        options.Theme = ScalarTheme.Purple;
    });

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "MailGate API v1");
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "MailGate API";
        options.OAuthClientId(keycloakSwaggerClientId);
        options.OAuthAppName("MailGate Swagger");
        options.OAuthUsePkce();
    });
}

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapNotificationEndpoints();
app.MapRecipientsAdminEndpoints();
app.MapRecipientGroupsAdminEndpoints();
app.MapSystemsAdminEndpoints();
app.MapNotificationsAdminEndpoints();
app.MapSmtpAdminEndpoints();

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/healthz/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapFallbackToFile("index.html");

app.Run();

public partial class Program;

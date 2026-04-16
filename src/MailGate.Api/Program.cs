using System.Text.Json.Serialization;
using FluentValidation;
using MailGate.Api.Dtos;
using MailGate.Api.Endpoints;
using MailGate.Api.Validators;
using MailGate.Infrastructure.Extensions;
using RabbitMQ.Client;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(o =>
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddOpenApi();

builder.Services.AddScoped<IValidator<NotificationRequestDto>, NotificationRequestValidator>();

builder.Services.AddInfrastructure(builder.Configuration);

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
}

app.UseHttpsRedirection();

app.MapNotificationEndpoints();

app.MapHealthChecks("/healthz");
app.MapHealthChecks("/healthz/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.Run();

public partial class Program;

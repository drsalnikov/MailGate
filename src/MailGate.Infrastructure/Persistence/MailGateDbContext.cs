using MassTransit;
using MailGate.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Infrastructure.Persistence;

public sealed class MailGateDbContext(DbContextOptions<MailGateDbContext> options) : DbContext(options)
{
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NotificationLog>(e =>
        {
            e.ToTable("notification_logs");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.MessageId).IsRequired();
            e.Property(x => x.Channel).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.Status).HasConversion<string>().HasMaxLength(50);
            e.Property(x => x.RecipientsJson).HasMaxLength(4096);
            e.Property(x => x.Subject).HasMaxLength(500);
            e.Property(x => x.ErrorMessage).HasMaxLength(2000);
            e.Property(x => x.IdempotencyKey).HasMaxLength(256);
            e.HasIndex(x => x.MessageId);
            e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter("\"IdempotencyKey\" IS NOT NULL");
        });

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}

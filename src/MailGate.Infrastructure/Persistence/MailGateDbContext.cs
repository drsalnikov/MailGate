using MassTransit;
using MailGate.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace MailGate.Infrastructure.Persistence;

public sealed class MailGateDbContext(DbContextOptions<MailGateDbContext> options) : DbContext(options)
{
    public DbSet<NotificationLog> NotificationLogs => Set<NotificationLog>();
    public DbSet<Recipient> Recipients => Set<Recipient>();
    public DbSet<RecipientGroup> RecipientGroups => Set<RecipientGroup>();
    public DbSet<NotificationSystem> Systems => Set<NotificationSystem>();
    public DbSet<SmtpConfiguration> SmtpConfigurations => Set<SmtpConfiguration>();

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
            e.Property(x => x.PayloadJson).HasColumnType("jsonb");
            e.HasIndex(x => x.MessageId);
            e.HasIndex(x => x.CreatedAt);
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.IdempotencyKey).IsUnique().HasFilter("\"IdempotencyKey\" IS NOT NULL");
        });

        modelBuilder.Entity<Recipient>(e =>
        {
            e.ToTable("recipients");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.FullName).IsRequired().HasMaxLength(200);
            e.Property(x => x.Email).IsRequired().HasMaxLength(320);
            e.HasIndex(x => x.Email).IsUnique();
        });

        modelBuilder.Entity<RecipientGroup>(e =>
        {
            e.ToTable("recipient_groups");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.HasIndex(x => x.Name).IsUnique();

            e.HasMany(x => x.Recipients)
                .WithMany(x => x.Groups)
                .UsingEntity<Dictionary<string, object>>(
                    "RecipientGroupMember",
                    right => right.HasOne<Recipient>()
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<RecipientGroup>()
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("recipient_group_members");
                        join.HasKey("GroupId", "RecipientId");
                        join.HasIndex("RecipientId");
                    });
        });

        modelBuilder.Entity<NotificationSystem>(e =>
        {
            e.ToTable("systems");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Code).IsRequired().HasMaxLength(100);
            e.Property(x => x.Name).IsRequired().HasMaxLength(200);
            e.Property(x => x.Description).HasMaxLength(1000);
            e.HasIndex(x => x.Code).IsUnique();

            e.HasMany(x => x.Recipients)
                .WithMany(x => x.Systems)
                .UsingEntity<Dictionary<string, object>>(
                    "RecipientSystem",
                    right => right.HasOne<Recipient>()
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade),
                    left => left.HasOne<NotificationSystem>()
                        .WithMany()
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade),
                    join =>
                    {
                        join.ToTable("recipient_systems");
                        join.HasKey("RecipientId", "SystemId");
                        join.HasIndex("SystemId");
                    });
        });

        modelBuilder.Entity<SmtpConfiguration>(e =>
        {
            e.ToTable("smtp_configurations");
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedNever();
            e.Property(x => x.Host).IsRequired().HasMaxLength(256);
            e.Property(x => x.FromAddress).IsRequired().HasMaxLength(320);
            e.Property(x => x.FromName).IsRequired().HasMaxLength(200);
            e.Property(x => x.Username).HasMaxLength(320);
            e.Property(x => x.EncryptedPassword).HasMaxLength(4096);
            e.Property(x => x.OAuth2TenantId).HasMaxLength(128);
            e.Property(x => x.OAuth2ClientId).HasMaxLength(128);
            e.Property(x => x.OAuth2EncryptedClientSecret).HasMaxLength(4096);
            e.Property(x => x.OAuth2SmtpUserPrincipalName).HasMaxLength(320);
        });

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
}

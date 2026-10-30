using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailGate.Infrastructure.Persistence.Migrations;

public partial class AddSmtpCertificateRevocationCheck : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "CheckCertificateRevocation",
            table: "smtp_configurations",
            type: "boolean",
            nullable: false,
            defaultValue: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CheckCertificateRevocation",
            table: "smtp_configurations");
    }
}

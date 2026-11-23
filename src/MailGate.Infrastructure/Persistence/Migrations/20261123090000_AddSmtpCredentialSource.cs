using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
namespace MailGate.Infrastructure.Persistence.Migrations;

public partial class AddSmtpCredentialSource : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Preserve existing configurations as Legacy; new UI explicitly saves Manual/Sidecar.
        migrationBuilder.AddColumn<int>(name: "CredentialSource", table: "smtp_configurations",
            type: "integer", nullable: false, defaultValue: 2);
        migrationBuilder.Sql("""ALTER TABLE smtp_configurations ALTER COLUMN "CredentialSource" DROP DEFAULT;""");
    }
    protected override void Down(MigrationBuilder migrationBuilder) =>
        migrationBuilder.DropColumn(name: "CredentialSource", table: "smtp_configurations");
}

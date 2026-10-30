using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MailGate.Infrastructure.Persistence.Migrations;

public partial class AddEmailTemplates : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "email_templates",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                DefinitionJson = table.Column<string>(type: "jsonb", nullable: false),
                Revision = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_email_templates", x => x.Id));
        migrationBuilder.CreateIndex(name: "IX_email_templates_UpdatedAt", table: "email_templates", column: "UpdatedAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder) => migrationBuilder.DropTable(name: "email_templates");
}

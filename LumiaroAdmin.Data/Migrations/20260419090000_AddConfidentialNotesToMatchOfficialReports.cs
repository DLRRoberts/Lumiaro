using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LumiaroAdmin.Data.Migrations;

public partial class AddConfidentialNotesToMatchOfficialReports : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ConfidentialNotes",
            table: "MatchOfficialReports",
            type: "nvarchar(4000)",
            maxLength: 4000,
            nullable: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ConfidentialNotes",
            table: "MatchOfficialReports");
    }
}


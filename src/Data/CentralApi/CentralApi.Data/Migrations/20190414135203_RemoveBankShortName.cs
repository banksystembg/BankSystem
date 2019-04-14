using Microsoft.EntityFrameworkCore.Migrations;

namespace CentralApi.Data.Migrations
{
    public partial class RemoveBankShortName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShortName",
                table: "Banks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShortName",
                table: "Banks",
                nullable: false,
                defaultValue: "");
        }
    }
}

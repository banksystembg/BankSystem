using Microsoft.EntityFrameworkCore.Migrations;

namespace CentralApi.Data.Migrations
{
    public partial class AddPaymentUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "Banks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "Banks");
        }
    }
}

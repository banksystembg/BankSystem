namespace CentralApi.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddCardPaymentUrlFieldInBanksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "CardPaymentUrl",
                "Banks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "CardPaymentUrl",
                "Banks");
        }
    }
}
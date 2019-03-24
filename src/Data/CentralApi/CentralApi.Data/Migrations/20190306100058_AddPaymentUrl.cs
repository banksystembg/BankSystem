namespace CentralApi.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddPaymentUrl : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "PaymentUrl",
                "Banks",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "PaymentUrl",
                "Banks");
        }
    }
}

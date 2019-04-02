namespace CentralApi.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddBankIdentificationCardNumbersFieldInBanksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                "BankIdentificationCardNumbers",
                "Banks",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "BankIdentificationCardNumbers",
                "Banks");
        }
    }
}
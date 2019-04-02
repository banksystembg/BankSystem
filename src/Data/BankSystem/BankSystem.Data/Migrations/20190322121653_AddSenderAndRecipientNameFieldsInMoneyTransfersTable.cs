namespace BankSystem.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddSenderAndRecipientNameFieldsInMoneyTransfersTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                "RecipientName",
                "Transfers",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                "SenderName",
                "Transfers",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                "RecipientName",
                "Transfers");

            migrationBuilder.DropColumn(
                "SenderName",
                "Transfers");
        }
    }
}
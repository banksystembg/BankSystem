namespace BankSystem.Data.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class CardExpirationDateString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                "ExpiryDate",
                "Cards",
                maxLength: 5,
                nullable: false,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                "ExpiryDate",
                "Cards",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 5);
        }
    }
}
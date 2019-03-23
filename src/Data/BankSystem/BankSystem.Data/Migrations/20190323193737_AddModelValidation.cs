using Microsoft.EntityFrameworkCore.Migrations;

namespace BankSystem.Data.Migrations
{
    public partial class AddModelValidation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Transfers",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SenderName",
                table: "Transfers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "RecipientName",
                table: "Transfers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "Transfers",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transfers",
                maxLength: 150,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SecurityCode",
                table: "Cards",
                maxLength: 3,
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cards",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UniqueId",
                table: "Accounts",
                maxLength: 34,
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                maxLength: 35,
                nullable: false,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Transfers",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 34);

            migrationBuilder.AlterColumn<string>(
                name: "SenderName",
                table: "Transfers",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "RecipientName",
                table: "Transfers",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Destination",
                table: "Transfers",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 34);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Transfers",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 150,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SecurityCode",
                table: "Cards",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Cards",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FullName",
                table: "AspNetUsers",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "UniqueId",
                table: "Accounts",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 34);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Accounts",
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 35);
        }
    }
}

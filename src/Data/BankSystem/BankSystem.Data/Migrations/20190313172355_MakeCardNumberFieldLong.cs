namespace BankSystem.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class MakeCardNumberFieldLong : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Number",
                table: "Cards",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Number",
                table: "Cards",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace CentralApi.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SwiftCode = table.Column<string>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    ApiKey = table.Column<string>(nullable: false),
                    ApiAddress = table.Column<string>(nullable: false),
                    PaymentUrl = table.Column<string>(nullable: true),
                    CardPaymentUrl = table.Column<string>(nullable: true),
                    BankIdentificationCardNumbers = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Banks");
        }
    }
}

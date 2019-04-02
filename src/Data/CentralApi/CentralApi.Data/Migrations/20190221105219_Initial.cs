namespace CentralApi.Data.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Banks",
                table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ShortName = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    SwiftCode = table.Column<string>(nullable: false),
                    Location = table.Column<string>(nullable: false),
                    AppId = table.Column<string>(nullable: false),
                    ApiKey = table.Column<string>(nullable: false),
                    ApiAddress = table.Column<string>(nullable: false)
                },
                constraints: table => { table.PrimaryKey("PK_Banks", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Banks");
        }
    }
}
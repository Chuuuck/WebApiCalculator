using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApiCalculator.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Calculations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 100, nullable: false),
                    Expression = table.Column<string>(maxLength: 100, nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    Result = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calculations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calculations");
        }
    }
}

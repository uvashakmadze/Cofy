using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cofy.IncomeTaxCalculator.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaxBands",
                columns: table => new
                {
                    Min = table.Column<int>(type: "int", nullable: false),
                    Max = table.Column<int>(type: "int", nullable: false),
                    Percent = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxBands", x => new { x.Min, x.Max, x.Percent });
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxBands_Min_Max_Percent",
                table: "TaxBands",
                columns: new[] { "Min", "Max", "Percent" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxBands");
        }
    }
}

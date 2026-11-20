using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amlex.Repository.Migrations
{
    /// <inheritdoc />
    public partial class EuroVocIncludeInQueryField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludeInQuery",
                table: "EuroVocCodes",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeInQuery",
                table: "EuroVocCodes");
        }
    }
}

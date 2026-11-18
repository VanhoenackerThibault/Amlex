using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amlex.Repository.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCoupleFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResourceTypes",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "LegalActs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ResourceTypes",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Sector",
                table: "LegalActs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

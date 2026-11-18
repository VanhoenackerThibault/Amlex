using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amlex.Repository.Migrations
{
    /// <inheritdoc />
    public partial class ChangedType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsWatchlist",
                table: "LegalActs");

            migrationBuilder.AlterColumn<bool>(
                name: "InForce",
                table: "LegalActs",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InForce",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsWatchlist",
                table: "LegalActs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

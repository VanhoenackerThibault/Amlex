using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amlex.Repository.Migrations
{
    /// <inheritdoc />
    public partial class CreateEuroVocCodesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "EuroVocCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EuroVocCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EuroVocUri = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SectionName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EuroVocCodes", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EuroVocCodes");

        }
    }
}

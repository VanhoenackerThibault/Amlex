using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Amlex.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Amends",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTeamId",
                table: "LegalActs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "LegalActs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndOfValidity",
                table: "LegalActs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EntryIntoForce",
                table: "LegalActs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EuroVocUris",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ProposesToAmend",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "RelevanceCategory",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RelevanceRationale",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelevanceScore",
                table: "LegalActs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Repeals",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "ResourceType",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Topics",
                table: "LegalActs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.CreateTable(
                name: "LegalActsFetchHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LustUpdateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LegalActsFetchHistory", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LegalActsFetchHistory");

            migrationBuilder.DropColumn(
                name: "Amends",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "AssignedTeamId",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "EndOfValidity",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "EntryIntoForce",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "EuroVocUris",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "ProposesToAmend",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "RelevanceCategory",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "RelevanceRationale",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "RelevanceScore",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "Repeals",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "ResourceType",
                table: "LegalActs");

            migrationBuilder.DropColumn(
                name: "Topics",
                table: "LegalActs");
        }
    }
}

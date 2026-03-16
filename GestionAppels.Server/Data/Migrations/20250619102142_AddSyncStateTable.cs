using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionAppels.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSyncStateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Statut",
                table: "Fiches",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SyncStates",
                columns: table => new
                {
                    SyncProcessName = table.Column<string>(type: "text", nullable: false),
                    LastSyncTimestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncStates", x => x.SyncProcessName);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncStates");

            migrationBuilder.DropColumn(
                name: "Statut",
                table: "Fiches");
        }
    }
}

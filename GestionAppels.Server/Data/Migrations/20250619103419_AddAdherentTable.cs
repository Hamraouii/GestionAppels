using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionAppels.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdherentTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adherents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Prenom = table.Column<string>(type: "text", nullable: false),
                    Ville = table.Column<string>(type: "text", nullable: false),
                    Sexe = table.Column<char>(type: "character(1)", nullable: false),
                    Adresse = table.Column<string>(type: "text", nullable: false),
                    Immatriculation = table.Column<string>(type: "text", nullable: false),
                    Cin = table.Column<string>(type: "text", nullable: false),
                    DateNaissance = table.Column<DateOnly>(type: "date", nullable: false),
                    Affiliation = table.Column<string>(type: "text", nullable: false),
                    StatutAdherent = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adherents", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adherents");
        }
    }
}

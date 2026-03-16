using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionAppels.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexToAdherentImmatriculationCin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Adherent_Immatriculation_Cin",
                table: "Adherents",
                columns: new[] { "Immatriculation", "Cin" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Adherent_Immatriculation_Cin",
                table: "Adherents");
        }
    }
}

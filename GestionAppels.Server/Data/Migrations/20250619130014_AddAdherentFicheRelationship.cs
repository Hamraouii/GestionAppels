using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionAppels.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdherentFicheRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Affiliation",
                table: "Fiches",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Adherents_Affiliation",
                table: "Adherents",
                column: "Affiliation");

            migrationBuilder.CreateIndex(
                name: "IX_Fiches_Affiliation",
                table: "Fiches",
                column: "Affiliation");

            migrationBuilder.AddForeignKey(
                name: "FK_Fiches_Adherents_Affiliation",
                table: "Fiches",
                column: "Affiliation",
                principalTable: "Adherents",
                principalColumn: "Affiliation",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Fiches_Adherents_Affiliation",
                table: "Fiches");

            migrationBuilder.DropIndex(
                name: "IX_Fiches_Affiliation",
                table: "Fiches");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Adherents_Affiliation",
                table: "Adherents");

            migrationBuilder.AlterColumn<string>(
                name: "Affiliation",
                table: "Fiches",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);
        }
    }
}

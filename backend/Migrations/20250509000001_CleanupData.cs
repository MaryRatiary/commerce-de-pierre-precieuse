using Microsoft.EntityFrameworkCore.Migrations;

namespace EcomApi.Migrations
{
    public partial class CleanupData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Désactiver temporairement les contraintes de clé étrangère
            migrationBuilder.Sql("SET CONSTRAINTS ALL DEFERRED;");

            // Supprimer les données de toutes les tables
            migrationBuilder.Sql("TRUNCATE TABLE \"Orders\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"Carts\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"Products\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"Categories\" CASCADE;");
            migrationBuilder.Sql("TRUNCATE TABLE \"Users\" CASCADE;");

            // Réactiver les contraintes de clé étrangère
            migrationBuilder.Sql("SET CONSTRAINTS ALL IMMEDIATE;");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // La restauration des données n'est pas possible dans ce cas
        }
    }
}
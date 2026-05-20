using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sprint3.Migrations
{
    /// <inheritdoc />
    public partial class AddUserProfilePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "FotoPerfil",
                table: "Usuarios",
                type: "longblob",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilContentType",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FotoPerfil",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "FotoPerfilContentType",
                table: "Usuarios");
        }
    }
}

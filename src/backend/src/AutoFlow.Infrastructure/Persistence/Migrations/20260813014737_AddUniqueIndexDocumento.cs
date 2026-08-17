using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutoFlow.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexDocumento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cliente_Documento",
                table: "Cliente",
                column: "Documento",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cliente_Documento",
                table: "Cliente");
        }
    }
}

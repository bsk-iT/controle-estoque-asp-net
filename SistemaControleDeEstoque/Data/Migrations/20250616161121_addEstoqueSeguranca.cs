using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaControleDeEstoque.Data.Migrations
{
    /// <inheritdoc />
    public partial class addEstoqueSeguranca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstoqueSeguranca",
                table: "Produto",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstoqueSeguranca",
                table: "Produto");
        }
    }
}

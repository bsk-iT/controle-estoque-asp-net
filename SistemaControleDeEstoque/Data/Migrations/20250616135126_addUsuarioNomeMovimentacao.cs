using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaControleDeEstoque.Data.Migrations
{
    /// <inheritdoc />
    public partial class addUsuarioNomeMovimentacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Movimentacao",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UsuarioNome",
                table: "Movimentacao",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioNome",
                table: "Movimentacao");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Movimentacao",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}

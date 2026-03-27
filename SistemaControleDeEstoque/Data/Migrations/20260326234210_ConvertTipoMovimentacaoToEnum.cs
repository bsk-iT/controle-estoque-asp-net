using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaControleDeEstoque.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConvertTipoMovimentacaoToEnum : Migration
    {
    /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.Sql("UPDATE dbo.Movimentacao SET Tipo = 'Saida' WHERE Tipo = N'Saída'");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
          migrationBuilder.Sql("UPDATE dbo.Movimentacao SET Tipo = N'Saída' WHERE Tipo = 'Saida'");
        }
    }
}

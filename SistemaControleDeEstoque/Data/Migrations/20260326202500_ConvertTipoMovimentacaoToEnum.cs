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
            // Primeiro, corrigir dados existentes: "Saída" -> "Saida" (remover acento)
            migrationBuilder.Sql(@"
                UPDATE [Movimentacao] 
                SET [Tipo] = 'Saida' 
                WHERE [Tipo] = 'Saída';
            ");

            // A coluna já é nvarchar, apenas renomeando o tipo de dado no modelo
            // EF Core vai gerenciar a validação via HasConversion<string>()
            // Não é necessário alterar a estrutura da coluna no banco
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverter a conversão dos dados: "Saida" -> "Saída"
            migrationBuilder.Sql(@"
                UPDATE [Movimentacao] 
                SET [Tipo] = 'Saída' 
                WHERE [Tipo] = 'Saida';
            ");
        }
    }
}

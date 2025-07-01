using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaControleDeEstoque.Models
{
    public class Relatorio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Tipo do Relatório")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public TipoRelatorio Tipo { get; set; }

        [Display(Name = "Data da Geração")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [DataType(DataType.Date)]
        public DateTime DataGeracao { get; set; } = DateTime.Now;

        [Display(Name = "Gerado por")]
        public string? UsuarioGerador { get; set; }

        #region Parâmetros de Filtro (Não mapeados no banco)

        [NotMapped]
        [Display(Name = "Data Início")]
        [DataType(DataType.Date)]
        public DateTime? DataInicio { get; set; }

        [NotMapped]
        [Display(Name = "Data Fim")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Relatorio), "ValidateDataFim")]
        public DateTime? DataFim { get; set; }

        [NotMapped]
        [Display(Name = "Fornecedor")]
        public int? FornecedorId { get; set; }

        [NotMapped]
        public Fornecedor? Fornecedor { get; set; }

        [NotMapped]
        [Display(Name = "Produto")]
        public int? ProdutoId { get; set; }

        [NotMapped]
        public Produto? Produto { get; set; }

        #endregion

        #region Filtros específicos para Relatório de Estoque

        [NotMapped]
        [Display(Name = "Incluir Produtos com Estoque Zero")]
        public bool IncluirProdutosZerados { get; set; } = false;

        [NotMapped]
        [Display(Name = "Ordenação")]
        public OrdenacaoRelatorio Ordenacao { get; set; } = OrdenacaoRelatorio.NomeProduto;

        [NotMapped]
        [Display(Name = "Apenas produtos abaixo do estoque mínimo")]
        public bool ApenasAbaixoDoMinimo { get; set; } = false;

        #endregion

        #region Filtros específicos para Relatório de Movimentações

        [NotMapped]
        [Display(Name = "Tipo de Movimentação")]
        public TipoMovimentacao? TipoMovimentacao { get; set; }

        [NotMapped]
        [Display(Name = "Resumido (agrupado por produto)")]
        public bool Resumido { get; set; } = false;

        #endregion

        #region Validações e lógica de negócio

        // Validação para DataFim
        public static ValidationResult? ValidateDataFim(DateTime? dataFim, ValidationContext context)
        {
            var relatorio = (Relatorio)context.ObjectInstance;
            if (relatorio.DataInicio.HasValue && dataFim.HasValue && dataFim < relatorio.DataInicio)
            {
                return new ValidationResult("A Data Fim deve ser maior ou igual à Data Início",
                                           new[] { nameof(DataFim) });
            }
            return ValidationResult.Success;
        }

        // Validações condicionais por tipo de relatório
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (Tipo == TipoRelatorio.Movimentacoes)
            {
                // Para relatórios de movimentações, a data de início é obrigatória
                if (!DataInicio.HasValue)
                {
                    results.Add(new ValidationResult(
                        "Data de início é obrigatória para relatórios de movimentações",
                        new[] { nameof(DataInicio) }));
                }
            }

            return results;
        }

        // Método para gerar o relatório conforme seu tipo
        public string GerarRelatorio()
        {
            return Tipo switch
            {
                TipoRelatorio.Estoque => GerarRelatorioEstoque(),
                TipoRelatorio.Movimentacoes => GerarRelatorioMovimentacoes(),
                _ => throw new NotImplementedException("Tipo de relatório não suportado")
            };
        }

        private string GerarRelatorioEstoque()
        {
            // Implementar lógica para gerar relatório de estoque
            // Esta é apenas uma implementação de exemplo
            return $"Relatório de Estoque gerado em {DateTime.Now}";
        }

        private string GerarRelatorioMovimentacoes()
        {
            // Implementar lógica para gerar relatório de movimentações
            // Esta é apenas uma implementação de exemplo
            string periodo = DataInicio.HasValue && DataFim.HasValue
                ? $" no período de {DataInicio.Value:dd/MM/yyyy} até {DataFim.Value:dd/MM/yyyy}"
                : "";

            return $"Relatório de Movimentações{periodo} gerado em {DateTime.Now}";
        }

        #endregion
    }

    public enum TipoRelatorio
    {
        [Display(Name = "Estoque")]
        Estoque,

        [Display(Name = "Movimentações")]
        Movimentacoes
    }

    public enum OrdenacaoRelatorio
    {
        [Display(Name = "Data Crescente")]
        DataCrescente,

        [Display(Name = "Data Decrescente")]
        DataDecrescente,

        [Display(Name = "Nome do Produto")]
        NomeProduto,

        [Display(Name = "Quantidade")]
        Quantidade,

        [Display(Name = "Valor")]
        Valor
    }

    public enum TipoMovimentacao
    {
        [Display(Name = "Todas")]
        Todas,

        [Display(Name = "Entradas")]
        Entradas,

        [Display(Name = "Saídas")]
        Saidas
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaControleDeEstoque.Models
{
    /// <summary>
    /// Representa um relatório gerado pelo sistema com diversas opções de filtragem e formatação.
    /// </summary>
    public class Relatorio
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identificador único do relatório.
        /// </summary>
        public int Id { get; set; }

        [Display(Name = "Tipo do Relatório")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        /// <summary>
        /// Define o tipo de relatório (Estoque ou Movimentações).
        /// </summary>
        public TipoRelatorio Tipo { get; set; }

        [Display(Name = "Data da Geração")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [DataType(DataType.Date)]
        /// <summary>
        /// Data e hora em que o relatório foi gerado.
        /// </summary>
        public DateTime DataGeracao { get; set; } = DateTime.Now;

        [Display(Name = "Gerado por")]
        /// <summary>
        /// Identificador do usuário que gerou o relatório.
        /// </summary>
        public string? UsuarioGerador { get; set; }

        #region Parâmetros de Filtro (Não mapeados no banco)

        [NotMapped]
        [Display(Name = "Data Início")]
        [DataType(DataType.Date)]
        /// <summary>
        /// Data inicial para filtrar os dados do relatório. Opcional para relatórios de estoque.
        /// </summary>
        public DateTime? DataInicio { get; set; }

        [NotMapped]
        [Display(Name = "Data Fim")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Relatorio), "ValidateDataFim")]
        /// <summary>
        /// Data final para filtrar os dados do relatório. Deve ser maior ou igual à Data Início.
        /// </summary>
        public DateTime? DataFim { get; set; }

        [NotMapped]
        [Display(Name = "Fornecedor")]
        /// <summary>
        /// Identificador do fornecedor para filtrar os dados do relatório.
        /// </summary>
        public int? FornecedorId { get; set; }

        [NotMapped]
        /// <summary>
        /// Referência ao fornecedor selecionado para o filtro.
        /// </summary>
        public Fornecedor? Fornecedor { get; set; }

        [NotMapped]
        [Display(Name = "Produto")]
        /// <summary>
        /// Identificador do produto para filtrar os dados do relatório.
        /// </summary>
        public int? ProdutoId { get; set; }

        [NotMapped]
        /// <summary>
        /// Referência ao produto selecionado para o filtro.
        /// </summary>
        public Produto? Produto { get; set; }

        #endregion

        #region Filtros específicos para Relatório de Estoque

        [NotMapped]
        [Display(Name = "Incluir Produtos com Estoque Zero")]
        /// <summary>
        /// Quando verdadeiro, inclui produtos com quantidade zero no relatório de estoque.
        /// </summary>
        public bool IncluirProdutosZerados { get; set; } = false;

        [NotMapped]
        [Display(Name = "Ordenação")]
        /// <summary>
        /// Define o critério de ordenação para o relatório de estoque.
        /// </summary>
        public OrdenacaoRelatorio Ordenacao { get; set; } = OrdenacaoRelatorio.NomeProduto;

        [NotMapped]
        [Display(Name = "Apenas produtos abaixo do estoque mínimo")]
        /// <summary>
        /// Quando verdadeiro, filtra apenas produtos com estoque abaixo do nível de segurança.
        /// </summary>
        public bool ApenasAbaixoDoMinimo { get; set; } = false;

        #endregion

        #region Filtros específicos para Relatório de Movimentações

        [NotMapped]
        [Display(Name = "Tipo de Movimentação")]
        /// <summary>
        /// Filtro para tipo específico de movimentação (Entradas, Saídas ou Todas).
        /// </summary>
        public TipoMovimentacao? TipoMovimentacao { get; set; }

        [NotMapped]
        [Display(Name = "Resumido (agrupado por produto)")]
        /// <summary>
        /// Quando verdadeiro, agrupa as movimentações por produto no relatório.
        /// </summary>
        public bool Resumido { get; set; } = false;

        #endregion

        #region Validações e lógica de negócio

        /// <summary>
        /// Valida se a Data Fim é posterior ou igual à Data Início.
        /// </summary>
        /// <param name="dataFim">Data final a ser validada</param>
        /// <param name="context">Contexto de validação</param>
        /// <returns>ValidationResult indicando sucesso ou falha na validação</returns>
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

        /// <summary>
        /// Executa validações adicionais com base no tipo de relatório selecionado.
        /// </summary>
        /// <param name="validationContext">Contexto de validação</param>
        /// <returns>Lista de resultados de validação</returns>
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

        /// <summary>
        /// Gera o relatório com base no tipo selecionado e parâmetros configurados.
        /// </summary>
        /// <returns>String contendo o resultado do relatório</returns>
        /// <exception cref="NotImplementedException">Lançada se o tipo de relatório não for suportado</exception>
        public string GerarRelatorio()
        {
            return Tipo switch
            {
                TipoRelatorio.Estoque => GerarRelatorioEstoque(),
                TipoRelatorio.Movimentacoes => GerarRelatorioMovimentacoes(),
                _ => throw new NotImplementedException("Tipo de relatório não suportado")
            };
        }

        /// <summary>
        /// Gera um relatório detalhado do estoque atual.
        /// </summary>
        /// <returns>String formatada com os dados do relatório</returns>
        private string GerarRelatorioEstoque()
        {
            // Implementar lógica para gerar relatório de estoque
            // Apenas uma implementação de exemplo
            return $"Relatório de Estoque gerado em {DateTime.Now}";
        }

        /// <summary>
        /// Gera um relatório de movimentações do estoque com base nos filtros configurados.
        /// </summary>
        /// <returns>String formatada com os dados do relatório</returns>
        private string GerarRelatorioMovimentacoes()
        {
            // Implementar lógica para gerar relatório de movimentações
            // Apenas uma implementação de exemplo
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
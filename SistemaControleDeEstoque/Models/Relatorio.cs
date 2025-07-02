using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaControleDeEstoque.Models
{
    /// <summary>
    /// Representa um relatório gerado pelo sistema com diversas opções de filtragem e formatação.
    /// </summary>
    public class Relatorio
    {
        /// <summary>
        /// Identificador único do relatório.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Define o tipo de relatório (Estoque ou Movimentações).
        /// </summary>
        [Display(Name = "Tipo do Relatório")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public TipoRelatorio Tipo { get; set; }

        /// <summary>
        /// Data e hora em que o relatório foi gerado.
        /// </summary>
        [Display(Name = "Data da Geração")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [DataType(DataType.Date)]
        public DateTime DataGeracao { get; set; } = DateTime.Now;

        /// <summary>
        /// Identificador do usuário que gerou o relatório.
        /// </summary>
        [Display(Name = "Gerado por")]
        public string? UsuarioGerador { get; set; }

        #region Parâmetros de Filtro (Não mapeados no banco)

        /// <summary>
        /// Data inicial para filtrar os dados do relatório. Opcional para relatórios de estoque.
        /// </summary>
        [NotMapped]
        [Display(Name = "Data Início")]
        [DataType(DataType.Date)]
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data final para filtrar os dados do relatório. Deve ser maior ou igual à Data Início.
        /// </summary>
        [NotMapped]
        [Display(Name = "Data Fim")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Relatorio), "ValidateDataFim")]
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// Identificador do fornecedor para filtrar os dados do relatório.
        /// </summary>
        [NotMapped]
        [Display(Name = "Fornecedor")]
        public int? FornecedorId { get; set; }

        /// <summary>
        /// Referência ao fornecedor selecionado para o filtro.
        /// </summary>
        [NotMapped]
        public Fornecedor? Fornecedor { get; set; }

        /// <summary>
        /// Identificador do produto para filtrar os dados do relatório.
        /// </summary>
        [NotMapped]
        [Display(Name = "Produto")]
        public int? ProdutoId { get; set; }

        /// <summary>
        /// Referência ao produto selecionado para o filtro.
        /// </summary>
        [NotMapped]
        public Produto? Produto { get; set; }

        #endregion

        #region Filtros específicos para Relatório de Estoque

        /// <summary>
        /// Quando verdadeiro, inclui produtos com quantidade zero no relatório de estoque.
        /// </summary>
        [NotMapped]
        [Display(Name = "Incluir Produtos com Estoque Zero")]
        public bool IncluirProdutosZerados { get; set; } = false;

        /// <summary>
        /// Define o critério de ordenação para o relatório de estoque.
        /// </summary>
        [NotMapped]
        [Display(Name = "Ordenação")]
        public OrdenacaoRelatorio Ordenacao { get; set; } = OrdenacaoRelatorio.NomeProduto;

        /// <summary>
        /// Quando verdadeiro, filtra apenas produtos com estoque abaixo do nível de segurança.
        /// </summary>
        [NotMapped]
        [Display(Name = "Apenas produtos abaixo do estoque mínimo")]
        public bool ApenasAbaixoDoMinimo { get; set; } = false;

        #endregion

        #region Filtros específicos para Relatório de Movimentações

        /// <summary>
        /// Filtro para tipo específico de movimentação (Entradas, Saídas ou Todas).
        /// </summary>
        [NotMapped]
        [Display(Name = "Tipo de Movimentação")]
        public TipoMovimentacao? TipoMovimentacao { get; set; }

        /// <summary>
        /// Quando verdadeiro, agrupa as movimentações por produto no relatório.
        /// </summary>
        [NotMapped]
        [Display(Name = "Resumido (agrupado por produto)")]
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

    /// <summary>
    /// Define os tipos de relatórios disponíveis no sistema.
    /// </summary>
    public enum TipoRelatorio
    {
        /// <summary>
        /// Relatório de inventário de estoque.
        /// </summary>
        [Display(Name = "Estoque")]
        Estoque,

        /// <summary>
        /// Relatório de movimentações de entrada e saída.
        /// </summary>
        [Display(Name = "Movimentações")]
        Movimentacoes
    }

    /// <summary>
    /// Define os critérios de ordenação disponíveis para relatórios.
    /// </summary>
    public enum OrdenacaoRelatorio
    {
        /// <summary>
        /// Ordenação por data crescente.
        /// </summary>
        [Display(Name = "Data Crescente")]
        DataCrescente,

        /// <summary>
        /// Ordenação por data decrescente.
        /// </summary>
        [Display(Name = "Data Decrescente")]
        DataDecrescente,

        /// <summary>
        /// Ordenação alfabética por nome do produto.
        /// </summary>
        [Display(Name = "Nome do Produto")]
        NomeProduto,

        /// <summary>
        /// Ordenação por quantidade.
        /// </summary>
        [Display(Name = "Quantidade")]
        Quantidade,

        /// <summary>
        /// Ordenação por valor.
        /// </summary>
        [Display(Name = "Valor")]
        Valor
    }

    /// <summary>
    /// Define os tipos de movimentação disponíveis para filtros de relatório.
    /// </summary>
    public enum TipoMovimentacao
    {
        /// <summary>
        /// Todas as movimentações (entradas e saídas).
        /// </summary>
        [Display(Name = "Todas")]
        Todas,

        /// <summary>
        /// Apenas movimentações de entrada.
        /// </summary>
        [Display(Name = "Entradas")]
        Entradas,

        /// <summary>
        /// Apenas movimentações de saída.
        /// </summary>
        [Display(Name = "Saídas")]
        Saidas
    }
}
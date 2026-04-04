using System.ComponentModel.DataAnnotations;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Models.ViewModels
{
    /// <summary>
    /// ViewModel para criação de relatórios.
    /// Expõe apenas os campos editáveis pelo usuário. UsuarioGerador e DataGeracao
    /// são preenchidos pelo controller, evitando overposting.
    /// </summary>
    public class RelatorioCreateViewModel : IValidatableObject
    {
        /// <summary>
        /// Define o tipo de relatório (Estoque ou Movimentações).
        /// </summary>
        [Display(Name = "Tipo do Relatório")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public TipoRelatorio Tipo { get; set; }

        /// <summary>
        /// Data inicial para filtrar os dados do relatório.
        /// </summary>
        [Display(Name = "Data Início")]
        [DataType(DataType.Date)]
        public DateTime? DataInicio { get; set; }

        /// <summary>
        /// Data final para filtrar os dados do relatório.
        /// </summary>
        [Display(Name = "Data Fim")]
        [DataType(DataType.Date)]
        public DateTime? DataFim { get; set; }

        /// <summary>
        /// Identificador do fornecedor para filtrar os dados do relatório.
        /// </summary>
        [Display(Name = "Fornecedor")]
        public int? FornecedorId { get; set; }

        /// <summary>
        /// Identificador do produto para filtrar os dados do relatório.
        /// </summary>
        [Display(Name = "Produto")]
        public int? ProdutoId { get; set; }

        // --- Filtros de Estoque ---

        /// <summary>
        /// Quando verdadeiro, inclui produtos com quantidade zero.
        /// </summary>
        [Display(Name = "Incluir Produtos com Estoque Zero")]
        public bool IncluirProdutosZerados { get; set; } = false;

        /// <summary>
        /// Critério de ordenação para o relatório de estoque.
        /// </summary>
        [Display(Name = "Ordenação")]
        public OrdenacaoRelatorio Ordenacao { get; set; } = OrdenacaoRelatorio.NomeProduto;

        /// <summary>
        /// Quando verdadeiro, filtra apenas produtos abaixo do estoque de segurança.
        /// </summary>
        [Display(Name = "Apenas produtos abaixo do estoque mínimo")]
        public bool ApenasAbaixoDoMinimo { get; set; } = false;

        // --- Filtros de Movimentações ---

        /// <summary>
        /// Filtro para tipo específico de movimentação.
        /// </summary>
        [Display(Name = "Tipo de Movimentação")]
        public FiltroTipoMovimentacao? TipoMovimentacao { get; set; }

        /// <summary>
        /// Quando verdadeiro, agrupa as movimentações por produto.
        /// </summary>
        [Display(Name = "Resumido (agrupado por produto)")]
        public bool Resumido { get; set; } = false;

        // --- Dados de apoio para a view ---

        /// <summary>
        /// Nome do usuário logado, exibido na view como somente leitura.
        /// </summary>
        [Display(Name = "Gerado por")]
        public string? UsuarioGerador { get; set; }

        /// <summary>
        /// Data de geração do relatório, exibida na view como somente leitura.
        /// </summary>
        [Display(Name = "Data da Geração")]
        [DataType(DataType.Date)]
        public DateTime DataGeracao { get; set; } = DateTime.Now;

        /// <inheritdoc/>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DataInicio.HasValue && DataFim.HasValue && DataInicio > DataFim)
            {
                yield return new ValidationResult(
                    "A data inicial não pode ser maior que a data final",
                    [nameof(DataInicio)]);
            }

            if (Tipo == TipoRelatorio.Movimentacoes && !DataInicio.HasValue)
            {
                yield return new ValidationResult(
                    "Data de início é obrigatória para relatórios de movimentações",
                    [nameof(DataInicio)]);
            }
        }
    }
}

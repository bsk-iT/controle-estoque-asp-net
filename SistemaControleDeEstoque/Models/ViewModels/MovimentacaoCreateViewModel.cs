using System.ComponentModel.DataAnnotations;
using SistemaControleDeEstoque.Models;

namespace SistemaControleDeEstoque.Models.ViewModels
{
    /// <summary>
    /// ViewModel para criação de movimentações de estoque.
    /// Expõe apenas os campos editáveis pelo usuário. Os campos de sistema
    /// (UsuarioId, UsuarioNome, DataMovimentacao) são preenchidos pelo controller.
    /// </summary>
    public class MovimentacaoCreateViewModel
    {
        /// <summary>
        /// Tipo da movimentação: Entrada ou Saída.
        /// </summary>
        [Display(Name = "Tipo da Movimentação")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public TipoMovimentacao Tipo { get; set; }

        /// <summary>
        /// Quantidade de itens movimentados.
        /// </summary>
        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        /// <summary>
        /// Identificador do produto que está sendo movimentado.
        /// </summary>
        [Display(Name = "Produto")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public int? ProdutoId { get; set; }

        // --- Dados de apoio para a view (não recebidos via POST) ---

        /// <summary>
        /// Nome do usuário logado, exibido somente leitura na view.
        /// </summary>
        [Display(Name = "Responsável")]
        public string? UsuarioNome { get; set; }

        /// <summary>
        /// Data da movimentação, preenchida pelo controller e exibida na view.
        /// </summary>
        [Display(Name = "Data da Movimentação")]
        [DataType(DataType.DateTime)]
        public DateTime DataMovimentacao { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Dicionário com ProdutoId → Quantidade disponível em estoque.
        /// Usado para validação no front-end e exibição de estoque disponível.
        /// </summary>
        public Dictionary<int, int> EstoqueProdutos { get; set; } = [];
    }
}

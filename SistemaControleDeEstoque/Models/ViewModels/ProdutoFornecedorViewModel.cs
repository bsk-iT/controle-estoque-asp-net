using System.ComponentModel.DataAnnotations;

namespace SistemaControleDeEstoque.Models.ViewModels
{
    /// <summary>
    /// ViewModel para criação e edição de associações produto-fornecedor.
    /// Expõe apenas os campos que o usuário pode preencher via formulário,
    /// prevenindo overposting de navigation properties do modelo.
    /// </summary>
    public class ProdutoFornecedorViewModel
    {
        /// <summary>
        /// Identificador da associação (usado apenas em edição).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identificador do produto.
        /// </summary>
        [Display(Name = "Produto")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public int ProdutoId { get; set; }

        /// <summary>
        /// Identificador do fornecedor.
        /// </summary>
        [Display(Name = "Fornecedor")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public int FornecedorId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace SistemaControleDeEstoque.Models
{
    /// <summary>
    /// Define os tipos de movimentação de estoque permitidos.
    /// </summary>
    public enum TipoMovimentacao
    {
        /// <summary>Adição ao estoque</summary>
        [Display(Name = "Entrada")]
        Entrada = 0,

        /// <summary>Retirada do estoque</summary>
        [Display(Name = "Saida")]
        Saida = 1
    }
}

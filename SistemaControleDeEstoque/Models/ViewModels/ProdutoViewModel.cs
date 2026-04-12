using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaControleDeEstoque.Models.ViewModels
{
    /// <summary>
    /// ViewModel para criação e edição de produtos.
    /// Expõe apenas os campos que o usuário pode preencher via formulário,
    /// prevenindo overposting de propriedades internas do modelo.
    /// </summary>
    public class ProdutoViewModel
    {
        /// <summary>
        /// Identificador do produto (usado apenas em edição).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do produto que será exibido na interface.
        /// </summary>
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(200, ErrorMessage = "{0} deve ter no máximo {1} caracteres")]
        public required string Nome { get; set; }

        /// <summary>
        /// Categoria ou tipo do produto (ex: Eletrônico, Alimentício).
        /// </summary>
        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(100, ErrorMessage = "{0} deve ter no máximo {1} caracteres")]
        public required string Tipo { get; set; }

        /// <summary>
        /// Quantidade atual disponível em estoque.
        /// </summary>
        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa")]
        public int Quantidade { get; set; }

        /// <summary>
        /// Valor unitário do produto em reais.
        /// </summary>
        [Display(Name = "Valor")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        [Precision(18, 2)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Identificador do fornecedor principal deste produto.
        /// </summary>
        [Display(Name = "Fornecedor Principal")]
        public int? FornecedorId { get; set; }

        /// <summary>
        /// Quantidade mínima recomendada para manter em estoque. Abaixo disso, o sistema alertará sobre necessidade de reposição.
        /// </summary>
        [Display(Name = "Estoque de Segurança")]
        [Range(0, int.MaxValue, ErrorMessage = "Estoque de segurança não pode ser negativo")]
        public int EstoqueSeguranca { get; set; } = 5;
    }
}

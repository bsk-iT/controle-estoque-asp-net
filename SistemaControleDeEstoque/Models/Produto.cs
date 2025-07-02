using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaControleDeEstoque.Models
{
    /// <summary>
    /// Representa um produto no sistema de controle de estoque.
    /// </summary>
    public class Produto
    {
        /// <summary>
        /// Identificador único do produto.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Nome completo do produto que será exibido na interface.
        /// </summary>
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public required string Nome { get; set; }

        /// <summary>
        /// Categoria ou tipo do produto (ex: Eletrônico, Alimentício).
        /// </summary>
        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public required string Tipo { get; set; }

        /// <summary>
        /// Quantidade atual disponível em estoque.
        /// </summary>
        [Display(Name = "Quantidade")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public int Quantidade { get; set; }

        /// <summary>
        /// Quantidade mínima recomendada para manter em estoque. Abaixo disso, o sistema alertará sobre necessidade de reposição.
        /// </summary>
        [Display(Name = "Estoque de Segurança")]
        [Range(0, int.MaxValue, ErrorMessage = "Estoque de segurança não pode ser negativo")]
        public int EstoqueSeguranca { get; set; } = 5;

        /// <summary>
        /// Valor unitário do produto em reais.
        /// </summary>
        [Display(Name = "Valor")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public decimal Valor { get; set; }

        /// <summary>
        /// Identificador do fornecedor principal deste produto.
        /// </summary>
        [Display(Name = "Fornecedor Principal")]
        public int? FornecedorId { get; set; }

        /// <summary>
        /// Referência ao fornecedor principal do produto.
        /// </summary>
        [ForeignKey("FornecedorId")]
        public Fornecedor? Fornecedor { get; set; }

        /// <summary>
        /// Coleção de todos os fornecedores que oferecem este produto.
        /// </summary>
        public ICollection<ProdutoFornecedor>? Fornecedores { get; set; }

        /// <summary>
        /// Indica se o produto está com estoque abaixo do nível de segurança definido.
        /// </summary>
        [NotMapped]
        public bool EstoqueBaixo => Quantidade < EstoqueSeguranca;
    }
}
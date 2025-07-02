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
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identificador único do produto.
        /// </summary>
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        /// <summary>
        /// Nome completo do produto que será exibido na interface.
        /// </summary>
        public required string Nome { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        /// <summary>
        /// Categoria ou tipo do produto (ex: Eletrônico, Alimentício).
        /// </summary>
        public required string Tipo { get; set; }

        [Display(Name = "Quantidade")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        /// <summary>
        /// Quantidade atual disponível em estoque.
        /// </summary>
        public int Quantidade { get; set; }

        [Display(Name = "Estoque de Segurança")]
        [Range(0, int.MaxValue, ErrorMessage = "Estoque de segurança não pode ser negativo")]
        /// <summary>
        /// Quantidade mínima recomendada para manter em estoque. Abaixo disso, o sistema alertará sobre necessidade de reposição.
        /// </summary>
        public int EstoqueSeguranca { get; set; } = 5;

        [Display(Name = "Valor")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        /// <summary>
        /// Valor unitário do produto em reais.
        /// </summary>
        public decimal Valor { get; set; }

        [Display(Name = "Fornecedor Principal")]
        /// <summary>
        /// Identificador do fornecedor principal deste produto.
        /// </summary>
        public int? FornecedorId { get; set; }

        [ForeignKey("FornecedorId")]
        /// <summary>
        /// Referência ao fornecedor principal do produto.
        /// </summary>
        public Fornecedor? Fornecedor { get; set; }

        /// <summary>
        /// Coleção de todos os fornecedores que oferecem este produto.
        /// </summary>
        public ICollection<ProdutoFornecedor>? Fornecedores { get; set; }

        [NotMapped]
        /// <summary>
        /// Indica se o produto está com estoque abaixo do nível de segurança definido.
        /// </summary>
        public bool EstoqueBaixo => Quantidade < EstoqueSeguranca;
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SistemaControleDeEstoque.Models
{
    public class Produto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public required string Nome { get; set; }

        [Display(Name = "Tipo")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public required string Tipo { get; set; }

        [Display(Name = "Quantidade")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantidade não pode ser negativa")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public int Quantidade { get; set; }

        [Display(Name = "Estoque de Segurança")]
        [Range(0, int.MaxValue, ErrorMessage = "Estoque de segurança não pode ser negativo")]
        public int EstoqueSeguranca { get; set; } = 5;

        [Display(Name = "Valor")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser positivo")]
        [DataType(DataType.Currency)]
        [Precision(18, 2)]
        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public decimal Valor { get; set; }

        [Display(Name = "Fornecedor Principal")]
        public int? FornecedorId { get; set; }

        [ForeignKey("FornecedorId")]
        public Fornecedor? Fornecedor { get; set; }

        public ICollection<ProdutoFornecedor>? Fornecedores { get; set; }

        [NotMapped]
        public bool EstoqueBaixo => Quantidade < EstoqueSeguranca;
    }
}
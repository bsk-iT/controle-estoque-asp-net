using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaControleDeEstoque.Models
{
    public class ProdutoFornecedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public required Produto Produto { get; set; }

        public int FornecedorId { get; set; }

        [ForeignKey("FornecedorId")]
        public required Fornecedor Fornecedor { get; set; }
    }
}
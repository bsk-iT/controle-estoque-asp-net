using SistemaControleDeEstoque.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaControleDeEstoque.Models
{
    public class Movimentacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Tipo da Movimentação")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public string? Tipo { get; set; }

        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        [Display(Name = "Data da Movimentação")]
        [DataType(DataType.DateTime)]
        public DateTime DataMovimentacao { get; set; } = DateTime.Now;

        [Display(Name = "Produto")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public int? ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        public Produto? Produto { get; set; }

        [Display(Name = "ID do Usuário")]
        public string? UsuarioId { get; set; }

        [Display(Name = "Responsável")]
        public string? UsuarioNome { get; set; }
    }
}
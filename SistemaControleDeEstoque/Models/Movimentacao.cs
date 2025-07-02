using SistemaControleDeEstoque.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaControleDeEstoque.Models
{
    /// <summary>
    /// Representa uma movimentação de estoque (entrada ou saída) de um produto.
    /// </summary>
    public class Movimentacao
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        /// <summary>
        /// Identificador único da movimentação.
        /// </summary>
        public int Id { get; set; }

        [Display(Name = "Tipo da Movimentação")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        /// <summary>
        /// Tipo da movimentação: "Entrada" para adições ao estoque ou "Saída" para retiradas.
        /// </summary>
        public string? Tipo { get; set; }

        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        /// <summary>
        /// Quantidade de itens movimentados nesta operação.
        /// </summary>
        public int Quantidade { get; set; }

        [Display(Name = "Data da Movimentação")]
        [DataType(DataType.DateTime)]
        /// <summary>
        /// Data e hora em que a movimentação foi realizada.
        /// </summary>
        public DateTime DataMovimentacao { get; set; } = DateTime.Now;

        [Display(Name = "Produto")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        /// <summary>
        /// Identificador do produto que está sendo movimentado.
        /// </summary>
        public int? ProdutoId { get; set; }

        [ForeignKey("ProdutoId")]
        /// <summary>
        /// Referência ao produto associado a esta movimentação.
        /// </summary>
        public Produto? Produto { get; set; }

        [Display(Name = "ID do Usuário")]
        /// <summary>
        /// Identificador do usuário que realizou a movimentação.
        /// </summary>
        public string? UsuarioId { get; set; }

        [Display(Name = "Responsável")]
        /// <summary>
        /// Nome do usuário responsável pela movimentação.
        /// </summary>
        public string? UsuarioNome { get; set; }
    }
}
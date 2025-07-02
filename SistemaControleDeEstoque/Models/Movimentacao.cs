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
        /// <summary>
        /// Identificador único da movimentação.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Tipo da movimentação: "Entrada" para adições ao estoque ou "Saída" para retiradas.
        /// </summary>
        [Display(Name = "Tipo da Movimentação")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public string? Tipo { get; set; }

        /// <summary>
        /// Quantidade de itens movimentados nesta operação.
        /// </summary>
        [Display(Name = "Quantidade")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que zero")]
        public int Quantidade { get; set; }

        /// <summary>
        /// Data e hora em que a movimentação foi realizada.
        /// </summary>
        [Display(Name = "Data da Movimentação")]
        [DataType(DataType.DateTime)]
        public DateTime DataMovimentacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Identificador do produto que está sendo movimentado.
        /// </summary>
        [Display(Name = "Produto")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public int? ProdutoId { get; set; }

        /// <summary>
        /// Referência ao produto associado a esta movimentação.
        /// </summary>
        [ForeignKey("ProdutoId")]
        public Produto? Produto { get; set; }

        /// <summary>
        /// Identificador do usuário que realizou a movimentação.
        /// </summary>
        [Display(Name = "ID do Usuário")]
        public string? UsuarioId { get; set; }

        /// <summary>
        /// Nome do usuário responsável pela movimentação.
        /// </summary>
        [Display(Name = "Responsável")]
        public string? UsuarioNome { get; set; }
    }
}
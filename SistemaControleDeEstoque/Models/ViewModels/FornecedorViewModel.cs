using System.ComponentModel.DataAnnotations;

namespace SistemaControleDeEstoque.Models.ViewModels
{
    /// <summary>
    /// ViewModel para criação e edição de fornecedores.
    /// Expõe apenas os campos que o usuário pode preencher via formulário,
    /// prevenindo overposting de propriedades internas do modelo.
    /// </summary>
    public class FornecedorViewModel
    {
        /// <summary>
        /// Identificador do fornecedor (usado apenas em edição).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome ou razão social do fornecedor.
        /// </summary>
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(200, ErrorMessage = "{0} deve ter no máximo {1} caracteres")]
        public required string Nome { get; set; }

        /// <summary>
        /// CNPJ do fornecedor (apenas números, 14 dígitos).
        /// </summary>
        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ deve ter 14 dígitos")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter apenas números")]
        public required string CNPJ { get; set; }

        /// <summary>
        /// Endereço de e-mail para contato com o fornecedor.
        /// </summary>
        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public required string Email { get; set; }

        /// <summary>
        /// Número de telefone do fornecedor.
        /// </summary>
        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Telefone deve ter entre 10 e 11 dígitos")]
        public required string Telefone { get; set; }
    }
}

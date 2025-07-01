using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace SistemaControleDeEstoque.Models
{
    public class Fornecedor
    {
        private string nome = string.Empty;
        private string cnpj = string.Empty;
        private string email = string.Empty;
        private string telefone = string.Empty;


        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        public string Nome { get => nome; set => nome = value; }

        [Display(Name = "CNPJ")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ deve ter 14 dígitos")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter apenas números")]
        public string CNPJ { get => cnpj; set => cnpj = value?.Replace(".", "").Replace("/", "").Replace("-", "").Trim() ?? string.Empty; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [DataType(DataType.EmailAddress)]
        public string Email { get => email; set => email = value; }

        [Display(Name = "Telefone")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(11, MinimumLength = 10, ErrorMessage = "Telefone deve ter máximo de 11 dígitos")]
        public string Telefone { get => telefone; set => telefone = value; }

        public ICollection<ProdutoFornecedor>? Produtos { get; set; }
    }
}

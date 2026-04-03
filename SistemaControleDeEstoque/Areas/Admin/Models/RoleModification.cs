using System.ComponentModel.DataAnnotations;

namespace SistemaControleDeEstoque.Areas.Admin.Models
{
    public class RoleModification
    {
        public string? RoleId { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public string? RoleName { get; set; }
        public string[]? AddIds { get; set; }
        public string[]? DeleteIds { get; set; }
    }
}

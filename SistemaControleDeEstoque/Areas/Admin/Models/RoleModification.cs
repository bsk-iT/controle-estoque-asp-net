using System.ComponentModel.DataAnnotations;

namespace SistemaControleDeEstoque.Areas.Admin.Models
{
    public class RoleModification
    {
        public string? RoleId { get; set; }
        [Required]
        public string? RoleName { get; set; }
        public string[]? AddIds { get; set; }
        public string[]? DeleteIds { get; set; }
    }
}

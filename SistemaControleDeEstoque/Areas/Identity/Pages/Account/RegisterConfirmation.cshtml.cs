#nullable disable

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SistemaControleDeEstoque.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        /// <summary>
        /// E-mail do usuário recém-cadastrado, exibido para referência.
        /// </summary>
        public string Email { get; set; }

        public IActionResult OnGet(string email)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }

            Email = email;
            return Page();
        }
    }
}

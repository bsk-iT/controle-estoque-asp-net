using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SistemaControleDeEstoque.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager) : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly SignInManager<IdentityUser> _signInManager = signInManager;

        public string Username { get; set; } = string.Empty;

        [TempData]
        public string StatusMessage { get; set; } = string.Empty;

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel
        {
            [Phone]
            [Display(Name = "Número de telefone")]
            public string NumTelefone { get; set; } = string.Empty;
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user) ?? string.Empty;
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user) ?? string.Empty;

            Username = userName;

            Input = new InputModel
            {
                NumTelefone = phoneNumber
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o usuário com ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.NumTelefone != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.NumTelefone);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Erro inesperado ao tentar definir o número de telefone.";
                    return RedirectToPage();
                }
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Seu perfil foi atualizado";
            return RedirectToPage();
        }
    }
}
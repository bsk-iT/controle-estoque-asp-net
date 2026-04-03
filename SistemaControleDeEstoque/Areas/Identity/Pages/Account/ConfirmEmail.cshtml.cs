#nullable disable

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace SistemaControleDeEstoque.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<ConfirmEmailModel> _logger;

        public ConfirmEmailModel(UserManager<IdentityUser> userManager, ILogger<ConfirmEmailModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Mensagem de status (sucesso ou erro) exibida na view.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Não foi possível carregar o usuário com ID '{userId}'.");
            }

            // Decodificar o token
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

            // Confirmar o email
            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                StatusMessage = "Sucesso! Seu e-mail foi confirmado. Agora você pode fazer login.";
                _logger.LogInformation("E-mail confirmado com sucesso para o usuário {UserId}.", userId);
                return Page();
            }

            // Se falhar, mostre os erros
            var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
            StatusMessage = $"Erro ao confirmar seu e-mail. O link pode ter expirado ou ser inválido. Detalhes: {errorMessages}";
            _logger.LogWarning("Falha ao confirmar e-mail para o usuário {UserId}. Erros: {Errors}", userId, errorMessages);
            return Page();
        }
    }
}

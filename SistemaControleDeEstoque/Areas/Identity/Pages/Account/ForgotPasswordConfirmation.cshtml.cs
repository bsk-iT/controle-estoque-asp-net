// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SistemaControleDeEstoque.Areas.Identity.Pages.Account
{
    /// <summary>
    /// Página exibida após a solicitação de recuperação de senha.
    /// Não revela se o e-mail existe ou não no sistema.
    /// </summary>
    [AllowAnonymous]
    public class ForgotPasswordConfirmationModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}

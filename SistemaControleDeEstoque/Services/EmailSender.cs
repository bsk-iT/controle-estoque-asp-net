using Microsoft.AspNetCore.Identity.UI.Services;

namespace SistemaControleDeEstoque.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implementação para enviar e-mails
            // Por enquanto, apenas retorna uma tarefa completa
            return Task.CompletedTask;
        }
    }
}
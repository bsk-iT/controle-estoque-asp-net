using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SistemaControleDeEstoque.Services
{
    /// <summary>
    /// Implementação de <see cref="IEmailSender"/> usando MailKit com SMTP do Gmail.
    /// Configure as credenciais via User Secrets (desenvolvimento) ou variáveis de ambiente (produção):
    ///   dotnet user-secrets set "Smtp:Password" "xxxx xxxx xxxx xxxx"
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var host = _configuration["Smtp:Host"]
                ?? throw new InvalidOperationException("Configuração 'Smtp:Host' não encontrada.");
            var port = int.Parse(_configuration["Smtp:Port"]
                ?? throw new InvalidOperationException("Configuração 'Smtp:Port' não encontrada."));
            var fromAddress = _configuration["Smtp:FromAddress"]
                ?? throw new InvalidOperationException("Configuração 'Smtp:FromAddress' não encontrada.");
            var fromName = _configuration["Smtp:FromName"]
                ?? throw new InvalidOperationException("Configuração 'Smtp:FromName' não encontrada.");
            var password = _configuration["Smtp:Password"]
                ?? throw new InvalidOperationException("Configuração 'Smtp:Password' não encontrada.");

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(fromAddress, password);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromAddress));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = subject;

                var builder = new BodyBuilder { HtmlBody = htmlMessage };
                message.Body = builder.ToMessageBody();

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("E-mail enviado para {Email} com assunto '{Subject}'.", email, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao enviar e-mail para {Email}.", email);
                throw;
            }
        }
    }
}

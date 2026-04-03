namespace SistemaControleDeEstoque.Services
{
    /// <summary>
    /// Templates HTML para emails transacionais (confirmação de cadastro, redefinição de senha, etc).
    /// Usa inline CSS para compatibilidade máxima com clientes de email (Gmail, Outlook, Apple Mail, etc).
    /// </summary>
    public static class EmailTemplates
    {
        private const string FontFamily = "'Playfair Display', Georgia, serif";
        private const string SansFamily = "-apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, sans-serif";

        /// <summary>
        /// Template para email de confirmação de cadastro.
        /// </summary>
        /// <param name="callbackUrl">URL completa do link de confirmação (href do botão)</param>
        /// <returns>HTML do email</returns>
        public static string ConfirmacaoEmail(string callbackUrl)
        {
            return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Confirme seu e-mail — Inventarii</title>
</head>
<body style=""margin: 0; padding: 0; font-family: {SansFamily}; background-color: #f9f9f9;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f9f9f9;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <!-- Card branco centralizado -->
                <table width=""560"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);"">
                    <!-- Cabeçalho com logo/nome -->
                    <tr>
                        <td style=""padding: 40px 40px 20px; text-align: center; border-bottom: 1px solid #f0f0f0;"">
                            <p style=""margin: 0; font-size: 11px; font-weight: 600; letter-spacing: 2px; color: #999; text-transform: uppercase;"">inventarii</p>
                        </td>
                    </tr>

                    <!-- Conteúdo principal -->
                    <tr>
                        <td style=""padding: 40px;"">
                            <!-- Título -->
                            <h1 style=""margin: 0 0 20px 0; font-family: {FontFamily}; font-size: 28px; font-weight: 400; color: #000; line-height: 1.2;"">
                                Confirme seu e-mail
                            </h1>

                            <!-- Parágrafo intro -->
                            <p style=""margin: 0 0 24px 0; font-size: 15px; color: #666; line-height: 1.6;"">
                                Obrigado por se cadastrar no Inventarii. Para completar seu cadastro, confirme seu endereço de e-mail clicando no botão abaixo.
                            </p>

                            <!-- Botão CTA -->
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin: 32px 0;"">
                                <tr>
                                    <td align=""center"">
                                        <a href=""{callbackUrl}"" style=""display: inline-block; padding: 12px 32px; background-color: #000; color: #fff; text-decoration: none; font-size: 14px; font-weight: 600; border-radius: 4px; letter-spacing: 0.5px;"">
                                            Confirmar E-mail
                                        </a>
                                    </td>
                                </tr>
                            </table>

                            <!-- Parágrafo alternativo -->
                            <p style=""margin: 32px 0 0 0; font-size: 13px; color: #999; line-height: 1.6;"">
                                Ou copie e cole este link no seu navegador:<br>
                                <span style=""word-break: break-all; color: #666;"">
                                    {callbackUrl}
                                </span>
                            </p>
                        </td>
                    </tr>

                    <!-- Rodapé com aviso de segurança -->
                    <tr>
                        <td style=""padding: 20px 40px 40px; border-top: 1px solid #f0f0f0; font-size: 12px; color: #999; line-height: 1.6;"">
                            <p style=""margin: 0 0 12px 0;"">
                                Se você não solicitou este cadastro, ignore este e-mail ou entre em contato conosco.
                            </p>
                            <p style=""margin: 0; font-size: 11px; color: #ccc;"">
                                Este é um e-mail automático. Não responda diretamente.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";
        }

        /// <summary>
        /// Template para email de redefinição de senha.
        /// </summary>
        /// <param name="callbackUrl">URL completa do link de redefinição (href do botão)</param>
        /// <returns>HTML do email</returns>
        public static string RedefinicaoSenha(string callbackUrl)
        {
            return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Redefinir sua senha — Inventarii</title>
</head>
<body style=""margin: 0; padding: 0; font-family: {SansFamily}; background-color: #f9f9f9;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #f9f9f9;"">
        <tr>
            <td align=""center"" style=""padding: 40px 20px;"">
                <!-- Card branco centralizado -->
                <table width=""560"" cellpadding=""0"" cellspacing=""0"" style=""background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0,0,0,0.08);"">
                    <!-- Cabeçalho com logo/nome -->
                    <tr>
                        <td style=""padding: 40px 40px 20px; text-align: center; border-bottom: 1px solid #f0f0f0;"">
                            <p style=""margin: 0; font-size: 11px; font-weight: 600; letter-spacing: 2px; color: #999; text-transform: uppercase;"">inventarii</p>
                        </td>
                    </tr>

                    <!-- Conteúdo principal -->
                    <tr>
                        <td style=""padding: 40px;"">
                            <!-- Título -->
                            <h1 style=""margin: 0 0 20px 0; font-family: {FontFamily}; font-size: 28px; font-weight: 400; color: #000; line-height: 1.2;"">
                                Redefinir sua senha
                            </h1>

                            <!-- Parágrafo intro -->
                            <p style=""margin: 0 0 24px 0; font-size: 15px; color: #666; line-height: 1.6;"">
                                Recebemos uma solicitação para redefinir a senha da sua conta. Clique no botão abaixo para criar uma nova senha.
                            </p>

                            <!-- Botão CTA -->
                            <table width=""100%"" cellpadding=""0"" cellspacing=""0"" style=""margin: 32px 0;"">
                                <tr>
                                    <td align=""center"">
                                        <a href=""{callbackUrl}"" style=""display: inline-block; padding: 12px 32px; background-color: #000; color: #fff; text-decoration: none; font-size: 14px; font-weight: 600; border-radius: 4px; letter-spacing: 0.5px;"">
                                            Redefinir Senha
                                        </a>
                                    </td>
                                </tr>
                            </table>

                            <!-- Parágrafo alternativo -->
                            <p style=""margin: 32px 0 0 0; font-size: 13px; color: #999; line-height: 1.6;"">
                                Ou copie e cole este link no seu navegador:<br>
                                <span style=""word-break: break-all; color: #666;"">
                                    {callbackUrl}
                                </span>
                            </p>

                            <!-- Aviso de expiração -->
                            <p style=""margin: 24px 0 0 0; padding: 16px; background-color: #fafafa; border-left: 3px solid #ddd; font-size: 13px; color: #666; line-height: 1.6;"">
                                <strong>Nota:</strong> Este link expira em 24 horas. Se você não conseguir usá-lo a tempo, solicite um novo link de redefinição de senha.
                            </p>
                        </td>
                    </tr>

                    <!-- Rodapé com aviso de segurança -->
                    <tr>
                        <td style=""padding: 20px 40px 40px; border-top: 1px solid #f0f0f0; font-size: 12px; color: #999; line-height: 1.6;"">
                            <p style=""margin: 0 0 12px 0;"">
                                Se você não solicitou redefinir sua senha, ignore este e-mail. Sua conta permanecerá protegida.
                            </p>
                            <p style=""margin: 0; font-size: 11px; color: #ccc;"">
                                Este é um e-mail automático. Não responda diretamente.
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>
";
        }
    }
}

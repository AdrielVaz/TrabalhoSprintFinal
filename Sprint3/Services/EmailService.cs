using Sprint3.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Sprint3.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task EnviarEmailAsync(string destino, string assunto, string corpoHtml)
        {
            var smtp = _configuration.GetSection("Email:Smtp");
            var host = smtp["Host"];
            var from = smtp["From"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                _logger.LogInformation("Email para {Destino}: {Assunto}. Conteudo: {Corpo}", destino, assunto, corpoHtml);
                return;
            }

            using var mensagem = new MailMessage
            {
                From = new MailAddress(from, smtp["FromName"] ?? "TASKPI"),
                Subject = assunto,
                Body = corpoHtml,
                IsBodyHtml = true
            };
            mensagem.To.Add(destino);

            using var client = new SmtpClient(host, int.TryParse(smtp["Port"], out var port) ? port : 587)
            {
                EnableSsl = bool.TryParse(smtp["EnableSsl"], out var enableSsl) ? enableSsl : true
            };

            var username = smtp["Username"];
            var password = smtp["Password"];
            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                client.Credentials = new NetworkCredential(username, password);
            }

            await client.SendMailAsync(mensagem);
        }
    }
}

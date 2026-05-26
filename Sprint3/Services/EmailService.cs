using Sprint3.Services.Interfaces;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using System.Net.Mail;

namespace Sprint3.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly HttpClient _httpClient;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, HttpClient httpClient)
        {
            _configuration = configuration;
            _logger = logger;
            _httpClient = httpClient;
        }

        public async Task EnviarEmailAsync(string destino, string assunto, string corpoHtml)
        {
            var provider = _configuration["Email:Provider"];
            var resendApiKey = _configuration["Email:Resend:ApiKey"];

            if (provider?.Equals("Resend", StringComparison.OrdinalIgnoreCase) == true ||
                !string.IsNullOrWhiteSpace(resendApiKey))
            {
                await EnviarComResendAsync(destino, assunto, corpoHtml);
                return;
            }

            await EnviarComSmtpAsync(destino, assunto, corpoHtml);
        }

        private async Task EnviarComSmtpAsync(string destino, string assunto, string corpoHtml)
        {
            var smtp = _configuration.GetSection("Email:Smtp");
            var host = smtp["Host"];
            var from = smtp["From"];

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidOperationException("Configuracao SMTP incompleta. Configure Email__Smtp__Host e Email__Smtp__From.");
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
                client.Credentials = new NetworkCredential(username.Trim(), password.Trim());
            }

            await client.SendMailAsync(mensagem);
        }

        private async Task EnviarComResendAsync(string destino, string assunto, string corpoHtml)
        {
            var resend = _configuration.GetSection("Email:Resend");
            var apiKey = resend["ApiKey"];
            var from = resend["From"] ?? _configuration["Email:Smtp:From"];
            var fromName = resend["FromName"] ?? _configuration["Email:Smtp:FromName"] ?? "TASKPI";

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(from))
            {
                throw new InvalidOperationException("Configuracao Resend incompleta. Configure Email__Resend__ApiKey e Email__Resend__From.");
            }

            using var request = new HttpRequestMessage(HttpMethod.Post, "https://api.resend.com/emails");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey.Trim());
            request.Content = JsonContent.Create(new
            {
                from = $"{fromName} <{from}>",
                to = new[] { destino },
                subject = assunto,
                html = corpoHtml
            });

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                var erro = await response.Content.ReadAsStringAsync();
                _logger.LogError("Falha ao enviar email pelo Resend. Status: {Status}. Resposta: {Resposta}", response.StatusCode, erro);
                throw new InvalidOperationException("Nao foi possivel enviar o email agora.");
            }
        }
    }
}

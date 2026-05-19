namespace Sprint3.Services.Interfaces
{
    public interface IEmailService
    {
        Task EnviarEmailAsync(string destino, string assunto, string corpoHtml);
    }
}

using Microsoft.AspNetCore.Http;

namespace Sprint3.DTOs
{
    public class UsuarioPerfilInput
    {
        public string Nome { get; set; } = string.Empty;

        public IFormFile? Foto { get; set; }
    }
}

namespace Sprint3.DTOs
{
    public class UsuarioPerfilOutput
    {
        public int Id { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string? FotoPerfilUrl { get; set; }
    }
}

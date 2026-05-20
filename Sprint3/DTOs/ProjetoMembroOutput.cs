namespace Sprint3.DTOs
{
    public class ProjetoMembroOutput
    {
        public int UsuarioId { get; set; }

        public string Nome { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string NivelAcesso { get; set; } = string.Empty;

        public string? FotoPerfilUrl { get; set; }
    }
}

namespace Sprint3.DTOs
{
    public class ConviteProjetoOutput
    {
        public int Id { get; set; }

        public int ProjetoId { get; set; }

        public string ProjetoDescricao { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string NivelAcesso { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; }
    }
}

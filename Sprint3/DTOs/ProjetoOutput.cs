namespace Sprint3.DTOs
{
    public class ProjetoOutput
    {
        public int Id { get; set; }

        public string Descricao { get; set; } = string.Empty;

        public string NivelAcesso { get; set; } = string.Empty;

        public bool Compartilhado { get; set; }
    }
}

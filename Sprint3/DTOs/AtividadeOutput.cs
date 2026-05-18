namespace Sprint3.DTOs
{
    public class AtividadeOutput
    {
        public int Id { get; set; }

        public int ProjetoId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public DateTime DataCriacao { get; set; }

        public List<TarefaOutput> Tarefas { get; set; } = new();
    }
}

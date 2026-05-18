using Sprint3.Models;

namespace Sprint3.DTOs
{
    public class TarefaOutput
    {
        public int Id { get; set; }

        public int ProjetoId { get; set; }

        public string Titulo { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public bool Concluida { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime? DataConclusao { get; set; }

        public Prioridade Prioridade { get; set; }
    }
}
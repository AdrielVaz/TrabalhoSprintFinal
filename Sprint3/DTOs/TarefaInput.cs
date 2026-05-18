using Sprint3.Models;

namespace Sprint3.DTOs
{
    public class TarefaInput
    {
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public bool Concluida { get; set; } = false;
        public DateTime DataCriacao { get; set; } = DateTime.Now;
        public DateTime? DataConclusao { get; set; }
        public Prioridade Prioridade { get; set; }
    }

   
}


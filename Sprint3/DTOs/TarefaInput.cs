using Sprint3.Models;

namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados para criar ou editar tarefa.
    /// </summary>
    public class TarefaInput
    {
        /// <summary>Título da tarefa.</summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>Descrição opcional da tarefa.</summary>
        public string? Descricao { get; set; }

        /// <summary>Indica se a tarefa está concluída.</summary>
        public bool Concluida { get; set; } = false;

        /// <summary>Data de criação da tarefa.</summary>
        public DateTime DataCriacao { get; set; } = DateTime.Now;

        /// <summary>Data de conclusão da tarefa, quando concluída.</summary>
        public DateTime? DataConclusao { get; set; }

        /// <summary>Prioridade da tarefa: Alta, Media ou Baixa.</summary>
        public Prioridade Prioridade { get; set; }
    }
}

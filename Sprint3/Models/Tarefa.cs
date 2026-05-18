using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models
{
    public class Tarefa
    {
        public int Id { get; set; }

        public int AtividadeId { get; set; }

        [ForeignKey(nameof(AtividadeId))]
        public Atividade Atividade { get; set; } = null!;

        [Required]
        public string Titulo { get; set; } = string.Empty;

        public string? Descricao { get; set; }

        public bool Concluida { get; set; } = false;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataConclusao { get; set; }

        public Prioridade Prioridade { get; set; }
    }

    public enum Prioridade
    {
        Alta,
        Media,
        Baixa
    }
}

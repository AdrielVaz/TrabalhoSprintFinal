using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models
{
    public class Projeto
    {
        public int Id { get; set; }

        [Required]
        public string Descricao { get; set; } = string.Empty;

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataConclusao { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        public List<Tarefa> Tarefas { get; set; } = new();
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models
{
    public class ProjetoAcesso
    {
        public int Id { get; set; }

        public int ProjetoId { get; set; }

        [ForeignKey(nameof(ProjetoId))]
        public Projeto Projeto { get; set; } = null!;

        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        public NivelAcessoProjeto NivelAcesso { get; set; }
    }

    public enum NivelAcessoProjeto
    {
        Adm,
        Observador,
        Ajudante
    }
}

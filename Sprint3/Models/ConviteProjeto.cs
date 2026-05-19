using System.ComponentModel.DataAnnotations.Schema;

namespace Sprint3.Models
{
    public class ConviteProjeto
    {
        public int Id { get; set; }

        public int ProjetoId { get; set; }

        [ForeignKey(nameof(ProjetoId))]
        public Projeto Projeto { get; set; } = null!;

        public string Email { get; set; } = string.Empty;

        public NivelAcessoProjeto NivelAcesso { get; set; }

        public ConviteProjetoStatus Status { get; set; } = ConviteProjetoStatus.Pendente;

        public int ConvidadoPorUsuarioId { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime? DataResposta { get; set; }
    }

    public enum ConviteProjetoStatus
    {
        Pendente,
        Aceito,
        Recusado
    }
}

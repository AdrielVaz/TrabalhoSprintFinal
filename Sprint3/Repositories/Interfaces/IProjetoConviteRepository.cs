using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces
{
    public interface IProjetoConviteRepository
    {
        Task<ConviteProjeto> Salvar(ConviteProjeto convite);
        Task<ConviteProjeto?> ObterPorId(int id);
        Task<ConviteProjeto?> ObterPendente(int projetoId, string email);
        Task<List<ConviteProjeto>> ListarPendentesPorEmail(string email);
    }
}

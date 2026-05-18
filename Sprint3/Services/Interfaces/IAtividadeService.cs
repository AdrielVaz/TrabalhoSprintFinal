using Sprint3.DTOs;

namespace Sprint3.Services.Interfaces
{
    public interface IAtividadeService
    {
        Task<AtividadeOutput> CriarAtividade(int usuarioId, int projetoId, AtividadeInput input);
        Task<List<AtividadeOutput>> ListarPorProjeto(int usuarioId, int projetoId);
        Task<AtividadeOutput> Atualizar(int usuarioId, int projetoId, int id, AtividadeInput input);
        Task<AtividadeOutput> Deletar(int usuarioId, int projetoId, int id);
    }
}

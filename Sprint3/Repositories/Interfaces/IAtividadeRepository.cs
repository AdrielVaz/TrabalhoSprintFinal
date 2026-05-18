using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces
{
    public interface IAtividadeRepository
    {
        Task<Atividade> Criar(Atividade atividade);
        Task<Atividade> Atualizar(Atividade atividade);
        Task<Atividade> Deletar(int id);
        Task<Atividade?> ObterPorId(int id);
        Task<List<Atividade>> ListarPorProjeto(int projetoId);
    }
}

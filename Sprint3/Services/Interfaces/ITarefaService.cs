using Sprint3.DTOs;

namespace Sprint3.Services.Interfaces
{
    public interface ITarefaService
    {
        Task<TarefaOutput> CriarTarefa(int usuarioId, int projetoId, int atividadeId, TarefaInput input);
        Task<TarefaOutput> DeletarTarefa(int usuarioId, int projetoId, int atividadeId, int id);
        Task<TarefaOutput> Atualizar(int usuarioId, int projetoId, int atividadeId, int id, TarefaInput tarefaInput);
        Task<List<TarefaOutput>> ListarPorAtividade(int usuarioId, int projetoId, int atividadeId);
        Task<TarefaOutput?> ObterPorId(int usuarioId, int projetoId, int atividadeId, int id);
    }
}

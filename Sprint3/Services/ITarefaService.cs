using Sprint3.DTOs;
using Sprint3.Models;

namespace Sprint3.Services
{
    public interface ITarefaService
    {
        Task<TarefaOutput> CriarTarefa(
            int usuarioId,
            int projetoId,
            TarefaInput input);

        Task<TarefaOutput> DeletarTarefa(
            int id,
            int projetoId);

        Task<TarefaOutput> Atualizar(
            int id,
            int projetoId,
            TarefaInput tarefaInput);

        Task<List<TarefaOutput>>ListarPorProjeto(
            int projetoId,
            int usuarioId);

        Task<TarefaOutput?> ObterPorId(
            int id,
            int projetId);
    }
}
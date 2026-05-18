using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces
{
    public interface ITarefaRepository
    {
        Task<Tarefa> Criar(Tarefa tarefa);
        Task<Tarefa> Atualizar(Tarefa tarefa, int atividadeId);
        Task<Tarefa> Deletar(int id, int atividadeId);
        Task<List<Tarefa>> ListarPorAtividade(int atividadeId);
        Task<Tarefa?> ObterPorId(int id);
    }
}

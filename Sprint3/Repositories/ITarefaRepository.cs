using Sprint3.DTOs;
using Sprint3.Models;

namespace Sprint3.Repositories
{
    public interface ITarefaRepository
    {
        Task<Tarefa> Criar(Tarefa tarefa);
        Task<Tarefa> Atualizar(Tarefa tarefa, int ProjetoId);
        Task<Tarefa> Deletar(int id, int ProjetoId);
        Task<List<Tarefa>> ListarPorProjeto(int ProjetoId);
        Task<Tarefa?> ObterPorId(int id);
    }

}

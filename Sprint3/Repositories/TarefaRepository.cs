using Sprint3.Data;
using Sprint3.Models;
using Microsoft.EntityFrameworkCore;

namespace Sprint3.Repositories
{
    public class TarefaRepository : ITarefaRepository
    {
        private readonly AppDbContext _db;

        public TarefaRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Tarefa> Criar(Tarefa tarefa)
        {
            _db.Tarefas.Add(tarefa);
            await _db.SaveChangesAsync();
            return tarefa;
        }

        public async Task<Tarefa> Deletar(int id, int IdProjeto)
        {
            var tarefa = await _db.Tarefas.FindAsync(id);
            if (tarefa == null)
            {
                throw new Exception("Tarefa não encontrada");
            }
            if (tarefa == null || tarefa.ProjetoId != IdProjeto)
            {
                throw new Exception("Tarefa não encontrada ou não pertence ao usuário");
            }

            _db.Tarefas.Remove(tarefa);
            await _db.SaveChangesAsync();
            return tarefa;
        }
        public async Task<Tarefa> Atualizar(Tarefa tarefa, int IdProjeto)
        {
            var tarefaExistente = await _db.Tarefas.FindAsync(tarefa.Id);
            if (tarefaExistente == null || tarefaExistente.ProjetoId != IdProjeto)
            {
                throw new Exception("Tarefa não encontrada ou não pertence ao projeto");
            }

            tarefaExistente.Titulo = tarefa.Titulo;
            tarefaExistente.Descricao = tarefa.Descricao;
            tarefaExistente.Concluida = tarefa.Concluida;
            tarefaExistente.Prioridade = tarefa.Prioridade;
            tarefaExistente.DataConclusao = tarefa.DataConclusao;
            _db.Tarefas.Update(tarefaExistente);
            await _db.SaveChangesAsync();
            return tarefaExistente;
        }

        public async Task<List<Tarefa>> ListarPorProjeto(int IdProjeto)
        {
            return await _db.Tarefas
                .Where(t => t.ProjetoId == IdProjeto)
                .ToListAsync();
        }

        public async Task<Tarefa?> ObterPorId(int id)
        {
            return await _db.Tarefas.FindAsync(id);
        }
    }
}

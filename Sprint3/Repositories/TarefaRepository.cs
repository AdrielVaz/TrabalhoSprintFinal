using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

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

        public async Task<Tarefa> Deletar(int id, int atividadeId)
        {
            var tarefa = await _db.Tarefas.FindAsync(id);
            if (tarefa == null || tarefa.AtividadeId != atividadeId)
            {
                throw new Exception("Tarefa não encontrada");
            }

            _db.Tarefas.Remove(tarefa);
            await _db.SaveChangesAsync();
            return tarefa;
        }

        public async Task<Tarefa> Atualizar(Tarefa tarefa, int atividadeId)
        {
            var tarefaExistente = await _db.Tarefas.FindAsync(tarefa.Id);
            if (tarefaExistente == null)
            {
                throw new Exception("Tarefa não encontrada");
            }

            tarefaExistente.AtividadeId = atividadeId;
            tarefaExistente.Titulo = tarefa.Titulo;
            tarefaExistente.Descricao = tarefa.Descricao;
            tarefaExistente.Concluida = tarefa.Concluida;
            tarefaExistente.Prioridade = tarefa.Prioridade;
            tarefaExistente.DataConclusao = tarefa.DataConclusao;
            _db.Tarefas.Update(tarefaExistente);
            await _db.SaveChangesAsync();
            return tarefaExistente;
        }

        public async Task<List<Tarefa>> ListarPorAtividade(int atividadeId)
        {
            return await _db.Tarefas
                .Where(t => t.AtividadeId == atividadeId)
                .ToListAsync();
        }

        public async Task<Tarefa?> ObterPorId(int id)
        {
            return await _db.Tarefas.FindAsync(id);
        }
    }
}

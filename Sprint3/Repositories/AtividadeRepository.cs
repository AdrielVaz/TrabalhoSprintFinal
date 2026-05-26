using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

namespace Sprint3.Repositories
{
    public class AtividadeRepository : IAtividadeRepository
    {
        private readonly AppDbContext _db;

        public AtividadeRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Atividade> Criar(Atividade atividade)
        {
            _db.Atividades.Add(atividade);
            await _db.SaveChangesAsync();
            return atividade;
        }

        public async Task<Atividade> Atualizar(Atividade atividade)
        {
            var atividadeExistente = await _db.Atividades.FindAsync(atividade.Id);
            if (atividadeExistente == null)
            {
                throw new Exception("Atividade não encontrada");
            }

            atividadeExistente.Titulo = atividade.Titulo;
            atividadeExistente.Descricao = atividade.Descricao;
            _db.Atividades.Update(atividadeExistente);
            await _db.SaveChangesAsync();
            return atividadeExistente;
        }

        public async Task<Atividade> Deletar(int id)
        {
            var atividade = await _db.Atividades.FindAsync(id);
            if (atividade == null)
            {
                throw new Exception("Atividade não encontrada");
            }

            _db.Atividades.Remove(atividade);
            await _db.SaveChangesAsync();
            return atividade;
        }

        public async Task<Atividade?> ObterPorId(int id)
        {
            return await _db.Atividades
                .AsNoTracking()
                .Include(a => a.Tarefas)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<Atividade>> ListarPorProjeto(int projetoId)
        {
            return await _db.Atividades
                .AsNoTracking()
                .Include(a => a.Tarefas)
                .Where(a => a.ProjetoId == projetoId)
                .ToListAsync();
        }
    }
}

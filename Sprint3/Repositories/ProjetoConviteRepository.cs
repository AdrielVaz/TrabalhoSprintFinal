using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

namespace Sprint3.Repositories
{
    public class ProjetoConviteRepository : IProjetoConviteRepository
    {
        private readonly AppDbContext _db;

        public ProjetoConviteRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ConviteProjeto> Salvar(ConviteProjeto convite)
        {
            if (convite.Id == 0)
                _db.ConvitesProjeto.Add(convite);
            else
                _db.ConvitesProjeto.Update(convite);

            await _db.SaveChangesAsync();
            return convite;
        }

        public async Task<ConviteProjeto?> ObterPorId(int id)
        {
            return await _db.ConvitesProjeto
                .Include(c => c.Projeto)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ConviteProjeto?> ObterPendente(int projetoId, string email)
        {
            return await _db.ConvitesProjeto
                .Include(c => c.Projeto)
                .FirstOrDefaultAsync(c =>
                    c.ProjetoId == projetoId &&
                    c.Email == email &&
                    c.Status == ConviteProjetoStatus.Pendente);
        }

        public async Task<List<ConviteProjeto>> ListarPendentesPorEmail(string email)
        {
            return await _db.ConvitesProjeto
                .Include(c => c.Projeto)
                .Where(c => c.Email == email && c.Status == ConviteProjetoStatus.Pendente)
                .ToListAsync();
        }
    }
}

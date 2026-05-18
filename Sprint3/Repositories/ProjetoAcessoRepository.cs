using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

namespace Sprint3.Repositories
{
    public class ProjetoAcessoRepository : IProjetoAcessoRepository
    {
        private readonly AppDbContext _db;

        public ProjetoAcessoRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<ProjetoAcesso> Salvar(ProjetoAcesso acesso)
        {
            var acessoExistente = await _db.ProjetoAcessos
                .FirstOrDefaultAsync(a => a.ProjetoId == acesso.ProjetoId && a.UsuarioId == acesso.UsuarioId);

            if (acessoExistente == null)
            {
                _db.ProjetoAcessos.Add(acesso);
                await _db.SaveChangesAsync();
                return acesso;
            }

            acessoExistente.NivelAcesso = acesso.NivelAcesso;
            _db.ProjetoAcessos.Update(acessoExistente);
            await _db.SaveChangesAsync();
            return acessoExistente;
        }

        public async Task<ProjetoAcesso?> ObterAcesso(int projetoId, int usuarioId)
        {
            return await _db.ProjetoAcessos
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.ProjetoId == projetoId && a.UsuarioId == usuarioId);
        }

        public async Task<List<ProjetoAcesso>> ListarPorProjeto(int projetoId)
        {
            return await _db.ProjetoAcessos
                .Include(a => a.Usuario)
                .Where(a => a.ProjetoId == projetoId)
                .ToListAsync();
        }

        public async Task<List<ProjetoAcesso>> ListarPorUsuario(int usuarioId)
        {
            return await _db.ProjetoAcessos
                .Include(a => a.Projeto)
                .Where(a => a.UsuarioId == usuarioId)
                .ToListAsync();
        }
    }
}

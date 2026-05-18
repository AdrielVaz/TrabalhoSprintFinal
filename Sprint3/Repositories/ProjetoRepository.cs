using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;

namespace Sprint3.Repositories
{
    public class ProjetoRepository : IProjetoRepository
    {
        private readonly AppDbContext _context;

        public ProjetoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Projeto> Criar(Projeto projeto)
        {
            _context.Projetos.Add(projeto);
            await _context.SaveChangesAsync();
            return projeto;
        }

        public async Task<Projeto> ObterPorId(int id)
        {
            var projeto = await _context.Projetos
                .Include(p => p.Usuario)
                .Include(p => p.Acessos)
                .ThenInclude(a => a.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (projeto == null)
                throw new Exception("Projeto não encontrado");

            return projeto;
        }

        public async Task<List<Projeto>> ListarPorUsuario(int usuarioId)
        {
            return await _context.Projetos
                .Include(p => p.Acessos)
                .Where(p => p.UsuarioId == usuarioId || p.Acessos.Any(a => a.UsuarioId == usuarioId))
                .ToListAsync();
        }

        public async Task<Projeto> Deletar(int id)
        {
            var projeto = await _context.Projetos.FindAsync(id);
            if (projeto == null)
            {
                throw new Exception("Projeto não encontrado");
            }

            _context.Projetos.Remove(projeto);
            await _context.SaveChangesAsync();
            return projeto;
        }
    }
}

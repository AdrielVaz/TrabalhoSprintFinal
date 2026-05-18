using Sprint3.Data;
using Sprint3.Models;
using Microsoft.EntityFrameworkCore;



namespace Sprint3.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _db;

        public UsuarioRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<Usuario?> ObterPorEmail(string email)
        {
            return await _db.Usuarios
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email);
        }

        public async Task<bool> EmailExiste(string email)
        {
            return await _db.Usuarios
                .AnyAsync(u => u.Email.ToLower() == email);
        }

        public async Task<Usuario> Criar(Usuario usuario)
        {
            _db.Usuarios.Add(usuario);
            await _db.SaveChangesAsync();
            return usuario;
        }
    }
}

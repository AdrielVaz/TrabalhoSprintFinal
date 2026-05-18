using Sprint3.Models;

namespace Sprint3.Repositories
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmail(string email);
        Task<bool> EmailExiste(string email);
        Task <Usuario> Criar(Usuario usuario);
    }
}

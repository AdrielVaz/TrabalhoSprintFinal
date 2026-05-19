using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmail(string email);
        Task<Usuario?> ObterPorId(int id);
        Task<bool> EmailExiste(string email);
        Task<Usuario> Criar(Usuario usuario);
        Task<Usuario> Atualizar(Usuario usuario);
    }
}

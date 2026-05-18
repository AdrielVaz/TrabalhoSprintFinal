using Sprint3.Models;

namespace Sprint3.Repositories
{
    public interface IProjetoRepository
    {
        Task<Projeto> Criar(Projeto projeto);
        Task<Projeto> ObterPorId(int id);
        Task<List<Projeto>> ListarPorUsuario(int usuarioId);
        Task<Projeto> Deletar(int id);
    }
}

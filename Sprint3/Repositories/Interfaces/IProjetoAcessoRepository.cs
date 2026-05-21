using Sprint3.Models;

namespace Sprint3.Repositories.Interfaces
{
    public interface IProjetoAcessoRepository
    {
        Task<ProjetoAcesso> Salvar(ProjetoAcesso acesso);
        Task<ProjetoAcesso?> ObterAcesso(int projetoId, int usuarioId);
        Task<ProjetoAcesso?> ObterPorProjetoEEmail(int projetoId, string email);
        Task<List<ProjetoAcesso>> ListarPorProjeto(int projetoId);
        Task<List<ProjetoAcesso>> ListarPorUsuario(int usuarioId);
        Task<ProjetoAcesso> Deletar(int id);
    }
}

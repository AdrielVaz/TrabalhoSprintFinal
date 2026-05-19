using Sprint3.DTOs;

namespace Sprint3.Services.Interfaces
{
    public interface IProjetoService
    {
        Task<ProjetoOutput> CriarProjeto(int usuarioId, ProjetoInput input);
        Task<List<ProjetoOutput>> ListarProjetos(int usuarioId);
        Task<ProjetoOutput> DeletarProjeto(int projetoId, int usuarioId);
        Task<ConviteProjetoOutput> CompartilharProjeto(int projetoId, int usuarioId, ProjetoAcessoInput input);
        Task<List<ProjetoMembroOutput>> ListarMembros(int projetoId, int usuarioId);
        Task<List<ConviteProjetoOutput>> ListarConvitesPendentes(string email);
        Task<ProjetoMembroOutput> AceitarConvite(int conviteId, int usuarioId);
        Task<ConviteProjetoOutput> RecusarConvite(int conviteId, int usuarioId);
    }
}

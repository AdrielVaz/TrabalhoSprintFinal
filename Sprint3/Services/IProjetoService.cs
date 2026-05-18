using Sprint3.DTOs;
using Sprint3.Models;

namespace Sprint3.Services
{
    public interface IProjetoService
    {
        Task<ProjetoOutput> CriarProjeto(int usuarioId, ProjetoInput input);
        Task<List<ProjetoOutput>> ListarProjetos(int usuarioId);

        Task<ProjetoOutput> DeletarProjeto(int projetoId, int usuarioId);
    }
}
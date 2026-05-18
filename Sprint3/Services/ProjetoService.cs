using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories;
namespace Sprint3.Services
{
    public class ProjetoService : IProjetoService
    {
        private readonly AppDbContext _context;
        private readonly IProjetoRepository _Projeto;


        public ProjetoService(AppDbContext context , IProjetoRepository Projeto)
        {

            _context = context;
            _Projeto = Projeto;
        }

        public async Task<ProjetoOutput> CriarProjeto(int usuarioId, ProjetoInput input)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == usuarioId);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var projeto = new Projeto
            {
                Descricao = input.Descricao,
                UsuarioId = usuarioId
            };

            _context.Projetos.Add(projeto);

            await _context.SaveChangesAsync();

            return new ProjetoOutput
            {
                Id = projeto.Id,
                Descricao = projeto.Descricao
            };
        }
        public async Task<List<ProjetoOutput>> ListarProjetos(int usuarioId)
        {
            var projetos =
                await _Projeto.ListarPorUsuario(usuarioId);

            return projetos.Select(p => new ProjetoOutput
            {
                Id = p.Id,
                Descricao = p.Descricao
            }).ToList();
        }
        public async Task<ProjetoOutput> DeletarProjeto(
    int projetoId,
    int usuarioId)
        {
            var projeto =
                await _Projeto.ObterPorId(projetoId);

            if (projeto == null || projeto.UsuarioId != usuarioId)
                throw new Exception("Projeto não encontrado");

            var projetoDeletado =
                await _Projeto.Deletar(projetoId);

            return new ProjetoOutput
            {
                Id = projetoDeletado.Id,
                Descricao = projetoDeletado.Descricao
            };
        }
    }
    }

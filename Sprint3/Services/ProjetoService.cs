using Microsoft.EntityFrameworkCore;
using Sprint3.Data;
using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services
{
    public class ProjetoService : IProjetoService
    {
        private readonly AppDbContext _context;
        private readonly IProjetoRepository _projetoRepository;
        private readonly IProjetoAcessoRepository _acessoRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ProjetoService(
            AppDbContext context,
            IProjetoRepository projetoRepository,
            IProjetoAcessoRepository acessoRepository,
            IUsuarioRepository usuarioRepository)
        {
            _context = context;
            _projetoRepository = projetoRepository;
            _acessoRepository = acessoRepository;
            _usuarioRepository = usuarioRepository;
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

            await _projetoRepository.Criar(projeto);

            await _acessoRepository.Salvar(new ProjetoAcesso
            {
                ProjetoId = projeto.Id,
                UsuarioId = usuarioId,
                NivelAcesso = NivelAcessoProjeto.Adm
            });

            return MapProjeto(projeto, NivelAcessoProjeto.Adm);
        }

        public async Task<List<ProjetoOutput>> ListarProjetos(int usuarioId)
        {
            var projetos = await _projetoRepository.ListarPorUsuario(usuarioId);

            return projetos.Select(p =>
            {
                var acesso = ObterNivelDoProjeto(p, usuarioId);
                return MapProjeto(p, acesso);
            }).ToList();
        }

        public async Task<ProjetoOutput> DeletarProjeto(int projetoId, int usuarioId)
        {
            var projeto = await _projetoRepository.ObterPorId(projetoId);
            var nivelAcesso = ObterNivelDoProjeto(projeto, usuarioId);

            if (nivelAcesso != NivelAcessoProjeto.Adm)
                throw new Exception("Apenas administradores podem excluir o projeto");

            var projetoDeletado = await _projetoRepository.Deletar(projetoId);

            return MapProjeto(projetoDeletado, nivelAcesso);
        }

        public async Task<ProjetoMembroOutput> CompartilharProjeto(int projetoId, int usuarioId, ProjetoAcessoInput input)
        {
            var projeto = await _projetoRepository.ObterPorId(projetoId);
            var nivelAcesso = ObterNivelDoProjeto(projeto, usuarioId);

            if (nivelAcesso != NivelAcessoProjeto.Adm)
                throw new Exception("Apenas administradores podem compartilhar o projeto");

            var email = input.Email.Trim().ToLowerInvariant();
            var usuario = await _usuarioRepository.ObterPorEmail(email);

            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var acesso = await _acessoRepository.Salvar(new ProjetoAcesso
            {
                ProjetoId = projetoId,
                UsuarioId = usuario.Id,
                NivelAcesso = input.NivelAcesso
            });

            return new ProjetoMembroOutput
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                NivelAcesso = acesso.NivelAcesso.ToString()
            };
        }

        public async Task<List<ProjetoMembroOutput>> ListarMembros(int projetoId, int usuarioId)
        {
            var projeto = await _projetoRepository.ObterPorId(projetoId);
            ObterNivelDoProjeto(projeto, usuarioId);

            var membros = await _acessoRepository.ListarPorProjeto(projetoId);

            var saida = membros.Select(a => new ProjetoMembroOutput
            {
                UsuarioId = a.UsuarioId,
                Nome = a.Usuario.Nome,
                Email = a.Usuario.Email,
                NivelAcesso = a.NivelAcesso.ToString()
            }).ToList();

            if (!saida.Any(m => m.UsuarioId == projeto.UsuarioId))
            {
                saida.Insert(0, new ProjetoMembroOutput
                {
                    UsuarioId = projeto.UsuarioId,
                    Nome = projeto.Usuario.Nome,
                    Email = projeto.Usuario.Email,
                    NivelAcesso = NivelAcessoProjeto.Adm.ToString()
                });
            }

            return saida;
        }

        private static NivelAcessoProjeto ObterNivelDoProjeto(Projeto projeto, int usuarioId)
        {
            if (projeto.UsuarioId == usuarioId)
                return NivelAcessoProjeto.Adm;

            var acesso = projeto.Acessos.FirstOrDefault(a => a.UsuarioId == usuarioId);
            if (acesso == null)
                throw new Exception("Projeto não encontrado");

            return acesso.NivelAcesso;
        }

        private static ProjetoOutput MapProjeto(Projeto projeto, NivelAcessoProjeto nivelAcesso)
        {
            return new ProjetoOutput
            {
                Id = projeto.Id,
                Descricao = projeto.Descricao,
                NivelAcesso = nivelAcesso.ToString()
            };
        }
    }
}

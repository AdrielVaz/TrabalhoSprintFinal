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
        private readonly IProjetoConviteRepository _conviteRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public ProjetoService(
            AppDbContext context,
            IProjetoRepository projetoRepository,
            IProjetoAcessoRepository acessoRepository,
            IProjetoConviteRepository conviteRepository,
            IUsuarioRepository usuarioRepository)
        {
            _context = context;
            _projetoRepository = projetoRepository;
            _acessoRepository = acessoRepository;
            _conviteRepository = conviteRepository;
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

        public async Task<ConviteProjetoOutput> CompartilharProjeto(int projetoId, int usuarioId, ProjetoAcessoInput input)
        {
            var projeto = await _projetoRepository.ObterPorId(projetoId);
            var nivelAcesso = ObterNivelDoProjeto(projeto, usuarioId);

            if (nivelAcesso != NivelAcessoProjeto.Adm)
                throw new Exception("Apenas administradores podem convidar pessoas");

            var email = input.Email.Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Email é obrigatório");

            var usuarioConvite = await _usuarioRepository.ObterPorEmail(email);
            if (usuarioConvite != null && usuarioConvite.Id == projeto.UsuarioId)
                throw new Exception("O administrador do projeto já possui acesso e não pode ser convidado");

            if (usuarioConvite != null && projeto.Acessos.Any(a => a.UsuarioId == usuarioConvite.Id))
                throw new Exception("Este usuário já participa do projeto");

            var conviteExistente = await _conviteRepository.ObterPendente(projetoId, email);
            if (conviteExistente != null)
            {
                conviteExistente.NivelAcesso = input.NivelAcesso;
                return MapConvite(await _conviteRepository.Salvar(conviteExistente));
            }

            var convite = await _conviteRepository.Salvar(new ConviteProjeto
            {
                ProjetoId = projetoId,
                Email = email,
                NivelAcesso = input.NivelAcesso,
                ConvidadoPorUsuarioId = usuarioId,
                Status = ConviteProjetoStatus.Pendente,
                DataCriacao = DateTime.Now
            });

            convite.Projeto = projeto;

            return MapConvite(convite);
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
                NivelAcesso = a.NivelAcesso.ToString(),
                FotoPerfilUrl = a.Usuario.FotoPerfil is { Length: > 0 } ? $"/api/Usuarios/{a.UsuarioId}/foto" : null
            }).ToList();

            if (!saida.Any(m => m.UsuarioId == projeto.UsuarioId))
            {
                saida.Insert(0, new ProjetoMembroOutput
                {
                    UsuarioId = projeto.UsuarioId,
                    Nome = projeto.Usuario.Nome,
                    Email = projeto.Usuario.Email,
                    NivelAcesso = NivelAcessoProjeto.Adm.ToString(),
                    FotoPerfilUrl = projeto.Usuario.FotoPerfil is { Length: > 0 } ? $"/api/Usuarios/{projeto.UsuarioId}/foto" : null
                });
            }

            return saida;
        }

        public async Task<List<ConviteProjetoOutput>> ListarConvitesPendentes(string email)
        {
            var convites = await _conviteRepository.ListarPendentesPorEmail(email.Trim().ToLowerInvariant());
            return convites.Select(MapConvite).ToList();
        }

        public async Task<ProjetoMembroOutput> AceitarConvite(int conviteId, int usuarioId)
        {
            var usuario = await _usuarioRepository.ObterPorId(usuarioId);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var convite = await _conviteRepository.ObterPorId(conviteId);
            if (convite == null || convite.Status != ConviteProjetoStatus.Pendente)
                throw new Exception("Convite não encontrado");

            if (convite.Email != usuario.Email.Trim().ToLowerInvariant())
                throw new Exception("Este convite pertence a outro usuário");

            var acesso = await _acessoRepository.Salvar(new ProjetoAcesso
            {
                ProjetoId = convite.ProjetoId,
                UsuarioId = usuario.Id,
                NivelAcesso = convite.NivelAcesso
            });

            convite.Status = ConviteProjetoStatus.Aceito;
            convite.DataResposta = DateTime.Now;
            await _conviteRepository.Salvar(convite);

            return new ProjetoMembroOutput
            {
                UsuarioId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                NivelAcesso = acesso.NivelAcesso.ToString(),
                FotoPerfilUrl = usuario.FotoPerfil is { Length: > 0 } ? $"/api/Usuarios/{usuario.Id}/foto" : null
            };
        }

        public async Task<ConviteProjetoOutput> RecusarConvite(int conviteId, int usuarioId)
        {
            var usuario = await _usuarioRepository.ObterPorId(usuarioId);
            if (usuario == null)
                throw new Exception("Usuário não encontrado");

            var convite = await _conviteRepository.ObterPorId(conviteId);
            if (convite == null || convite.Status != ConviteProjetoStatus.Pendente)
                throw new Exception("Convite não encontrado");

            if (convite.Email != usuario.Email.Trim().ToLowerInvariant())
                throw new Exception("Este convite pertence a outro usuário");

            convite.Status = ConviteProjetoStatus.Recusado;
            convite.DataResposta = DateTime.Now;

            return MapConvite(await _conviteRepository.Salvar(convite));
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

        private static ConviteProjetoOutput MapConvite(ConviteProjeto convite)
        {
            return new ConviteProjetoOutput
            {
                Id = convite.Id,
                ProjetoId = convite.ProjetoId,
                ProjetoDescricao = convite.Projeto?.Descricao ?? string.Empty,
                Email = convite.Email,
                NivelAcesso = convite.NivelAcesso.ToString(),
                Status = convite.Status.ToString(),
                DataCriacao = convite.DataCriacao
            };
        }
    }
}

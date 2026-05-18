using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services
{
    public class AtividadeService : IAtividadeService
    {
        private readonly IAtividadeRepository _atividadeRepo;
        private readonly IProjetoRepository _projetoRepo;

        public AtividadeService(IAtividadeRepository atividadeRepo, IProjetoRepository projetoRepo)
        {
            _atividadeRepo = atividadeRepo;
            _projetoRepo = projetoRepo;
        }

        public async Task<AtividadeOutput> CriarAtividade(int usuarioId, int projetoId, AtividadeInput input)
        {
            var projeto = await _projetoRepo.ObterPorId(projetoId);
            ValidarAcesso(projeto, usuarioId, true);

            var atividade = new Atividade
            {
                ProjetoId = projetoId,
                Titulo = input.Titulo,
                Descricao = input.Descricao,
                DataCriacao = DateTime.Now
            };

            var atividadeCriada = await _atividadeRepo.Criar(atividade);

            return MapAtividade(atividadeCriada);
        }

        public async Task<List<AtividadeOutput>> ListarPorProjeto(int usuarioId, int projetoId)
        {
            var projeto = await _projetoRepo.ObterPorId(projetoId);
            ValidarAcesso(projeto, usuarioId, false);

            var atividades = await _atividadeRepo.ListarPorProjeto(projetoId);

            return atividades.Select(MapAtividade).ToList();
        }

        public async Task<AtividadeOutput> Atualizar(int usuarioId, int projetoId, int id, AtividadeInput input)
        {
            var atividade = await ObterAtividadePermitida(usuarioId, projetoId, id, true);

            atividade.Titulo = input.Titulo;
            atividade.Descricao = input.Descricao;

            var atividadeAtualizada = await _atividadeRepo.Atualizar(atividade);

            return MapAtividade(atividadeAtualizada);
        }

        public async Task<AtividadeOutput> Deletar(int usuarioId, int projetoId, int id)
        {
            await ObterAtividadePermitida(usuarioId, projetoId, id, true);

            var atividadeDeletada = await _atividadeRepo.Deletar(id);

            return MapAtividade(atividadeDeletada);
        }

        private async Task<Atividade> ObterAtividadePermitida(int usuarioId, int projetoId, int atividadeId, bool exigeEdicao)
        {
            var projeto = await _projetoRepo.ObterPorId(projetoId);
            ValidarAcesso(projeto, usuarioId, exigeEdicao);

            var atividade = await _atividadeRepo.ObterPorId(atividadeId);

            if (atividade == null || atividade.ProjetoId != projetoId)
                throw new Exception("Atividade não encontrada");

            return atividade;
        }

        private static void ValidarAcesso(Projeto projeto, int usuarioId, bool exigeEdicao)
        {
            var nivel = projeto.UsuarioId == usuarioId
                ? NivelAcessoProjeto.Adm
                : projeto.Acessos.FirstOrDefault(a => a.UsuarioId == usuarioId)?.NivelAcesso;

            if (nivel == null)
                throw new Exception("Projeto não encontrado");

            if (exigeEdicao && nivel == NivelAcessoProjeto.Observador)
                throw new Exception("Observadores não podem alterar atividades");
        }

        private static AtividadeOutput MapAtividade(Atividade atividade)
        {
            return new AtividadeOutput
            {
                Id = atividade.Id,
                ProjetoId = atividade.ProjetoId,
                Titulo = atividade.Titulo,
                Descricao = atividade.Descricao,
                DataCriacao = atividade.DataCriacao,
                Tarefas = atividade.Tarefas.Select(t => new TarefaOutput
                {
                    Id = t.Id,
                    AtividadeId = t.AtividadeId,
                    Titulo = t.Titulo,
                    Descricao = t.Descricao,
                    Concluida = t.Concluida,
                    DataCriacao = t.DataCriacao,
                    DataConclusao = t.DataConclusao,
                    Prioridade = t.Prioridade
                }).ToList()
            };
        }
    }
}

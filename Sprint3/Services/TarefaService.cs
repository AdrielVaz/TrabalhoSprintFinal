using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using Sprint3.Services.Interfaces;

namespace Sprint3.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _tarefaRepo;
        private readonly IAtividadeRepository _atividadeRepo;
        private readonly IProjetoRepository _projetoRepo;

        public TarefaService(
            ITarefaRepository tarefaRepo,
            IAtividadeRepository atividadeRepo,
            IProjetoRepository projetoRepo)
        {
            _tarefaRepo = tarefaRepo;
            _atividadeRepo = atividadeRepo;
            _projetoRepo = projetoRepo;
        }

        public async Task<TarefaOutput> CriarTarefa(int usuarioId, int projetoId, int atividadeId, TarefaInput input)
        {
            var atividade = await ObterAtividadePermitida(usuarioId, projetoId, atividadeId, true);

            var tarefa = new Tarefa
            {
                AtividadeId = atividade.Id,
                Titulo = input.Titulo,
                Descricao = input.Descricao,
                Concluida = input.Concluida,
                DataCriacao = DateTime.Now,
                DataConclusao = input.DataConclusao,
                Prioridade = input.Prioridade
            };

            var tarefaCriada = await _tarefaRepo.Criar(tarefa);

            return MapTarefa(tarefaCriada);
        }

        public async Task<List<TarefaOutput>> ListarPorAtividade(int usuarioId, int projetoId, int atividadeId)
        {
            await ObterAtividadePermitida(usuarioId, projetoId, atividadeId, false);

            var tarefas = await _tarefaRepo.ListarPorAtividade(atividadeId);

            return tarefas.Select(MapTarefa).ToList();
        }

        public async Task<TarefaOutput?> ObterPorId(int usuarioId, int projetoId, int atividadeId, int id)
        {
            await ObterAtividadePermitida(usuarioId, projetoId, atividadeId, false);

            var tarefa = await _tarefaRepo.ObterPorId(id);

            if (tarefa == null || tarefa.AtividadeId != atividadeId)
                return null;

            return MapTarefa(tarefa);
        }

        public async Task<TarefaOutput> DeletarTarefa(int usuarioId, int projetoId, int atividadeId, int id)
        {
            await ObterAtividadePermitida(usuarioId, projetoId, atividadeId, true);

            var tarefaDeletada = await _tarefaRepo.Deletar(id, atividadeId);

            return MapTarefa(tarefaDeletada);
        }

        public async Task<TarefaOutput> Atualizar(
            int usuarioId,
            int projetoId,
            int atividadeId,
            int id,
            TarefaInput tarefaInput)
        {
            await ObterAtividadePermitida(usuarioId, projetoId, atividadeId, true);

            var tarefaExistente = await _tarefaRepo.ObterPorId(id);

            if (tarefaExistente == null)
                throw new Exception("Tarefa não encontrada");

            var atividadeAtual = await _atividadeRepo.ObterPorId(tarefaExistente.AtividadeId);
            if (atividadeAtual == null || atividadeAtual.ProjetoId != projetoId)
                throw new Exception("Tarefa não encontrada");

            tarefaExistente.Titulo = tarefaInput.Titulo;
            tarefaExistente.Descricao = tarefaInput.Descricao;
            tarefaExistente.Concluida = tarefaInput.Concluida;
            tarefaExistente.DataConclusao = tarefaInput.DataConclusao;
            tarefaExistente.Prioridade = tarefaInput.Prioridade;

            var tarefaAtualizada = await _tarefaRepo.Atualizar(tarefaExistente, atividadeId);

            return MapTarefa(tarefaAtualizada);
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
                throw new Exception("Observadores não podem alterar tarefas");
        }

        private static TarefaOutput MapTarefa(Tarefa tarefa)
        {
            return new TarefaOutput
            {
                Id = tarefa.Id,
                AtividadeId = tarefa.AtividadeId,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Concluida = tarefa.Concluida,
                DataCriacao = tarefa.DataCriacao,
                DataConclusao = tarefa.DataConclusao,
                Prioridade = tarefa.Prioridade
            };
        }
    }
}

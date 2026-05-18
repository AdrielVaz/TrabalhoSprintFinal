using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories;

namespace Sprint3.Services
{
    public class TarefaService : ITarefaService
    {
        private readonly ITarefaRepository _tarefaRepo;
        private readonly IProjetoRepository _projetoRepo;

        public TarefaService(ITarefaRepository tarefaRepo, IProjetoRepository projetoRepo)
        {
            _tarefaRepo = tarefaRepo;
            _projetoRepo = projetoRepo;
        }

        public async Task<TarefaOutput> CriarTarefa(
         int usuarioId,
         int projetoId,
         TarefaInput input)
        {
            var projeto = await _projetoRepo.ObterPorId(projetoId);

            if (projeto == null || projeto.UsuarioId != usuarioId)
                throw new Exception("Projeto não encontrado");

            var tarefa = new Tarefa
            {
                ProjetoId = projetoId,
                Titulo = input.Titulo,
                Descricao = input.Descricao,
                Concluida = input.Concluida,
                DataCriacao = DateTime.Now,
                DataConclusao = input.DataConclusao,
                Prioridade = input.Prioridade
            };

            var tarefaCriada = await _tarefaRepo.Criar(tarefa);

            return new TarefaOutput
            {
                Id = tarefaCriada.Id,
                ProjetoId = tarefaCriada.ProjetoId,
                Titulo = tarefaCriada.Titulo,
                Descricao = tarefaCriada.Descricao,
                Concluida = tarefaCriada.Concluida,
                DataCriacao = tarefaCriada.DataCriacao,
                DataConclusao = tarefaCriada.DataConclusao,
                Prioridade = tarefaCriada.Prioridade
            };
        }
        public async Task<List<TarefaOutput>> ListarPorProjeto(
        int projetoId,
        int usuarioId)
        {
            var projeto = await _projetoRepo.ObterPorId(projetoId);

            if (projeto == null || projeto.UsuarioId != usuarioId)
                throw new Exception("Projeto não encontrado");

            var tarefas = await _tarefaRepo.ListarPorProjeto(projetoId);

            return tarefas.Select(t => new TarefaOutput
            {
                Id = t.Id,
                ProjetoId = t.ProjetoId,
                Titulo = t.Titulo,
                Descricao = t.Descricao,
                Concluida = t.Concluida,
                DataCriacao = t.DataCriacao,
                DataConclusao = t.DataConclusao,
                Prioridade = t.Prioridade
            }).ToList();
        }

        public async Task<TarefaOutput?> ObterPorId(int id, int projetoId)
        {
            var tarefa = await _tarefaRepo.ObterPorId(id);

            if (tarefa == null || tarefa.ProjetoId != projetoId)
                return null;

            return new TarefaOutput
            {
                Id = tarefa.Id,
                ProjetoId = tarefa.ProjetoId,
                Titulo = tarefa.Titulo,
                Descricao = tarefa.Descricao,
                Concluida = tarefa.Concluida,
                DataCriacao = tarefa.DataCriacao,
                DataConclusao = tarefa.DataConclusao,
                Prioridade = tarefa.Prioridade
            };
        }
        public async Task<TarefaOutput> DeletarTarefa(int id, int IdProjeto)
        {
            var tarefa = await _tarefaRepo.ObterPorId(id);

            if (tarefa == null || tarefa.ProjetoId != IdProjeto)
                throw new Exception("Tarefa não encontrada ou não pertence ao projeto");

            var tarefaDeletada = await _tarefaRepo.Deletar(id, IdProjeto);

            return new TarefaOutput
            {
                Id = tarefaDeletada.Id,
                ProjetoId = tarefaDeletada.ProjetoId,
                Titulo = tarefaDeletada.Titulo,
                Descricao = tarefaDeletada.Descricao,
                Concluida = tarefaDeletada.Concluida,
                DataCriacao = tarefaDeletada.DataCriacao,
                DataConclusao = tarefaDeletada.DataConclusao,
                Prioridade = tarefaDeletada.Prioridade
            };
        }

        public async Task<TarefaOutput> Atualizar(
            int id,
            int IdProjeto,
            TarefaInput tarefaInput)
        {
            var tarefaExistente = await _tarefaRepo.ObterPorId(id);

            if (tarefaExistente == null || tarefaExistente.ProjetoId != IdProjeto)
                throw new Exception("Tarefa não encontrada ou não pertence ao projeto");

            tarefaExistente.Titulo = tarefaInput.Titulo;
            tarefaExistente.Descricao = tarefaInput.Descricao;
            tarefaExistente.Concluida = tarefaInput.Concluida;
            tarefaExistente.DataConclusao = tarefaInput.DataConclusao;
            tarefaExistente.Prioridade = tarefaInput.Prioridade;

            var tarefaAtualizada =
                await _tarefaRepo.Atualizar(tarefaExistente, IdProjeto);

            return new TarefaOutput
            {
                Id = tarefaAtualizada.Id,
                ProjetoId = tarefaAtualizada.ProjetoId,
                Titulo = tarefaAtualizada.Titulo,
                Descricao = tarefaAtualizada.Descricao,
                Concluida = tarefaAtualizada.Concluida,
                DataCriacao = tarefaAtualizada.DataCriacao,
                DataConclusao = tarefaAtualizada.DataConclusao,
                Prioridade = tarefaAtualizada.Prioridade
            };
        }
    }
    }

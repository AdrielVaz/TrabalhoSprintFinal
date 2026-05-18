using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Services.Interfaces;
using System.Security.Claims;

namespace Sprint3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasController : ControllerBase
    {
        private readonly ITarefaService _tarefaService;

        public TarefasController(ITarefaService tarefaService)
        {
            _tarefaService = tarefaService;
        }

        [Authorize]
        [HttpPost("projetos/{projetoId}/atividades/{atividadeId}/tarefas")]
        public async Task<IActionResult> CriarTarefa(int projetoId, int atividadeId, [FromBody] TarefaInput input)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            try
            {
                var tarefa = await _tarefaService.CriarTarefa(usuarioId.Value, projetoId, atividadeId, input);
                return Ok(tarefa);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("projetos/{projetoId}/atividades/{atividadeId}/tarefas")]
        public async Task<IActionResult> ListarTarefas(int projetoId, int atividadeId)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            try
            {
                var tarefas = await _tarefaService.ListarPorAtividade(usuarioId.Value, projetoId, atividadeId);
                return Ok(tarefas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("projetos/{projetoId}/atividades/{atividadeId}/tarefas/{id}")]
        public async Task<IActionResult> Deletar(int projetoId, int atividadeId, int id)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            try
            {
                var tarefaDeletada = await _tarefaService.DeletarTarefa(usuarioId.Value, projetoId, atividadeId, id);
                return Ok(tarefaDeletada);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("projetos/{projetoId}/atividades/{atividadeId}/tarefas/{id}")]
        public async Task<IActionResult> AtualizarTarefa(int projetoId, int atividadeId, int id, [FromBody] TarefaInput input)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            try
            {
                var tarefaAtualizada = await _tarefaService.Atualizar(usuarioId.Value, projetoId, atividadeId, id, input);
                return Ok(tarefaAtualizada);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private int? ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim == null ? null : int.Parse(userIdClaim.Value);
        }
    }
}

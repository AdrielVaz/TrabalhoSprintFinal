using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Services;
using System.IdentityModel.Tokens.Jwt;
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
        [HttpPost("projetos/{projetoId}/tarefas")]
        public async Task<IActionResult> CriarTarefa(
         int projetoId,
         [FromBody] TarefaInput input)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim.Value);

            var tarefa = await _tarefaService.CriarTarefa(
                usuarioId,
                projetoId,
                input);

            return Ok(tarefa);
        }
        [Authorize]
        [HttpGet("Listartarefas/{projetoId}")]
        public async Task<IActionResult> ListarTarefas(int projetoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim.Value);

            var tarefas = await _tarefaService.ListarPorProjeto(projetoId, usuarioId);

            return Ok(tarefas);
        }
        [Authorize]
        [HttpDelete("projetos/{projetoId}/tarefas/{id}")]
        public async Task<IActionResult> Deletar(int id,int projetoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            try
            {
                var tarefaDeletada =
                    await _tarefaService.DeletarTarefa(id, projetoId);

                return Ok(tarefaDeletada);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpPut("projetos/{projetoId}/tarefas/{id}")]
        public async Task<IActionResult> AtualizarTarefa(
        int projetoId,
        int id,
        [FromBody] TarefaInput input)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            try
            {
                var tarefaAtualizada =
                    await _tarefaService.Atualizar(id, projetoId, input);

                return Ok(tarefaAtualizada);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


    }
}

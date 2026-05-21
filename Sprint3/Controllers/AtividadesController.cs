using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Services.Interfaces;
using System.Security.Claims;

namespace Sprint3.Controllers
{
    /// <summary>
    /// Endpoints para organizar um projeto em atividades/colunas.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AtividadesController : ControllerBase
    {
        private readonly IAtividadeService _atividadeService;

        public AtividadesController(IAtividadeService atividadeService)
        {
            _atividadeService = atividadeService;
        }

        /// <summary>
        /// Cria uma atividade dentro de um projeto. Requer permissão de edição.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="input">Título e descrição da atividade.</param>
        [Authorize]
        [HttpPost("projetos/{projetoId}/atividades")]
        public async Task<IActionResult> CriarAtividade(int projetoId, [FromBody] AtividadeInput input)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            try
            {
                var atividade = await _atividadeService.CriarAtividade(usuarioId.Value, projetoId, input);
                return Ok(atividade);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lista as atividades de um projeto, incluindo suas tarefas.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        [Authorize]
        [HttpGet("projetos/{projetoId}/atividades")]
        public async Task<IActionResult> ListarAtividades(int projetoId)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            try
            {
                var atividades = await _atividadeService.ListarPorProjeto(usuarioId.Value, projetoId);
                return Ok(atividades);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Atualiza o título e a descrição de uma atividade. Requer permissão de edição.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="id">Identificador da atividade.</param>
        /// <param name="input">Novos dados da atividade.</param>
        [Authorize]
        [HttpPut("projetos/{projetoId}/atividades/{id}")]
        public async Task<IActionResult> AtualizarAtividade(int projetoId, int id, [FromBody] AtividadeInput input)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            try
            {
                var atividade = await _atividadeService.Atualizar(usuarioId.Value, projetoId, id, input);
                return Ok(atividade);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma atividade e suas tarefas. Requer permissão de edição.
        /// </summary>
        /// <param name="projetoId">Identificador do projeto.</param>
        /// <param name="id">Identificador da atividade.</param>
        [Authorize]
        [HttpDelete("projetos/{projetoId}/atividades/{id}")]
        public async Task<IActionResult> DeletarAtividade(int projetoId, int id)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null)
                return Unauthorized();

            try
            {
                var atividade = await _atividadeService.Deletar(usuarioId.Value, projetoId, id);
                return Ok(atividade);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private int? ObterUsuarioId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim == null ? null : int.Parse(userIdClaim.Value);
        }
    }
}

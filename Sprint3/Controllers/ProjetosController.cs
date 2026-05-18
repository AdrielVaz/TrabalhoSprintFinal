using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Services;
using System.Security.Claims;

namespace Sprint3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjetosController : ControllerBase
    {
        private readonly IProjetoService _projetoService;

        public ProjetosController(IProjetoService projetoService)
        {
            _projetoService = projetoService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CriarProjeto([FromBody] ProjetoInput input)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim.Value);

            try
            {
                var projeto = await _projetoService.CriarProjeto(usuarioId, input);

                return Ok(projeto);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpPost("projetos/Listar")]
        public async Task<IActionResult> ListarProjetos()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim.Value);

            var projetos = await _projetoService.ListarProjetos(usuarioId);

            return Ok(projetos);
        }
        [Authorize]
        [HttpDelete("{projetoId}")]
        public async Task<IActionResult> Deletar(int projetoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();
            int usuarioId = int.Parse(userIdClaim.Value);
            try
            {
                var projeto = await _projetoService.DeletarProjeto(projetoId, usuarioId);
                return Ok(projeto);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}
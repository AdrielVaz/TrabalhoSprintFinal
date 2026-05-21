using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Services.Interfaces;
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
        [HttpPut("{projetoId}")]
        public async Task<IActionResult> Atualizar(int projetoId, [FromBody] ProjetoInput input)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            try
            {
                var projeto = await _projetoService.AtualizarProjeto(projetoId, int.Parse(userIdClaim.Value), input);
                return Ok(projeto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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

        [Authorize]
        [HttpPost("{projetoId}/acessos")]
        public async Task<IActionResult> CompartilharProjeto(int projetoId, [FromBody] ProjetoAcessoInput input)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim.Value);

            try
            {
                var membro = await _projetoService.CompartilharProjeto(projetoId, usuarioId, input);
                return Ok(membro);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{projetoId}/membros")]
        public async Task<IActionResult> RemoverMembro(int projetoId, [FromBody] RemoverProjetoMembroInput input)
        {
            return await RemoverMembroProjeto(projetoId, input);
        }

        [Authorize]
        [HttpPost("{projetoId}/membros/remover")]
        public async Task<IActionResult> RemoverMembroPorPost(int projetoId, [FromBody] RemoverProjetoMembroInput input)
        {
            return await RemoverMembroProjeto(projetoId, input);
        }

        private async Task<IActionResult> RemoverMembroProjeto(int projetoId, RemoverProjetoMembroInput input)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            try
            {
                var membro = await _projetoService.RemoverMembroProjeto(projetoId, int.Parse(userIdClaim.Value), input);
                return Ok(membro);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{projetoId}/membros")]
        public async Task<IActionResult> ListarMembros(int projetoId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            int usuarioId = int.Parse(userIdClaim.Value);

            try
            {
                var membros = await _projetoService.ListarMembros(projetoId, usuarioId);
                return Ok(membros);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("convites/pendentes")]
        public async Task<IActionResult> ListarConvitesPendentes()
        {
            var email =
                User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email)?.Value ??
                User.FindFirst(ClaimTypes.Email)?.Value ??
                User.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            if (string.IsNullOrWhiteSpace(email))
                return Unauthorized();

            var convites = await _projetoService.ListarConvitesPendentes(email);
            return Ok(convites);
        }

        [Authorize]
        [HttpPost("convites/{conviteId}/aceitar")]
        public async Task<IActionResult> AceitarConvite(int conviteId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            try
            {
                var membro = await _projetoService.AceitarConvite(conviteId, int.Parse(userIdClaim.Value));
                return Ok(membro);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("convites/{conviteId}/recusar")]
        public async Task<IActionResult> RecusarConvite(int conviteId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Unauthorized();

            try
            {
                var convite = await _projetoService.RecusarConvite(conviteId, int.Parse(userIdClaim.Value));
                return Ok(convite);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

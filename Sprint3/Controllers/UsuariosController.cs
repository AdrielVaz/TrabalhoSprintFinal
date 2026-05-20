using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using System.Security.Claims;

namespace Sprint3.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private const long TamanhoMaximoFoto = 2 * 1024 * 1024;
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuariosController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        [HttpGet("me")]
        public async Task<IActionResult> ObterPerfil()
        {
            var usuario = await ObterUsuarioLogado();
            if (usuario == null)
                return Unauthorized(new { message = "Usuario nao autenticado." });

            return Ok(MapPerfil(usuario));
        }

        [HttpPut("me")]
        [RequestSizeLimit(TamanhoMaximoFoto + 1024 * 128)]
        public async Task<IActionResult> AtualizarPerfil([FromForm] UsuarioPerfilInput input)
        {
            var usuario = await ObterUsuarioLogado();
            if (usuario == null)
                return Unauthorized(new { message = "Usuario nao autenticado." });

            if (string.IsNullOrWhiteSpace(input.Nome))
                return BadRequest(new { message = "Nome e obrigatorio." });

            usuario.Nome = input.Nome.Trim();

            if (input.Foto != null && input.Foto.Length > 0)
            {
                if (input.Foto.Length > TamanhoMaximoFoto)
                    return BadRequest(new { message = "A foto deve ter no maximo 2 MB." });

                if (string.IsNullOrWhiteSpace(input.Foto.ContentType) ||
                    !input.Foto.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest(new { message = "Envie um arquivo de imagem valido." });
                }

                using var stream = new MemoryStream();
                await input.Foto.CopyToAsync(stream);
                usuario.FotoPerfil = stream.ToArray();
                usuario.FotoPerfilContentType = input.Foto.ContentType;
            }

            await _usuarioRepository.Atualizar(usuario);
            return Ok(MapPerfil(usuario, true));
        }

        [HttpGet("{id:int}/foto")]
        public async Task<IActionResult> ObterFoto(int id)
        {
            var usuario = await _usuarioRepository.ObterPorId(id);

            if (usuario?.FotoPerfil == null || usuario.FotoPerfil.Length == 0)
                return NotFound();

            return File(usuario.FotoPerfil, usuario.FotoPerfilContentType ?? "image/jpeg");
        }

        private async Task<Usuario?> ObterUsuarioLogado()
        {
            var claimId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(claimId, out var usuarioId))
                return null;

            return await _usuarioRepository.ObterPorId(usuarioId);
        }

        private static UsuarioPerfilOutput MapPerfil(Usuario usuario, bool bustCache = false)
        {
            return new UsuarioPerfilOutput
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                FotoPerfilUrl = usuario.FotoPerfil is { Length: > 0 }
                    ? $"/api/Usuarios/{usuario.Id}/foto{(bustCache ? "?v=" + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() : string.Empty)}"
                    : null
            };
        }
    }
}

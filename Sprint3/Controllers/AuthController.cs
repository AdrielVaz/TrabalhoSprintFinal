using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Sprint3.Data;
using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using Sprint3.Security;
using Sprint3.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sprint3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            return Ok(new
            {
                autenticado = User.Identity?.IsAuthenticated,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IConfiguration _configuration;
        private readonly IProjetoService _projetoService;


        public AuthController(IUsuarioRepository usuarioRepo, IConfiguration configuration, IProjetoService projetoService)
        {
            _usuarioRepo = usuarioRepo;
            _configuration = configuration;
            _projetoService = projetoService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Senha))
            {
                return BadRequest(new { message = "Email e senha sao obrigatorios." });
            }

            string email = input.Email.Trim().ToLowerInvariant();

            var usuario = await _usuarioRepo.ObterPorEmail(email);

            if (usuario is null)
            {
                return Unauthorized(new { message = "Credenciais invalidas." });
            }

            bool senhaValida = PasswordHasher.Verify(input.Senha, usuario.HashSenha);

            if (!senhaValida)
            {
                return Unauthorized(new { message = "Credenciais invalidas." });
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, usuario.Email),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("name", usuario.Nome)
            };
            var jwtSettings = _configuration.GetSection("Jwt");

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!)
            );
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            Response.Cookies.Append("authToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            return Ok(new
            {
                id = usuario.Id,
                nome = usuario.Nome,
                email = usuario.Email,
                convitesPendentes = await _projetoService.ListarConvitesPendentes(usuario.Email)
            });
            
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CadastroInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Nome) ||
                string.IsNullOrWhiteSpace(input.Email) ||
                string.IsNullOrWhiteSpace(input.Senha))
            {
                return BadRequest(new { message = "Nome, email e senha sao obrigatorios." });
            }

            string email = input.Email.Trim().ToLowerInvariant();

            bool exists = await _usuarioRepo.EmailExiste(email);
            if (exists)
            {
                return Conflict(new { message = "Email ja cadastrado." });
            }

            var usuario = new Usuario
            {
                Nome = input.Nome.Trim(),
                Email = email,
                HashSenha = PasswordHasher.Hash(input.Senha)
            };

            await _usuarioRepo.Criar(usuario);

            return CreatedAtAction(nameof(Register), new
            {
                id = usuario.Id,
                nome = usuario.Nome,
                email = usuario.Email
            });
        }
   }
}

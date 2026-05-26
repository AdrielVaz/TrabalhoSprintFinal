using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Sprint3.DTOs;
using Sprint3.Models;
using Sprint3.Repositories.Interfaces;
using Sprint3.Security;
using Sprint3.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;

namespace Sprint3.Controllers
{
    /// <summary>
    /// Endpoints de autenticação, cadastro, confirmação de email e recuperação de senha.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepo;
        private readonly IConfiguration _configuration;
        private readonly IProjetoService _projetoService;
        private readonly IEmailService _emailService;

        public AuthController(
            IUsuarioRepository usuarioRepo,
            IConfiguration configuration,
            IProjetoService projetoService,
            IEmailService emailService)
        {
            _usuarioRepo = usuarioRepo;
            _configuration = configuration;
            _projetoService = projetoService;
            _emailService = emailService;
        }

        /// <summary>
        /// Retorna informações de debug do usuário autenticado e suas claims.
        /// </summary>
        [HttpGet("debug")]
        public IActionResult Debug()
        {
            return Ok(new
            {
                autenticado = User.Identity?.IsAuthenticated,
                claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        /// <summary>
        /// Autentica o usuário, grava o cookie de autenticação e retorna dados básicos do perfil.
        /// </summary>
        /// <param name="input">Email e senha do usuário.</param>
        /// <returns>Dados do usuário, foto do perfil e convites pendentes.</returns>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Senha))
            {
                return BadRequest(new { message = "Email e senha sao obrigatorios." });
            }

            string email = input.Email.Trim().ToLowerInvariant();
            var usuario = await _usuarioRepo.ObterPorEmail(email);

            if (usuario is null || !PasswordHasher.Verify(input.Senha, usuario.HashSenha))
            {
                return Unauthorized(new { message = "Credenciais invalidas." });
            }

            if (!usuario.EmailConfirmado)
            {
                return Unauthorized(new { message = "Confirme seu email antes de entrar." });
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim("id", usuario.Id.ToString()),
                new Claim("nome", usuario.Nome),
                new Claim("email", usuario.Email)
            };

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),
                    AllowRefresh = true
                }
            );

            Response.Cookies.Delete("authToken_legacy", new CookieOptions
            {
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            return Ok(new
            {
                id = usuario.Id,
                nome = usuario.Nome,
                email = usuario.Email,
                fotoPerfilUrl = usuario.FotoPerfil is { Length: > 0 } ? $"/api/Usuarios/{usuario.Id}/foto" : null,
                convitesPendentes = await _projetoService.ListarConvitesPendentes(usuario.Email)
            });
        }

        /// <summary>
        /// Cadastra um novo usuário e envia um email de confirmação.
        /// </summary>
        /// <param name="input">Nome, email e senha do novo usuário.</param>
        /// <returns>Dados básicos do cadastro e mensagem para confirmar email.</returns>
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

            if (await _usuarioRepo.EmailExiste(email))
            {
                return Conflict(new { message = "Email ja cadastrado." });
            }

            var usuario = new Usuario
            {
                Nome = input.Nome.Trim(),
                Email = email,
                HashSenha = PasswordHasher.Hash(input.Senha),
                EmailConfirmado = false,
                EmailConfirmacaoToken = GerarToken(),
                EmailConfirmacaoExpiraEm = DateTime.UtcNow.AddHours(24)
            };

            await _usuarioRepo.Criar(usuario);
            await EnviarConfirmacaoEmail(usuario);

            return CreatedAtAction(nameof(Register), new
            {
                id = usuario.Id,
                nome = usuario.Nome,
                email = usuario.Email,
                message = $"Cadastro realizado. Confirme sua conta na caixa de entrada de {usuario.Email}."
            });
        }
        /// <summary>
        /// Encerra a sessão removendo o cookie JWT de autenticação.
        /// </summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("authToken", new CookieOptions
            {
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });

            return Ok(new
            {
                returnUrl = "/",
                message = "Logout realizado com sucesso"
            });
        }
        /// <summary>
        /// Confirma o email do usuário usando o token enviado por email.
        /// </summary>
        /// <param name="email">Email da conta pendente de confirmação.</param>
        /// <param name="token">Token de confirmação recebido por email.</param>
        [HttpGet("confirmar-email")]
        public async Task<IActionResult> ConfirmarEmail([FromQuery] string email, [FromQuery] string token)
        {
            var usuario = await _usuarioRepo.ObterPorEmail(email.Trim().ToLowerInvariant());

            if (usuario == null ||
                usuario.EmailConfirmacaoToken != token ||
                usuario.EmailConfirmacaoExpiraEm < DateTime.UtcNow)
            {
                return BadRequest("Link de confirmacao invalido ou expirado.");
            }

            usuario.EmailConfirmado = true;
            usuario.EmailConfirmacaoToken = null;
            usuario.EmailConfirmacaoExpiraEm = null;
            await _usuarioRepo.Atualizar(usuario);

            return Content("<html><body style=\"font-family:Arial;text-align:center;padding:40px\"><h2>Email confirmado</h2><p>Sua conta foi confirmada. Voce ja pode fazer login.</p><a href=\"/\">Ir para login</a></body></html>", "text/html");
        }

        /// <summary>
        /// Reenvia o email de confirmação para uma conta ainda não confirmada.
        /// </summary>
        /// <param name="input">Email que receberá o novo link de confirmação.</param>
        [HttpPost("reenviar-confirmacao")]
        public async Task<IActionResult> ReenviarConfirmacao([FromBody] ReenviarConfirmacaoInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Email))
                return BadRequest(new { message = "Email e obrigatorio." });

            var usuario = await _usuarioRepo.ObterPorEmail(input.Email.Trim().ToLowerInvariant());

            if (usuario == null || usuario.EmailConfirmado)
            {
                return Ok(new { message = "Se houver uma conta pendente, enviaremos um email de confirmacao." });
            }

            usuario.EmailConfirmacaoToken = GerarToken();
            usuario.EmailConfirmacaoExpiraEm = DateTime.UtcNow.AddHours(24);
            await _usuarioRepo.Atualizar(usuario);
            await EnviarConfirmacaoEmail(usuario);

            return Ok(new { message = "Se houver uma conta pendente, enviaremos um email de confirmacao." });
        }

        /// <summary>
        /// Solicita o envio de um link para redefinir a senha.
        /// </summary>
        /// <param name="input">Email da conta que receberá as instruções.</param>
        [HttpPost("esqueci-senha")]
        public async Task<IActionResult> EsqueciSenha([FromBody] EsqueciSenhaInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Email))
                return BadRequest(new { message = "Email e obrigatorio." });

            var usuario = await _usuarioRepo.ObterPorEmail(input.Email.Trim().ToLowerInvariant());

            if (usuario != null)
            {
                usuario.RedefinirSenhaToken = GerarToken();
                usuario.RedefinirSenhaExpiraEm = DateTime.UtcNow.AddHours(1);
                await _usuarioRepo.Atualizar(usuario);
                await EnviarRedefinicaoSenha(usuario);
            }

            return Ok(new { message = "Se o email existir, enviaremos instrucoes para redefinir a senha." });
        }

        /// <summary>
        /// Redefine a senha usando o token enviado por email.
        /// </summary>
        /// <param name="input">Email, token e nova senha.</param>
        [HttpPost("redefinir-senha")]
        public async Task<IActionResult> RedefinirSenha([FromBody] RedefinirSenhaInput input)
        {
            if (string.IsNullOrWhiteSpace(input.Email) ||
                string.IsNullOrWhiteSpace(input.Token) ||
                string.IsNullOrWhiteSpace(input.NovaSenha))
            {
                return BadRequest(new { message = "Dados obrigatorios ausentes." });
            }

            if (input.NovaSenha.Length < 6)
                return BadRequest(new { message = "A senha deve ter pelo menos 6 caracteres." });

            var usuario = await _usuarioRepo.ObterPorEmail(input.Email.Trim().ToLowerInvariant());

            if (usuario == null ||
                usuario.RedefinirSenhaToken != input.Token ||
                usuario.RedefinirSenhaExpiraEm < DateTime.UtcNow)
            {
                return BadRequest(new { message = "Link de redefinicao invalido ou expirado." });
            }

            usuario.HashSenha = PasswordHasher.Hash(input.NovaSenha);
            usuario.RedefinirSenhaToken = null;
            usuario.RedefinirSenhaExpiraEm = null;
            await _usuarioRepo.Atualizar(usuario);

            return Ok(new { message = "Senha alterada com sucesso." });
        }

        private async Task EnviarConfirmacaoEmail(Usuario usuario)
        {
            var link = GerarUrl("/api/Auth/confirmar-email", usuario.Email, usuario.EmailConfirmacaoToken!);
            await _emailService.EnviarEmailAsync(
                usuario.Email,
                "Confirme sua conta TASKPI",
                $"<p>Ola, {HtmlEncoder.Default.Encode(usuario.Nome)}.</p><p>Confirme sua conta pelo link abaixo:</p><p><a href=\"{link}\">Confirmar email</a></p>");
        }

        private async Task EnviarRedefinicaoSenha(Usuario usuario)
        {
            var link = GerarUrl("/redefinir-senha", usuario.Email, usuario.RedefinirSenhaToken!);
            await _emailService.EnviarEmailAsync(
                usuario.Email,
                "Redefinicao de senha TASKPI",
                $"<p>Ola, {HtmlEncoder.Default.Encode(usuario.Nome)}.</p><p>Use o link abaixo para redefinir sua senha:</p><p><a href=\"{link}\">Redefinir senha</a></p><p>Este link expira em 1 hora.</p>");
        }

        private string GerarUrl(string path, string email, string token)
        {
            var request = HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{path}?email={Uri.EscapeDataString(email)}&token={Uri.EscapeDataString(token)}";
        }

        private static string GerarToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
namespace Sprint3.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EndpointController : ControllerBase
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return Redirect("/index.html");
        }

        [HttpGet("/cadastro")]
        public IActionResult Cadastro()
        {
            return Redirect("/CadastroComponent/Cadastro.html");
        }

        [HttpGet("/esqueci-senha")]
        public IActionResult EsqueciSenha()
        {
            return Redirect("/AuthComponent/EsqueciSenha.html");
        }

        [HttpGet("/redefinir-senha")]
        public IActionResult RedefinirSenha()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "AuthComponent", "RedefinirSenha.html");
            if (!System.IO.File.Exists(path)) return NotFound();
            return PhysicalFile(path, "text/html");
        }

        [Authorize]
        [HttpGet("/mainscreen")]
        public IActionResult MainScreen()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MainScreenComponent", "MainScreen.html");
            if (!System.IO.File.Exists(path)) return NotFound();
            return PhysicalFile(path, "text/html");
        }

        [Authorize]
        [HttpGet("/perfil")]
        public IActionResult Perfil()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "PerfilComponent", "Perfil.html");
            if (!System.IO.File.Exists(path)) return NotFound();
            return PhysicalFile(path, "text/html");
        }

    }
}

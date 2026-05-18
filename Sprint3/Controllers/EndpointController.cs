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
        [Authorize]
        [HttpGet("/mainscreen")]
        public IActionResult MainScreen()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "MainScreenComponent", "MainScreen.html");
            if (!System.IO.File.Exists(path)) return NotFound();
            return PhysicalFile(path, "text/html");
        }


    }
}

using Sprint3.Models;

namespace Sprint3.DTOs
{
    public class ProjetoAcessoInput
    {
        public string Email { get; set; } = string.Empty;

        public NivelAcessoProjeto NivelAcesso { get; set; }
    }
}

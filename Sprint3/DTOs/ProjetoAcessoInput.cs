using Sprint3.Models;

namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados para convidar usuário para um projeto.
    /// </summary>
    public class ProjetoAcessoInput
    {
        /// <summary>Email do usuário convidado.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Nível de acesso concedido: Adm, Observador ou Ajudante.</summary>
        public NivelAcessoProjeto NivelAcesso { get; set; }
    }
}

using Microsoft.AspNetCore.Http;

namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados para atualizar perfil do usuário autenticado.
    /// </summary>
    public class UsuarioPerfilInput
    {
        /// <summary>Novo nome exibido no sistema.</summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>Arquivo de imagem para foto de perfil. Limite de 2 MB.</summary>
        public IFormFile? Foto { get; set; }
    }
}

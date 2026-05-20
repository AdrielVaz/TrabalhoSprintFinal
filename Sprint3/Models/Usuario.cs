using System.ComponentModel.DataAnnotations;

namespace Sprint3.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        public string Nome { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string HashSenha { get; set; } = string.Empty;

        public bool EmailConfirmado { get; set; }

        public string? EmailConfirmacaoToken { get; set; }

        public DateTime? EmailConfirmacaoExpiraEm { get; set; }

        public string? RedefinirSenhaToken { get; set; }

        public DateTime? RedefinirSenhaExpiraEm { get; set; }

        public byte[]? FotoPerfil { get; set; }

        public string? FotoPerfilContentType { get; set; }

        [Required]
        public List<Projeto> Projetos { get; set; } = new List<Projeto>();

        public List<ProjetoAcesso> ProjetosCompartilhados { get; set; } = new List<ProjetoAcesso>();

    }
}

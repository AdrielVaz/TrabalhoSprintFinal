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
        [Required]
        public List<Projeto> Projetos { get; set; } = new List<Projeto>(); 

    }
}

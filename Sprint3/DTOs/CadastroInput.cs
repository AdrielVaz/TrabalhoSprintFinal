namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados para cadastro de usuário.
    /// </summary>
    public class CadastroInput
    {
        /// <summary>Nome exibido no sistema.</summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>Email usado para login e confirmação de conta.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Senha inicial da conta.</summary>
        public string Senha { get; set; } = string.Empty;
    }
}

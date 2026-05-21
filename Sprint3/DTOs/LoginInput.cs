namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados para login do usuário.
    /// </summary>
    public class LoginInput
    {
        /// <summary>Email cadastrado.</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Senha da conta.</summary>
        public string Senha { get; set; } = string.Empty;
    }
}

namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados para remover participante de um projeto.
    /// </summary>
    public class RemoverProjetoMembroInput
    {
        /// <summary>Email do participante que será removido.</summary>
        public string Email { get; set; } = string.Empty;
    }
}

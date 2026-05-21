namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados para criar ou editar projeto.
    /// </summary>
    public class ProjetoInput
    {
        /// <summary>Nome ou descrição do projeto.</summary>
        public string Descricao { get; set; } = string.Empty;
    }
}

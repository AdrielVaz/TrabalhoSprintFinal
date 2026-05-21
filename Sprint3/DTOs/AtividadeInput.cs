namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados para criar ou editar atividade.
    /// </summary>
    public class AtividadeInput
    {
        /// <summary>Título da atividade/coluna.</summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>Descrição opcional da atividade.</summary>
        public string? Descricao { get; set; }
    }
}

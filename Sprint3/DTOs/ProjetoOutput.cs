namespace Sprint3.DTOs
{
    /// <summary>
    /// Dados retornados para um projeto acessível pelo usuário.
    /// </summary>
    public class ProjetoOutput
    {
        /// <summary>Identificador do projeto.</summary>
        public int Id { get; set; }

        /// <summary>Nome ou descrição do projeto.</summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>Nível de acesso do usuário autenticado neste projeto.</summary>
        public string NivelAcesso { get; set; } = string.Empty;

        /// <summary>Indica se o projeto foi compartilhado com o usuário autenticado.</summary>
        public bool Compartilhado { get; set; }
    }
}

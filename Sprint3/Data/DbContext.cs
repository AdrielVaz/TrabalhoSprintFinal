using Microsoft.EntityFrameworkCore;
using Sprint3.Models;

namespace Sprint3.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options)
                : base(options){}
            public DbSet<Usuario> Usuarios { get; set; }

            public DbSet<Tarefa> Tarefas { get; set; }

            public DbSet<Projeto> Projetos { get; set; }
    }  
}

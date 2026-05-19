using Microsoft.EntityFrameworkCore;
using Sprint3.Models;

namespace Sprint3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Tarefa> Tarefas { get; set; }

        public DbSet<Projeto> Projetos { get; set; }

        public DbSet<Atividade> Atividades { get; set; }

        public DbSet<ProjetoAcesso> ProjetoAcessos { get; set; }

        public DbSet<ConviteProjeto> ConvitesProjeto { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProjetoAcesso>()
                .HasIndex(a => new { a.ProjetoId, a.UsuarioId })
                .IsUnique();

            modelBuilder.Entity<ProjetoAcesso>()
                .HasOne(a => a.Projeto)
                .WithMany(p => p.Acessos)
                .HasForeignKey(a => a.ProjetoId);

            modelBuilder.Entity<ProjetoAcesso>()
                .HasOne(a => a.Usuario)
                .WithMany(u => u.ProjetosCompartilhados)
                .HasForeignKey(a => a.UsuarioId);

            modelBuilder.Entity<Atividade>()
                .HasOne(a => a.Projeto)
                .WithMany(p => p.Atividades)
                .HasForeignKey(a => a.ProjetoId);

            modelBuilder.Entity<ConviteProjeto>()
                .HasOne(c => c.Projeto)
                .WithMany(p => p.Convites)
                .HasForeignKey(c => c.ProjetoId);

            modelBuilder.Entity<Tarefa>()
                .HasOne(t => t.Atividade)
                .WithMany(a => a.Tarefas)
                .HasForeignKey(t => t.AtividadeId);
        }
    }
}
